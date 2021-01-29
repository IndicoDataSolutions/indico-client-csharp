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
        /// <param name="submissionId"></param>
        /// <param name="changes"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> SubmitReviewAsync(int submissionId, JObject changes, CancellationToken cancellationToken);
        
        /// <summary>
        /// Rejects <see cref="ISubmission"/>.
        /// </summary>
        /// <param name="submissionId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> RejectAsync(int submissionId, CancellationToken cancellationToken);
    }
}
