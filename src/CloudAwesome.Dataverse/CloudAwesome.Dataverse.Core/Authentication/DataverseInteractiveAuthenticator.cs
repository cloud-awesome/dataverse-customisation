using CloudAwesome.Dataverse.Core.Models;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;

namespace CloudAwesome.Dataverse.Core.Authentication;

public sealed class DataverseInteractiveAuthenticator
{
	public const string DefaultClientId = "ec8c9d8a-ac00-4871-9c4a-0696f1bac679";
	private const string CacheFileName = "cloudawesome-dataverse-msal.cache";
	private const string RedirectUri = "http://localhost";

	private readonly DataverseConnection _connection;
	private readonly SemaphoreSlim _initialiseLock = new(1, 1);
	private IPublicClientApplication? _app;

	public DataverseInteractiveAuthenticator(DataverseConnection connection)
	{
		_connection = connection;
	}

	public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
	{
		var app = await GetApplicationAsync().ConfigureAwait(false);
		var scopes = GetScopes(_connection.Url);
		var accounts = await app.GetAccountsAsync().ConfigureAwait(false);

		try
		{
			var silentResult = await app
				.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
				.ExecuteAsync(cancellationToken)
				.ConfigureAwait(false);

			return silentResult.AccessToken;
		}
		catch (MsalUiRequiredException)
		{
			var interactiveResult = await app
				.AcquireTokenInteractive(scopes)
				.WithPrompt(Prompt.SelectAccount)
				.ExecuteAsync(cancellationToken)
				.ConfigureAwait(false);

			return interactiveResult.AccessToken;
		}
	}

	public static string[] GetScopes(string? environmentUrl)
	{
		return [$"{NormalizeEnvironmentUrl(environmentUrl)}/user_impersonation"];
	}

	public static string NormalizeEnvironmentUrl(string? environmentUrl)
	{
		if (string.IsNullOrWhiteSpace(environmentUrl))
		{
			throw new InvalidOperationException(
				"A Dataverse environment URL is required for interactive user authentication.");
		}

		var trimmedUrl = environmentUrl.Trim().TrimEnd('/');
		if (!Uri.TryCreate(trimmedUrl, UriKind.Absolute, out var uri) ||
		    (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp))
		{
			throw new InvalidOperationException(
				$"'{environmentUrl}' is not a valid Dataverse environment URL.");
		}

		return uri.ToString().TrimEnd('/');
	}

	private async Task<IPublicClientApplication> GetApplicationAsync()
	{
		if (_app is not null)
		{
			return _app;
		}

		await _initialiseLock.WaitAsync().ConfigureAwait(false);
		try
		{
			if (_app is not null)
			{
				return _app;
			}

			var clientId = string.IsNullOrWhiteSpace(_connection.ClientId)
				? DefaultClientId
				: _connection.ClientId;

			var tenantId = string.IsNullOrWhiteSpace(_connection.TenantId)
				? "common"
				: _connection.TenantId;

			var app = PublicClientApplicationBuilder
				.Create(clientId)
				.WithAuthority($"https://login.microsoftonline.com/{tenantId}")
				.WithRedirectUri(RedirectUri)
				.Build();

			var cacheHelper = await MsalCacheHelper
				.CreateAsync(BuildCacheProperties())
				.ConfigureAwait(false);

			cacheHelper.RegisterCache(app.UserTokenCache);
			_app = app;

			return _app;
		}
		finally
		{
			_initialiseLock.Release();
		}
	}

	private static StorageCreationProperties BuildCacheProperties()
	{
		var cacheDirectory = Path.Combine(
			GetApplicationDataDirectory(),
			"CloudAwesome",
			"DataverseCli",
			"msal");

		Directory.CreateDirectory(cacheDirectory);

		return new StorageCreationPropertiesBuilder(CacheFileName, cacheDirectory)
			.WithMacKeyChain("CloudAwesome.Dataverse.Cli", CacheFileName)
			.WithLinuxKeyring(
				"com.cloudawesome.dataverse.cli",
				"default",
				"CloudAwesome Dataverse CLI MSAL token cache",
				new KeyValuePair<string, string>("Product", "CloudAwesome.Dataverse.Cli"),
				new KeyValuePair<string, string>("Version", "1"))
			.Build();
	}

	private static string GetApplicationDataDirectory()
	{
		var localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

		return string.IsNullOrWhiteSpace(localApplicationData)
			? MsalCacheHelper.UserRootDirectory
			: localApplicationData;
	}
}
