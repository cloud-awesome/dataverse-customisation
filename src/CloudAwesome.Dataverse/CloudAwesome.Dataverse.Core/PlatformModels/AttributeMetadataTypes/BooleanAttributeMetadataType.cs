using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace CloudAwesome.Dataverse.Core.PlatformModels.AttributeMetadataTypes
{
    public class BooleanAttributeMetadataType: IAttributeMetadataType
    {
        private readonly CdsAttribute _attribute;
        private readonly string _publisherPrefix;
        private readonly AttributeMetadata? _existingMetadata;

        public BooleanAttributeMetadataType(CdsAttribute attribute, string publisherPrefix, AttributeMetadata? existingMetadata = null)
        {
            _attribute = attribute;
            _publisherPrefix = publisherPrefix;
            _existingMetadata = existingMetadata;
        }

        public string Name => nameof(BooleanAttributeMetadataType);
        
        public AttributeMetadata AttributeMetadata
        {
            get
            {
                BooleanAttributeMetadata attributeMetadata;
                if (_existingMetadata == null)
                {
                    // Create
                    attributeMetadata = new BooleanAttributeMetadata()
                    {
                        LogicalName = _attribute.SchemaName,
                        SchemaName = _attribute.SchemaName,
                        DisplayName = _attribute.DisplayName.CreateLabelFromString(),
                        Description = _attribute.Description.CreateLabelFromString(),
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(_attribute.RequiredLevel),
                        IsAuditEnabled = new BooleanManagedProperty(_attribute.IsAuditEnabled),
                        DefaultValue = false,
                        OptionSet = new BooleanOptionSetMetadata()
                        {
                            // TODO - Include 'TwoOptions' values, and it's default in manifest
                            TrueOption = new OptionMetadata()
                            {
                                Label = "Yes".CreateLabelFromString()
                            },
                            FalseOption = new OptionMetadata()
                            {
                                Label = "No".CreateLabelFromString()
                            }
                        }
                    };
                }
                else
                {
                    attributeMetadata = (BooleanAttributeMetadata) _existingMetadata;
                    attributeMetadata.DisplayName = _attribute.DisplayName.CreateLabelFromString();
                    attributeMetadata.Description = string.IsNullOrEmpty(_attribute.Description)
                        ? attributeMetadata.Description
                        : _attribute.Description.CreateLabelFromString();
                    attributeMetadata.RequiredLevel = new AttributeRequiredLevelManagedProperty(_attribute.RequiredLevel);
                    attributeMetadata.IsAuditEnabled = new BooleanManagedProperty(_attribute.IsAuditEnabled);
                    // TODO - Include 'TwoOptions' values, and it's default in manifest and update labels
                }

                return attributeMetadata;
            }
        }
    }
}
