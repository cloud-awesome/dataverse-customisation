using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.EarlyBoundModels;
using CloudAwesome.Dataverse.Customisation.CustomisationGeneration;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CloudAwesome.Dataverse.Customisation;

public class GenerateConfigurations
{
	public void Run(IOrganizationService client, TracingHelper t, GenerateCustomisationsManifest manifest)
	{
		// TODO: Validate manifest
		
		// TODO: Get target solution publisher prefix
		var publisherPrefix = GetPublisherPrefixFromSolution(client, manifest.SolutionName);
		
		// TODO: Generate global option sets
		
		// TODO: Generate security roles
		
		// TODO: Generate entity model
		EntityModel.Generate(manifest, client, t, publisherPrefix);
		
		// TODO: Generate MDAs
		
		
	}

	private string GetPublisherPrefixFromSolution(IOrganizationService client, string solutionName)
	{
		var fetchQuery = SolutionPublisherQuery.Replace("{{SolutionName}}", solutionName);
		var publisher = 
			QueryExtensions.RetrieveRecordFromQuery(client, new FetchExpression(fetchQuery))
				?.ToEntity<Publisher>();

		return publisher is null ?  "new_" : publisher.CustomizationPrefix;
	}
        
	private const string SolutionPublisherQuery =
		@"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""true"">
	            <entity name=""publisher"">
		            <attribute name=""publisherid""/>
		            <attribute name=""friendlyname""/>
		            <attribute name=""uniquename""/>
		            <attribute name=""customizationprefix""/>
		            <link-entity name=""solution"" from=""publisherid"" to=""publisherid"" link-type=""inner"" alias=""p"">
			            <filter type=""and"">
				            <condition attribute=""uniquename"" operator=""eq"" value=""{{SolutionName}}""/>
			            </filter>
		            </link-entity>
	            </entity>
            </fetch>";
}
