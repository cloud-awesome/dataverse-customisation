using System.Text.Json.Serialization;

namespace CloudAwesome.Dataverse.Customisation.MetadataGeneration;

public class MetadataGenerationManifest
{
	[JsonPropertyName("$schema")]
	public string? Schema { get; set; }

	[JsonPropertyName("output")]
	public MetadataGenerationOutputOptions Output { get; set; } = new();

	[JsonPropertyName("entities")]
	public MetadataGenerationEntityOptions Entities { get; set; } = new();

	[JsonPropertyName("metadata")]
	public MetadataGenerationMetadataOptions Metadata { get; set; } = new();
}

public class MetadataGenerationOutputOptions
{
	[JsonPropertyName("path")]
	public string Path { get; set; } = string.Empty;

	[JsonPropertyName("splitFilesPerEntity")]
	public bool SplitFilesPerEntity { get; set; }

	[JsonPropertyName("includeGeneratedOnUtc")]
	public bool IncludeGeneratedOnUtc { get; set; } = true;
}

public class MetadataGenerationEntityOptions
{
	[JsonPropertyName("include")]
	public string[] Include { get; set; } = [];

	[JsonPropertyName("includeAllEntities")]
	public bool IncludeAllEntities { get; set; }

	[JsonPropertyName("includeRelatedEntities")]
	public bool IncludeRelatedEntities { get; set; }

	[JsonPropertyName("includeIntersectEntities")]
	public bool IncludeIntersectEntities { get; set; } = true;
}

public class MetadataGenerationMetadataOptions
{
	[JsonPropertyName("includeAttributes")]
	public bool IncludeAttributes { get; set; } = true;

	[JsonPropertyName("includeRelationships")]
	public bool IncludeRelationships { get; set; } = true;

	[JsonPropertyName("includeAlternateKeys")]
	public bool IncludeAlternateKeys { get; set; } = true;

	[JsonPropertyName("includeOptionLabels")]
	public bool IncludeOptionLabels { get; set; }

	[JsonPropertyName("includeUnpublished")]
	public bool IncludeUnpublished { get; set; }
}
