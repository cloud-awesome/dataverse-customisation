using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.EarlyBoundEntities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CloudAwesome.Dataverse.Security;

public class ImportSecurityRoles
{
	public void Run(ImportSecurityRolesManifest manifest, IOrganizationService client, TracingHelper t)
	{
		t.Debug($"Entering ImportSecurityRoles.Run");

		foreach (var manifestTeam in manifest.Teams)
		{
			var teamRecord = client.Retrieve(
				Team.EntityLogicalName,
				manifestTeam.Id,
				new ColumnSet(Team.Fields.Id));

			if (teamRecord is null)
			{
				t.Warning($"Could not find team with ID {manifestTeam.Id} ({manifestTeam.Name}). Skipping.");
				continue;
			}
			
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
				}
			}.RetrieveMultiple(client);

			var missingRoles = new List<Guid>();
			var surplusRoles = new List<Guid>();

			if (teamRolesQuery.Entities.Count == 0)
			{
				missingRoles.AddRange(
					manifestTeam.Roles.Select(securityRole => securityRole.Id));
			}
			else
			{
				// Find the cross-sections
				missingRoles = manifestTeam.Roles
					.Where(r1 => teamRolesQuery.Entities.All(r2 => r2.ToEntity<TeamRoles>().RoleId != r1.Id))
					.Select(x => x.Id)
					.ToList();

				surplusRoles = teamRolesQuery.Entities
					.Where(r2 => manifestTeam.Roles.All(r1 => r1.Id != r2.ToEntity<TeamRoles>().RoleId))
					.Select(x => x.ToEntity<TeamRoles>().RoleId!.Value)
					.ToList();
			}

			t.Info($"Missing Roles to be added for Team {manifestTeam.Name}: {missingRoles.Count}");
			foreach (var missingRole in missingRoles)
			{
				AddSecurityRoleToTeam(client, manifestTeam.Id, missingRole);
			}
			
			t.Info($"Surplus Roles to be removed for Team {manifestTeam.Name}: {surplusRoles.Count}");
			foreach (var surplusRole in surplusRoles)
			{
				RemoveSecurityRoleFromTeam(client, manifestTeam.Id, surplusRole);
			}
		}
	}
	
	private void AddSecurityRoleToTeam(IOrganizationService client, Guid teamId, Guid roleId)
	{
		var teamReference = new EntityReference(Team.EntityLogicalName, teamId);
		var roleReference = new EntityReference(Role.EntityLogicalName, roleId);

		client.Associate(
			teamReference.LogicalName,
			teamReference.Id,
			new Relationship(Team.Fields.TeamRoles_Association),
			[roleReference]
		);
	}
	
	private void RemoveSecurityRoleFromTeam(IOrganizationService client, Guid teamId, Guid roleId)
	{
		var teamReference = new EntityReference(Team.EntityLogicalName, teamId);
		var roleReference = new EntityReference(Role.EntityLogicalName, roleId);

		client.Disassociate(
			teamReference.LogicalName,
			teamReference.Id,
			new Relationship(Team.Fields.TeamRoles_Association),
			[roleReference]
		);
	}
}