using CloudAwesome.Dataverse.Core.Models;
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
	
	public static IOrganizationService GetServiceClient(string url, string bearerToken)
	{
		var serviceClient = new ServiceClient(
			new Uri(url),
			_ => Task.FromResult(bearerToken), 
			true
		);

		return serviceClient;
	}

	public static IOrganizationService GetServiceClient(DataverseConnection dataverseConnection)
	{
		return dataverseConnection.ConnectionType switch
		{
			DataverseConnectionType.AppRegistration => GetServiceClient(dataverseConnection.Url,
				dataverseConnection.ClientId, dataverseConnection.ClientSecret),
			
			DataverseConnectionType.ConnectionString => GetServiceClient(dataverseConnection.ConnectionString),
			
			DataverseConnectionType.BearerToken => GetServiceClient(dataverseConnection.Url,
				dataverseConnection.BearerToken),
			
			DataverseConnectionType.UserNameAndPassword => throw new NotImplementedException(),
			_ => throw new ArgumentOutOfRangeException()
		};
	}
}