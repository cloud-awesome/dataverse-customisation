using System.Text.Json.Serialization;
using CloudAwesome.Dataverse.Core.Models;
using CloudAwesome.Dataverse.Core.PlatformModels;

namespace CloudAwesome.Dataverse.Customisation;

public class GenerateCustomisationsManifest
{
	[JsonPropertyName("solutionName")]
	public string SolutionName { get; set; }
	
	[JsonPropertyName("clobber")]
	public bool Clobber { get; set; }

	[JsonPropertyName("loggingConfiguration")]
	public LoggingConfiguration? LoggingConfiguration { get; set; }

	[JsonPropertyName("entities")]
	public CdsEntity[]? Entities { get; set; }

	// TODO: Port over support for these
	/*[JsonPropertyName("optionSets")]
	public CdsOptionSet[]? OptionSets { get; set; }

	[JsonPropertyName("securityRoles")]
	public CdsSecurityRole[]? SecurityRoles { get; set; }

	[JsonPropertyName("modelDrivenApps")]
	public CdsModelDrivenApp[]? ModelDrivenApps { get; set; }*/
}