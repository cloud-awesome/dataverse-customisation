using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.PlatformModels;
using Microsoft.Xrm.Sdk;

namespace CloudAwesome.Dataverse.Processe.Plugins
{
    internal static class RegisterEntityImage
    {
        internal static EntityReference Run(CdsEntityImage entityImage, EntityReference parentPluginStep, 
            IOrganizationService client, TracingHelper t)
        {
            t.Debug($"Processing Entity Image = {entityImage.Name}");
            var createdImage = entityImage.Register(client, parentPluginStep);
            t.Info($"Entity image {entityImage.Name} registered with ID {createdImage.Id}");

            return createdImage;
        }
        
    }
}