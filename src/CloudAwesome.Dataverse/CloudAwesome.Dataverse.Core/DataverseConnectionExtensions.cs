using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;

namespace CloudAwesome.Dataverse.Core;

public static class DataverseConnectionExtensions
{
	public static IOrganizationService GetServiceClient(string connectionString)
	{
		return new ServiceClient(connectionString);
	}

	public static IOrganizationService GetServiceClient(string url, string appId, string appSecret)
	{
		var connectionString =
			"AuthType=ClientSecret;" +
			$"ClientId={appId};" +
			$"ClientSecret='{appSecret}';" +
			$"Url={url}";
		return GetServiceClient(connectionString);
	}
	
	public static IOrganizationService GetCrmServiceClientWithBearerToken(string url, string bearerToken)
	{
		var serviceClient = new ServiceClient(
			new Uri(url),
			_ => Task.FromResult(bearerToken), 
			true
		);

		return serviceClient;
	}
}