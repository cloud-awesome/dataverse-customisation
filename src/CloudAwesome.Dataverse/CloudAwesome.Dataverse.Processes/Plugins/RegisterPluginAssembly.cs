using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.PlatformModels;
using Microsoft.Xrm.Sdk;

namespace CloudAwesome.Dataverse.Processes.Plugins
{
    internal static class RegisterPluginAssembly
    {
        internal static EntityReference Run(IOrganizationService client, PluginRegistrationManifest manifest, 
            CdsPluginAssembly pluginAssembly, string targetSolutionName, TracingHelper t)
        {
            t.Debug($"Processing Assembly FriendlyName {pluginAssembly.FriendlyName}");

            if (!File.Exists(pluginAssembly.Assembly))
            {
                t.Critical($"Assembly {pluginAssembly.Assembly} cannot be found!");
                return null;
            }

            t.Debug($"Registering Assembly {pluginAssembly.FriendlyName}");
            t.Debug($"Using auth type {manifest.DataverseConnection.ConnectionType}");
            var createdAssembly = pluginAssembly.Register(client);
            t.Info($"Assembly {pluginAssembly.FriendlyName} registered with ID {createdAssembly.Id}");

            SolutionWrapper.AddSolutionComponent(client, targetSolutionName, createdAssembly.Id, ComponentType.PluginAssembly);
            t.Debug($"Assembly '{pluginAssembly.FriendlyName}' added to solution {targetSolutionName}");

            return createdAssembly;
        }
        
    }
}