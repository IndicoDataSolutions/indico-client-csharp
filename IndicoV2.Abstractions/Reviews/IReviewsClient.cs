using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Submissions.Models;
using Newtonsoft.Json.Linq;

namespace IndicoV2.Reviews
{
    public interface IReviewsClient
    {
        /// <summary>
        /// Submits review for <see cref="ISubmission"/>
        /// </summary>
        /// <param name="submissionId"><see cref="ISubmission"/>'s Id</param>
        /// <param name="changes">Changes</param>
        /// <param name="forceComplete">Force complete review.</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <param name="rejected">If review rejected.</param>
        /// <returns>Job's Id</returns>
        Task<string> SubmitReviewAsync(int submissionId, JObject changes, bool rejected = false, bool? forceComplete = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Submits review for <see cref="ISubmission"/>
        /// </summary>
        /// <param name="submissionId"><see cref="ISubmission"/>'s Id</param>
        /// <param name="changes">Changes</param>
        /// <param name="forceComplete">Force complete review.</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <param name="rejected">If review rejected.</param>
        /// <returns>Job's Id</returns>
        Task<string> SubmitReviewAsync(int submissionId, JArray changes, bool rejected = false, bool? forceComplete = null, CancellationToken cancellationToken = default);
    }
}
