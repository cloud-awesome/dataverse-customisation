using System.Text.Json.Serialization;
using System.Xml.Serialization;
using CloudAwesome.Dataverse.Core.Models;
using CloudAwesome.Dataverse.Core.PlatformModels;

namespace CloudAwesome.Dataverse.Processes;

public class PluginRegistrationManifest
{
	[JsonPropertyName("$schema")]
	public string JsonSchema { get; set; }
	
	[JsonPropertyName("solutionName")]
	[JsonInclude]
	public string SolutionName { get; set; }
	
	[JsonPropertyName("clobber")]
	[JsonInclude]
	public bool Clobber { get; set; }
	
	[JsonPropertyName("updateAssemblyOnly")]
	public bool UpdateAssemblyOnly { get; set; }

	[JsonPropertyName("logging")]
	public LoggingConfiguration LoggingConfiguration { get; set; }

	[JsonPropertyName("dataverseConnection")]
	public DataverseConnection DataverseConnection { get; set; }

	[JsonPropertyName("assemblies")]
	public CdsPluginAssembly[] PluginAssemblies { get; set; }

	/*[JsonPropertyName("serviceEndpoints")]
	[XmlArrayItem("ServiceEndpoint")]
	public CdsServiceEndpoint[] ServiceEndpoints { get; set; }

	[JsonPropertyName("webhooks")]
	[XmlArrayItem("Webhook")]
	public CdsWebhook[] Webhooks { get; set; }*/
}