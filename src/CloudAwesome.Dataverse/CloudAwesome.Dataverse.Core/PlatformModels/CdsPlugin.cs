using System.Text.Json.Serialization;
using System.Xml.Serialization;
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
        [XmlArrayItem("Step")]
        public CdsPluginStep[] Steps { get; set; }

        [JsonPropertyName("customApis")]
        [XmlArrayItem("CustomApi")]
        public CdsCustomApi[] CustomApis { get; set; }
    }
}
