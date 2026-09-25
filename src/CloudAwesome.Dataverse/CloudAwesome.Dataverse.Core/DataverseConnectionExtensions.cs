using CloudAwesome.Dataverse.Core.Authentication;
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

	public static IOrganizationService GetServiceClientWithInteractiveUser(DataverseConnection dataverseConnection)
	{
		var url = DataverseInteractiveAuthenticator.NormalizeEnvironmentUrl(dataverseConnection.Url);
		var authenticator = new DataverseInteractiveAuthenticator(dataverseConnection);

		return new ServiceClient(
			new Uri(url),
			_ => authenticator.GetAccessTokenAsync(),
			true
		);
	}

	public static IOrganizationService GetServiceClient(DataverseConnection dataverseConnection)
	{
		return dataverseConnection.ConnectionType switch
		{
			DataverseConnectionType.AppRegistration => GetServiceClient(dataverseConnection.Url!,
				dataverseConnection.ClientId!, dataverseConnection.ClientSecret!),

			DataverseConnectionType.ConnectionString => GetServiceClient(dataverseConnection.ConnectionString!),

			DataverseConnectionType.BearerToken => GetServiceClient(dataverseConnection.Url!,
				dataverseConnection.BearerToken!),

			DataverseConnectionType.InteractiveUser => GetServiceClientWithInteractiveUser(dataverseConnection),

			DataverseConnectionType.UserNameAndPassword => throw new NotSupportedException(
				"Username/password authentication is not supported. Use interactive user authentication with --url, or use an existing connection string, app registration, or bearer token."),
			_ => throw new ArgumentOutOfRangeException()
		};
	}
}
