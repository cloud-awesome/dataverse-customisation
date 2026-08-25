using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;

namespace CloudAwesome.Dataverse.Core.PlatformModels.AttributeMetadataTypes
{
    public class LookupAttributeMetadataType: IAttributeMetadataType
    {
        private readonly CdsAttribute _attribute;
        private readonly string _publisherPrefix;
        private readonly AttributeMetadata _existingMetadata;

        public LookupAttributeMetadataType(CdsAttribute attribute, string publisherPrefix, AttributeMetadata existingMetadata = null)
        {
            _attribute = attribute;
            _publisherPrefix = publisherPrefix;
            _existingMetadata = existingMetadata;
        }

        public string Name => nameof(LookupAttributeMetadataType);
        
        public AttributeMetadata AttributeMetadata
        {
            get
            {
                LookupAttributeMetadata attributeMetadata;
                if (_existingMetadata == null)
                {
                    // Create
                    attributeMetadata = new LookupAttributeMetadata()
                    {
                        LogicalName = _attribute.SchemaName,
                        SchemaName = _attribute.SchemaName,
                        DisplayName = _attribute.DisplayName.CreateLabelFromString(),
                        Description = _attribute.Description.CreateLabelFromString(),
                        RequiredLevel = new AttributeRequiredLevelManagedProperty(_attribute.RequiredLevel),
                        IsAuditEnabled = new BooleanManagedProperty(_attribute.IsAuditEnabled),
                        //Targets = 
                        //Format = LookupFormat.Regarding
                    };
                }
                else
                {
                    attributeMetadata = (LookupAttributeMetadata) _existingMetadata;
                    attributeMetadata.DisplayName = _attribute.DisplayName.CreateLabelFromString();
                    attributeMetadata.Description = string.IsNullOrEmpty(_attribute.Description)
                        ? attributeMetadata.Description
                        : _attribute.Description.CreateLabelFromString();
                    attributeMetadata.RequiredLevel = new AttributeRequiredLevelManagedProperty(_attribute.RequiredLevel);
                    attributeMetadata.IsAuditEnabled = new BooleanManagedProperty(_attribute.IsAuditEnabled);
                    
                }
                
                return attributeMetadata;
            }
        }
    }
}
