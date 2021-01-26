using System;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Jobs.Models;
using IndicoV2.Submissions.Models;

namespace IndicoV2.Jobs
{
    public interface IJobsClient
    {
        /// <summary>
        /// Generates <seealso cref="ISubmission"/>'s result
        /// </summary>
        /// <param name="submissionId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Created Job's Id</returns>
        Task<Guid> GenerateSubmissionResultAsync(int submissionId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets Job's status
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Job's Status</returns>
        Task<JobStatus> GetStatusAsync(Guid jobId, CancellationToken cancellationToken = default);
        //Task<IJob> GetJobAsync(int submissionId, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets Job's result
        /// </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        Task<IJobResult> GetResult(Guid jobId);
    }
}
