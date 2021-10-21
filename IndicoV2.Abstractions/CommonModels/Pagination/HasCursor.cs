using System;
using System.Collections.Generic;
using System.Text;

namespace IndicoV2.CommonModels.Pagination
{
    public sealed class HasCursor<T> : IHasCursor<T>
    {
        public PageInfo PageInfo { get; set; }

        public T Data {get; set;}
    }
}
