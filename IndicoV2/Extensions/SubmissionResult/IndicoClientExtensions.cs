using IndicoV2.Extensions.SubmissionResult;

namespace IndicoV2
{
    public static partial class IndicoClientExtensions
    {
        public static ISubmissionResultAwaiter GetSubmissionResultAwaiter(this IndicoClient indicoClient) =>
            new SubmissionResultAwaiter(indicoClient.Submissions(), indicoClient.Jobs(), indicoClient.JobAwaiter(), indicoClient.Storage());
    }
}
