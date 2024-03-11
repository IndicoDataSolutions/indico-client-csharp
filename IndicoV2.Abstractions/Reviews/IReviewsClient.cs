using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Submissions.Models;
using IndicoV2.Reviews.Models;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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

        /// <summary>
        /// Gets reviews for <see cref="ISubmission"/> with changes
        /// </summary>
        /// <param name="submissionId"><see cref="ISubmission"/>'s Id</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <returns>Reviews</returns>
        Task<IEnumerable<ReviewDetailed>> GetReviewsAsync(int submissionId, CancellationToken cancellationToken = default);
    }
}
