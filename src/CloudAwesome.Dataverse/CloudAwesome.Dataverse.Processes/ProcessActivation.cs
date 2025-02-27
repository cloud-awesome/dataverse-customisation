using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.EarlyBoundEntities;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace CloudAwesome.Dataverse.Processes;

public class ProcessActivation
{
	public void SetStatusFromManifest(ProcessActivationManifest manifest, IOrganizationService client, TracingHelper t)
	{
		t.Debug($"Entering ProcessActivationWrapper.SetStatusFromManifest");

		var activate = manifest.Status == ProcessActivationStatus.Enabled;
		var traceMessagePrefixMessage = activate ? "Enabling" : "Deactivating";
		
		t.Info($"{traceMessagePrefixMessage} processes");

		foreach (var solution in manifest.Solutions)
		{
			if (solution.AllFlows)
			{
				var flows = GetCloudFlowsFromSolution(client, solution.Name);
				var flowStatus = TargetFlowStatus(activate);
				var flowState = TargetFlowState(activate);
				
				ProcessFlows(flows, flowState, flowStatus, client, t);
			}
		}
	}

	private void ProcessPluginSteps(TracingHelper t)
	{
		t.Warning("Plugin Steps are not supported while this feature is in alpha.");
	}

	private void ProcessFlows(EntityCollection flows, OptionSetValue flowState, OptionSetValue flowStatus, 
		IOrganizationService client, TracingHelper t)
	{
		var i = 0;
		
		foreach (var flow in flows.Entities)
		{
			var workflow = (new Workflow(Guid.Parse(flow["objectid"].ToString() ?? throw new InvalidOperationException()))
				.Retrieve(client));
			i++;

			try
			{
				var setState = new SetStateRequest
				{
					EntityMoniker = new EntityReference(workflow.LogicalName, workflow.Id),
					State = flowState,
					Status = flowStatus
				};

				client.Execute(setState);
				
				t.Info($"{i}/{flows.Entities.Count}. '{workflow["name"]}' updated");
			}
			catch (Exception e)
			{
				t.Error($"*** Failed to set: {i}/{flows.Entities.Count}. {workflow["name"]}, ({e.Message})");
			}
		}
	}

	private EntityCollection GetCloudFlowsFromSolution(IOrganizationService client, string solutionName)
	{
		return SolutionWrapper.RetrieveSolutionComponents(client, solutionName, ComponentType.Workflow);
	}
	
	

	private OptionSetValue TargetFlowState(bool activate)
	{
		return activate
			? new OptionSetValue((int)Workflow_StateCode.Activated)
			: new OptionSetValue((int)Workflow_StateCode.Draft);
	}
	
	private OptionSetValue TargetFlowStatus(bool activate)
	{
		return activate
			? new OptionSetValue((int)Workflow_StatusCode.Activated)
			: new OptionSetValue((int)Workflow_StatusCode.Draft);
	}
	
	private OptionSetValue TargetPluginStepState(bool activate)
	{
		return activate
			? new OptionSetValue((int)SdkMessageProcessingStep_StateCode.Enabled)
			: new OptionSetValue((int)SdkMessageProcessingStep_StateCode.Disabled);
	}
	
	private OptionSetValue TargetPluginStepStatus(bool activate)
	{
		return activate
			? new OptionSetValue((int)SdkMessageProcessingStep_StatusCode.Enabled)
			: new OptionSetValue((int)SdkMessageProcessingStep_StatusCode.Disabled);
	}
}