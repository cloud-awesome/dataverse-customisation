using System.ComponentModel;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CloudAwesome.Dataverse.Cli.Commands;

public class PlaceholderCommand: Command<PlaceholderSettings>
{
	protected override int Execute(CommandContext context, PlaceholderSettings settings, CancellationToken cancellationToken)
	{
		AnsiConsole.MarkupLine($"[green]Running 'placeholder' command with placeholder:[/] {settings.Text}");
		return 0;
	}
}

public class PlaceholderSettings : CommandSettings
{
	[CommandArgument(0, "<text>")]
	[Description("Just any text for a placeholder")]
	public string Text { get; set; }
}
