using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Submissions.Models;

namespace IndicoV2.Submissions
{
    /// <summary>
    /// Client responsible for creating and listing <seealso cref="ISubmission"/>.
    /// </summary>
    public interface ISubmissionsClient
    {
        /// <summary>
        /// Submits new FileStreams to the workflow.
        /// </summary>
        /// <returns>List of created <see cref="ISubmission"/>.Ids</returns>
        Task<IEnumerable<int>> CreateAsync(int workflowId, Stream[] streams, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Submits files to the workflow.
        /// </summary>
        /// <returns>List of created <see cref="ISubmission"/>.Ids</returns>
        Task<IEnumerable<int>> CreateAsync(int workflowId, string[] paths, CancellationToken cancellationToken = default);
        
        Task<ISubmission> GetAsync(int submissionId, CancellationToken cancellationToken = default);
    }
}
