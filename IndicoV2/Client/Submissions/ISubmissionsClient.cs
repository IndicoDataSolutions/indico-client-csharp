using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Client.Submissions.Models;

namespace IndicoV2.Client.Submissions
{
    public interface ISubmissionsClient
    {
        Task<IEnumerable<int>> CreateAsync(int workflowId, Stream[] streams, CancellationToken cancellationToken = default);
        Task<IEnumerable<int>> CreateAsync(int workflowId, Uri[] streams, CancellationToken cancellationToken = default);
        Task<Job> GetJobAsync(int submissionId, CancellationToken cancellationToken = default);
    }
}
