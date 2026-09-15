using System.Text.Json.Serialization;

namespace CloudAwesome.Dataverse.Customisation.MetadataGeneration;

public class MetadataDocument
{
	[JsonPropertyName("$schema")]
	public string Schema { get; set; } = MetadataGenerationConstants.MetadataSchemaUrl;

	[JsonPropertyName("contractVersion")]
	public string ContractVersion { get; set; } = MetadataGenerationConstants.ContractVersion;

	[JsonPropertyName("generatedOnUtc")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public DateTimeOffset? GeneratedOnUtc { get; set; }

	[JsonPropertyName("source")]
	public MetadataSource Source { get; set; } = new();

	[JsonPropertyName("entities")]
	public List<MetadataEntity> Entities { get; set; } = [];
}

public class MetadataIndexDocument
{
	[JsonPropertyName("$schema")]
	public string Schema { get; set; } = MetadataGenerationConstants.MetadataSchemaUrl;

	[JsonPropertyName("contractVersion")]
	public string ContractVersion { get; set; } = MetadataGenerationConstants.ContractVersion;

	[JsonPropertyName("generatedOnUtc")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public DateTimeOffset? GeneratedOnUtc { get; set; }

	[JsonPropertyName("source")]
	public MetadataSource Source { get; set; } = new();

	[JsonPropertyName("entityFiles")]
	public List<MetadataEntityFile> EntityFiles { get; set; } = [];
}

public class MetadataEntityFile
{
	[JsonPropertyName("logicalName")]
	public string LogicalName { get; set; } = string.Empty;

	[JsonPropertyName("path")]
	public string Path { get; set; } = string.Empty;
}

public class MetadataEntityDocument
{
	[JsonPropertyName("$schema")]
	public string Schema { get; set; } = MetadataGenerationConstants.EntityMetadataSchemaUrl;

	[JsonPropertyName("contractVersion")]
	public string ContractVersion { get; set; } = MetadataGenerationConstants.ContractVersion;

	[JsonPropertyName("entity")]
	public MetadataEntity Entity { get; set; } = new();
}

public class MetadataSource
{
	[JsonPropertyName("environmentUrl")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? EnvironmentUrl { get; set; }

	[JsonPropertyName("publisher")]
	public string Publisher { get; set; } = MetadataGenerationConstants.Publisher;

	[JsonPropertyName("publisherVersion")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? PublisherVersion { get; set; }
}

public class MetadataEntity
{
	[JsonPropertyName("logicalName")]
	public string LogicalName { get; set; } = string.Empty;

	[JsonPropertyName("schemaName")]
	public string? SchemaName { get; set; }

	[JsonPropertyName("collectionLogicalName")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? CollectionLogicalName { get; set; }

	[JsonPropertyName("collectionSchemaName")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? CollectionSchemaName { get; set; }

	[JsonPropertyName("primaryIdAttribute")]
	public string? PrimaryIdAttribute { get; set; }

	[JsonPropertyName("primaryNameAttribute")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? PrimaryNameAttribute { get; set; }

	[JsonPropertyName("ownershipType")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? OwnershipType { get; set; }

	[JsonPropertyName("isActivity")]
	public bool IsActivity { get; set; }

	[JsonPropertyName("isIntersect")]
	public bool IsIntersect { get; set; }

	[JsonPropertyName("validMessages")]
	public List<string> ValidMessages { get; set; } = [];

	[JsonPropertyName("attributes")]
	public List<MetadataAttribute> Attributes { get; set; } = [];

	[JsonPropertyName("stateStatus")]
	public List<MetadataStateStatus> StateStatus { get; set; } = [];

	[JsonPropertyName("alternateKeys")]
	public List<MetadataAlternateKey> AlternateKeys { get; set; } = [];

	[JsonPropertyName("relationships")]
	public List<MetadataRelationship> Relationships { get; set; } = [];
}

public class MetadataAttribute
{
	[JsonPropertyName("logicalName")]
	public string LogicalName { get; set; } = string.Empty;

	[JsonPropertyName("schemaName")]
	public string? SchemaName { get; set; }

	[JsonPropertyName("attributeType")]
	public string? AttributeType { get; set; }

	[JsonPropertyName("attributeTypeName")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? AttributeTypeName { get; set; }

	[JsonPropertyName("requiredLevel")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? RequiredLevel { get; set; }

	[JsonPropertyName("isPrimaryId")]
	public bool IsPrimaryId { get; set; }

	[JsonPropertyName("isPrimaryName")]
	public bool IsPrimaryName { get; set; }

	[JsonPropertyName("isValidForCreate")]
	public bool? IsValidForCreate { get; set; }

	[JsonPropertyName("isValidForUpdate")]
	public bool? IsValidForUpdate { get; set; }

	[JsonPropertyName("isValidForRead")]
	public bool? IsValidForRead { get; set; }

	[JsonPropertyName("isSecured")]
	public bool? IsSecured { get; set; }

	[JsonPropertyName("lookupTargets")]
	public List<string> LookupTargets { get; set; } = [];

	[JsonPropertyName("maxLength")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public int? MaxLength { get; set; }

	[JsonPropertyName("format")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? Format { get; set; }

	[JsonPropertyName("minValue")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public decimal? MinValue { get; set; }

	[JsonPropertyName("maxValue")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public decimal? MaxValue { get; set; }

	[JsonPropertyName("precision")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public int? Precision { get; set; }

	[JsonPropertyName("dateTimeBehavior")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? DateTimeBehavior { get; set; }

	[JsonPropertyName("defaultValue")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public object? DefaultValue { get; set; }

	[JsonPropertyName("options")]
	public List<MetadataOption> Options { get; set; } = [];
}

public class MetadataOption
{
	[JsonPropertyName("value")]
	public int? Value { get; set; }

	[JsonPropertyName("label")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? Label { get; set; }

	[JsonPropertyName("state")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public int? State { get; set; }
}

public class MetadataStateStatus
{
	[JsonPropertyName("state")]
	public int State { get; set; }

	[JsonPropertyName("stateLabel")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? StateLabel { get; set; }

	[JsonPropertyName("defaultStatus")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public int? DefaultStatus { get; set; }

	[JsonPropertyName("statuses")]
	public List<MetadataStatus> Statuses { get; set; } = [];
}

public class MetadataStatus
{
	[JsonPropertyName("value")]
	public int Value { get; set; }

	[JsonPropertyName("label")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? Label { get; set; }
}

public class MetadataRelationship
{
	[JsonPropertyName("schemaName")]
	public string? SchemaName { get; set; }

	[JsonPropertyName("relationshipType")]
	public string RelationshipType { get; set; } = string.Empty;

	[JsonPropertyName("referencingEntity")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? ReferencingEntity { get; set; }

	[JsonPropertyName("referencingAttribute")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? ReferencingAttribute { get; set; }

	[JsonPropertyName("referencedEntity")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? ReferencedEntity { get; set; }

	[JsonPropertyName("referencedAttribute")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? ReferencedAttribute { get; set; }

	[JsonPropertyName("intersectEntity")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? IntersectEntity { get; set; }

	[JsonPropertyName("entityRole")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? EntityRole { get; set; }

	[JsonPropertyName("cascadeConfiguration")]
	public Dictionary<string, string?> CascadeConfiguration { get; set; } = [];
}

public class MetadataAlternateKey
{
	[JsonPropertyName("keyName")]
	public string? KeyName { get; set; }

	[JsonPropertyName("entityLogicalName")]
	public string? EntityLogicalName { get; set; }

	[JsonPropertyName("attributeLogicalNames")]
	public List<string> AttributeLogicalNames { get; set; } = [];

	[JsonPropertyName("keyStatus")]
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
	public string? KeyStatus { get; set; }
}

public static class MetadataGenerationConstants
{
	public const string ContractVersion = "1.0";
	public const string Publisher = "CloudAwesome.Dataverse.Cli";
	public const string MetadataSchemaUrl = "https://schemas.cloudawesome.dev/dataverse-simulate/metadata/v1/dataverse-simulate.metadata.schema.json";
	public const string EntityMetadataSchemaUrl = "https://schemas.cloudawesome.dev/dataverse-simulate/metadata/v1/dataverse-simulate.metadata.entity.schema.json";
	public const string ManifestSchemaUrl = "https://schemas.cloudawesome.dev/dataverse-simulate/metadata/v1/dataverse-simulate.metadata-manifest.schema.json";
}
