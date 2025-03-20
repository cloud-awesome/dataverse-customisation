using System.Text.Json.Serialization;
using CloudAwesome.Dataverse.Security.Models;

namespace CloudAwesome.Dataverse.Security;

public class ExportSecurityRolesManifest
{
	[JsonPropertyName("teams")]
	public List<TeamModel> Teams { get; set; } = new List<TeamModel>();
	
	[JsonPropertyName("outputFilePath")]
	public string OutputFilePath { get; set; }
}