﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace IndicoV2.Submissions.Models
{
    public class OrFilter : MergeableFilter
    {
        public List<IFilter> Or { get; set; }
    }
}
