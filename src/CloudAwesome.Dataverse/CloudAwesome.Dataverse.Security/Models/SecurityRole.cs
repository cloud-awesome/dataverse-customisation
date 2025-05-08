using System.Text.Json.Serialization;

namespace CloudAwesome.Dataverse.Security.Models;

public class SecurityRole
{
	[JsonPropertyName("name")]
	public string Name { get; set; }
}