using CloudAwesome.Dataverse.Core.EarlyBoundModels;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CloudAwesome.Dataverse.Processes.Plugins
{
    internal class PluginQueries
    {
        internal static QueryBase GetSdkMessageQuery(string sdkMessageName)
        {
            return new QueryExpression(SdkMessage.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(SdkMessage.PrimaryIdAttribute, 
                    SdkMessage.PrimaryNameAttribute),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(SdkMessage.PrimaryNameAttribute, 
                            ConditionOperator.Equal, sdkMessageName)
                    }
                }
            };
        }

        internal static QueryBase GetSdkMessageFilterQuery(string entityName, Guid sdkMessageId)
        {
            return new QueryExpression(SdkMessageFilter.EntityLogicalName)
            {
                ColumnSet = new ColumnSet(SdkMessageFilter.Fields.Name, 
                    SdkMessageFilter.Fields.PrimaryObjectTypeCode, SdkMessageFilter.Fields.SdkMessageId),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(SdkMessageFilter.Fields.PrimaryObjectTypeCode, 
                            ConditionOperator.Equal, entityName),
                        new ConditionExpression(SdkMessageFilter.Fields.SdkMessageId, 
                            ConditionOperator.Equal, sdkMessageId.ToString())
                    }
                }
            };
        }

        internal static QueryBase GetChildPluginTypesQuery(EntityReference existingAssembly)
        {
            return new QueryByAttribute()
            {
                EntityName = PluginType.EntityLogicalName,
                ColumnSet = new ColumnSet(PluginType.PrimaryIdAttribute,
                    PluginType.PrimaryNameAttribute),
                Attributes = { PluginType.Fields.PluginAssemblyId },
                Values = { existingAssembly.Id }
            };
        }

        internal static QueryBase GetChildPluginStepsQuery(List<Guid> parentPluginsList)
        {
            return new QueryExpression()
            {
                EntityName = SdkMessageProcessingStep.EntityLogicalName,
                ColumnSet = new ColumnSet(SdkMessageProcessingStep.PrimaryIdAttribute,
                    SdkMessageProcessingStep.PrimaryNameAttribute),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(SdkMessageProcessingStep.Fields.PluginTypeId,
                            ConditionOperator.In, parentPluginsList)
                    }
                }
            };
        }

        /*internal static QueryBase GetChildCustomApisQuery(List<Guid> parentPluginsList)
        {
            return new QueryExpression()
            {
                EntityName = CustomAPI.EntityLogicalName,
                ColumnSet = new ColumnSet(CustomAPI.PrimaryIdAttribute,
                    CustomAPI.PrimaryNameAttribute),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(CustomAPI.Fields.PluginTypeId,
                            ConditionOperator.In, parentPluginsList)
                    }
                }
            };
        }*/

        internal static QueryBase GetChildPluginStepsQuery(EntityReference existingEndpoint)
        {
            return new QueryExpression()
            {
                EntityName = SdkMessageProcessingStep.EntityLogicalName,
                ColumnSet = new ColumnSet(SdkMessageProcessingStep.PrimaryIdAttribute,
                    SdkMessageProcessingStep.PrimaryNameAttribute),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(SdkMessageProcessingStep.Fields.EventHandler, 
                            ConditionOperator.Equal, existingEndpoint.Id)
                    }
                }
            };
        }

        internal static QueryBase GetChildEntityImagesQuery(List<Guid> parentStepsList)
        {
            return new QueryExpression()
            {
                EntityName = SdkMessageProcessingStepImage.EntityLogicalName,
                ColumnSet = new ColumnSet(SdkMessageProcessingStepImage.PrimaryIdAttribute,
                    SdkMessageProcessingStepImage.PrimaryNameAttribute),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression(SdkMessageProcessingStepImage.Fields.SdkMessageProcessingStepId,
                            ConditionOperator.In, parentStepsList)
                    }
                }
            };
        }
    }
}