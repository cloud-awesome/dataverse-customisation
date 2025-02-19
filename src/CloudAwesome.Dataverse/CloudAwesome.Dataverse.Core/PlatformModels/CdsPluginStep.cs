using System;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using CloudAwesome.Dataverse.Core.PlatformModels;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Newtonsoft.Json;

namespace CloudAwesome.Xrm.Customisation.Models
{
    [JsonObject]
    public class CdsPluginStep
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("friendlyName")]
        public string FriendlyName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        /*[JsonPropertyName("stage")]
        public SdkMessageProcessingStep_Stage Stage { get; set; }

        [JsonPropertyName("executionMode")]
        public SdkMessageProcessingStep_Mode ExecutionMode { get; set; }*/

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("primaryEntity")]
        public string PrimaryEntity { get; set; }

        [JsonPropertyName("executionOrder")]
        public int ExecutionOrder { get; set; }

        [JsonPropertyName("asyncAutoDelete")]
        public bool AsyncAutoDelete { get; set; }

        [JsonPropertyName("unsecureConfiguration")]
        public string UnsecureConfiguration { get; set; }

        [JsonPropertyName("secureConfiguration")]
        public string SecureConfiguration { get; set; }

        [JsonPropertyName("filteringAttributes")]
        [XmlArrayItem("Attribute")]
        public string[] FilteringAttributes { get; set; }

        [JsonPropertyName("entityImages")]
        [XmlArrayItem("EntityImage")]
        public CdsEntityImage[] EntityImages { get; set; }
    }
}
