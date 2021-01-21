using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace IndicoV2.Submissions.Models
{
    public class AndFilter : MergeableFilter
    {
        public List<IFilter> And { get; set; }

        public override dynamic ToAnonymousType()
        {
            dynamic anonymousType = new ExpandoObject();

            if (And != null)
                anonymousType.ands = MergeFilters(And);

            return anonymousType;
        }
    }
}
