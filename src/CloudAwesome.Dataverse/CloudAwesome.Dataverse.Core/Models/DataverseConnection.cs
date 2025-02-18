using System.Diagnostics.CodeAnalysis;

namespace CloudAwesome.Dataverse.Core.Models;

/// <summary>
/// Configuration for creating a connection to the Common Data Service (aka CDS, Dynamics365, DataVerse)
/// </summary>
[ExcludeFromCodeCoverage]
public class DataverseConnection
{
	/// <summary>
	/// Connection type. Currently supports a connection string, an AAD App registration, or username and password (not recommended)
	/// </summary>
	public DataverseConnectionType ConnectionType { get; set; }

	/// <summary>
	/// CDS Connection string, required if ConnectionType == ConnectionString
	/// </summary>
	public string ConnectionString { get; set; }

	/// <summary>
	/// Base URL for CDS environment. Required if ConnectionType == AppRegistration or UserNameAndPassword
	/// </summary>
	public string CdsUrl { get; set; }

	/// <summary>
	/// Required if ConnectionType == UserNAmeAndPassword
	/// </summary>
	public string CdsUserName { get; set; }

	/// <summary>
	/// Required if ConnectionType == UserNAmeAndPassword
	/// </summary>
	public string CdsPassword { get; set; }

	/// <summary>
	/// Required if ConnectionType == AppRegistration
	/// </summary>
	public string CdsAppId { get; set; }

	/// <summary>
	/// Required if ConnectionType == AppRegistration
	/// </summary>
	public string CdsAppSecret { get; set; }
        
	/// <summary>
	/// Required if ConnectionType = BearerToken
	/// </summary>
	public string BearerToken { get; set; }
}