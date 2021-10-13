using IndicoV2.DataSets;
using IndicoV2.Extensions.DataSets;
using IndicoV2.Extensions.Jobs;
using IndicoV2.Extensions.SubmissionResult;
using IndicoV2.Extensions.Workflows;
using IndicoV2.Jobs;
using IndicoV2.Models;
using IndicoV2.Ocr;
using IndicoV2.Reporting;
using IndicoV2.Reviews;
using IndicoV2.Storage;
using IndicoV2.Submissions;
using IndicoV2.V1Adapters.DataSets;
using IndicoV2.V1Adapters.Jobs;
using IndicoV2.V1Adapters.Models;
using IndicoV2.V1Adapters.Ocr;
using IndicoV2.V1Adapters.Reviews;
using IndicoV2.V1Adapters.Storage;
using IndicoV2.Workflows;

namespace IndicoV2
{
    public static class IndicoClientExtensions
    {
        /// <summary>
        /// Gets <seealso cref="ISubmissionsClient"/>
        /// </summary>
        /// <param name="indicoClient">Instance of <seealso cref="IndicoClient"/></param>
        /// <returns>Instance of <seealso cref="ISubmissionsClient"/></returns>
        public static ISubmissionsClient Submissions(this IndicoClient indicoClient) => new SubmissionsClient(indicoClient);

        /// <summary>
        /// Gets <seealso cref="IWorkflowsClient"/>
        /// </summary>
        /// <param name="indicoClient">Instance of <seealso cref="IndicoClient"/></param>
        /// <returns>Instance of <seealso cref="IWorkflowsClient"/></returns>
        public static IWorkflowsClient Workflows(this IndicoClient indicoClient) => new WorkflowsClient(indicoClient);

        /// <summary>
        /// Gets <seealso cref="IStorageClient"/>
        /// </summary>
        /// <param name="indicoClient">Instance of <seealso cref="IndicoClient"/></param>
        /// <returns>Instance of <seealso cref="IStorageClient"/></returns>
        public static IStorageClient Storage(this IndicoClient indicoClient) => new V1StorageClientAdapter(indicoClient.LegacyClient);

        /// <summary>
        /// Gets <seealso cref="IReviewsClient"/>
        /// </summary>
        /// <param name="indicoClient">Instance of <seealso cref="IndicoClient"/></param>
        /// <returns>Instance of <seealso cref="IReviewsClient"/></returns>
        public static IReviewsClient Reviews(this IndicoClient indicoClient) =>
            new ReviewsV1ClientAdapter(indicoClient.LegacyClient);

        /// <summary>
        /// Gets <seealso cref="IJobsClient"/>
        /// </summary>
        /// <param name="indicoClient">Indico client</param>
        /// <returns>Instance of <seealso cref="IJobsClient"/> /></returns>
        public static IModelClient Models(this IndicoClient indicoClient) => new V1ModelClientAdapter(indicoClient.LegacyClient);

        /// <summary>
        /// Gets <seealso cref="IJobsClient"/>
        /// </summary>
        /// <param name="indicoClient">Indico client</param>
        /// <returns>Instance of <seealso cref="IJobsClient"/> /></returns>
        public static IJobsClient Jobs(this IndicoClient indicoClient) => new V1JobsClientAdapter(indicoClient.LegacyClient, new JobStatusConverter());

        /// <summary>
        /// Gets <seealso cref="IOcrClient"/>
        /// </summary>
        /// <param name="indicoClient">Instance of <seealso cref="IndicoClient"/></param>
        /// <returns>Instance of <seealso cref="IOcrClient"/></returns>
        public static IOcrClient Ocr(this IndicoClient indicoClient) =>
            new OcrV1ClientAdapter(indicoClient.LegacyClient, indicoClient.Storage());

        public static WorkflowAwaiter WorkflowAwaiter(this IndicoClient client) => new WorkflowAwaiter(client.Workflows());

        /// <summary>
        /// Gets <seealso cref="ISubmissionResultAwaiter"/>
        /// </summary>
        /// <param name="indicoClient">Instance of <seealso cref="IndicoClient"/></param>
        /// <returns></returns>
        public static ISubmissionResultAwaiter GetSubmissionResultAwaiter(this IndicoClient indicoClient) =>
            new SubmissionResultAwaiter(indicoClient.Submissions(), indicoClient.Storage());

        public static IJobAwaiter JobAwaiter(this IndicoClient indicoClient) => new JobAwaiter(indicoClient.Jobs());

        public static IDataSetAwaiter DataSetAwaiter(this IndicoClient indicoClient) =>
            new DataSetAwaiter(indicoClient.DataSets());

        public static IUserReportingClient UserReporting(this IndicoClient indicoClient) =>
            indicoClient.IndicoStrawberryShakeClient.UserReporting();

        /// <summary>
        /// Gets <seealso cref="IDataSetClient"/>
        /// </summary>
        /// <param name="indicoClient">Instance of <seealso cref="IndicoClient"/></param>
        /// <returns>Instance of <seealso cref="IDataSetClient"/></returns>
        public static IDataSetClient DataSets(this IndicoClient indicoClient) =>
            new DataSetClient(new DataSetsV1ClientAdapter(indicoClient.LegacyClient),
                indicoClient.IndicoStrawberryShakeClient.DataSets(), indicoClient.Storage());
    }
}
