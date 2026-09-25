using CloudAwesome.Dataverse.Core.Models;
using Spectre.Console;
using Spectre.Console.Cli;

namespace CloudAwesome.Dataverse.Cli.CommandInterfaces;

public abstract class SupportsDataverseConnection: CommandSettings
{
	internal readonly DataverseConnection ConnectionDetails = new DataverseConnection();
	internal bool OverrideManifestConnectionDetails = false;
	private bool _connectionTypeSpecified = false;

	[CommandOption("--connection-type")]
	public DataverseConnectionType ConnectionType
	{
		get => ConnectionDetails.ConnectionType;
		set
		{
			ConnectionDetails.ConnectionType = value;
			OverrideManifestConnectionDetails = true;
			_connectionTypeSpecified = true;
		}
	}

	[CommandOption("--url")]
	public string? Url
	{
		get => ConnectionDetails.Url;
		set => ConnectionDetails.Url = value;
	}

	[CommandOption("--client-id")]
	public string? ClientId
	{
		get => ConnectionDetails.ClientId;
		set => ConnectionDetails.ClientId = value;
	}

	[CommandOption("--tenant-id")]
	public string? TenantId
	{
		get => ConnectionDetails.TenantId;
		set => ConnectionDetails.TenantId = value;
	}

	[CommandOption("--profile")]
	public string? ProfileName
	{
		get => ConnectionDetails.ProfileName;
		set => ConnectionDetails.ProfileName = value;
	}

	[CommandOption("--client-secret")]
	public string? clientSecret
	{
		set => ConnectionDetails.ClientSecret = value;
	}

	[CommandOption("--username")]
	public string? UserName
	{
		set => ConnectionDetails.UserName = value;
	}

	[CommandOption("--password")]
	public string? UserPassword
	{
		set => ConnectionDetails.Password = value;
	}

	[CommandOption("--connection-string")]
	public string? ConnectionString
	{
		get => ConnectionDetails.ConnectionString;
		set => ConnectionDetails.ConnectionString = value;
	}

	[CommandOption("--bearer-token")]
	public string? BearerToken
	{
		get => ConnectionDetails.BearerToken;
		set => ConnectionDetails.BearerToken = value;
	}

	public override ValidationResult Validate()
	{
		ApplyDefaultConnectionType();

		return ConnectionDetails.ConnectionType switch
		{
			DataverseConnectionType.ConnectionString when string.IsNullOrWhiteSpace(ConnectionDetails.ConnectionString) =>
				ValidationResult.Error("--connection-string is required when --connection-type is connectionString."),

			DataverseConnectionType.AppRegistration when string.IsNullOrWhiteSpace(ConnectionDetails.Url) ||
			                                             string.IsNullOrWhiteSpace(ConnectionDetails.ClientId) ||
			                                             string.IsNullOrWhiteSpace(ConnectionDetails.ClientSecret) =>
				ValidationResult.Error("--url, --client-id, and --client-secret are required when --connection-type is appRegistration."),

			DataverseConnectionType.BearerToken when string.IsNullOrWhiteSpace(ConnectionDetails.Url) ||
			                                      string.IsNullOrWhiteSpace(ConnectionDetails.BearerToken) =>
				ValidationResult.Error("--url and --bearer-token are required when --connection-type is bearerToken."),

			DataverseConnectionType.InteractiveUser when string.IsNullOrWhiteSpace(ConnectionDetails.Url) =>
				ValidationResult.Error("--url is required when --connection-type is interactiveUser."),

			DataverseConnectionType.UserNameAndPassword =>
				ValidationResult.Error("Username/password authentication is not supported. Use --url for interactive user authentication."),

			_ => ValidationResult.Success()
		};
	}

	private void ApplyDefaultConnectionType()
	{
		if (_connectionTypeSpecified)
		{
			return;
		}

		if (!string.IsNullOrWhiteSpace(ConnectionDetails.ConnectionString))
		{
			ConnectionDetails.ConnectionType = DataverseConnectionType.ConnectionString;
			return;
		}

		if (!string.IsNullOrWhiteSpace(ConnectionDetails.BearerToken))
		{
			ConnectionDetails.ConnectionType = DataverseConnectionType.BearerToken;
			return;
		}

		if (!string.IsNullOrWhiteSpace(ConnectionDetails.ClientSecret))
		{
			ConnectionDetails.ConnectionType = DataverseConnectionType.AppRegistration;
			return;
		}

		if (!string.IsNullOrWhiteSpace(ConnectionDetails.Url))
		{
			ConnectionDetails.ConnectionType = DataverseConnectionType.InteractiveUser;
		}
	}
}
