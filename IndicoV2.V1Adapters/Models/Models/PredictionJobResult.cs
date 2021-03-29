using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IndicoV2.Models.Models;
using IndicoV2.V1Adapters.CommonModels.Predictions;
using IndicoV2.V1Adapters.Converters;
using Newtonsoft.Json;

namespace IndicoV2.V1Adapters.Models.Models
{
    [JsonConverter(typeof(CastingConverter<Prediction[][]>))]
    public class PredictionJobResult : IPredictionJobResult
    {
        private readonly Prediction[][] _predictions;

        private PredictionJobResult(Prediction[][] predictions) => _predictions = predictions;

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IEnumerator<IReadOnlyCollection<IPrediction>> GetEnumerator() => _predictions.Cast<IReadOnlyCollection<Prediction>>().GetEnumerator();

        public int Count => _predictions.Length;

        public static implicit operator PredictionJobResult(Prediction[][] predictions) =>
            new PredictionJobResult(predictions);
    }
}
