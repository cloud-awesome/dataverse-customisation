using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.EarlyBoundModels;
using CloudAwesome.Dataverse.Core.Models;
using CloudAwesome.Dataverse.Core.PlatformModels;
using CloudAwesome.Xrm.Simulate;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;

namespace CloudAwesome.Dataverse.Processes.Tests;

[TestFixture]
public class PluginRegistrationTests
{
    private IOrganizationService _organizationService = null!;
    
    [SetUp]
    public void SetUp()
    {
        _organizationService = _organizationService.Simulate();
    }
    
    [Test]
    public void Register_creates_plugin_assembly_type_step_image_and_adds_components_to_solution()
    {
        var sdkMessageId = Guid.NewGuid();
        var sdkMessageFilterId = Guid.NewGuid();
        
        var solutionRequests = new List<AddSolutionComponentRequest>();
        _organizationService
            .Simulated()
            .CustomOrgRequests().Add<AddSolutionComponentRequest>(
                (request, _) => 
                {
                    solutionRequests.Add(request);
                    return new OrganizationResponse
                    {
                        ResponseName = request.RequestName
                    };
                });

        _organizationService.Simulated().Data().Add(
            new SdkMessage
            {
                Id = sdkMessageId,
                Name = "Create"
            });
        _organizationService.Simulated().Data().Add(
            new Entity(SdkMessageFilter.EntityLogicalName)
            {
                Id = sdkMessageFilterId,
                [SdkMessageFilter.Fields.Name] = "Create of account",
                [SdkMessageFilter.Fields.PrimaryObjectTypeCode] = "account",
                [SdkMessageFilter.Fields.SdkMessageId] = sdkMessageId
            });

        var manifest = new PluginRegistrationManifest
        {
            SolutionName = "core_solution",
            DataverseConnection = new DataverseConnection
            {
                ConnectionType = DataverseConnectionType.ConnectionString
            },
            PluginAssemblies =
            [
                new CdsPluginAssembly
                {
                    Name = "CloudAwesome.Dataverse.Processes.Tests",
                    FriendlyName = "Processes Tests",
                    Assembly = typeof(PluginRegistrationTests).Assembly.Location,
                    Plugins =
                    [
                        new CdsPlugin
                        {
                            Name = "Example.Plugin",
                            FriendlyName = "Example Plugin",
                            Description = "Registers an example plugin.",
                            Steps =
                            [
                                new CdsPluginStep
                                {
                                    Name = "Example.Plugin: Create of account",
                                    FriendlyName = "Create of account",
                                    Description = "Runs on account create.",
                                    Message = "Create",
                                    PrimaryEntity = "account",
                                    Stage = SdkMessageProcessingStep_Stage.PostOperation,
                                    ExecutionMode = SdkMessageProcessingStep_Mode.Synchronous,
                                    ExecutionOrder = 1,
                                    FilteringAttributes = ["name", "accountnumber"],
                                    EntityImages =
                                    [
                                        new CdsEntityImage
                                        {
                                            Name = "PreImage",
                                            Attributes = ["name"]
                                        }
                                    ]
                                }
                            ]
                        }
                    ]
                }
            ]
        };

        PluginRegistration.Register(manifest, _organizationService, new TracingHelper());

        var createdAssembly = _organizationService.Simulated().Data().Get(PluginAssembly.EntityLogicalName).Single();
        var createdPluginType = _organizationService.Simulated().Data().Get(PluginType.EntityLogicalName).Single();
        var createdStep = _organizationService.Simulated().Data().Get(SdkMessageProcessingStep.EntityLogicalName).Single();
        var createdImage = _organizationService.Simulated().Data().Get(SdkMessageProcessingStepImage.EntityLogicalName).Single();

        Assert.That(createdAssembly.GetAttributeValue<string>(PluginAssembly.Fields.Name),
            Is.EqualTo("CloudAwesome.Dataverse.Processes.Tests"));
        Assert.That(createdAssembly.GetAttributeValue<string>(PluginAssembly.Fields.Content), Is.Not.Empty);

        Assert.That(createdPluginType.GetAttributeValue<EntityReference>(PluginType.Fields.PluginAssemblyId).Id,
            Is.EqualTo(createdAssembly.Id));
        Assert.That(createdPluginType.GetAttributeValue<string>(PluginType.Fields.TypeName), Is.EqualTo("Example.Plugin"));

        Assert.That(createdStep.GetAttributeValue<EntityReference>(SdkMessageProcessingStep.Fields.EventHandler).Id,
            Is.EqualTo(createdPluginType.Id));
        Assert.That(createdStep.GetAttributeValue<EntityReference>(SdkMessageProcessingStep.Fields.SdkMessageId).Id,
            Is.EqualTo(sdkMessageId));
        Assert.That(createdStep.GetAttributeValue<EntityReference>(SdkMessageProcessingStep.Fields.SdkMessageFilterId).Id,
            Is.EqualTo(sdkMessageFilterId));
        Assert.That(createdStep.GetAttributeValue<string>(SdkMessageProcessingStep.Fields.FilteringAttributes),
            Is.EqualTo("name,accountnumber"));

        Assert.That(createdImage.GetAttributeValue<EntityReference>(SdkMessageProcessingStepImage.Fields.SdkMessageProcessingStepId).Id,
            Is.EqualTo(createdStep.Id));
        Assert.That(createdImage.GetAttributeValue<string>(SdkMessageProcessingStepImage.Fields.Attributes1),
            Is.EqualTo("name"));

        Assert.That(solutionRequests.Select(r => r.ComponentType),
            Is.EquivalentTo(new[] { (int)ComponentType.PluginAssembly, (int)ComponentType.SdkMessageProcessingStep }));
        Assert.That(solutionRequests.Select(r => r.SolutionUniqueName),
            Is.All.EqualTo("core_solution"));
    }
}
