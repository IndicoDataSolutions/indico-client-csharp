using System;

namespace IndicoV2.Jobs.Models.Results
{
    public interface IUrlJobResult : IJobResult
    {
        Uri Url { get; }
    }
}
