using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.PlatformModels;
using CloudAwesome.Dataverse.Processe.Plugins;
using CloudAwesome.Dataverse.Processes.Plugins;
using CloudAwesome.Xrm.Customisation.Models;
using Microsoft.Xrm.Sdk;

namespace CloudAwesome.Dataverse.Processes;

public class PluginRegistration
{
	public static void Register(PluginRegistrationManifest manifest, IOrganizationService client, TracingHelper t)
	{
		t.Debug($"Entering PluginWrapper.RegisterPlugins");
		
		if (manifest.Clobber)
		{
			t.Warning($"Manifest has 'Clobber' set to true. Deleting referenced Plugins before re-registering");
			Unregister(manifest, client, t);
		}

		foreach (var pluginAssembly in manifest.PluginAssemblies) 
		{
            var targetSolutionName = DefineSolutionNameFromManifest(manifest, pluginAssembly);
            var createdAssembly = RegisterPluginAssembly.Run(client, manifest, pluginAssembly, targetSolutionName, t);
            if (manifest.UpdateAssemblyOnly) continue;
            
            foreach (var plugin in pluginAssembly.Plugins)
            {
                var createdPluginType = RegisterPluginType.Run(plugin, createdAssembly, client, t);

                if (plugin.Steps.Any())
                {
                    foreach (var pluginStep in plugin.Steps)
                    {
                        var createdStep = RegisterPluginStep.Run(pluginStep, createdPluginType, targetSolutionName, client, t);
                        
                        if (pluginStep.EntityImages.Any()) continue;
                        foreach (var entityImage in pluginStep.EntityImages)
                        {
                            var image = RegisterEntityImage.Run(entityImage, createdStep, client, t);
                        }
                    }
                }

                /*if (plugin.CustomApis.Any()) continue;
                foreach (var api in plugin.CustomApis)
                {
                    var createdApi = RegisterCustomApi.Run(api, createdPluginType, targetSolutionName, client, t);

                    if (api.RequestParameters != null)
                    {
                        foreach (var requestParameter in api.RequestParameters)
                        {
                            var createdRequestParameter = RegisterCustomApiRequestParameter.Run(requestParameter,
                                createdApi, targetSolutionName, client, t);
                        }
                    }

                    if (api.ResponseProperties != null)
                    {
                        foreach (var responseProperty in api.ResponseProperties)
                        {
                            var createdResponseProperty = RegisterCustomerApiResponseProperty.Run(responseProperty,
                                createdApi, targetSolutionName, client, t);
                        }
                    }
                }*/
            }
        }
        
        t.Debug($"Exiting PluginWrapper.RegisterPlugins");
	}

	public static void Unregister(PluginRegistrationManifest manifest, IOrganizationService client, TracingHelper t)
	{
		t.Debug($"Entering PluginWrapper.UnregisterPlugins");

            // TODO - need to clobber Custom APIs and child parameters/properties too!!

            foreach (var pluginAssembly in manifest.PluginAssemblies)
            {
                t.Debug($"Getting PluginAssemblyInfo from file {pluginAssembly.Assembly}");
                if (!File.Exists(pluginAssembly.Assembly))
                {
                    t.Critical($"Assembly {pluginAssembly.Assembly} cannot be found!");
                    continue;
                }

                var pluginAssemblyInfo = new PluginAssemblyInfo(pluginAssembly.Assembly);
                var existingAssembly = 
                    pluginAssembly.GetExistingQuery(pluginAssemblyInfo.Version)
                        .RetrieveSingleRecord(client);

                if (existingAssembly == null) return;

                var childPluginTypesResults = PluginQueries.GetChildPluginTypesQuery(existingAssembly.ToEntityReference()).RetrieveMultiple(client);
                var pluginsList = childPluginTypesResults.Entities.Select(e => e.Id).ToList();

                // TODO - Clobber Custom APIs
                //PluginQueries.GetChildCustomApisQuery(pluginsList).DeleteAllResults(client);

                var childStepsResults = PluginQueries.GetChildPluginStepsQuery(pluginsList).RetrieveMultiple(client);
                var pluginStepsList = childStepsResults.Entities.Select(e => e.Id).ToList();

                if (pluginStepsList.Count > 0)
                {
                    PluginQueries.GetChildEntityImagesQuery(pluginStepsList).DeleteAllResults(client);
                }

                if (pluginsList.Count > 0)
                {
                    PluginQueries.GetChildPluginStepsQuery(pluginsList).DeleteAllResults(client);
                }

                PluginQueries.GetChildPluginTypesQuery(existingAssembly.ToEntityReference()).DeleteAllResults(client);
                pluginAssembly.GetExistingQuery(pluginAssemblyInfo.Version).DeleteSingleRecord(client);
            }

            t.Debug($"Exiting PluginWrapper.UnregisterPlugins");
	}
	
	private static string DefineSolutionNameFromManifest(PluginRegistrationManifest manifest, CdsPluginAssembly assembly)
	{
		return !string.IsNullOrEmpty(assembly.SolutionName) ? 
			assembly.SolutionName : 
			manifest.SolutionName;
	} 
}