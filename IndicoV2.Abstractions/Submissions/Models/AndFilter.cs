﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace IndicoV2.Submissions.Models
{
    public class AndFilter : IFilter
    {
        public List<IFilter> And { get; set; }
    }
}
