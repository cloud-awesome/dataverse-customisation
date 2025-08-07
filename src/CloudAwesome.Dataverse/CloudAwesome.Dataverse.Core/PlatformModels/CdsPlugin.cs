using System.Text.Json.Serialization;
using System.Xml.Serialization;
using CloudAwesome.Dataverse.Core.EarlyBoundModels;
using CloudAwesome.Xrm.Customisation.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;

namespace CloudAwesome.Dataverse.Core.PlatformModels
{
    [JsonObject]
    public class CdsPlugin
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("friendlyName")]
        public string FriendlyName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        public EntityReference ParentAssembly { get; set; }

        [JsonPropertyName("steps")]
        public CdsPluginStep[] Steps { get; set; }

        [JsonPropertyName("customApis")]
        public CdsCustomApi[] CustomApis { get; set; }
        
        public EntityReference Register(IOrganizationService client, EntityReference parentAssembly)
        {
            if (parentAssembly.LogicalName != PluginAssembly.EntityLogicalName)
                throw new ArgumentException($"Entity Reference '{nameof(parentAssembly)}' must be of type '{PluginAssembly.EntityLogicalName}'");

            this.ParentAssembly = parentAssembly;
            return this.Register(client);
        }

        public EntityReference Register(IOrganizationService client)
        {
            var pluginType = new PluginType()
            {
                PluginAssemblyId = this.ParentAssembly,
                TypeName = this.Name,
                FriendlyName = this.FriendlyName,
                Name = this.Name,
                Description = this.Description
            };

            var existingPluginQuery = this.GetExistingQuery(this.ParentAssembly.Id);
            return pluginType.CreateOrUpdate(client, existingPluginQuery);
        }

        public QueryBase GetExistingQuery(Guid parentAssemblyId)
        {
            return new QueryExpression()
            {
                EntityName = PluginType.EntityLogicalName,
                ColumnSet = new ColumnSet(PluginType.PrimaryIdAttribute, 
                    PluginType.PrimaryNameAttribute),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(PluginType.PrimaryNameAttribute, 
                            ConditionOperator.Equal, this.Name),
                        new ConditionExpression(PluginType.Fields.PluginAssemblyId, 
                            ConditionOperator.Equal, parentAssemblyId)
                    }
                }
            };
        }
    }
}
