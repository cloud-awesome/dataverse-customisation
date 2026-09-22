using System.Reflection;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace CloudAwesome.Dataverse.Customisation.MetadataGeneration;

public class MetadataDocumentBuilder
{
	public MetadataDocument Build(
		IEnumerable<EntityMetadata> sourceEntities,
		MetadataGenerationManifest manifest,
		MetadataGenerationRunOptions options,
		DateTimeOffset generatedOnUtc)
	{
		var document = new MetadataDocument
		{
			GeneratedOnUtc = manifest.Output.IncludeGeneratedOnUtc ? generatedOnUtc : null,
			Source = new MetadataSource
			{
				EnvironmentUrl = options.EnvironmentUrl,
				PublisherVersion = options.PublisherVersion
			},
			Entities = sourceEntities
				.OrderBy(entity => entity.LogicalName, StringComparer.Ordinal)
				.Select(entity => MapEntity(entity, manifest))
				.ToList()
		};

		return document;
	}

	private static MetadataEntity MapEntity(EntityMetadata entity, MetadataGenerationManifest manifest)
	{
		return new MetadataEntity
		{
			LogicalName = entity.LogicalName ?? string.Empty,
			SchemaName = entity.SchemaName,
			CollectionLogicalName = GetStringProperty(entity, "LogicalCollectionName") ?? GetStringProperty(entity, "EntitySetName"),
			CollectionSchemaName = GetStringProperty(entity, "CollectionSchemaName"),
			PrimaryIdAttribute = entity.PrimaryIdAttribute,
			PrimaryNameAttribute = entity.PrimaryNameAttribute,
			OwnershipType = entity.OwnershipType?.ToString(),
			IsActivity = entity.IsActivity ?? false,
			IsIntersect = entity.IsIntersect ?? false,
			IsValidForQueue = entity.IsValidForQueue?.Value ?? false,
			ValidMessages = GetValidMessages(entity),
			Attributes = manifest.Metadata.IncludeAttributes ? MapAttributes(entity, manifest).ToList() : [],
			StateStatus = manifest.Metadata.IncludeAttributes ? MapStateStatus(entity, manifest).ToList() : [],
			AlternateKeys = manifest.Metadata.IncludeAlternateKeys ? MapAlternateKeys(entity).ToList() : [],
			Relationships = manifest.Metadata.IncludeRelationships ? MapRelationships(entity).ToList() : []
		};
	}

	private static IEnumerable<MetadataAttribute> MapAttributes(EntityMetadata entity, MetadataGenerationManifest manifest)
	{
		return (entity.Attributes ?? [])
			.Where(attribute => !string.IsNullOrWhiteSpace(attribute.LogicalName))
			.OrderBy(attribute => attribute.LogicalName, StringComparer.Ordinal)
			.Select(attribute => MapAttribute(attribute, manifest));
	}

	private static MetadataAttribute MapAttribute(AttributeMetadata attribute, MetadataGenerationManifest manifest)
	{
		var mapped = new MetadataAttribute
		{
			LogicalName = attribute.LogicalName ?? string.Empty,
			SchemaName = attribute.SchemaName,
			AttributeType = attribute.AttributeType?.ToString(),
			AttributeTypeName = attribute.AttributeTypeName?.Value,
			RequiredLevel = attribute.RequiredLevel?.Value.ToString(),
			IsPrimaryId = attribute.IsPrimaryId ?? false,
			IsPrimaryName = attribute.IsPrimaryName ?? false,
			IsValidForCreate = attribute.IsValidForCreate,
			IsValidForUpdate = attribute.IsValidForUpdate,
			IsValidForRead = attribute.IsValidForRead,
			IsSecured = attribute.IsSecured,
			DefaultValue = GetDefaultValue(attribute)
		};

		switch (attribute)
		{
			case StringAttributeMetadata stringAttribute:
				mapped.MaxLength = stringAttribute.MaxLength;
				mapped.Format = stringAttribute.FormatName?.Value ?? stringAttribute.Format?.ToString();
				break;
			case MemoAttributeMetadata memoAttribute:
				mapped.MaxLength = memoAttribute.MaxLength;
				mapped.Format = memoAttribute.FormatName?.Value ?? memoAttribute.Format?.ToString();
				break;
			case IntegerAttributeMetadata integerAttribute:
				mapped.MinValue = integerAttribute.MinValue;
				mapped.MaxValue = integerAttribute.MaxValue;
				mapped.Format = integerAttribute.Format?.ToString();
				break;
			case DecimalAttributeMetadata decimalAttribute:
				mapped.MinValue = decimalAttribute.MinValue;
				mapped.MaxValue = decimalAttribute.MaxValue;
				mapped.Precision = decimalAttribute.Precision;
				break;
			case DoubleAttributeMetadata doubleAttribute:
				mapped.MinValue = ToDecimal(doubleAttribute.MinValue);
				mapped.MaxValue = ToDecimal(doubleAttribute.MaxValue);
				mapped.Precision = doubleAttribute.Precision;
				break;
			case MoneyAttributeMetadata moneyAttribute:
				mapped.MinValue = ToDecimal(moneyAttribute.MinValue);
				mapped.MaxValue = ToDecimal(moneyAttribute.MaxValue);
				mapped.Precision = moneyAttribute.Precision;
				break;
			case DateTimeAttributeMetadata dateTimeAttribute:
				mapped.DateTimeBehavior = dateTimeAttribute.DateTimeBehavior?.Value;
				mapped.Format = dateTimeAttribute.Format?.ToString();
				break;
			case LookupAttributeMetadata lookupAttribute:
				mapped.LookupTargets = (lookupAttribute.Targets ?? [])
					.OrderBy(target => target, StringComparer.Ordinal)
					.ToList();
				break;
			case BooleanAttributeMetadata booleanAttribute:
				mapped.Options = MapBooleanOptions(booleanAttribute, manifest).ToList();
				break;
			case EnumAttributeMetadata enumAttribute:
				mapped.Options = MapOptions(enumAttribute.OptionSet?.Options, manifest).ToList();
				break;
		}

		return mapped;
	}

	private static IEnumerable<MetadataStateStatus> MapStateStatus(EntityMetadata entity, MetadataGenerationManifest manifest)
	{
		var stateAttribute = entity.Attributes?.OfType<StateAttributeMetadata>().SingleOrDefault();
		var statusAttribute = entity.Attributes?.OfType<StatusAttributeMetadata>().SingleOrDefault();
		if (stateAttribute?.OptionSet?.Options is null)
		{
			return [];
		}

		var statusesByState = (statusAttribute?.OptionSet?.Options ?? [])
			.OfType<StatusOptionMetadata>()
			.Where(status => status.Value.HasValue)
			.GroupBy(status => status.State ?? -1)
			.ToDictionary(group => group.Key, group => group.OrderBy(status => status.Value).ToList());

		return stateAttribute.OptionSet.Options
			.OfType<StateOptionMetadata>()
			.Where(state => state.Value.HasValue)
			.OrderBy(state => state.Value)
			.Select(state =>
			{
				var stateValue = state.Value!.Value;
				statusesByState.TryGetValue(stateValue, out var statuses);

				return new MetadataStateStatus
				{
					State = stateValue,
					StateLabel = manifest.Metadata.IncludeOptionLabels ? GetLabel(state.Label) : null,
					DefaultStatus = state.DefaultStatus,
					Statuses = (statuses ?? [])
						.Where(status => status.Value.HasValue)
						.Select(status => new MetadataStatus
						{
							Value = status.Value!.Value,
							Label = manifest.Metadata.IncludeOptionLabels ? GetLabel(status.Label) : null
						})
						.ToList()
				};
			});
	}

	private static IEnumerable<MetadataAlternateKey> MapAlternateKeys(EntityMetadata entity)
	{
		return (entity.Keys ?? [])
			.Where(key => !string.IsNullOrWhiteSpace(key.LogicalName) || key.KeyAttributes?.Length > 0)
			.OrderBy(key => key.LogicalName, StringComparer.Ordinal)
			.Select(key => new MetadataAlternateKey
			{
				KeyName = key.LogicalName ?? key.SchemaName,
				EntityLogicalName = entity.LogicalName,
				AttributeLogicalNames = (key.KeyAttributes ?? [])
					.OrderBy(attribute => attribute, StringComparer.Ordinal)
					.ToList(),
				KeyStatus = key.EntityKeyIndexStatus.ToString()
			});
	}

	private static IEnumerable<MetadataRelationship> MapRelationships(EntityMetadata entity)
	{
		var relationships = new List<MetadataRelationship>();

		relationships.AddRange((entity.OneToManyRelationships ?? [])
			.Select(relationship => MapOneToManyRelationship(relationship, "one-to-many")));
		relationships.AddRange((entity.ManyToOneRelationships ?? [])
			.Select(relationship => MapOneToManyRelationship(relationship, "many-to-one")));
		relationships.AddRange((entity.ManyToManyRelationships ?? [])
			.Select(MapManyToManyRelationship));

		return relationships
			.OrderBy(relationship => relationship.SchemaName, StringComparer.Ordinal)
			.ThenBy(relationship => relationship.RelationshipType, StringComparer.Ordinal);
	}

	private static MetadataRelationship MapOneToManyRelationship(
		OneToManyRelationshipMetadata relationship,
		string relationshipType)
	{
		return new MetadataRelationship
		{
			SchemaName = relationship.SchemaName,
			RelationshipType = relationshipType,
			ReferencingEntity = relationship.ReferencingEntity,
			ReferencingAttribute = relationship.ReferencingAttribute,
			ReferencedEntity = relationship.ReferencedEntity,
			ReferencedAttribute = relationship.ReferencedAttribute,
			EntityRole = relationship.ReferencedEntityNavigationPropertyName,
			CascadeConfiguration = MapCascadeConfiguration(relationship.CascadeConfiguration)
		};
	}

	private static MetadataRelationship MapManyToManyRelationship(ManyToManyRelationshipMetadata relationship)
	{
		return new MetadataRelationship
		{
			SchemaName = relationship.SchemaName,
			RelationshipType = "many-to-many",
			ReferencingEntity = relationship.Entity1LogicalName,
			ReferencingAttribute = relationship.Entity1IntersectAttribute,
			ReferencedEntity = relationship.Entity2LogicalName,
			ReferencedAttribute = relationship.Entity2IntersectAttribute,
			IntersectEntity = relationship.IntersectEntityName
		};
	}

	private static IEnumerable<MetadataOption> MapBooleanOptions(
		BooleanAttributeMetadata attribute,
		MetadataGenerationManifest manifest)
	{
		var trueOption = attribute.OptionSet?.TrueOption;
		var falseOption = attribute.OptionSet?.FalseOption;

		return new[] { falseOption, trueOption }
			.Where(option => option is not null)
			.Select(option => new MetadataOption
			{
				Value = option!.Value,
				Label = manifest.Metadata.IncludeOptionLabels ? GetLabel(option.Label) : null
			})
			.OrderBy(option => option.Value);
	}

	private static IEnumerable<MetadataOption> MapOptions(
		IEnumerable<OptionMetadata>? options,
		MetadataGenerationManifest manifest)
	{
		return (options ?? [])
			.Where(option => option.Value.HasValue)
			.OrderBy(option => option.Value)
			.Select(option => new MetadataOption
			{
				Value = option.Value,
				Label = manifest.Metadata.IncludeOptionLabels ? GetLabel(option.Label) : null,
				State = option is StatusOptionMetadata statusOption ? statusOption.State : null
			});
	}

	private static List<string> GetValidMessages(EntityMetadata entity)
	{
		var messages = new List<string>();

		if ((entity.IsIntersect ?? false) == false)
		{
			messages.Add("create");
			messages.Add("update");
			messages.Add("delete");
		}

		messages.Add("retrieve");
		messages.Add("retrieveMultiple");
		messages.Add("associate");
		messages.Add("disassociate");

		return messages.OrderBy(message => message, StringComparer.Ordinal).ToList();
	}

	private static Dictionary<string, string?> MapCascadeConfiguration(CascadeConfiguration? cascadeConfiguration)
	{
		if (cascadeConfiguration is null)
		{
			return [];
		}

		return new Dictionary<string, string?>
		{
			["assign"] = cascadeConfiguration.Assign?.ToString(),
			["delete"] = cascadeConfiguration.Delete?.ToString(),
			["merge"] = cascadeConfiguration.Merge?.ToString(),
			["reparent"] = cascadeConfiguration.Reparent?.ToString(),
			["rollupView"] = cascadeConfiguration.RollupView?.ToString(),
			["share"] = cascadeConfiguration.Share?.ToString(),
			["unshare"] = cascadeConfiguration.Unshare?.ToString()
		};
	}

	private static object? GetDefaultValue(AttributeMetadata attribute)
	{
		return attribute switch
		{
			BooleanAttributeMetadata booleanAttribute => booleanAttribute.DefaultValue,
			PicklistAttributeMetadata picklistAttribute => picklistAttribute.DefaultFormValue,
			StateAttributeMetadata stateAttribute => stateAttribute.DefaultFormValue,
			StatusAttributeMetadata statusAttribute => statusAttribute.DefaultFormValue,
			_ => null
		};
	}

	private static string? GetLabel(Label? label)
	{
		return label?.UserLocalizedLabel?.Label
		       ?? label?.LocalizedLabels?.OrderBy(localizedLabel => localizedLabel.LanguageCode).FirstOrDefault()?.Label;
	}

	private static decimal? ToDecimal(double? value)
	{
		return value.HasValue ? Convert.ToDecimal(value.Value) : null;
	}

	private static string? GetStringProperty(object value, string propertyName)
	{
		return value.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)?.GetValue(value) as string;
	}
}
