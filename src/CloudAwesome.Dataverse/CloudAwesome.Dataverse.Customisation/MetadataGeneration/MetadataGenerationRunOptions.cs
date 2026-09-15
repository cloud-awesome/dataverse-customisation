namespace CloudAwesome.Dataverse.Customisation.MetadataGeneration;

public class MetadataGenerationRunOptions
{
	public string? OutputPathOverride { get; set; }

	public IReadOnlyCollection<string> IncludeEntityOverrides { get; set; } = [];

	public bool? SplitFilesPerEntityOverride { get; set; }

	public bool ValidateOnly { get; set; }

	public bool FailOnUnsupportedMetadata { get; set; }

	public string? EnvironmentUrl { get; set; }

	public string? PublisherVersion { get; set; }
}
