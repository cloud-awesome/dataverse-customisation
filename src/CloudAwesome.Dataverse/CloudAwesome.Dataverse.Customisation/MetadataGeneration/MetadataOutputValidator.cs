namespace CloudAwesome.Dataverse.Customisation.MetadataGeneration;

public static class MetadataOutputValidator
{
	public static void ValidateManifest(MetadataGenerationManifest manifest)
	{
		if (string.IsNullOrWhiteSpace(manifest.Output.Path))
		{
			throw new InvalidOperationException("Manifest output.path is required.");
		}

		var includedEntities = manifest.Entities.Include
			.Where(entity => !string.IsNullOrWhiteSpace(entity))
			.Select(entity => entity.Trim())
			.ToList();

		if (!manifest.Entities.IncludeAllEntities && includedEntities.Count == 0)
		{
			throw new InvalidOperationException("Manifest entities.include is required unless entities.includeAllEntities is true.");
		}

		var duplicateEntities = includedEntities
			.GroupBy(entity => entity, StringComparer.OrdinalIgnoreCase)
			.Where(group => group.Count() > 1)
			.Select(group => group.Key)
			.ToList();

		if (duplicateEntities.Count > 0)
		{
			throw new InvalidOperationException($"Manifest entities.include contains duplicate logical names: {string.Join(", ", duplicateEntities)}.");
		}
	}

	public static void ValidateDocument(MetadataDocument document)
	{
		if (document.ContractVersion != MetadataGenerationConstants.ContractVersion)
		{
			throw new InvalidOperationException($"Unsupported metadata contract version '{document.ContractVersion}'.");
		}

		var duplicateEntities = document.Entities
			.GroupBy(entity => entity.LogicalName, StringComparer.OrdinalIgnoreCase)
			.Where(group => group.Count() > 1)
			.Select(group => group.Key)
			.ToList();

		if (duplicateEntities.Count > 0)
		{
			throw new InvalidOperationException($"Generated metadata contains duplicate entities: {string.Join(", ", duplicateEntities)}.");
		}

		foreach (var entity in document.Entities)
		{
			ValidateEntity(entity);
		}
	}

	private static void ValidateEntity(MetadataEntity entity)
	{
		if (string.IsNullOrWhiteSpace(entity.LogicalName))
		{
			throw new InvalidOperationException("Generated metadata contains an entity without a logicalName.");
		}

		var attributeNames = entity.Attributes
			.Where(attribute => !string.IsNullOrWhiteSpace(attribute.LogicalName))
			.Select(attribute => attribute.LogicalName)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		var duplicateAttributes = entity.Attributes
			.GroupBy(attribute => attribute.LogicalName, StringComparer.OrdinalIgnoreCase)
			.Where(group => group.Count() > 1)
			.Select(group => group.Key)
			.ToList();

		if (duplicateAttributes.Count > 0)
		{
			throw new InvalidOperationException($"Generated metadata for entity '{entity.LogicalName}' contains duplicate attributes: {string.Join(", ", duplicateAttributes)}.");
		}

		if (attributeNames.Count > 0 && !string.IsNullOrWhiteSpace(entity.PrimaryIdAttribute) && !attributeNames.Contains(entity.PrimaryIdAttribute))
		{
			throw new InvalidOperationException($"Generated metadata for entity '{entity.LogicalName}' references primary id attribute '{entity.PrimaryIdAttribute}', but that attribute is not present.");
		}

		if (attributeNames.Count > 0 && !string.IsNullOrWhiteSpace(entity.PrimaryNameAttribute) && !attributeNames.Contains(entity.PrimaryNameAttribute))
		{
			throw new InvalidOperationException($"Generated metadata for entity '{entity.LogicalName}' references primary name attribute '{entity.PrimaryNameAttribute}', but that attribute is not present.");
		}

		foreach (var key in entity.AlternateKeys.Where(_ => attributeNames.Count > 0))
		{
			var missingKeyAttributes = key.AttributeLogicalNames
				.Where(attributeName => !attributeNames.Contains(attributeName))
				.ToList();

			if (missingKeyAttributes.Count > 0)
			{
				throw new InvalidOperationException($"Generated metadata for alternate key '{key.KeyName}' on entity '{entity.LogicalName}' references missing attributes: {string.Join(", ", missingKeyAttributes)}.");
			}
		}
	}
}
