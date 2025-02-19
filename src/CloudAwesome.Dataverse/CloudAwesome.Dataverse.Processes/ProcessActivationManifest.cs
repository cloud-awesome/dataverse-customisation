using CloudAwesome.Dataverse.Core.Models;
using CloudAwesome.Dataverse.Core.PlatformModels;

namespace CloudAwesome.Dataverse.Processes;

public enum ProcessActivationStatus { Enabled = 1, Disabled = 2 }

public class ProcessActivationManifest
{
	public ProcessActivationStatus Status { get; set; }
	public DataverseConnection DataverseConnection { get; set; }
	public LoggingConfiguration LoggingConfiguration { get; set; }

	public CdsPluginAssembly[] PluginAssemblies { get; set; }

	public CdsSolution[] Solutions { get; set; }

	public CdsEntity[] Entities { get; set; }

	public string[] Workflows { get; set; }
	
	public string[] ModernFlows { get; set; }
	
	public string[] RecordCreationRules { get; set; }
}