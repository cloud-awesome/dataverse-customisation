using Microsoft.Xrm.Sdk.Metadata;

namespace CloudAwesome.Dataverse.Core.PlatformModels
{
    public interface IAttributeMetadataType
    {
        string Name { get; }
        AttributeMetadata AttributeMetadata { get; }
    }
}