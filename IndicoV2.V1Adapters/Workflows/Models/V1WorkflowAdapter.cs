using IndicoV2.Abstractions.Workflows.Models;

namespace IndicoV2.V1Adapters.Workflows.Models
{
    public class V1WorkflowAdapter : Workflow
    {
        private readonly Indico.Entity.Workflow _workflowV1;

        public V1WorkflowAdapter(Indico.Entity.Workflow workflowV1)
        {
            _workflowV1 = workflowV1;
        }

        public override int Id => _workflowV1.Id;
    }
}
