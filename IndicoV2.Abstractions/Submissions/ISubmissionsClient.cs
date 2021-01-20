using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Submissions.Models;

namespace IndicoV2.Submissions
{
    public interface ISubmissionsClient
    {
        Task<IEnumerable<int>> CreateAsync(int workflowId, Stream[] streams, CancellationToken cancellationToken = default);
        Task<IEnumerable<int>> CreateAsync(int workflowId, Uri[] uris, CancellationToken cancellationToken = default);
        Task<IEnumerable<int>> CreateAsync(int workflowId, string[] paths, CancellationToken cancellationToken = default);
        
        Task<IEnumerable<ISubmission>> ListAsync(CancellationToken cancellationToken = default);
        Task<ISubmission> GetAsync(int submissionId, CancellationToken cancellationToken = default);
        Task<IJob> GenerateSubmissionResult(int submissionId, CancellationToken cancellationToken = default);
        Task<IJob> GetJobAsync(int submissionId, CancellationToken cancellationToken = default);
    }
}
