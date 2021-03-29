// unset

using System.Collections.Generic;

namespace IndicoV2.V1Adapters.CommonModels.Predictions
{
    public interface IPrediction
    {
        string Label { get; }
        string Text { get; }
        int Start { get; }
        int End { get; }
        IReadOnlyDictionary<string, double> Confidence { get; }
    }
}