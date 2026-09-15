namespace CloudAwesome.Dataverse.Customisation.MetadataGeneration;

public class MetadataGenerationResult
{
	public string OutputPath { get; init; } = string.Empty;

	public bool ValidateOnly { get; init; }

	public int EntityCount { get; init; }

	public IReadOnlyCollection<string> WrittenFiles { get; init; } = [];
}
