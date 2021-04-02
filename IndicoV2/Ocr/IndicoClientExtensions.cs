using IndicoV2.Ocr;
using IndicoV2.V1Adapters.Ocr;

namespace IndicoV2
{
    public static partial class IndicoClientExtensions
    {
        /// <summary>
        /// Gets <seealso cref="IOcrClient"/>
        /// </summary>
        /// <param name="indicoClient">Instance of <seealso cref="IndicoClient"/></param>
        /// <returns>Instance of <seealso cref="IOcrClient"/></returns>
        public static IOcrClient Ocr(this IndicoClient indicoClient) =>
            new OcrV1ClientAdapter(indicoClient.LegacyClient, indicoClient.Storage());
    }
}
