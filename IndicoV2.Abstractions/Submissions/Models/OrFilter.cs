using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace IndicoV2.Submissions.Models
{
    public class OrFilter : MergeableFilter
    {
        public List<IFilter> Or { get; set; }

        public override dynamic ToAnonymousType()
        {
            dynamic anonymousType = new ExpandoObject();
            
            if (Or != null)
                anonymousType.ors = MergeFilters(Or);

            return anonymousType;
        }
    }
}
