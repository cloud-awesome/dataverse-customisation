using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.EarlyBoundModels;
using CloudAwesome.Dataverse.Security.Models;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace CloudAwesome.Dataverse.Security;

public class ImportSecurityRoles
{
	private readonly Dictionary<(string roleName, Guid businessUnitId), Guid> _roleLookup = new();
	
	public void Run(IOrganizationService client, TracingHelper t, ImportSecurityRolesManifest manifest)
	{
		t.Debug($"Entering ImportSecurityRoles.Run");
		PreloadRoles(client, t);
		
		foreach (var manifestTeam in manifest.Teams)
		{
			var teamRecord = 
				client.Retrieve(
						Team.EntityLogicalName, 
						manifestTeam.Id, 
						new ColumnSet(Team.Fields.Id, Team.Fields.BusinessUnitId))
					.ToEntity<Team>();

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

			var missingRoles = new List<string>();
			var surplusRoles = new List<string>();

			if (teamRolesQuery.Entities.Count == 0)
			{
				missingRoles.AddRange(
					manifestTeam.Roles.Select(securityRole => securityRole));
			}
			else
			{
				var currentTeamRoles = teamRolesQuery.Entities.Select(e =>
				{
					var teamRole = e.ToEntity<TeamRoles>();
					
					var roleNameAddress = $"roleAlias.{Role.Fields.Name}";
					string? roleName = null;
    
					if (e.Contains(roleNameAddress) && e[roleNameAddress] is AliasedValue aliasedValue)
					{
						roleName = aliasedValue.Value?.ToString();
					}

					return new SecurityRole
					{
						Name = roleName!
					};
				}).ToList();
				
				// Find the cross-sections
				missingRoles = manifestTeam.Roles
					.Where(r1 => currentTeamRoles.All(r2 => r2.Name != r1))
					.Select(x => x)
					.ToList();

				surplusRoles = currentTeamRoles
					.Where(r2 => manifestTeam.Roles.All(r1 => r1 != r2.Name))
					.Select(x => x.Name)
					.ToList();
			}

			t.Info($"Missing Roles to be added for Team {manifestTeam.Name}: {missingRoles.Count}");
			foreach (var missingRole in missingRoles)
			{
				try
				{
					AddSecurityRoleToTeam(client, manifestTeam.Id, teamRecord.BusinessUnitId.Id, missingRole);
				}
				catch (InvalidOperationException e)
				{
					t.Warning(e.Message);
				}
			}
			
			t.Info($"Surplus Roles to be removed for Team {manifestTeam.Name}: {surplusRoles.Count}");
			foreach (var surplusRole in surplusRoles)
			{
				try
				{
					RemoveSecurityRoleFromTeam(client, manifestTeam.Id, teamRecord.BusinessUnitId.Id, surplusRole);
				}
				catch (InvalidOperationException e)
				{
					t.Warning(e.Message);
				}
			}
		}
	}
	
	private void AddSecurityRoleToTeam(IOrganizationService client, Guid teamId, Guid businessUnitId, string roleName)
	{
		var teamReference = new EntityReference(Team.EntityLogicalName, teamId);
		var roleReference = FindRoleReference(roleName, businessUnitId);

		client.Associate(
			teamReference.LogicalName,
			teamReference.Id,
			new Relationship(Team.Fields.TeamRoles_Association),
			[roleReference]
		);
	}
	
	private void RemoveSecurityRoleFromTeam(IOrganizationService client, Guid teamId, Guid businessUnitId, string roleName)
	{
		var teamReference = new EntityReference(Team.EntityLogicalName, teamId);
		var roleReference = FindRoleReference(roleName, businessUnitId);

		client.Disassociate(
			teamReference.LogicalName,
			teamReference.Id,
			new Relationship(Team.Fields.TeamRoles_Association),
			[roleReference]
		);
	}

	private void PreloadRoles(IOrganizationService client, TracingHelper t)
	{
		t.Info("Preloading all security roles into memory...");

		var allRoles = new QueryExpression
		{
			EntityName = Role.EntityLogicalName,
			ColumnSet = new ColumnSet(Role.Fields.RoleId, Role.Fields.Name, Role.Fields.BusinessUnitId)
		}.RetrieveMultiple(client).Entities;

		foreach (var role in allRoles)
		{
			var roleName = role.GetAttributeValue<string>(Role.Fields.Name);
			var businessUnitId = role.GetAttributeValue<EntityReference>(Role.Fields.BusinessUnitId)?.Id ?? Guid.Empty;
			var roleId = role.Id;

			if (!string.IsNullOrEmpty(roleName) && businessUnitId != Guid.Empty)
			{
				var key = (roleName, businessUnitId);

				if (!_roleLookup.ContainsKey(key))
				{
					_roleLookup.Add(key, roleId);
				}
				else
				{
					t.Warning($"Duplicate role found: '{roleName}' in BU {businessUnitId}");
				}
			}
		}

		t.Debug($"Preloaded {_roleLookup.Count} role records.");
	}
	
	private EntityReference FindRoleReference(string roleName, Guid businessUnitId)
	{
		var key = (roleName, businessUnitId);

		if (!_roleLookup.TryGetValue(key, out var roleId))
		{
			throw new InvalidOperationException($"Role '{roleName}' not found for Business Unit {businessUnitId}.");
		}

		return new EntityReference(Role.EntityLogicalName, roleId);
	}
}