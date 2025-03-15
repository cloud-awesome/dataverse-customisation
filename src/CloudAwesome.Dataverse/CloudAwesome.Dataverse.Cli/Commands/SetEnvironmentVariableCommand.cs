using CloudAwesome.Dataverse.Cli.CommandInterfaces;
using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.Models;
using CloudAwesome.Dataverse.Customisation;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;
using KeyValuePair = CloudAwesome.Dataverse.Customisation.KeyValuePair;

namespace CloudAwesome.Dataverse.Cli.Commands;

public class SetEnvironmentVariableCommand: Command<SetEnvironmentVariableSettings>
{
	public override int Execute(CommandContext context, SetEnvironmentVariableSettings settings)
	{
		var client = DataverseConnectionExtensions.GetServiceClient(settings.ConnectionDetails);
		
		var tracer = new TracingHelper(new LoggingConfiguration
		{
			LoggerConfigurationType = LoggingConfigurationType.Console,
			LogLevelToTrace = LogLevel.Debug
		});

		var manifest = settings.Manifest is null 
			? new SetEnvironmentVariableManifest() 
			: SerialisationWrapper.DeserialiseJsonFromFile<SetEnvironmentVariableManifest>(settings.Manifest);

		if (settings.VariableDefinitionName is not null && settings.VariableDefinitionValue is not null)
		{
			manifest.Variables.Add(new KeyValuePair
			{
				Key = settings.VariableDefinitionName,
				Value = settings.VariableDefinitionValue
			});
		}
		
		var process = new SetEnvironmentVariable();
		//process.Run(client, tracer, settings.VariableDefinitionName, settings.VariableDefinitionValue);
		process.Run(client, tracer, manifest);

		return 0;
	}
}

public class SetEnvironmentVariableSettings : SupportsDataverseConnection
{
	[CommandOption("--manifest")]
	public string? Manifest { get; set; }
	
	[CommandOption("--variable")]
	public string? VariableDefinitionName { get; set; }
	
	[CommandOption("--value")]
	public string? VariableDefinitionValue { get; set; }
}