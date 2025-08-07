using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.PlatformModels;
using Microsoft.Xrm.Sdk;

namespace CloudAwesome.Dataverse.Processes.Plugins
{
    internal static class RegisterPluginType
    {
        internal static EntityReference Run(CdsPlugin plugin, EntityReference parentAssembly, 
            IOrganizationService client, TracingHelper t)
        {
            t.Debug($"PluginType FriendlyName = {plugin.FriendlyName}");
            var createdPluginType = plugin.Register(client, parentAssembly);
            t.Info($"Plugin {plugin.FriendlyName} registered with ID {createdPluginType.Id.ToString()}");

            return createdPluginType;
        }
        
    }
}