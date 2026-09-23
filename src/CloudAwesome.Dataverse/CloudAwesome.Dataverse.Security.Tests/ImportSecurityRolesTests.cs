using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.Loggers;
using CloudAwesome.Dataverse.Security.Models;
using CloudAwesome.Xrm.Simulate;
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
    }

    [Test]
    public void Run_does_not_change_roles_when_assignments_already_match_manifest()
    {
        // Arrange
        _organizationService.Simulated().SecurityModel()
            .WithBusinessUnit(_businessUnitId, "Root")
            .WithTeam(TeamId, _businessUnitId, "Delivery Team")
            .WithRole("Basic User")
            .AssignRoleToTeam("Basic User", TeamId);
        
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
        Assert.That(_organizationService.Simulated().SecurityModel().Model.RoleAssignments.Count, Is.EqualTo(1));
        Assert.That(_organizationService.Simulated().SecurityModel().Model.RoleAssignments.Single().RoleName, Is.EqualTo("Basic User"));
    }

    [Test]
    public void Run_adds_missing_roles()
    {
        // Arrange
        _organizationService.Simulated().SecurityModel()
            .WithBusinessUnit(_businessUnitId, "Root")
            .WithTeam(TeamId, _businessUnitId, "Delivery Team")
            .WithRole("Basic User");
        
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
        
        //Act
        new ImportSecurityRoles().Run(_organizationService, new TracingHelper(), manifest);
        
        //Assert
        Assert.That(_organizationService.Simulated().SecurityModel().Model.RoleAssignments.Count, Is.EqualTo(1));
        
    }
    
    [Test]
    public void Run_removes_surplus_roles()
    {
        // Arrange
        _organizationService.Simulated().SecurityModel()
            .WithBusinessUnit(_businessUnitId, "Root")
            .WithTeam(TeamId, _businessUnitId, "Delivery Team")
            .WithRole("Delivery Role")
            .AssignRoleToTeam("Delivery Role", TeamId);
        
        var manifest = new ImportSecurityRolesManifest
        {
            Teams =
            [
                new TeamModel
                {
                    Id = TeamId,
                    Name = "Delivery Team",
                    Roles = []
                }
            ]
        };
        
        //Act
        new ImportSecurityRoles().Run(_organizationService, new TracingHelper(), manifest);
        
        //Assert
        Assert.That(_organizationService.Simulated().SecurityModel().Model.RoleAssignments.Count, Is.EqualTo(0));
    }
}
