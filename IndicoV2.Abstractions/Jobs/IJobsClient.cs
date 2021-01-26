using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Jobs.Models;

namespace IndicoV2.Jobs
{
    public interface IJobsClient
    {
        Task<IJob> GenerateSubmissionResultAsync(int submissionId, CancellationToken cancellationToken = default);
        Task<JobStatus> GetStatusAsync(int jobId, CancellationToken cancellationToken = default);
        Task<IJob> GetJobAsync(int submissionId, CancellationToken cancellationToken = default);
        Task<IJobResult> GetResult(int jobId);
    }
}
