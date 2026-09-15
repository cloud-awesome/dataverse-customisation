using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.EarlyBoundModels;
using CloudAwesome.Dataverse.Security.Models;
using CloudAwesome.Xrm.Simulate;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;

namespace CloudAwesome.Dataverse.Security.Tests;

[TestFixture]
public class ExportSecurityRolesTests
{
    private IOrganizationService _organizationService = null!;

    [SetUp]
    public void SetUp()
    {
        _organizationService = _organizationService.Simulate();
    }

    [Test]
    [Ignore("Awaiting bug fix in dataverse-simulate")]
    public void Run_writes_import_manifest_for_team_role_assignments()
    {
        var teamId = Guid.NewGuid();
        var businessUnitId = Guid.NewGuid();
        var outputFilePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, $"{Guid.NewGuid()}.json");

        AddTeam(teamId, "Delivery Team", businessUnitId);
        AssignRole(teamId, AddRole("Basic User", businessUnitId));
        AssignRole(teamId, AddRole("System Customizer", businessUnitId));

        var manifest = new ExportSecurityRolesManifest
        {
            OutputFilePath = outputFilePath,
            Teams =
            [
                new TeamModel
                {
                    Id = teamId,
                    Name = "Delivery Team"
                }
            ]
        };

        new ExportSecurityRoles().Run(_organizationService, new TracingHelper(), manifest);

        var importManifest = SerialisationWrapper.DeserialiseJsonFromFile<ImportSecurityRolesManifest>(outputFilePath);
        var team = importManifest.Teams.Single();
        Assert.That(team.Id, Is.EqualTo(teamId));
        Assert.That(team.Name, Is.EqualTo("Delivery Team"));
        Assert.That(team.Roles, Is.EquivalentTo(new[] { "Basic User", "System Customizer" }));
    }

    private void AddTeam(Guid teamId, string name, Guid businessUnitId)
    {
        _organizationService.Simulated().Data().Add(
            new Team
            {
                Id = teamId,
                Name = name,
                BusinessUnitId = new EntityReference("businessunit", businessUnitId)
            });
    }

    private Guid AddRole(string name, Guid businessUnitId)
    {
        var roleId = Guid.NewGuid();
        _organizationService.Simulated().Data().Add(
            new Role
            {
                Id = roleId,
                Name = name,
                BusinessUnitId = new EntityReference("businessunit", businessUnitId)
            });

        return roleId;
    }

    private void AssignRole(Guid teamId, Guid roleId)
    {
        _organizationService.Simulated().Data().Add(
            new TeamRoles
            {
                Id = Guid.NewGuid(),
                TeamId = teamId,
                RoleId = roleId
            });
    }
}
