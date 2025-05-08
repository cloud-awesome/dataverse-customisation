using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.EarlyBoundModels;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CloudAwesome.Dataverse.Customisation;

public class SetEnvironmentVariable
{
	public void Run(IOrganizationService client, TracingHelper t, SetEnvironmentVariableManifest manifest)
	{
		var environmentVariables = new QueryExpression
		{
			EntityName = EnvironmentVariableDefinition.EntityLogicalName,
			ColumnSet = new ColumnSet(EnvironmentVariableDefinition.PrimaryIdAttribute, EnvironmentVariableDefinition.Fields.SchemaName, EnvironmentVariableDefinition.Fields.DisplayName),
		}.RetrieveMultiple(client).Entities.ToList();
		
		var environmentVariableValues = new QueryExpression
		{
			EntityName = EnvironmentVariableValue.EntityLogicalName,
			ColumnSet = new ColumnSet(true),
		}.RetrieveMultiple(client).Entities.ToList();
		
		foreach (var variableValue in manifest.Variables)
		{
			var definition = environmentVariables
				.Where(d => d.Attributes["schemaname"].ToString() == variableValue.Key)
				.ToList();

			if (!definition.Any())
			{
				t.Error($"Could not find an Environment Variable matching the input name ({variableValue.Key}). " +
				        $"Ensure that you are providing the definition's schema name and that it exists in the target dataverse environment");
				
				continue;
			}

			var existingValues = environmentVariableValues
				.Where(v => v.Attributes["schemaname"].ToString() == variableValue.Key)
				.ToList();
			
			switch (existingValues.Count)
			{
				case < 1:
				{
					// Create
					var newValue = new EnvironmentVariableValue
					{
						EnvironmentVariableDefinitionId = definition.SingleOrDefault()!.ToEntityReference(),
						Value = variableValue.Value
					};

					newValue.Create(client);
			
					t.Info($"A value for Environment Variable ({variableValue.Key}) has been created");
					return;
				}
				case > 1:
					// This shouldn't be possible, so just including as a possible edge case
					t.Warning($"Multiple values were found related to this definition ({variableValue.Key}). " +
					          $"Only the most recently modified will be updated.");
					break;
			}

			// Update
			var targetValue = existingValues.FirstOrDefault()!;
			targetValue["value"] = variableValue.Value;

			targetValue.Update(client);
		
			t.Info($"Value for Environment Variable ({variableValue.Key}) has been updated");
		}
	}
}