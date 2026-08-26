using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.EarlyBoundModels;
using CloudAwesome.Dataverse.Core.Models;
using CloudAwesome.Xrm.Simulate;
using Microsoft.Extensions.Logging;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using NUnit.Framework;

namespace CloudAwesome.Dataverse.Customisation.Tests;

[TestFixture]
public class SetEnvironmentVariableTests
{
    private IOrganizationService _organizationService = null!;
    
    [SetUp]
    public void SetUp()
    {
        _organizationService = _organizationService.Simulate();
    }
    
    [Test]
    public void Run_creates_values_for_all_manifest_variables_without_existing_values()
    {
        var firstDefinitionId = Guid.NewGuid();
        var secondDefinitionId = Guid.NewGuid();
        
        _organizationService.Simulated().Data().Add(
            new EnvironmentVariableDefinition 
            { 
                EnvironmentVariableDefinitionId = firstDefinitionId,
                SchemaName = "ca_first" 
            });
        _organizationService.Simulated().Data().Add(
            new EnvironmentVariableDefinition 
            { 
                EnvironmentVariableDefinitionId = secondDefinitionId,
                SchemaName = "ca_second" 
            });

        var manifest = new SetEnvironmentVariableManifest
        {
            Variables =
            [
                new KeyValuePair { Key = "ca_first", Value = "first-value" },
                new KeyValuePair { Key = "ca_second", Value = "second-value" }
            ]
        };

        var process = new SetEnvironmentVariable();
        process.Run(_organizationService, new TracingHelper(), manifest);

        var createdValues = _organizationService.Simulated().Data().Get<EnvironmentVariableValue>();
        
        Assert.That(createdValues, Has.Count.EqualTo(2));
        Assert.That(createdValues.Select(e => e.GetAttributeValue<string>(EnvironmentVariableValue.Fields.Value)),
            Is.EquivalentTo(new[] { "first-value", "second-value" }));
        Assert.That(
            createdValues.Select(e => e.GetAttributeValue<EntityReference>(EnvironmentVariableValue.Fields.EnvironmentVariableDefinitionId).Id),
            Is.EquivalentTo(new[] { firstDefinitionId, secondDefinitionId }));
    }
}
