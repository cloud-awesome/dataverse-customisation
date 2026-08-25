using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace CloudAwesome.Dataverse.Core.PlatformModels.AttributeMetadataTypes
{
    public class IntegerAttributeMetadataType: IAttributeMetadataType
    {
        private readonly CdsAttribute _attribute;
        private readonly string _publisherPrefix;
        private readonly AttributeMetadata _existingMetadata;

        public IntegerAttributeMetadataType(CdsAttribute attribute, string publisherPrefix, AttributeMetadata existingMetadata = null)
        {
            _attribute = attribute;
            _publisherPrefix = publisherPrefix;
            _existingMetadata = existingMetadata;
        }

        public string Name => nameof(IntegerAttributeMetadataType);
        
        public AttributeMetadata AttributeMetadata
        {
            get
            {
                IntegerAttributeMetadata attributeMetadata;
                if (_existingMetadata == null)
                {
                    // Create
                    attributeMetadata = new IntegerAttributeMetadata()
                    {
                        LogicalName = _attribute.SchemaName,
                        SchemaName = _attribute.SchemaName,
                        DisplayName = _attribute.DisplayName.CreateLabelFromString(),
                        Description = _attribute.Description.CreateLabelFromString(),
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(_attribute.RequiredLevel),
                        IsAuditEnabled = new BooleanManagedProperty(_attribute.IsAuditEnabled),
                        MinValue = _attribute.MinValue,
                        MaxValue = _attribute.MaxValue
                    };
                }
                else
                {
                    attributeMetadata = (IntegerAttributeMetadata) _existingMetadata;
                    attributeMetadata.DisplayName = _attribute.DisplayName.CreateLabelFromString();
                    attributeMetadata.Description = string.IsNullOrEmpty(_attribute.Description)
                        ? attributeMetadata.Description
                        : _attribute.Description.CreateLabelFromString();
                    attributeMetadata.RequiredLevel = new AttributeRequiredLevelManagedProperty(_attribute.RequiredLevel);
                    attributeMetadata.IsAuditEnabled = new BooleanManagedProperty(_attribute.IsAuditEnabled);
                    attributeMetadata.MinValue = _attribute.MinValue;
                    attributeMetadata.MaxValue = _attribute.MaxValue;
                }
                
                return attributeMetadata;
            }
        }
    }
}
