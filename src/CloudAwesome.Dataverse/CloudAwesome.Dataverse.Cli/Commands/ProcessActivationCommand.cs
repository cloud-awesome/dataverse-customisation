using System.ComponentModel;
using CloudAwesome.Dataverse.Cli.CommandInterfaces;
using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.Models;
using CloudAwesome.Dataverse.Core.PlatformModels;
using CloudAwesome.Dataverse.Processes;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;
using ProcessActivation = CloudAwesome.Dataverse.Customisation.ProcessActivation;

namespace CloudAwesome.Dataverse.Cli.Commands;

public class ProcessActivationCommand: Command<ProcessActivationSettings>
{
	public override int Execute(CommandContext context, ProcessActivationSettings settings)
	{
		var client = DataverseConnectionExtensions.GetServiceClient(settings.ConnectionDetails);

		var activate = (bool) (context.Data ?? true) 
			? ProcessActivationStatus.Enabled 
			: ProcessActivationStatus.Disabled;

		var tracer = new TracingHelper(new LoggingConfiguration
		{
			LoggerConfigurationType = LoggingConfigurationType.Console,
			LogLevelToTrace = LogLevel.Debug
		});

		ProcessActivationManifest manifest;
		if (settings.Manifest is null)
		{
			manifest = new ProcessActivationManifest
			{
				Solutions =
				[
					new CdsSolution
					{
						Name = settings.SolutionName,
						AllFlows = settings.AllFlows,
						AllPluginSteps = settings.AllPluginSteps,
						AllSlas = settings.AllSlas,
						SetSlasAsDefault = settings.SetSlasAsDefault
					}
				],
				Status = activate
			};
		}
		else
		{
			manifest = settings.Manifest;
		}
		
		var process = new ProcessActivation();
		process.Run(client, tracer, manifest);

		return 0;
	}
}

public class ProcessActivationSettings : SupportsDataverseConnection
{
	[CommandOption("--manifest")]
	public ProcessActivationManifest? Manifest { get; set; }
	
	[CommandOption("--solution")]
	public string SolutionName { get; set; }
	
	[CommandOption("--all-flows")]
	public bool AllFlows { get; set; } = false;
	
	[CommandOption("--all-plugins")]
	public bool AllPluginSteps { get; set; } = false;
	
	[CommandOption("--all-slas")]
	public bool AllSlas { get; set; } = false;
	
	[CommandOption("--default-slas")]
	[DefaultValue(true)]
	public bool SetSlasAsDefault { get; set; } = true;
}