using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Security;
using Microsoft.Xrm.Sdk;

namespace CloudAwesome.Dataverse.Customisation;

public class SecurityRoleAssignment
{
	public void Export(IOrganizationService client, TracingHelper t, ExportSecurityRolesManifest manifest)
	{
		var process = new ExportSecurityRoles();
		process.Run(manifest, client, t);
	}

	public void Import(IOrganizationService client, TracingHelper t, ImportSecurityRolesManifest manifest)
	{
		var process = new ImportSecurityRoles();
		process.Run(manifest, client, t);
	}
}