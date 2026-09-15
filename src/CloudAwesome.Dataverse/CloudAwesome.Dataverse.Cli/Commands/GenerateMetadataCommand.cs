using System.Reflection;
using CloudAwesome.Dataverse.Cli.CommandInterfaces;
using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.Models;
using CloudAwesome.Dataverse.Customisation.MetadataGeneration;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace CloudAwesome.Dataverse.Cli.Commands;

public class GenerateMetadataCommand : Command<GenerateMetadataSettings>
{
	protected override int Execute(CommandContext context, GenerateMetadataSettings settings, CancellationToken cancellationToken)
	{
		var tracer = new TracingHelper(new LoggingConfiguration
		{
			LoggerConfigurationType = LoggingConfigurationType.Console,
			LogLevelToTrace = LogLevel.Debug
		});

		if (settings.Manifest is null)
		{
			tracer.Error("Manifest path has not been provided");
			return -1;
		}

		try
		{
			var manifest = SerialisationWrapper.DeserialiseJsonFromFile<MetadataGenerationManifest>(settings.Manifest);
			var client = DataverseConnectionExtensions.GetServiceClient(settings.ConnectionDetails);

			var process = new GenerateMetadata();
			var result = process.Run(client, tracer, manifest, new MetadataGenerationRunOptions
			{
				OutputPathOverride = settings.Output,
				IncludeEntityOverrides = settings.IncludeEntity,
				SplitFilesPerEntityOverride = settings.SplitFilesPerEntity ? true : null,
				ValidateOnly = settings.ValidateOnly,
				FailOnUnsupportedMetadata = settings.FailOnUnsupportedMetadata,
				EnvironmentUrl = settings.Environment ?? settings.Url,
				PublisherVersion = GetCliVersion()
			});

			if (!result.ValidateOnly)
			{
				tracer.Info($"Metadata written to {result.OutputPath}");
			}

			return 0;
		}
		catch (Exception exception)
		{
			tracer.Error(exception.Message);
			return -1;
		}
	}

	private static string? GetCliVersion()
	{
		return typeof(Program).Assembly
			.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
			.InformationalVersion
			.Split('+')[0];
	}
}

public class GenerateMetadataSettings : SupportsDataverseConnection
{
	[CommandOption("--manifest")]
	public string? Manifest { get; set; }

	[CommandOption("--output")]
	public string? Output { get; set; }

	[CommandOption("--environment")]
	public string? Environment
	{
		get => ConnectionDetails.Url;
		set => ConnectionDetails.Url = value;
	}

	[CommandOption("--include-entity")]
	public string[] IncludeEntity { get; set; } = [];

	[CommandOption("--split-files-per-entity")]
	public bool SplitFilesPerEntity { get; set; }

	[CommandOption("--validate-only")]
	public bool ValidateOnly { get; set; }

	[CommandOption("--fail-on-unsupported-metadata")]
	public bool FailOnUnsupportedMetadata { get; set; }
}
