using System;
using System.Collections.Generic;
using System.Text;
using IndicoV2.StrawberryShake;

namespace IndicoV2.Workflows.Models
{
    public class Workflow : IWorkflow
    {
        public int Id { get; set; } = 0;
        public bool ReviewEnabled { get; set; } = false;
        public string Name { get; set; }
    }
}
