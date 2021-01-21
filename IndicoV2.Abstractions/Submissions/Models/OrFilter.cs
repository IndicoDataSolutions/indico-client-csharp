using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace IndicoV2.Submissions.Models
{
    public class OrFilter : IFilter
    {
        public List<IFilter> Or { get; set; }
    }
}
