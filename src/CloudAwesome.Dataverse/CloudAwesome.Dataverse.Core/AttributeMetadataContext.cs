using CloudAwesome.Dataverse.Core.PlatformModels;
using CloudAwesome.Dataverse.Core.PlatformModels.AttributeMetadataTypes;
using Microsoft.Xrm.Sdk.Metadata;

namespace CloudAwesome.Dataverse.Core;

public class AttributeMetadataContext
{
	public readonly Dictionary<CdsAttributeDataType, IAttributeMetadataType> Strategies = new Dictionary<CdsAttributeDataType, IAttributeMetadataType>();

	public AttributeMetadataContext(CdsAttribute attribute, string publisherPrefix, AttributeMetadata existingMetadata = null)
	{
		Strategies.Add(CdsAttributeDataType.Boolean, new BooleanAttributeMetadataType(attribute, publisherPrefix, existingMetadata));
		Strategies.Add(CdsAttributeDataType.DateTime, new DateTimeAttributeMetadataType(attribute, publisherPrefix, existingMetadata));
		Strategies.Add(CdsAttributeDataType.Integer, new IntegerAttributeMetadataType(attribute, publisherPrefix, existingMetadata));
		Strategies.Add(CdsAttributeDataType.Lookup, new LookupAttributeMetadataType(attribute, publisherPrefix, existingMetadata));
		Strategies.Add(CdsAttributeDataType.Memo, new MemoAttributeMetadataType(attribute, publisherPrefix, existingMetadata));
		Strategies.Add(CdsAttributeDataType.Picklist, new PicklistAttributeMetadataType(attribute, publisherPrefix, existingMetadata));
		Strategies.Add(CdsAttributeDataType.String, new StringAttributeMetadataType(attribute, publisherPrefix, existingMetadata));
	}

	public IAttributeMetadataType GetAttributeMetadataType(CdsAttributeDataType dataType)
	{
		var strategy = Strategies[dataType];
		return strategy;
	}
}