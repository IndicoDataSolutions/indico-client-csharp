using System.Collections.Generic;
using IndicoV2.CommonModels.Predictions;
using IndicoV2.Jobs.Models.Results;

namespace IndicoV2.Models.Models
{
    public interface IPredictionJobResult : IJobResult, IReadOnlyCollection<IReadOnlyCollection<IPrediction>>
    {
    }
}