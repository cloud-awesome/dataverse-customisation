using CloudAwesome.Dataverse.Cli.CommandInterfaces;
using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.Models;
using CloudAwesome.Dataverse.Customisation;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CloudAwesome.Dataverse.Cli.Commands;

public class WhoAmICommand: Command<WhoAmISettings>
{
	public override int Execute(CommandContext context, WhoAmISettings settings)
	{
		var client = DataverseConnectionExtensions.GetServiceClient(settings.ConnectionDetails);

		var tracer = new TracingHelper(new LoggingConfiguration
		{
			LoggerConfigurationType = LoggingConfigurationType.Console,
			LogLevelToTrace = LogLevel.Debug
		});
		
		var whoAmI = new WhoAmI();
		whoAmI.Run(client, tracer);
		
		return 0;
	}
}

public class WhoAmISettings : SupportsDataverseConnection
{
	
}