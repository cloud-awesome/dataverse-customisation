using System.Text.Json.Serialization;

namespace CloudAwesome.Dataverse.Security.Models;

public class TeamModel
{
	[JsonPropertyName("name")]
	public string Name { get; set; }

	[JsonPropertyName("id")]
	public Guid Id { get; set; }

	[JsonPropertyName("roles")] 
	public List<SecurityRole> Roles { get; set; } = new List<SecurityRole>();
}