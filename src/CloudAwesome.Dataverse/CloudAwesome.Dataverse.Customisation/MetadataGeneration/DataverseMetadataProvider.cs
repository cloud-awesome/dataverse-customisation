using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace CloudAwesome.Dataverse.Customisation.MetadataGeneration;

public class DataverseMetadataProvider : IMetadataProvider
{
	public IReadOnlyCollection<EntityMetadata> RetrieveEntities(IOrganizationService client, MetadataGenerationManifest manifest)
	{
		var entityFilters = EntityFilters.Entity;
		if (manifest.Metadata.IncludeAttributes)
		{
			entityFilters |= EntityFilters.Attributes;
		}

		if (manifest.Metadata.IncludeRelationships)
		{
			entityFilters |= EntityFilters.Relationships;
		}

		if (manifest.Metadata.IncludeAlternateKeys)
		{
			entityFilters = EntityFilters.All;
		}

		var entities = manifest.Entities.IncludeAllEntities
			? RetrieveAllEntities(client, entityFilters, manifest.Metadata.IncludeUnpublished)
			: RetrieveIncludedEntities(client, manifest.Entities.Include, entityFilters, manifest.Metadata.IncludeUnpublished);

		if (!manifest.Entities.IncludeRelatedEntities && !manifest.Entities.IncludeIntersectEntities)
		{
			return entities.Values.OrderBy(e => e.LogicalName, StringComparer.Ordinal).ToList();
		}

		var discoveredEntityNames = DiscoverAdditionalEntityNames(entities.Values, manifest);
		foreach (var logicalName in discoveredEntityNames.Where(logicalName => !entities.ContainsKey(logicalName)))
		{
			entities[logicalName] = RetrieveEntity(client, logicalName, entityFilters, manifest.Metadata.IncludeUnpublished);
		}

		return entities.Values.OrderBy(e => e.LogicalName, StringComparer.Ordinal).ToList();
	}

	private static Dictionary<string, EntityMetadata> RetrieveAllEntities(
		IOrganizationService client,
		EntityFilters entityFilters,
		bool includeUnpublished)
	{
		var response = (RetrieveAllEntitiesResponse)client.Execute(new RetrieveAllEntitiesRequest
		{
			EntityFilters = entityFilters,
			RetrieveAsIfPublished = includeUnpublished
		});

		return response.EntityMetadata
			.Where(entity => !string.IsNullOrWhiteSpace(entity.LogicalName))
			.GroupBy(entity => entity.LogicalName, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
	}

	private static Dictionary<string, EntityMetadata> RetrieveIncludedEntities(
		IOrganizationService client,
		IEnumerable<string> logicalNames,
		EntityFilters entityFilters,
		bool includeUnpublished)
	{
		var entities = new Dictionary<string, EntityMetadata>(StringComparer.OrdinalIgnoreCase);
		foreach (var logicalName in logicalNames.Select(NormaliseLogicalName).Distinct(StringComparer.OrdinalIgnoreCase))
		{
			entities[logicalName] = RetrieveEntity(client, logicalName, entityFilters, includeUnpublished);
		}

		return entities;
	}

	private static EntityMetadata RetrieveEntity(
		IOrganizationService client,
		string logicalName,
		EntityFilters entityFilters,
		bool includeUnpublished)
	{
		try
		{
			var response = (RetrieveEntityResponse)client.Execute(new RetrieveEntityRequest
			{
				LogicalName = logicalName,
				EntityFilters = entityFilters,
				RetrieveAsIfPublished = includeUnpublished
			});

			return response.EntityMetadata;
		}
		catch (Exception exception)
		{
			throw new InvalidOperationException($"Could not retrieve metadata for entity '{logicalName}'.", exception);
		}
	}

	private static IReadOnlyCollection<string> DiscoverAdditionalEntityNames(
		IEnumerable<EntityMetadata> entities,
		MetadataGenerationManifest manifest)
	{
		var logicalNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (var entity in entities)
		{
			if (manifest.Entities.IncludeRelatedEntities)
			{
				foreach (var relationship in entity.OneToManyRelationships ?? [])
				{
					AddIfNotEmpty(logicalNames, relationship.ReferencedEntity);
					AddIfNotEmpty(logicalNames, relationship.ReferencingEntity);
				}

				foreach (var relationship in entity.ManyToOneRelationships ?? [])
				{
					AddIfNotEmpty(logicalNames, relationship.ReferencedEntity);
					AddIfNotEmpty(logicalNames, relationship.ReferencingEntity);
				}

				foreach (var relationship in entity.ManyToManyRelationships ?? [])
				{
					AddIfNotEmpty(logicalNames, relationship.Entity1LogicalName);
					AddIfNotEmpty(logicalNames, relationship.Entity2LogicalName);
				}
			}

			if (manifest.Entities.IncludeIntersectEntities)
			{
				foreach (var relationship in entity.ManyToManyRelationships ?? [])
				{
					AddIfNotEmpty(logicalNames, relationship.IntersectEntityName);
				}
			}
		}

		return logicalNames.OrderBy(name => name, StringComparer.OrdinalIgnoreCase).ToList();
	}

	private static string NormaliseLogicalName(string logicalName)
	{
		if (string.IsNullOrWhiteSpace(logicalName))
		{
			throw new InvalidOperationException("Entity logical names cannot be empty.");
		}

		return logicalName.Trim();
	}

	private static void AddIfNotEmpty(HashSet<string> values, string? value)
	{
		if (!string.IsNullOrWhiteSpace(value))
		{
			values.Add(value);
		}
	}
}
