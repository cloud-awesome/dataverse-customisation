using CloudAwesome.Dataverse.Core.Models;
using Spectre.Console.Cli;

namespace CloudAwesome.Dataverse.Cli.CommandInterfaces;

public abstract class SupportsDataverseConnection: CommandSettings
{
	internal readonly DataverseConnection ConnectionDetails = new DataverseConnection();
	internal bool OverrideManifestConnectionDetails = false;

	[CommandOption("--connection-type")]
	public DataverseConnectionType ConnectionType
	{
		get => ConnectionDetails.ConnectionType;
		set
		{
			ConnectionDetails.ConnectionType = value;
			OverrideManifestConnectionDetails = true;
		}
	}
	
	[CommandOption("--url")]
	public string Url
	{
		get => ConnectionDetails.Url; 
		set => ConnectionDetails.Url = value;
	}

	[CommandOption("--client-id")]
	public string ClientId
	{
		get => ConnectionDetails.ClientId;
		set => ConnectionDetails.ClientId = value;
	}
        
	[CommandOption("--client-secret")]
	public string clientSecret
	{
		set => ConnectionDetails.ClientSecret = value;
	}
        
	[CommandOption("--username")]
	public string UserName
	{
		set => ConnectionDetails.UserName = value;
	}
        
	[CommandOption("--password")]
	public string UserPassword
	{
		set => ConnectionDetails.UserName = value;
	}
	
	[CommandOption("--connection-string")]
	public string ConnectionString
	{
		get => ConnectionDetails.ConnectionString;
		set => ConnectionDetails.ConnectionString = value;
	}
}