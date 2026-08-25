using CloudAwesome.Dataverse.Cli.CommandInterfaces;
using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.Models;
using CloudAwesome.Dataverse.Customisation;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace CloudAwesome.Dataverse.Cli.Commands;

public class GenerateCustomisationsCommand: Command<GenerateCustomisationsSettings>
{
	protected override int Execute(CommandContext context, GenerateCustomisationsSettings settings, CancellationToken cancellationToken)
	{
		var client = DataverseConnectionExtensions.GetServiceClient(settings.ConnectionDetails);
		
		var tracer = new TracingHelper(new LoggingConfiguration
		{
			LoggerConfigurationType = LoggingConfigurationType.Console,
			LogLevelToTrace = LogLevel.Debug
		});

		var process = new GenerateConfigurations();
		process.Run(client, tracer, settings.Manifest ?? throw new InvalidOperationException("A manifest is required."));
		
		return 0;
	}
}

public class GenerateCustomisationsSettings : SupportsDataverseConnection
{
	[CommandOption("--manifest")]
	public GenerateCustomisationsManifest? Manifest { get; set; }
}
