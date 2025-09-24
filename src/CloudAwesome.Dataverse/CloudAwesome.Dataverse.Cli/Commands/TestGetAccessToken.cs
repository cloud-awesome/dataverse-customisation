using Microsoft.Identity.Client;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CloudAwesome.Dataverse.Cli.Commands;

public class TestGetAccessToken: Command<TestGetAccessTokenSettings>
{
	public override int Execute(CommandContext context, TestGetAccessTokenSettings settings)
	{
        // Default values
        string defaultClientId = "7376191c-f402-4c39-b190-47f0ab154c6c";
        string authority = settings.TenantId != null
            ? $"https://login.microsoftonline.com/{settings.TenantId}"
            : "https://login.microsoftonline.com/common";

        string clientId = string.IsNullOrWhiteSpace(settings.ClientId)
            ? defaultClientId
            : settings.ClientId;

        var scopes = new[] { $"{settings.Url}/.default" };

        try
        {
            var app = PublicClientApplicationBuilder
                .Create(clientId)
                .WithAuthority(authority)
                .WithRedirectUri("http://localhost")
                .Build();

            var accounts = app.GetAccountsAsync().GetAwaiter().GetResult();
            AuthenticationResult result;

            try
            {
                // Try silent first
                result = app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                    .ExecuteAsync().GetAwaiter().GetResult();
            }
            catch (MsalUiRequiredException)
            {
                // Silent failed — fall back to interactive
                result = app.AcquireTokenInteractive(scopes)
                    .WithPrompt(Prompt.SelectAccount)
                    .ExecuteAsync()
                    .GetAwaiter()
                    .GetResult();
            }

            Console.WriteLine(result.AccessToken);
            return 0;
        }
        catch (MsalServiceException ex) when (ex.ErrorCode == "invalid_client")
        {
            Console.Error.WriteLine(
                "Authentication failed: Invalid client ID. Check if your app is registered properly.");
        }
        catch (MsalClientException ex) when (ex.ErrorCode == "authentication_canceled")
        {
            Console.Error.WriteLine("Authentication was canceled by the user.");
        }
        catch (MsalClientException ex) when (ex.ErrorCode == "unknown_error")
        {
            Console.Error.WriteLine("An unknown error occurred during authentication.");
        }
        catch (MsalServiceException ex) when (ex.ErrorCode == "access_denied")
        {
            Console.Error.WriteLine("Access denied. You may not have consented to required permissions.");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Unexpected error during authentication: {ex.Message}");
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
        if (string.IsNullOrEmpty(Url))
        {
            return ValidationResult.Error("URL parameter must be passed through. Pass through the target dataverse environment using the --url flag");
        }
        
        return ValidationResult.Success();
    }
}