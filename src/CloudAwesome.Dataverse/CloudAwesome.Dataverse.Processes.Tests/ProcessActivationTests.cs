using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.EarlyBoundModels;
using CloudAwesome.Dataverse.Core.PlatformModels;
using CloudAwesome.Xrm.Simulate;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using NUnit.Framework;

namespace CloudAwesome.Dataverse.Processes.Tests;

[TestFixture]
public class ProcessActivationTests
{
    private IOrganizationService _organizationService = null!;
    
    [SetUp]
    public void SetUp()
    {
        _organizationService = _organizationService.Simulate();
    }
    
    [Test]
    public void SetStatusFromManifest_enables_all_flows_in_configured_solution()
    {
        var setStateRequests = new List<SetStateRequest>();
        _organizationService
            .Simulated()
            .CustomOrgRequests().Add<SetStateRequest>(
                (request, _) => 
                { 
                    setStateRequests.Add(request); 
                    return new OrganizationResponse 
                    { 
                        ResponseName = request.RequestName 
                    }; 
                });

        _organizationService.Simulated().Data().Add(_draftApprovalWorkflow);
        _organizationService.Simulated().Data().Add(_coreSolution);
        _organizationService.Simulated().Data().Add(_workflowSolutionComponent);
        
        var manifest = new ProcessActivationManifest
        {
            Status = ProcessActivationStatus.Enabled,
            Solutions =
            [
                new CdsSolution
                {
                    Name = "core_solution",
                    AllFlows = true
                }
            ]
        };

        new ProcessActivation().SetStatusFromManifest(_organizationService, new TracingHelper(), manifest);

        var request = setStateRequests.Single();
        Assert.That(request.EntityMoniker.LogicalName, Is.EqualTo(Workflow.EntityLogicalName));
        Assert.That(request.EntityMoniker.Id, Is.EqualTo(FlowId));
        Assert.That(request.State.Value, Is.EqualTo((int)Workflow_StateCode.Activated));
        Assert.That(request.Status.Value, Is.EqualTo((int)Workflow_StatusCode.Activated));
    }
    
    private static readonly Guid SolutionId = Guid.NewGuid();
    private static readonly Guid FlowId = Guid.NewGuid();

    private readonly Workflow _draftApprovalWorkflow = new Workflow
    {
        Id = FlowId,
        Name = "Account approval flow",
        StateCode = Workflow_StateCode.Draft,
        StatusCode = Workflow_StatusCode.Draft
    };
    
    private readonly Entity _coreSolution = new Entity("solution")
    {
        Id = SolutionId,
        ["solutionid"] = SolutionId,
        ["uniquename"] = "core_solution"
    };

    private readonly Entity _workflowSolutionComponent = new Entity("solutioncomponent")
    {
        ["componenttype"] = (int)ComponentType.Workflow,
        ["objectid"] = FlowId.ToString(),
        ["solutionid"] = SolutionId
    };
}
