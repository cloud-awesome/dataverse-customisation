using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace CloudAwesome.Dataverse.Customisation.MetadataGeneration;

public interface IMetadataProvider
{
	IReadOnlyCollection<EntityMetadata> RetrieveEntities(IOrganizationService client, MetadataGenerationManifest manifest);
}
