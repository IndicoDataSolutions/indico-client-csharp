using System;
using System.Collections.Generic;

namespace Indico.Entity
{
    [Obsolete("This is the V1 Version of the object. Please use V2 where possible.")]
    public abstract class Filter
    {
        protected List<dynamic> MergeFilters(IReadOnlyList<Filter> filters)
        {
            var list = new List<dynamic>();
            if(filters != null)
            {
                foreach (var filter in filters)
                {
                    list.Add(filter.ToAnonymousType());
                }
            }

            return list;
        }

        public abstract dynamic ToAnonymousType();
    }
}
