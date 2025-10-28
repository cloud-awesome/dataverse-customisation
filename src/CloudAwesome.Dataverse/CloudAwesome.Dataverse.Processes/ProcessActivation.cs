using CloudAwesome.Dataverse.Core;
using CloudAwesome.Dataverse.Core.EarlyBoundModels;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace CloudAwesome.Dataverse.Processes;

public class ProcessActivation
{
	private bool activate = false;
	
	public void SetStatusFromManifest(IOrganizationService client, TracingHelper t, ProcessActivationManifest manifest)
	{
		t.Debug($"Entering ProcessActivationWrapper.SetStatusFromManifest");

		activate = manifest.Status == ProcessActivationStatus.Enabled;
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

			if (solution.AllPluginSteps)
			{
				var plugins = GetPluginStepsFromSolution(client, solution.Name);
				var pluginStatus = TargetPluginStepStatus(activate);
				var pluginState = TargetPluginStepState(activate);
				
				ProcessPluginSteps(plugins, pluginState, pluginStatus, client, t);
			}

			if (solution.AllSlas)
			{
				var slas = GetSlasFromSolution(client, solution.Name);
				var slaStatus = TargetSlaStatus(activate);
				var slaState = TargetSlasState(activate);
				
				ProcessSlas(slas, slaState, slaStatus, solution.SetSlasAsDefault, client, t);
			}
		}
		
		t.Info($"{traceMessagePrefixMessage} processes complete.");
	}
	
	private void ProcessPluginSteps(EntityCollection steps, OptionSetValue pluginState, OptionSetValue pluginStatus, 
		IOrganizationService client, TracingHelper t)
	{
		var i = 0;
		
		foreach (var step in steps.Entities)
		{
			var workflow = (new Workflow(Guid.Parse(step["objectid"].ToString() ?? throw new InvalidOperationException()))
				.Retrieve(client));
			i++;

			try
			{
				var setState = new SetStateRequest
				{
					EntityMoniker = new EntityReference(workflow.LogicalName, workflow.Id),
					State = pluginState,
					Status = pluginStatus
				};

				client.Execute(setState);
				
				t.Info($"{i}/{steps.Entities.Count}. '{workflow["name"]}' updated");
			}
			catch (Exception e)
			{
				t.Error($"*** Failed to set: {i}/{steps.Entities.Count}. {workflow["name"]}, ({e.Message})");
			}
		}
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

	private void ProcessSlas(EntityCollection slas, OptionSetValue slaState, OptionSetValue slaStatus, 
		bool setAsDefault, IOrganizationService client, TracingHelper t)
	{
		var i = 0;
		
		foreach (var sla in slas.Entities)
		{
			var slaRecord = new Sla(Guid.Parse(sla["objectid"].ToString() ?? throw new InvalidOperationException()))
				.Retrieve(client);
			i++;

			if (slaRecord.Attributes[Sla.Fields.StateCode].Equals(slaState))
			{
				t.Info($"{i}/{slas.Entities.Count}. '{slaRecord["name"]}' already at target state");
				
				if (setAsDefault && activate && !slaRecord.Attributes[Sla.Fields.IsDefault].Equals(true))
				{
					slaRecord.Attributes[Sla.Fields.IsDefault] = true;
					client.Update(slaRecord);
					
					t.Info($"  - '{slaRecord["name"]}' set to default");
				}
				
				continue;
			}
			
			try
			{
				var setState = new SetStateRequest
				{
					EntityMoniker = new EntityReference(slaRecord.LogicalName, slaRecord.Id),
					State = slaState,
					Status = slaStatus
				};

				client.Execute(setState);

				if (setAsDefault && activate)
				{
					var defaultSla = new Sla(slaRecord.Id);
					defaultSla.IsDefault = true;
					client.Update(defaultSla);
				}
				
				t.Info($"{i}/{slas.Entities.Count}. '{slaRecord["name"]}' updated");
			}
			catch (Exception e)
			{
				t.Error($"*** Failed to set: {i}/{slas.Entities.Count}. {slaRecord["name"]}, ({e.Message})");
			}
		}
	}
	
	private EntityCollection GetCloudFlowsFromSolution(IOrganizationService client, string solutionName)
	{
		return SolutionWrapper.RetrieveSolutionComponents(client, solutionName, ComponentType.Workflow);
	}
	
	private EntityCollection GetPluginStepsFromSolution(IOrganizationService client, string solutionName)
	{
		return SolutionWrapper.RetrieveSolutionComponents(client, solutionName, ComponentType.SdkMessageProcessingStep);
	}
	
	private EntityCollection GetSlasFromSolution(IOrganizationService client, string solutionName)
	{
		return SolutionWrapper.RetrieveSolutionComponents(client, solutionName, ComponentType.Sla);
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
	
	private OptionSetValue TargetSlasState(bool activate)
	{
		return activate
			? new OptionSetValue((int)Sla_StateCode.Active)
			: new OptionSetValue((int)Sla_StateCode.Draft);
	}
	
	private OptionSetValue TargetSlaStatus(bool activate)
	{
		return activate
			? new OptionSetValue((int)Sla_StatusCode.Active)
			: new OptionSetValue((int)Sla_StatusCode.Draft);
	}
}