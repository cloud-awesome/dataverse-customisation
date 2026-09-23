using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.EarlyBoundModels;
using CloudAwesome.Dataverse.Core.Loggers;
using CloudAwesome.Dataverse.Core.PlatformModels;
using CloudAwesome.Xrm.Simulate;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.Logging;
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

        var tracer = new TracingHelper(new ConsoleLogger(LogLevel.Debug));
        
        new ProcessActivation().SetStatusFromManifest(_organizationService, tracer, manifest);

        var workflow = _organizationService.Simulated().Data().Get<Workflow>();
        Assert.That(workflow.Count(), Is.EqualTo(1));
        Assert.That(workflow.Single().Id, Is.EqualTo(FlowId));
        Assert.That(workflow.Single().StateCode, Is.EqualTo(Workflow_StateCode.Activated));
        Assert.That(workflow.Single().StatusCode, Is.EqualTo(Workflow_StatusCode.Activated));
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
