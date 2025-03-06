using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.EarlyBoundEntities;
using CloudAwesome.Dataverse.Core.Exceptions;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CloudAwesome.Dataverse.Customisation;

public class SetEnvironmentVariable
{
	public void Run(IOrganizationService client, TracingHelper t, string key, string value)
	{
		var query = new QueryExpression
		{
			EntityName = EnvironmentVariableDefinition.EntityLogicalName,
			ColumnSet = new ColumnSet(EnvironmentVariableDefinition.PrimaryIdAttribute),
			Criteria = new FilterExpression
			{
				Conditions =
				{
					new ConditionExpression(EnvironmentVariableDefinition.Fields.SchemaName,
						ConditionOperator.Equal, key)
				}
			}
		};

		var definition = query.RetrieveSingleRecord(client);

		if (definition is null)
		{
			throw new QueryBaseException(
				$"Could not find an Environment Variable matching the input name ({key}). " +
				$"Ensure that you are providing the definition's schema name and that it exists in the target dataverse environment");
		}
		
		var query2 = new QueryExpression
		{
			EntityName = EnvironmentVariableValue.EntityLogicalName,
			ColumnSet = new ColumnSet(true),
			Criteria = new FilterExpression
			{
				Conditions =
				{
					new ConditionExpression(EnvironmentVariableValue.Fields.StateCode, 
						ConditionOperator.Equal, (int) EnvironmentVariableDefinition_StateCode.Active),
					new ConditionExpression(EnvironmentVariableValue.Fields.EnvironmentVariableDefinitionId,
						ConditionOperator.Equal, definition.Id)
				}
			},
			Orders =
			{
				new OrderExpression(EnvironmentVariableValue.Fields.ModifiedOn, OrderType.Descending)
			}
		};

		var values = query2.RetrieveMultiple(client);

		switch (values.Entities.Count)
		{
			case < 1:
			{
				// Create
				var newValue = new EnvironmentVariableValue
				{
					EnvironmentVariableDefinitionId = definition.ToEntityReference(),
					Value = value
				};

				newValue.Create(client);
			
				t.Info($"A value for Environment Variable ({key}) has been created");
				return;
			}
			case > 1:
				// This shouldn't be possible, so just including as a possible edge case
				t.Warning($"Multiple values were found related to this definition ({key}). Only the most recently modified will be updated.");
				break;
		}

		// Update
		var targetValue = values.Entities.FirstOrDefault()!;
		targetValue["value"] = value;

		targetValue.Update(client);
		
		t.Info($"Value for Environment Variable ({key}) has been updated");
	}
}