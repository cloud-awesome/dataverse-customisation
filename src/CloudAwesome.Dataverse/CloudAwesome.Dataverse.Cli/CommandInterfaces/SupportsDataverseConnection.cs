using CloudAwesome.Dataverse.Core.Models;
using Spectre.Console.Cli;

namespace CloudAwesome.Dataverse.Cli.CommandInterfaces;

public abstract class SupportsDataverseConnection: CommandSettings
{
	internal readonly DataverseConnection CdsConnectionDetails = new DataverseConnection();
	internal bool OverrideManifestConnectionDetails = false;

	[CommandOption("--connection-type")]
	public DataverseConnectionType ConnectionType
	{
		get => CdsConnectionDetails.ConnectionType;
		set
		{
			CdsConnectionDetails.ConnectionType = value;
			OverrideManifestConnectionDetails = true;
		}
	}
	
	[CommandOption("--url")]
	public string Url
	{
		get => CdsConnectionDetails.CdsUrl; 
		set => CdsConnectionDetails.CdsUrl = value;
	}

	[CommandOption("--client-id")]
	public string ClientId
	{
		get => CdsConnectionDetails.CdsAppId;
		set => CdsConnectionDetails.CdsAppId = value;
	}
        
	[CommandOption("--client-secret")]
	public string clientSecret
	{
		set => CdsConnectionDetails.CdsAppSecret = value;
	}
        
	[CommandOption("--username")]
	public string UserName
	{
		set => CdsConnectionDetails.CdsUserName = value;
	}
        
	[CommandOption("--password")]
	public string UserPassword
	{
		set => CdsConnectionDetails.CdsUserName = value;
	}
	
	[CommandOption("--connection-string")]
	public string ConnectionString
	{
		get => CdsConnectionDetails.ConnectionString;
		set => CdsConnectionDetails.ConnectionString = value;
	}
}