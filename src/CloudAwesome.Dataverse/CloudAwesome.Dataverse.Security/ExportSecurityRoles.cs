using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.EarlyBoundModels;
using CloudAwesome.Dataverse.Security.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CloudAwesome.Dataverse.Security;

public class ExportSecurityRoles
{
	public void Run(IOrganizationService client, TracingHelper t, ExportSecurityRolesManifest manifest)
	{
		t.Debug($"Entering ExportSecurityRoles.Run");

		var importManifest = new ImportSecurityRolesManifest();

		foreach (var manifestTeam in manifest.Teams)
		{
			var teamRolesQuery = new QueryExpression
			{
				EntityName = TeamRoles.EntityLogicalName,
				ColumnSet = new ColumnSet(true),
				Criteria = new FilterExpression
				{
					Conditions =
					{
						new ConditionExpression(TeamRoles.Fields.TeamId, ConditionOperator.Equal, manifestTeam.Id)
					}
				},
				LinkEntities =
				{
					new LinkEntity
					{
						LinkFromEntityName = TeamRoles.EntityLogicalName,
						LinkToEntityName = Role.EntityLogicalName,
						LinkFromAttributeName = TeamRoles.Fields.RoleId,
						LinkToAttributeName = Role.Fields.RoleId,
						EntityAlias = "roleAlias",
						Columns = new ColumnSet(Role.Fields.Name)
					}
				}
			}.RetrieveMultiple(client);

			if (teamRolesQuery.Entities.Count == 0)
			{
				t.Warning($"Could not find the team with ID {manifestTeam.Id} ({manifestTeam.Name}) in the TeamRoles table. " +
				        $"Either the team does not exist or it has no security roles assigned.");
				
				continue;
			}

			var teamRoles = teamRolesQuery.Entities.Select(e => e.ToEntity<TeamRoles>()).ToList();
			
			var team = new TeamModel()
			{
				Name = manifestTeam.Name,
				Id = manifestTeam.Id
			};
			
			foreach (var teamRole in teamRoles)
			{
				var roleNameAddress = $"roleAlias.{Role.Fields.Name}";
				if (!teamRole.Contains(roleNameAddress) ||
				    teamRole[roleNameAddress] is not AliasedValue aliasedValue) continue;
				
				team.Roles.Add(aliasedValue.Value.ToString()!);
			}
			
			importManifest.Teams.Add(team);
		}

		if (importManifest.Teams.Count == 0)
		{
			t.Warning("No TeamRoles have been found. Nothing to export");
			return;
		}
		
		t.Info($"Creating json import manifest including {importManifest.Teams.Count} Teams.");
		SerialisationWrapper.SerialiseJsonToFile(manifest.OutputFilePath, importManifest);

		t.Info($"Import manifest file created at {manifest.OutputFilePath}");
	}
}