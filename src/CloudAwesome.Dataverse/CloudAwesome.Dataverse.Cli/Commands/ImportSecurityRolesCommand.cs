using CloudAwesome.Dataverse.Cli.CommandInterfaces;
using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.Models;
using CloudAwesome.Dataverse.Customisation;
using CloudAwesome.Dataverse.Security;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace CloudAwesome.Dataverse.Cli.Commands;

public class ImportSecurityRolesCommand: Command<ImportSecurityRolesSettings>
{
	public override int Execute(CommandContext context, ImportSecurityRolesSettings settings)
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

		var manifest = SerialisationWrapper.DeserialiseJsonFromFile<ImportSecurityRolesManifest>(settings.Manifest);
		var client = DataverseConnectionExtensions.GetServiceClient(settings.ConnectionDetails);
		
		var process = new SecurityRoleAssignment();
		process.Import(client, tracer, manifest);

		return 0;
	}
}

public class ImportSecurityRolesSettings : SupportsDataverseConnection
{
	[CommandOption("--manifest")]
	public string? Manifest { get; set; }
}