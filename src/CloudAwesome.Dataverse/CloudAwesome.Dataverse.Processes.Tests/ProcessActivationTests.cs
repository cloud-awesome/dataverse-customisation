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
    [Test]
    public void SetStatusFromManifest_enables_all_flows_in_configured_solution()
    {
        var flowId = Guid.NewGuid();
        var solutionId = Guid.NewGuid();
        IOrganizationService service = null!;
        service = service.Simulate();
        var setStateRequests = new List<SetStateRequest>();
        service
            .Simulated()
            .CustomOrgRequests()
            .Add<SetStateRequest>((request, _) =>
            {
                setStateRequests.Add(request);
                return new OrganizationResponse
                {
                    ResponseName = request.RequestName
                };
            });

        service.Simulated().Data().Add(new Entity("solution")
        {
            Id = solutionId,
            ["solutionid"] = solutionId,
            ["uniquename"] = "core_solution"
        });
        service.Simulated().Data().Add(new Entity("solutioncomponent")
        {
            ["componenttype"] = (int)ComponentType.Workflow,
            ["objectid"] = flowId.ToString(),
            ["solutionid"] = solutionId
        });
        service.Simulated().Data().Add(new Workflow
        {
            Id = flowId,
            Name = "Account approval flow"
        });

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

        new ProcessActivation().SetStatusFromManifest(service, new TracingHelper(), manifest);

        var request = setStateRequests.Single();
        Assert.That(request.EntityMoniker.LogicalName, Is.EqualTo(Workflow.EntityLogicalName));
        Assert.That(request.EntityMoniker.Id, Is.EqualTo(flowId));
        Assert.That(request.State.Value, Is.EqualTo((int)Workflow_StateCode.Activated));
        Assert.That(request.Status.Value, Is.EqualTo((int)Workflow_StatusCode.Activated));
    }
}
