using System.Text.Json;
using System.Reflection;
using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Customisation.MetadataGeneration;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using NUnit.Framework;

namespace CloudAwesome.Dataverse.Customisation.Tests;

[TestFixture]
public class MetadataGenerationTests
{
	[Test]
	public void ValidateManifest_requires_include_list_unless_include_all_entities_is_set()
	{
		var manifest = new MetadataGenerationManifest
		{
			Output = new MetadataGenerationOutputOptions { Path = "metadata.json" }
		};

		var exception = Assert.Throws<InvalidOperationException>(() => MetadataOutputValidator.ValidateManifest(manifest));

		Assert.That(exception!.Message, Does.Contain("entities.include is required"));
	}

	[Test]
	public void ApplyOverrides_replaces_output_split_mode_and_included_entities()
	{
		var manifest = new MetadataGenerationManifest
		{
			Output = new MetadataGenerationOutputOptions { Path = "manifest.json" },
			Entities = new MetadataGenerationEntityOptions
			{
				IncludeAllEntities = true,
				Include = ["account"]
			}
		};

		GenerateMetadata.ApplyOverrides(manifest, new MetadataGenerationRunOptions
		{
			OutputPathOverride = "override.json",
			SplitFilesPerEntityOverride = true,
			IncludeEntityOverrides = ["contact;lead"]
		});

		Assert.Multiple(() =>
		{
			Assert.That(manifest.Output.Path, Is.EqualTo("override.json"));
			Assert.That(manifest.Output.SplitFilesPerEntity, Is.True);
			Assert.That(manifest.Entities.IncludeAllEntities, Is.False);
			Assert.That(manifest.Entities.Include, Is.EqualTo(new[] { "contact", "lead" }));
		});
	}

	[Test]
	public void Run_writes_combined_metadata_with_deterministic_entity_and_attribute_ordering()
	{
		var outputPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, $"{Guid.NewGuid():N}", "metadata.json");
		var manifest = NewManifest(outputPath);
		var generator = new GenerateMetadata(new FakeMetadataProvider(NewContactMetadata(), NewAccountMetadata()));

		var result = generator.Run(new EmptyOrganizationService(), new TracingHelper(), manifest);

		var document = JsonSerializer.Deserialize<MetadataDocument>(File.ReadAllText(outputPath));

		Assert.Multiple(() =>
		{
			Assert.That(result.EntityCount, Is.EqualTo(2));
			Assert.That(result.WrittenFiles, Has.Count.EqualTo(1));
			Assert.That(document!.Entities.Select(entity => entity.LogicalName), Is.EqualTo(new[] { "account", "contact" }));
			Assert.That(document.Entities[0].Attributes.Select(attribute => attribute.LogicalName), Is.EqualTo(new[] { "accountid", "name" }));
		});
	}

	[Test]
	public void Run_writes_split_index_and_entity_files()
	{
		var outputPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, $"{Guid.NewGuid():N}", "metadata.index.json");
		var manifest = NewManifest(outputPath);
		manifest.Output.SplitFilesPerEntity = true;

		var generator = new GenerateMetadata(new FakeMetadataProvider(NewAccountMetadata()));

		var result = generator.Run(new EmptyOrganizationService(), new TracingHelper(), manifest);
		var index = JsonSerializer.Deserialize<MetadataIndexDocument>(File.ReadAllText(outputPath));
		var accountPath = Path.Combine(Path.GetDirectoryName(outputPath)!, "entities", "account.json");

		Assert.Multiple(() =>
		{
			Assert.That(result.WrittenFiles, Has.Count.EqualTo(2));
			Assert.That(index!.EntityFiles.Single().LogicalName, Is.EqualTo("account"));
			Assert.That(File.Exists(accountPath), Is.True);
		});
	}

	private static MetadataGenerationManifest NewManifest(string outputPath)
	{
		return new MetadataGenerationManifest
		{
			Output = new MetadataGenerationOutputOptions { Path = outputPath },
			Entities = new MetadataGenerationEntityOptions { Include = ["account"] }
		};
	}

	private static EntityMetadata NewAccountMetadata()
	{
		var accountId = new UniqueIdentifierAttributeMetadata { LogicalName = "accountid", SchemaName = "AccountId" };
		SetSdkProperty(accountId, nameof(AttributeMetadata.IsPrimaryId), true);

		var entity = new EntityMetadata
		{
			LogicalName = "account",
			SchemaName = "Account",
			OwnershipType = OwnershipTypes.UserOwned,
			IsActivity = false
		};

		SetSdkProperty(entity, nameof(EntityMetadata.PrimaryIdAttribute), "accountid");
		SetSdkProperty(entity, nameof(EntityMetadata.PrimaryNameAttribute), "name");
		SetSdkProperty(entity, nameof(EntityMetadata.IsIntersect), false);
		SetSdkProperty(entity, nameof(EntityMetadata.Attributes), new AttributeMetadata[]
		{
			new StringAttributeMetadata { LogicalName = "name", SchemaName = "Name", MaxLength = 160 },
			accountId
		});

		return entity;
	}

	private static EntityMetadata NewContactMetadata()
	{
		var contactId = new UniqueIdentifierAttributeMetadata { LogicalName = "contactid", SchemaName = "ContactId" };
		SetSdkProperty(contactId, nameof(AttributeMetadata.IsPrimaryId), true);

		var entity = new EntityMetadata
		{
			LogicalName = "contact",
			SchemaName = "Contact",
			OwnershipType = OwnershipTypes.UserOwned,
			IsActivity = false
		};

		SetSdkProperty(entity, nameof(EntityMetadata.PrimaryIdAttribute), "contactid");
		SetSdkProperty(entity, nameof(EntityMetadata.PrimaryNameAttribute), "fullname");
		SetSdkProperty(entity, nameof(EntityMetadata.IsIntersect), false);
		SetSdkProperty(entity, nameof(EntityMetadata.Attributes), new AttributeMetadata[]
		{
			contactId,
			new StringAttributeMetadata { LogicalName = "fullname", SchemaName = "FullName", MaxLength = 160 }
		});

		return entity;
	}

	private static void SetSdkProperty<T>(object target, string propertyName, T value)
	{
		var property = target.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)
		               ?? throw new InvalidOperationException($"Could not find property '{propertyName}'.");
		var setter = property.GetSetMethod(true)
		             ?? throw new InvalidOperationException($"Property '{propertyName}' does not have a setter.");

		setter.Invoke(target, [value]);
	}

	private sealed class FakeMetadataProvider(params EntityMetadata[] entities) : IMetadataProvider
	{
		public IReadOnlyCollection<EntityMetadata> RetrieveEntities(IOrganizationService client, MetadataGenerationManifest manifest)
		{
			return entities;
		}
	}

	private sealed class EmptyOrganizationService : IOrganizationService
	{
		public Guid Create(Entity entity)
		{
			throw new NotImplementedException();
		}

		public Entity Retrieve(string entityName, Guid id, Microsoft.Xrm.Sdk.Query.ColumnSet columnSet)
		{
			throw new NotImplementedException();
		}

		public void Update(Entity entity)
		{
			throw new NotImplementedException();
		}

		public void Delete(string entityName, Guid id)
		{
			throw new NotImplementedException();
		}

		public OrganizationResponse Execute(OrganizationRequest request)
		{
			throw new NotImplementedException();
		}

		public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
		{
			throw new NotImplementedException();
		}

		public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities)
		{
			throw new NotImplementedException();
		}

		public EntityCollection RetrieveMultiple(Microsoft.Xrm.Sdk.Query.QueryBase query)
		{
			throw new NotImplementedException();
		}
	}
}
