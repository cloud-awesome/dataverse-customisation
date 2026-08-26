using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.EarlyBoundModels;
using CloudAwesome.Dataverse.Core.Loggers;
using CloudAwesome.Dataverse.Security.Models;
using CloudAwesome.Xrm.Simulate;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;

namespace CloudAwesome.Dataverse.Security.Tests;

[TestFixture]
public class ImportSecurityRolesTests
{
    private readonly Guid _businessUnitId = Guid.NewGuid();
    private static readonly Guid TeamId = Guid.NewGuid();
    private IOrganizationService _organizationService = null!;
    
    [SetUp]
    public void SetUp()
    {
        _organizationService = _organizationService.Simulate();
        _organizationService.Simulated().Data().Add(
            new Team
            {
                Id = TeamId,
                Name = "Delivery Team",
                BusinessUnitId = new EntityReference("businessunit", _businessUnitId)
            });
    }

    [Test]
    public void Run_does_not_change_roles_when_assignments_already_match_manifest()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        _organizationService.Simulated().Data().Add(
            new Role
            {
                Id = roleId,
                Name = "Basic User",
                BusinessUnitId = new EntityReference("businessunit", _businessUnitId) 
            });
        _organizationService.Simulated().Data().Add(
            new TeamRoles
            {
                Id = Guid.NewGuid(),
                TeamId = TeamId,
                RoleId = roleId
            });
        
        var manifest = new ImportSecurityRolesManifest
        {
            Teams =
            [
                new TeamModel
                {
                    Id = TeamId,
                    Name = "Delivery Team",
                    Roles = ["Basic User"]
                }
            ]
        };
        
        // Act
        new ImportSecurityRoles().Run(_organizationService, new TracingHelper(), manifest);

        // Assert
        var assignedRoles = 
            _organizationService
                .Simulated().Data().Get<TeamRoles>()
                .Where(teamRole => teamRole.TeamId == TeamId)
                .Select(teamRole => teamRole.RoleId!.Value)
                .ToList();
        
        Assert.That(assignedRoles.Count, Is.EqualTo(1));
        Assert.That(assignedRoles, Is.EquivalentTo([roleId]));
    }

    [Test]
    public void Run_adds_missing_roles()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        _organizationService.Simulated().Data().Add(
            new Role
            {
                Id = roleId,
                Name = "Basic User",
                BusinessUnitId = new EntityReference("businessunit", _businessUnitId) 
            });
        
        var manifest = new ImportSecurityRolesManifest
        {
            Teams =
            [
                new TeamModel
                {
                    Id = TeamId,
                    Name = "Delivery Team",
                    Roles = ["Basic User"]
                }
            ]
        };
        
        // Outputs dataverse-simulate log messages to the console
        var tracer = new TracingHelper(new ConsoleLogger(LogLevel.Debug));
        
        //Act
        new ImportSecurityRoles().Run(_organizationService, tracer, manifest);
        
        //Assert
        var teamRelatedEntities = 
            _organizationService
                .Simulated().Data().Get<Team>()
                .Where(teamRole => teamRole.TeamId == TeamId)
                .Select(r => r.RelatedEntities)
                .ToList();
        
        Assert.That(teamRelatedEntities.Count, Is.EqualTo(1));
        Assert.That(teamRelatedEntities[0].ContainsKey(new Relationship(Team.Fields.TeamRoles_Association)));
        // TODO - check the actual role that's related
    }

    /*
    [Test]
    public void Run_removes_surplus_roles()
    {
        var roleId = AddRole("Basic User", _businessUnitId);
        AssignRole(TeamId, roleId);

        RunImport([]);

        AssertAssignedRoleIds([]);
    }

    [Test]
    public void Run_adds_missing_roles_and_removes_surplus_roles()
    {
        var existingRoleId = AddRole("Basic User", _businessUnitId);
        var surplusRoleId = AddRole("Salesperson", _businessUnitId);
        var missingRoleId = AddRole("System Customizer", _businessUnitId);
        AssignRole(TeamId, existingRoleId);
        AssignRole(TeamId, surplusRoleId);

        RunImport(["Basic User", "System Customizer"]);

        AssertAssignedRoleIds([existingRoleId, missingRoleId]);
        Assert.That(AssignedRoleIds(), Does.Not.Contain(surplusRoleId));
    }

    [Test]
    public void Run_uses_role_from_team_business_unit_when_role_names_are_duplicated()
    {
        var otherBusinessUnitId = Guid.NewGuid();
        var otherBusinessUnitRoleId = AddRole("Basic User", otherBusinessUnitId);
        var matchingRoleId = AddRole("Basic User", _businessUnitId);

        RunImport(["Basic User"]);

        AssertAssignedRoleIds([matchingRoleId]);
        Assert.That(AssignedRoleIds(), Does.Not.Contain(otherBusinessUnitRoleId));
    }*/
}
