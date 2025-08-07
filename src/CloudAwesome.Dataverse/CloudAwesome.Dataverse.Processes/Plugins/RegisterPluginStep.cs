using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.PlatformModels;
using CloudAwesome.Xrm.Customisation.Models;
using Microsoft.Xrm.Sdk;

namespace CloudAwesome.Dataverse.Processes.Plugins
{
    internal static class RegisterPluginStep
    {
        internal static EntityReference Run(CdsPluginStep pluginStep, EntityReference parentPluginType, string targetSolutionName, 
            IOrganizationService client, TracingHelper t)
        {
            t.Debug($"Processing Step = {pluginStep.FriendlyName}");

            var sdkMessage = PluginQueries.GetSdkMessageQuery(pluginStep.Message).RetrieveSingleRecord(client);
            var sdkMessageFilter = PluginQueries.GetSdkMessageFilterQuery(pluginStep.PrimaryEntity, sdkMessage.Id).RetrieveSingleRecord(client);

            var createdStep = pluginStep.Register(client, parentPluginType, sdkMessage.ToEntityReference(), sdkMessageFilter.ToEntityReference());
            t.Info($"Plugin step {pluginStep.FriendlyName} registered with ID {createdStep.Id}");

            SolutionWrapper.AddSolutionComponent(client, targetSolutionName, createdStep.Id, ComponentType.SdkMessageProcessingStep);
            t.Debug($"Plugin Step '{pluginStep.FriendlyName}' added to solution {targetSolutionName}");

            return createdStep;
        }
        
    }
}