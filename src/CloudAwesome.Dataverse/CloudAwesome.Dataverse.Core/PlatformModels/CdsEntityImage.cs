using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace CloudAwesome.Dataverse.Core.PlatformModels
{
    public enum EntityImageType { PreImage, PostImage }

    [JsonObject]
    public class CdsEntityImage
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("postImage")]
        public bool PostImage { get; set; }

        [JsonPropertyName("preImage")]
        public bool PreImage { get; set; }
        
        public EntityImageType Type { get; set; }

        [JsonPropertyName("attributes")]
        [XmlArrayItem("Attribute")]
        public string[] Attributes { get; set; }
    }
}
