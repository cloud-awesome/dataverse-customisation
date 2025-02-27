using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Processes;
using Microsoft.Xrm.Sdk;

namespace CloudAwesome.Dataverse.Customisation;

public class ProcessActivation
{
	public void Run(IOrganizationService client, TracingHelper t, ProcessActivationManifest manifest)
	{
		var process = new CloudAwesome.Dataverse.Processes.ProcessActivation();
		process.SetStatusFromManifest(manifest, client, t);
	}
}