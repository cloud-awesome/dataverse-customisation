using System.ServiceModel;
using System.Xml;
using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.Exceptions;
using CloudAwesome.Dataverse.Core.PlatformModels;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;

namespace CloudAwesome.Dataverse.Customisation.CustomisationGeneration;

public static class EntityModel
{
    /// <summary>
    /// Create entities and attributes from a ConfigurationManifest
    /// </summary>
    /// <param name="manifest"></param>
    /// <param name="client"></param>
    /// <param name="t"></param>
    /// <param name="publisherPrefix"></param>
    public static void Generate(GenerateCustomisationsManifest manifest, IOrganizationService client, 
        TracingHelper t, string publisherPrefix)
    {
        if (manifest.Entities == null)
        {
            t.Debug($"No entities in manifest to be processed");
        }
        else
        {
            foreach (var entity in manifest.Entities)
            {
                t.Debug($"Processing entity: {entity.DisplayName}");
                CreateOrUpdate(client, publisherPrefix, manifest, entity);
                t.Info($"Entity {entity.DisplayName} created or updated");

                if (entity.Attributes == null)
                {
                    t.Debug($"No attributes to process for entity {entity.DisplayName}");
                }
                else
                {
                    XmlDocument? formXml = null;
                    
                    var attributeModel = new AttributeModel();
                    foreach (var attribute in entity.Attributes)
                    {
                        t.Debug($"Processing attribute: {attribute.DisplayName}");
                        attribute.EntitySchemaName = entity.SchemaName;

                        try
                        {
                            var attributeMetaData = attributeModel.CreateOrUpdate(client, attribute, manifest.SolutionName, publisherPrefix);

                            if (attribute.AddToForm)
                            {
                                attributeModel.AddToSystemForms();
                                t.Debug($"Attribute {attribute.DisplayName} added to form");
                            }

                            // TODO - add to views
                            if (attribute.AddToViewOrder.HasValue)
                            {
                                attributeModel.AddToSystemViews();
                                t.Debug($"Attribute {attribute.DisplayName} added to views");
                            }
                                
                            t.Info($"Attribute {attribute.DisplayName} successfully processed");
                        }
                        catch (NotCustomisableException e)
                        {
                            t.Warning(e.Message);
                        }
                    }

                    if (formXml != null)
                    {
                        //FormHelper.UpdateFormXml(client, entity.SchemaName, formXml, "Information");   
                    }
                }

                /*if (entity.EntityPermissions == null)
                {
                    t.Debug($"Entity {entity.DisplayName} has no security permissions to process");
                }
                else
                {
                    // TODO - extract this out into a new method (and refactor the CreateSecurityRoles method too)

                    var rootBusinessUnit = XrmClient.GetRootBusinessUnit(client);
                        
                    foreach (var entityPermission in entity.EntityPermissions)
                    {
                        SecurityRoles.UpdateRoleEntityPermission(client, entity.SchemaName, entityPermission.RoleName,
                            "Create", entityPermission.Create, rootBusinessUnit, t);
                        SecurityRoles.UpdateRoleEntityPermission(client, entity.SchemaName, entityPermission.RoleName,
                            "Read", entityPermission.Read, rootBusinessUnit, t);
                        SecurityRoles.UpdateRoleEntityPermission(client, entity.SchemaName, entityPermission.RoleName,
                            "Write", entityPermission.Write, rootBusinessUnit, t);
                        SecurityRoles.UpdateRoleEntityPermission(client, entity.SchemaName, entityPermission.RoleName,
                            "Delete", entityPermission.Delete, rootBusinessUnit, t);
                        SecurityRoles.UpdateRoleEntityPermission(client, entity.SchemaName, entityPermission.RoleName,
                            "Append", entityPermission.Append, rootBusinessUnit, t);
                        SecurityRoles.UpdateRoleEntityPermission(client, entity.SchemaName, entityPermission.RoleName,
                            "AppendTo", entityPermission.AppendTo, rootBusinessUnit, t);
                        SecurityRoles.UpdateRoleEntityPermission(client, entity.SchemaName, entityPermission.RoleName,
                            "Share", entityPermission.Share, rootBusinessUnit, t);
                    }
                }*/

                t.Info($"Entity {entity.DisplayName} successfully processed");
            }
        }
    }
        
    public static void CreateOrUpdate(IOrganizationService client, string publisherPrefix, 
        GenerateCustomisationsManifest manifest, CdsEntity entity)
    {
        entity._solutionName = manifest.SolutionName;
        entity._publisherPrefix = publisherPrefix;

        entity.SchemaName = string.IsNullOrEmpty(entity.SchemaName)
            ? entity.DisplayName.GenerateLogicalNameFromDisplayName(publisherPrefix)
            : entity.SchemaName;

        var existingMetadata = new EntityMetadata();
        bool existingEntity;
        try
        {
            var retrieveEntity = new RetrieveEntityRequest()
            {
                LogicalName = entity.SchemaName,
                EntityFilters = EntityFilters.Entity
            };
            existingMetadata = ((RetrieveEntityResponse) client.Execute(retrieveEntity)).EntityMetadata;
            existingEntity = true;
        }
        catch (FaultException)
        {
            existingEntity = false;
        }
            
        if (existingEntity)
        {
            if (existingMetadata.IsCustomizable != null &&
                !existingMetadata.IsCustomizable.Value)
            {
                throw new NotCustomisableException(
                    $"Entity {entity.SchemaName} is a managed entity and cannot be customised");
            }
            entity.Update(client, existingMetadata);
        }
        else
        {
            entity.Create(client);
        }
    }
}
