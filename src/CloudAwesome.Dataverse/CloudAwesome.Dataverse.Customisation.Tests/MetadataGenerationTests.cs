using System.Text.Json;
using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Customisation.MetadataGeneration;
using CloudAwesome.Xrm.Simulate;
using CloudAwesome.Xrm.Simulate.Metadata;
using Microsoft.Xrm.Sdk;
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
		manifest.Entities.IncludeAllEntities = true;
		manifest.Entities.Include = [];
		var organizationService = CreateMetadataBackedService();
		var generator = new GenerateMetadata();

		var result = generator.Run(organizationService, new TracingHelper(), manifest);

		var document = JsonSerializer.Deserialize<MetadataDocument>(File.ReadAllText(outputPath));

		Assert.Multiple(() =>
		{
			Assert.That(result.EntityCount, Is.EqualTo(2));
			Assert.That(result.WrittenFiles, Has.Count.EqualTo(1));
			Assert.That(document!.Entities.Select(entity => entity.LogicalName), Is.EqualTo(new[] { "account", "contact" }));
			Assert.That(document.Entities[0].SchemaName, Is.EqualTo("Account"));
			Assert.That(document.Entities[0].Attributes.Select(attribute => attribute.LogicalName), Is.EqualTo(new[] { "accountid", "name" }));
		});
	}

	[Test]
	public void Run_writes_split_index_and_entity_files()
	{
		var outputPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, $"{Guid.NewGuid():N}", "metadata.index.json");
		var manifest = NewManifest(outputPath);
		manifest.Output.SplitFilesPerEntity = true;
		var organizationService = CreateMetadataBackedService();
		var generator = new GenerateMetadata();

		var result = generator.Run(organizationService, new TracingHelper(), manifest);
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

	private static IOrganizationService CreateMetadataBackedService()
	{
		IOrganizationService organizationService = null!;
		return organizationService.Simulate(new SimulatorOptions
		{
			Metadata = SimulatedMetadata.Load(CreateSimulatorMetadataIndex())
		});
	}

	private static string CreateSimulatorMetadataIndex()
	{
		var rootDirectory = Path.Combine(TestContext.CurrentContext.WorkDirectory, $"{Guid.NewGuid():N}");
		var entityDirectory = Path.Combine(rootDirectory, "entities");
		Directory.CreateDirectory(entityDirectory);

		WriteJson(Path.Combine(rootDirectory, "metadata.index.json"), new MetadataIndexDocument
		{
			EntityFiles =
			[
				new MetadataEntityFile { LogicalName = "account", Path = "entities/account.json" },
				new MetadataEntityFile { LogicalName = "contact", Path = "entities/contact.json" }
			]
		});

		WriteJson(Path.Combine(entityDirectory, "account.json"), new MetadataEntityDocument
		{
			Entity = new MetadataEntity
			{
				LogicalName = "account",
				SchemaName = "Account",
				CollectionLogicalName = "accounts",
				CollectionSchemaName = "Accounts",
				PrimaryIdAttribute = "accountid",
				PrimaryNameAttribute = "name",
				OwnershipType = "UserOwned",
				IsValidForQueue = true,
				Attributes =
				[
					NewPrimaryIdAttribute("accountid", "AccountId"),
					NewStringAttribute("name", "Name")
				]
			}
		});

		WriteJson(Path.Combine(entityDirectory, "contact.json"), new MetadataEntityDocument
		{
			Entity = new MetadataEntity
			{
				LogicalName = "contact",
				SchemaName = "Contact",
				CollectionLogicalName = "contacts",
				CollectionSchemaName = "Contacts",
				PrimaryIdAttribute = "contactid",
				PrimaryNameAttribute = "fullname",
				OwnershipType = "UserOwned",
				Attributes =
				[
					NewPrimaryIdAttribute("contactid", "ContactId"),
					NewStringAttribute("fullname", "FullName")
				]
			}
		});

		return Path.Combine(rootDirectory, "metadata.index.json");
	}

	private static MetadataAttribute NewPrimaryIdAttribute(string logicalName, string schemaName)
	{
		return new MetadataAttribute
		{
			LogicalName = logicalName,
			SchemaName = schemaName,
			AttributeType = "Uniqueidentifier",
			AttributeTypeName = "UniqueidentifierType",
			RequiredLevel = "SystemRequired",
			IsPrimaryId = true,
			IsValidForCreate = true,
			IsValidForUpdate = false,
			IsValidForRead = true,
			IsSecured = false
		};
	}

	private static MetadataAttribute NewStringAttribute(string logicalName, string schemaName)
	{
		return new MetadataAttribute
		{
			LogicalName = logicalName,
			SchemaName = schemaName,
			AttributeType = "String",
			AttributeTypeName = "StringType",
			RequiredLevel = "None",
			IsPrimaryName = true,
			IsValidForCreate = true,
			IsValidForUpdate = true,
			IsValidForRead = true,
			IsSecured = false,
			MaxLength = 160,
			Format = "Text"
		};
	}

	private static void WriteJson<T>(string path, T value)
	{
		using var stream = File.Create(path);
		JsonSerializer.Serialize(stream, value, new JsonSerializerOptions { WriteIndented = true });
	}
}
