using System;
using System.Collections.Generic;
using System.Text;
using IndicoV2.StrawberryShake;

namespace IndicoV2.Workflows.Models
{
    public class WorkflowSs : IWorkflow
    {
        private readonly IListWorkflows_Workflows_Workflows _ssWorkflow;
        public WorkflowSs(IListWorkflows_Workflows_Workflows workflow) => _ssWorkflow = workflow;
        public int Id => _ssWorkflow.Id ?? 0;
        public bool ReviewEnabled => _ssWorkflow.ReviewEnabled ?? false;
        public string Name => _ssWorkflow.Name;
    }
}
