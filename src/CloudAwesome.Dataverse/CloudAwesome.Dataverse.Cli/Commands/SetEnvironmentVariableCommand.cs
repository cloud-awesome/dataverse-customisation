using CloudAwesome.Dataverse.Cli.CommandInterfaces;
using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.Models;
using CloudAwesome.Dataverse.Customisation;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;

namespace CloudAwesome.Dataverse.Cli.Commands;

public class SetEnvironmentVariableCommand: Command<SetEnvironmentVariableSettings>
{
	public override int Execute(CommandContext context, SetEnvironmentVariableSettings settings)
	{
		if (string.IsNullOrEmpty(settings.VariableDefinitionName))
		{
			// This validation will be removed when manifest input is supported, and moved closer to the business logic 
			throw new Exception("Variable Definition Name has not been supplied");
		}
		
		if (string.IsNullOrEmpty(settings.VariableDefinitionValue))
		{
			// This validation will be removed when manifest input is supported, and moved closer to the business logic
			throw new Exception("Value has not been supplied");
		}
		
		var client = DataverseConnectionExtensions.GetServiceClient(settings.ConnectionDetails);

		var tracer = new TracingHelper(new LoggingConfiguration
		{
			LoggerConfigurationType = LoggingConfigurationType.Console,
			LogLevelToTrace = LogLevel.Debug
		});
		
		var process = new SetEnvironmentVariable();
		process.Run(client, tracer, settings.VariableDefinitionName, settings.VariableDefinitionValue);

		return 0;
	}
}

public class SetEnvironmentVariableSettings : SupportsDataverseConnection
{
	/*[CommandOption("--manifest")]
	public ProcessActivationManifest? Manifest { get; set; }*/
	
	[CommandOption("--variable")]
	public string VariableDefinitionName { get; set; }
	
	[CommandOption("--value")]
	public string VariableDefinitionValue { get; set; }
}