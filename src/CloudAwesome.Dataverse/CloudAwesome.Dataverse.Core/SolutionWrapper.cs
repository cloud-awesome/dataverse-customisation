using CloudAwesome.Dataverse.Core.PlatformModels;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CloudAwesome.Dataverse.Core;

public static class SolutionWrapper
    {
        public static void AddSolutionComponent(IOrganizationService client, string solutionName,
            Guid solutionComponentId, ComponentType solutionComponentTypeCode)
        {
            if (string.IsNullOrEmpty(solutionName))
            {
                return;
            }

            client.Execute(new AddSolutionComponentRequest
            {
                ComponentType = (int)solutionComponentTypeCode,
                ComponentId = solutionComponentId,
                SolutionUniqueName = solutionName
            });
        }

        public static EntityCollection RetrieveSolutionComponents(IOrganizationService client, string solutionName, ComponentType componentType = ComponentType.All)
        {
            var solutionComponents = new QueryExpression()
            {
                EntityName = "solutioncomponent",
                ColumnSet = new ColumnSet(true),
                Criteria = new FilterExpression()
                {
                    Conditions =
                    {
                        new ConditionExpression("componenttype", ConditionOperator.Equal, (int)componentType)
                    }
                },
                LinkEntities =
                {
                    new LinkEntity()
                    {
                        LinkFromEntityName = "solutioncomponent",
                        LinkToEntityName = "solution",
                        LinkFromAttributeName = "solutionid",
                        LinkToAttributeName = "solutionid",
                        JoinOperator = JoinOperator.Inner,
                        LinkCriteria = new FilterExpression()
                        {
                            Conditions =
                            {
                                new ConditionExpression("uniquename", ConditionOperator.Equal, solutionName)
                            }
                        }
                    }
                }
            }.RetrieveMultiple(client);

            return solutionComponents;
        }
    }