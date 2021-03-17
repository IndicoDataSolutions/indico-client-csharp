using System.Collections.Generic;
using IndicoV2.Jobs.Models.Results;
using IndicoV2.V1Adapters.CommonModels.Predictions;

namespace IndicoV2.Models.Models
{
    public interface IPredictionJobResult : IJobResult, IReadOnlyCollection<IReadOnlyCollection<IPrediction>>
    {
    }
}