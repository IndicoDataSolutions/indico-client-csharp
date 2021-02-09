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
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asyncronous operations.</param>
        /// <returns>Job's Id</returns>
        Task<string> SubmitReviewAsync(int submissionId, JObject changes, CancellationToken cancellationToken);

        /// <summary>
        /// Rejects <see cref="ISubmission"/>.
        /// </summary>
        /// <param name="submissionId"><see cref="ISubmission"/>'s Id</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asyncronous operations.</param>
        /// <returns>Job's Id</returns>
        Task<string> RejectAsync(int submissionId, CancellationToken cancellationToken);
    }
}
