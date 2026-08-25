using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace CloudAwesome.Dataverse.Core.PlatformModels;

public enum CdsAttributeDataType
{
	Boolean, DateTime, Integer, Lookup, Memo, Picklist, String
}

public class CdsAttribute
{
	public string PublisherPrefix = string.Empty;
	public string SolutionName = string.Empty;

	public string EntitySchemaName { get; set; } = string.Empty;
	public string DisplayName { get; set; } = string.Empty;
	public string SchemaName { get; set; } = string.Empty;
	public CdsAttributeDataType DataType { get; set; }
	public string GlobalOptionSet { get; set; } = string.Empty; // TODO - OptionSet Default Value
	public string Description { get; set; } = string.Empty;
	public AttributeRequiredLevel RequiredLevel { get; set; }
	public bool IsAuditEnabled { get; set; }
	public string SourceType { get; set; } = string.Empty;
	public int? MaxLength { get; set; }
	public DateTimeFormat DateTimeFormat { get; set; }
	public StringFormat? StringFormat { get; set; }
	public string AutoNumberFormat { get; set; } = string.Empty;
	public int? MinValue { get; set; }
	public int? MaxValue { get; set; }
	public string ReferencedEntity { get; set; } = string.Empty;
	public string RelationshipNameSuffix { get; set; } = string.Empty;
	public bool ParentCascade { get; set; }
	public bool AddToForm { get; set; }
	public int? AddToViewOrder { get; set; }
	
	public AttributeMetadata Create(IOrganizationService client) 
    {
        var attributeContext = new AttributeMetadataContext(this, this.PublisherPrefix);
        var attributeMetadata = attributeContext.GetAttributeMetadataType(this.DataType).AttributeMetadata;

        if (this.DataType == CdsAttributeDataType.Lookup)
        {
            var lookupRelationshipName = $"{PublisherPrefix}_{this.EntitySchemaName.ToLower()}_{this.ReferencedEntity.ToLower()}";
            if (!string.IsNullOrEmpty(this.RelationshipNameSuffix))
            {
                lookupRelationshipName = $"{lookupRelationshipName}_{this.RelationshipNameSuffix.ToLower().GenerateLogicalNameFromDisplayName("")}";
            }

            var relationshipMetadata = new OneToManyRelationshipMetadata()
            {
                AssociatedMenuConfiguration = new AssociatedMenuConfiguration()
                {
                    Behavior = AssociatedMenuBehavior.DoNotDisplay,
                    Group = AssociatedMenuGroup.Details
                },
                CascadeConfiguration = new CascadeConfiguration()
                {
                    Assign = this.ParentCascade ? CascadeType.Cascade : CascadeType.NoCascade,
                    Delete = this.ParentCascade ? CascadeType.Cascade : CascadeType.RemoveLink,
                    Merge = this.ParentCascade ? CascadeType.Cascade : CascadeType.NoCascade,
                    Reparent = this.ParentCascade ? CascadeType.Cascade : CascadeType.NoCascade,
                    RollupView = this.ParentCascade ? CascadeType.Cascade : CascadeType.NoCascade,
                    Share = this.ParentCascade ? CascadeType.Cascade : CascadeType.NoCascade,
                    Unshare = this.ParentCascade ? CascadeType.Cascade : CascadeType.NoCascade
                },
                ReferencedEntity = this.ReferencedEntity.ToLower(),
                ReferencingEntity = this.EntitySchemaName,
                SchemaName = lookupRelationshipName
            };

            var request = new CreateOneToManyRequest()
            {
                Lookup = (LookupAttributeMetadata) attributeMetadata,
                OneToManyRelationship = relationshipMetadata,
                SolutionUniqueName = this.SolutionName
            };
            var response = (CreateOneToManyResponse) client.Execute(request);
            attributeMetadata.MetadataId = response.AttributeId;
        }
        else
        {
            var request = new CreateAttributeRequest()
            {
                Attribute = attributeMetadata,
                EntityName = this.EntitySchemaName,
                SolutionUniqueName = this.SolutionName
            };
            var response = (CreateAttributeResponse) client.Execute(request);
            attributeMetadata.MetadataId = response.AttributeId;
        }

        return attributeMetadata;
    }

    public AttributeMetadata Update(IOrganizationService client, AttributeMetadata existingMetadata)
    {
        var attributeContext = new AttributeMetadataContext(this, this.PublisherPrefix, existingMetadata);
        var attributeMetadata = attributeContext.GetAttributeMetadataType(this.DataType).AttributeMetadata;

        var request = new UpdateAttributeRequest()
        {
            Attribute = attributeMetadata,
            EntityName = this.EntitySchemaName,
            SolutionUniqueName = this.SolutionName
        };
        client.Execute(request);
        
        return attributeMetadata;
    }
}
