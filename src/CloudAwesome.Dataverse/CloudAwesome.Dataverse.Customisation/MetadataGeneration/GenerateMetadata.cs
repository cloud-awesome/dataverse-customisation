using System.Text.Json;
using System.Text.Json.Serialization;
using CloudAwesome.Dataverse.Core;
using Microsoft.Xrm.Sdk;

namespace CloudAwesome.Dataverse.Customisation.MetadataGeneration;

public class GenerateMetadata
{
	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		WriteIndented = true,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
		Converters =
		{
			new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
		}
	};

	private readonly IMetadataProvider _metadataProvider;
	private readonly MetadataDocumentBuilder _documentBuilder;

	public GenerateMetadata()
		: this(new DataverseMetadataProvider(), new MetadataDocumentBuilder())
	{
	}

	public GenerateMetadata(IMetadataProvider metadataProvider, MetadataDocumentBuilder? documentBuilder = null)
	{
		_metadataProvider = metadataProvider;
		_documentBuilder = documentBuilder ?? new MetadataDocumentBuilder();
	}

	public MetadataGenerationResult Run(
		IOrganizationService client,
		TracingHelper tracer,
		MetadataGenerationManifest manifest,
		MetadataGenerationRunOptions? options = null)
	{
		options ??= new MetadataGenerationRunOptions();
		ApplyOverrides(manifest, options);
		MetadataOutputValidator.ValidateManifest(manifest);

		if (options.ValidateOnly)
		{
			tracer.Info("Metadata generation manifest and connection were validated. No files were written.");
			return new MetadataGenerationResult
			{
				OutputPath = manifest.Output.Path,
				ValidateOnly = true
			};
		}

		var sourceEntities = _metadataProvider.RetrieveEntities(client, manifest);
		var document = _documentBuilder.Build(sourceEntities, manifest, options, DateTimeOffset.UtcNow);
		MetadataOutputValidator.ValidateDocument(document);

		var writtenFiles = manifest.Output.SplitFilesPerEntity
			? WriteSplitDocuments(manifest.Output.Path, document)
			: WriteCombinedDocument(manifest.Output.Path, document);

		tracer.Info($"Generated Dataverse metadata for {document.Entities.Count} entities.");

		return new MetadataGenerationResult
		{
			OutputPath = manifest.Output.Path,
			EntityCount = document.Entities.Count,
			WrittenFiles = writtenFiles
		};
	}

	public static void ApplyOverrides(MetadataGenerationManifest manifest, MetadataGenerationRunOptions options)
	{
		if (!string.IsNullOrWhiteSpace(options.OutputPathOverride))
		{
			manifest.Output.Path = options.OutputPathOverride;
		}

		if (options.SplitFilesPerEntityOverride.HasValue)
		{
			manifest.Output.SplitFilesPerEntity = options.SplitFilesPerEntityOverride.Value;
		}

		if (options.IncludeEntityOverrides.Count > 0)
		{
			manifest.Entities.IncludeAllEntities = false;
			manifest.Entities.Include = options.IncludeEntityOverrides
				.SelectMany(SplitEntityOverride)
				.Select(entity => entity.Trim())
				.Where(entity => !string.IsNullOrWhiteSpace(entity))
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.OrderBy(entity => entity, StringComparer.Ordinal)
				.ToArray();
		}
	}

	private static IReadOnlyCollection<string> WriteCombinedDocument(string outputPath, MetadataDocument document)
	{
		EnsureOutputDirectory(outputPath);
		WriteJson(outputPath, document);
		return [Path.GetFullPath(outputPath)];
	}

	private static IReadOnlyCollection<string> WriteSplitDocuments(string outputPath, MetadataDocument document)
	{
		EnsureOutputDirectory(outputPath);

		var rootDirectory = Path.GetDirectoryName(Path.GetFullPath(outputPath)) ?? Directory.GetCurrentDirectory();
		var entityDirectory = Path.Combine(rootDirectory, "entities");
		Directory.CreateDirectory(entityDirectory);

		var writtenFiles = new List<string>();
		var index = new MetadataIndexDocument
		{
			GeneratedOnUtc = document.GeneratedOnUtc,
			Source = document.Source
		};

		foreach (var entity in document.Entities.OrderBy(entity => entity.LogicalName, StringComparer.Ordinal))
		{
			var entityFileName = $"{SanitiseFileName(entity.LogicalName)}.json";
			var relativePath = Path.Combine("entities", entityFileName).Replace('\\', '/');
			var entityPath = Path.Combine(entityDirectory, entityFileName);

			WriteJson(entityPath, new MetadataEntityDocument { Entity = entity });
			writtenFiles.Add(Path.GetFullPath(entityPath));

			index.EntityFiles.Add(new MetadataEntityFile
			{
				LogicalName = entity.LogicalName,
				Path = relativePath
			});
		}

		WriteJson(outputPath, index);
		writtenFiles.Insert(0, Path.GetFullPath(outputPath));

		return writtenFiles;
	}

	private static IEnumerable<string> SplitEntityOverride(string value)
	{
		return value.Split([';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
	}

	private static void EnsureOutputDirectory(string outputPath)
	{
		var directory = Path.GetDirectoryName(Path.GetFullPath(outputPath));
		if (!string.IsNullOrWhiteSpace(directory))
		{
			Directory.CreateDirectory(directory);
		}
	}

	private static string SanitiseFileName(string logicalName)
	{
		var invalidCharacters = Path.GetInvalidFileNameChars().ToHashSet();
		return new string(logicalName.Select(character => invalidCharacters.Contains(character) ? '_' : character).ToArray());
	}

	private static void WriteJson<T>(string path, T value)
	{
		using var stream = File.Create(path);
		JsonSerializer.Serialize(stream, value, JsonOptions);
	}
}
