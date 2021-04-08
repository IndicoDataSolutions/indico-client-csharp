using IndicoV2.Extensions.SubmissionResult;

namespace IndicoV2
{
    public static partial class IndicoClientExtensions
    {
        /// <summary>
        /// Gets <seealso cref="ISubmissionResultAwaiter"/>
        /// </summary>
        /// <param name="indicoClient">Instance of <seealso cref="IndicoClient"/></param>
        /// <returns></returns>
        public static ISubmissionResultAwaiter GetSubmissionResultAwaiter(this IndicoClient indicoClient) =>
            new SubmissionResultAwaiter(indicoClient.Submissions(), indicoClient.Storage());
    }
}
