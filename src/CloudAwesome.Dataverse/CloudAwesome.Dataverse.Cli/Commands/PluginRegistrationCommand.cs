using CloudAwesome.Dataverse.Cli.CommandInterfaces;
using Spectre.Console.Cli;

namespace CloudAwesome.Dataverse.Cli.Commands;

public class PluginRegistrationCommand: Command<PluginRegistrationSettings>
{
	public override int Execute(CommandContext context, PluginRegistrationSettings settings)
	{
		Console.WriteLine("Just testing...");
		return 0;
	}
}

public class PluginRegistrationSettings : SupportsDataverseConnection, IRequiresManifest
{
	[CommandOption("--manifest")]
	public string? Manifest { get; set; }
	
	[CommandOption("--clobber")]
	public bool Clobber { get; set; } = false;
}