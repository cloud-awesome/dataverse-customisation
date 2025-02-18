using CloudAwesome.Dataverse.Cli.CommandInterfaces;
using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.Models;
using Microsoft.Crm.Sdk.Messages;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CloudAwesome.Dataverse.Cli.Commands;

public class WhoAmICommand: Command<WhoAmISettings>
{
	public override int Execute(CommandContext context, WhoAmISettings settings)
	{
		AnsiConsole.MarkupLine($"[green]Running 'who-am-i' command with placeholder:[/] {settings.ConnectionType}");

		if (settings.ConnectionType == DataverseConnectionType.BearerToken)
		{
			AnsiConsole.MarkupLine($"[red]{settings.ConnectionType} is not currently supported[/]");
			return 0;
		}
		
		var client = DataverseConnectionExtensions.GetServiceClient(settings.ConnectionString);
		var request = new WhoAmIRequest();

		var response = (WhoAmIResponse)client.Execute(request);
		
		AnsiConsole.MarkupLine($"[green]UserId:[/] {response.UserId}");
		AnsiConsole.MarkupLine($"[green]BusinessUnitId:[/] {response.BusinessUnitId}");
		AnsiConsole.MarkupLine($"[green]OrganizationId:[/] {response.OrganizationId}");
		
		return 0;
	}
}

public class WhoAmISettings : SupportsDataverseConnection
{
	
}