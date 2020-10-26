using System.Collections.Generic;

namespace Indico.Entity
{
    public abstract class Filter
    {
        protected List<dynamic> MergeFilters(IReadOnlyList<Filter> filters)
        {
            List<dynamic> list = new List<dynamic>();
            if(filters != null) 
                foreach(var filter in filters)
                    list.Add(filter.ToAnonymousType());
            return list;
        }

        public abstract dynamic ToAnonymousType();
    }
}
