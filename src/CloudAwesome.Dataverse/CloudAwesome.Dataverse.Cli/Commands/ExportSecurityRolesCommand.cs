using CloudAwesome.Dataverse.Cli.CommandInterfaces;
using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.Models;
using CloudAwesome.Dataverse.Customisation;
using CloudAwesome.Dataverse.Security;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace CloudAwesome.Dataverse.Cli.Commands;

public class ExportSecurityRolesCommand: Command<ExportSecurityRolesSettings>
{
	public override int Execute(CommandContext context, ExportSecurityRolesSettings settings)
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

		var manifest = SerialisationWrapper.DeserialiseJsonFromFile<ExportSecurityRolesManifest>(settings.Manifest);

		if (string.IsNullOrEmpty(settings.OutputFilePath) && string.IsNullOrEmpty(manifest.OutputFilePath))
		{
			tracer.Error("OutputFilePath has not been provided (either in the manifest or as a command line argument)");
			return -1;
		}
		
		var client = DataverseConnectionExtensions.GetServiceClient(settings.ConnectionDetails);
		
		var process = new SecurityRoleAssignment();
		process.Export(client, tracer, manifest);

		return 0;
	}
}

public class ExportSecurityRolesSettings : SupportsDataverseConnection
{
	[CommandOption("--manifest")]
	public string? Manifest { get; set; }
	
	[CommandOption("--output-filepath")]
	public string? OutputFilePath { get; set; }
	
}