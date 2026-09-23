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
    public void Run_writes_import_manifest_for_team_role_assignments()
    {
        var teamId = Guid.NewGuid();
        var businessUnitId = Guid.NewGuid();
        var outputFilePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, $"{Guid.NewGuid()}.json");

        _organizationService.Simulated().SecurityModel()
            .WithBusinessUnit(businessUnitId, "Root")
            .WithTeam(teamId, businessUnitId, "Delivery Team")
            .WithRole("Basic User")
            .WithRole("System Customizer")
            .AssignRoleToTeam("Basic User", teamId)
            .AssignRoleToTeam("System Customizer", teamId);
        
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
}
