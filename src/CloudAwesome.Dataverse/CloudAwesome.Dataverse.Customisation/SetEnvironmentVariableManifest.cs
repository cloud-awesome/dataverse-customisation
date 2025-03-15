using System.Text.Json.Serialization;

namespace CloudAwesome.Dataverse.Customisation;

public class SetEnvironmentVariableManifest
{
	[JsonPropertyName("variables")]
	public List<KeyValuePair> Variables { get; set; }
}

public class KeyValuePair
{
	[JsonPropertyName("key")]
	public string Key { get; set; }
	
	[JsonPropertyName("value")]
	public string Value { get; set; }
}