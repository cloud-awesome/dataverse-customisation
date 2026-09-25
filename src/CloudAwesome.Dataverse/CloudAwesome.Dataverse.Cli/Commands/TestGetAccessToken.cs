using CloudAwesome.Dataverse.Core.Authentication;
using CloudAwesome.Dataverse.Core.Models;
using Microsoft.Identity.Client;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CloudAwesome.Dataverse.Cli.Commands;

public class TestGetAccessToken: Command<TestGetAccessTokenSettings>
{
	protected override int Execute(CommandContext context, TestGetAccessTokenSettings settings, CancellationToken cancellationToken)
	{
		try
		{
			var connection = new DataverseConnection
			{
				ConnectionType = DataverseConnectionType.InteractiveUser,
				Url = settings.Url,
				TenantId = settings.TenantId,
				ClientId = settings.ClientId
			};

			var authenticator = new DataverseInteractiveAuthenticator(connection);
			var accessToken = authenticator.GetAccessTokenAsync(cancellationToken)
				.GetAwaiter()
				.GetResult();

			Console.WriteLine(accessToken);
			return 0;
		}
		catch (MsalServiceException ex) when (ex.ErrorCode == "invalid_client")
		{
			Console.Error.WriteLine("Authentication failed: invalid client ID. Check that the public-client app registration is configured correctly.");
		}
		catch (MsalClientException ex) when (ex.ErrorCode == "authentication_canceled")
		{
			Console.Error.WriteLine("Authentication was canceled by the user.");
		}
		catch (MsalServiceException ex) when (ex.ErrorCode == "access_denied")
		{
			Console.Error.WriteLine("Access denied. Consent may be required for the Dataverse delegated permission.");
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine($"Token acquisition failed: {ex.Message}");
		}

		return 1;
	}
}

public sealed class TestGetAccessTokenSettings: CommandSettings
{
	[CommandOption("--url")]
	public string? Url { get; set; }

	[CommandOption("--tenant-id")]
	public string? TenantId { get; set; }

	[CommandOption("--client-id")]
	public string? ClientId { get; set; }

	public override ValidationResult Validate()
	{
		if (string.IsNullOrWhiteSpace(Url))
		{
			return ValidationResult.Error("URL parameter must be passed through. Pass through the target Dataverse environment using the --url flag.");
		}

		return ValidationResult.Success();
	}
}
