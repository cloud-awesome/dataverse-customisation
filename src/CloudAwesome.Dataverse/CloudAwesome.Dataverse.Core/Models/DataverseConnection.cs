using System.Diagnostics.CodeAnalysis;

namespace CloudAwesome.Dataverse.Core.Models;

/// <summary>
/// Configuration for creating a connection to the Common Data Service (aka CDS, Dynamics365, DataVerse)
/// </summary>
[ExcludeFromCodeCoverage]
public class DataverseConnection
{
	/// <summary>
	/// Connection type. Supports a connection string, an AAD app registration, bearer token, or interactive user login.
	/// </summary>
	public DataverseConnectionType ConnectionType { get; set; }

	/// <summary>
	/// CDS Connection string, required if ConnectionType == ConnectionString
	/// </summary>
	public string? ConnectionString { get; set; }

	/// <summary>
	/// Base URL for CDS environment. Required if ConnectionType == AppRegistration, BearerToken, or InteractiveUser
	/// </summary>
	public string? Url { get; set; }

	/// <summary>
	/// Required if ConnectionType == UserNAmeAndPassword
	/// </summary>
	public string? UserName { get; set; }

	/// <summary>
	/// Required if ConnectionType == UserNAmeAndPassword
	/// </summary>
	public string? Password { get; set; }

	/// <summary>
	/// Required if ConnectionType == AppRegistration
	/// </summary>
	public string? ClientId { get; set; }

	/// <summary>
	/// Optional tenant ID used when ConnectionType == InteractiveUser. Defaults to the Microsoft Entra common authority.
	/// </summary>
	public string? TenantId { get; set; }

	/// <summary>
	/// Optional profile name for future interactive user profile selection.
	/// </summary>
	public string? ProfileName { get; set; }

	/// <summary>
	/// Required if ConnectionType == AppRegistration
	/// </summary>
	public string? ClientSecret { get; set; }
        
	/// <summary>
	/// Required if ConnectionType = BearerToken
	/// </summary>
	public string? BearerToken { get; set; }
}
