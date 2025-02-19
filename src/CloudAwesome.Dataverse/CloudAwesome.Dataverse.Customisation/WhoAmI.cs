using CloudAwesome.Dataverse.Core;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace CloudAwesome.Dataverse.Customisation;

public class WhoAmI
{
	public void Run(IOrganizationService client, TracingHelper t)
	{
		var request = new WhoAmIRequest();
		var response = (WhoAmIResponse) client.Execute(request);
		
		t.Info($"UserId: {response.UserId}");
		t.Info($"BusinessUnitId: {response.BusinessUnitId}");
		t.Info($"OrganizationId: {response.OrganizationId}");
	}
}