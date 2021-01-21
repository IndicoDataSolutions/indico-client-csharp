using System;
using System.Collections.Generic;
using System.Text;

namespace IndicoV2.Submissions.Models
{
    public abstract class MergeableFilter : IMergableFilter
    {
        public List<dynamic> MergeFilters(IReadOnlyList<IFilter> filters)
        {
            List<dynamic> list = new List<dynamic>();
            if (filters != null)
                foreach (var filter in filters)
                    list.Add(filter.ToAnonymousType());
            return list;
        }

        public abstract dynamic ToAnonymousType();
    }
}
