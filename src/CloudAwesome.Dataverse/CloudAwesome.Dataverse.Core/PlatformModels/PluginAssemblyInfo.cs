using System.IO;
using System.Reflection;

namespace CloudAwesome.Xrm.Customisation.Models
{
    public class PluginAssemblyInfo
    {
        public string Version { get; set; }
        public string Culture { get; set; }
        public string PublicKeyToken { get; set; }

        public PluginAssemblyInfo(string version, string culture, string publicKeyToken)
        {
            Version = version;
            Culture = culture;
            PublicKeyToken = publicKeyToken;
        }

        public PluginAssemblyInfo(string assemblyFilePath)
        {
            var assemblyFileInfo = new FileInfo(assemblyFilePath);
            var assembly = Assembly.LoadFile(assemblyFileInfo.FullName);
            var assemblyParts = assembly.FullName.Split(',');

            Version = assemblyParts[1].Split('=')[1].Trim();
            Culture = assemblyParts[2].Split('=')[1].Trim();
            PublicKeyToken = assemblyParts[3].Split('=')[1].Trim();
        }
    }
}
