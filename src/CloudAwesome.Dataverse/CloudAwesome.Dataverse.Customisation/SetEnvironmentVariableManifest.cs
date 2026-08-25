using System.Text.Json.Serialization;

namespace CloudAwesome.Dataverse.Customisation;

public class SetEnvironmentVariableManifest
{
	[JsonPropertyName("variables")]
	public List<KeyValuePair> Variables { get; set; } = new List<KeyValuePair>();
}

public class KeyValuePair
{
	[JsonPropertyName("key")]
	public string Key { get; set; } = string.Empty;
	
	[JsonPropertyName("value")]
	public string Value { get; set; } = string.Empty;
}
