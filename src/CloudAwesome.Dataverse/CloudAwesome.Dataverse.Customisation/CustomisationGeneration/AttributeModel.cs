using System.ServiceModel;
using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.Exceptions;
using CloudAwesome.Dataverse.Core.PlatformModels;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace CloudAwesome.Dataverse.Customisation.CustomisationGeneration;

public class AttributeModel
{
	public void AddToSystemForms()
	{
		//throw new NotImplementedException();
	}

	public void AddToSystemViews()
	{
		//throw new NotImplementedException();
	}
    
    public AttributeMetadata CreateOrUpdate(IOrganizationService client, CdsAttribute cdsAttribute, string publisherPrefix, string solutionName)    
    {
        bool existingAttribute;
        cdsAttribute.PublisherPrefix = publisherPrefix;
        cdsAttribute.SolutionName = solutionName;

        cdsAttribute.SchemaName = string.IsNullOrEmpty(cdsAttribute.SchemaName)
            ? cdsAttribute.DisplayName.GenerateLogicalNameFromDisplayName(publisherPrefix)
            : cdsAttribute.SchemaName;
        
        var existingMetadata = new AttributeMetadata();
        try
        {
            var attribute = new RetrieveAttributeRequest()
            {
                EntityLogicalName = cdsAttribute.EntitySchemaName,
                LogicalName = cdsAttribute.SchemaName,
            };
            existingMetadata = ((RetrieveAttributeResponse) client.Execute(attribute)).AttributeMetadata;
            existingAttribute = true;
        }
        catch (FaultException)
        {
            existingAttribute = false;
        }

        AttributeMetadata attributeMetadata;
        if (existingAttribute)
        {
            if (existingMetadata.IsCustomizable != null && 
                !existingMetadata.IsCustomizable.Value)
            {
                throw  new NotCustomisableException(
                    $"Attribute '{cdsAttribute.SchemaName}' on entity '{cdsAttribute.EntitySchemaName}' is managed and cannot be customised");
            }
            attributeMetadata = cdsAttribute.Update(client, existingMetadata);
        }
        else
        {
            attributeMetadata = cdsAttribute.Create(client);
        }

        return attributeMetadata;
    }
}