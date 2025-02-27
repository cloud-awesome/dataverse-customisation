using CloudAwesome.Dataverse.Core.Models;
using CloudAwesome.Dataverse.Core.PlatformModels;

namespace CloudAwesome.Dataverse.Processes;

public enum ProcessActivationStatus { Enabled = 1, Disabled = 2 }

public class ProcessActivationManifest
{
	public ProcessActivationStatus Status { get; set; }
	public DataverseConnection DataverseConnection { get; set; }
	public LoggingConfiguration LoggingConfiguration { get; set; }

	public CdsPluginAssembly[] PluginAssemblies { get; set; } = Array.Empty<CdsPluginAssembly>();

	public CdsSolution[] Solutions { get; set; } = Array.Empty<CdsSolution>();

	public CdsEntity[] Entities { get; set; } = Array.Empty<CdsEntity>();

	public string[] Workflows { get; set; } = Array.Empty<string>();
	
	public string[] ModernFlows { get; set; } = Array.Empty<string>();
	
	public string[] RecordCreationRules { get; set; } = Array.Empty<string>();
}