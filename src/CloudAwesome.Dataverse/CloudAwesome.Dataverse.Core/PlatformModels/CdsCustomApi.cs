using System.Text.Json.Serialization;
using Microsoft.Xrm.Sdk;

namespace CloudAwesome.Dataverse.Core.PlatformModels
{
    public enum ApiBindingType { Global, Entity, EntityCollection }

    public enum CustomProcessingStepType { None, AsyncOnly, SyncAndAsync }

    public class CdsCustomApi
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("friendlyName")]
        public string FriendlyName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("executePrivilegeName")]
        public string ExecutePrivilegeName { get; set; }

        [JsonPropertyName("isFunction")]
        public bool? IsFunction { get; set; }

        [JsonPropertyName("isPrivate")]
        public bool? IsPrivate { get; set; }
        
        public EntityReference ParentPlugin { get; set; }

        /*[JsonPropertyName("allowedCustomProcessingStepName")]
        public CustomAPI_AllowedCustomProcessingStepType AllowedCustomProcessingStepType { get; set; }

        [JsonPropertyName("bindingType")]
        public CustomAPI_BindingType BindingType { get; set; }

        [JsonPropertyName("requestParameters")]
        [XmlArrayItem("RequestParameter")]
        public CdsRequestParameter[] RequestParameters { get; set; }

        [JsonPropertyName("responseProperties")]
        [XmlArrayItem("ResponseProperty")]
        public CdsResponseProperty[] ResponseProperties { get; set; }*/
    }
}
