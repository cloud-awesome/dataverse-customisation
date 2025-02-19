namespace CloudAwesome.Dataverse.Core.PlatformModels
{
    public class CdsSolution
    {
        public string Name { get; set; }
        public bool AllPluginSteps { get; set; }
        public bool AllFlows { get; set; }
        public bool AllWorkflows { get; set; }
        public bool AllCaseCreationRules { get; set; }
        public bool AllBusinessRules { get; set; }
    }
}
