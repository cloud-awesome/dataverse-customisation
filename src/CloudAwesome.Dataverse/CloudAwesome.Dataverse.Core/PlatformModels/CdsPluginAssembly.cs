using System.IO.Abstractions;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using CloudAwesome.Xrm.Customisation.Models;
using Newtonsoft.Json;

namespace CloudAwesome.Dataverse.Core.PlatformModels
{
    [JsonObject]
    public class CdsPluginAssembly
    {
        private readonly IFileSystem _fileSystem;

        public CdsPluginAssembly() : this (new FileSystem())
        {
        }

        public CdsPluginAssembly(IFileSystem fileSystem)
        {
            this._fileSystem = fileSystem;
        }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("friendlyName")]
        public string FriendlyName { get; set; }

        /// <summary>
        /// FilePath to the built assembly dll
        /// </summary>
        [JsonPropertyName("filePath")]
        public string Assembly { get; set; }

        [JsonPropertyName("solutionName")]
        public string SolutionName { get; set; }

        /// <summary>
        /// Process all child plugins/steps - Used in the ProcessActivation function
        /// </summary>
        public bool AllChildren { get; set; }

        /// <summary>
        /// Child plugins
        /// </summary>
        [XmlArrayItem("Plugin")]
        [JsonPropertyName("plugins")]
        public CdsPlugin[] Plugins { get; set; }

        /// <summary>
        /// Grandchild steps
        /// </summary>
        [XmlArrayItem("Step")]
        [JsonPropertyName("steps")]
        public CdsPluginStep[] Steps { get; set; }

    }
}
