using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Submissions.Models;

namespace IndicoV2.Submissions
{
    /// <summary>
    /// <c>ISubmissionClient</c> defines all operations on <c><see cref="ISubmission">ISubmission</see></c>.
    /// </summary>
    public interface ISubmissionsClient
    {
        /// <summary>
        /// Method creates <c><see cref="ISubmission"/></c>.
        /// </summary>
        /// <param name="workflowId">Workflow Id.</param>
        /// <param name="streams"><c><see cref="Stream">Stream collection</see></c> to create submissions from.</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asyncronous operations.</param>
        /// <returns><c><see cref="IEnumerable{T}">IEnumerable</see></c> of submissions ids.</returns>
        Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<Stream> streams, CancellationToken cancellationToken = default);

        /// <summary>
        /// Method creates <c><see cref="ISubmission"/></c>.
        /// </summary>
        /// <param name="workflowId">Workflow Id.</param>
        /// <param name="uris"><c><see cref="Uri">Uri collection</see></c> to create submissions from.</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asyncronous operations.</param>
        /// <returns><c><see cref="IEnumerable{T}">IEnumerable</see></c> of submissions ids.</returns>
        Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<Uri> uris, CancellationToken cancellationToken = default);

        /// <summary>
        /// Method creates <c><see cref="ISubmission"/></c>.
        /// </summary>
        /// <param name="workflowId">Workflow Id.</param>
        /// <param name="paths">Filepaths collection to create submissions from.</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <returns><c><see cref="IEnumerable{T}"/></c> of submissions ids.</returns>
        Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<string> paths, CancellationToken cancellationToken = default);

        /// <summary>
        /// Method lists <c><see cref="ISubmission"/></c>.
        /// </summary>
        /// <param name="submissionIds">Submissions ids.</param>
        /// <param name="workflowIds">Workflows ids.</param>
        /// <param name="filters">Submission filter. Use <c><see cref="SubmissionFilter"/></c> or composite filters with <c><see cref="AndFilter"/></c> and <c><see cref="OrFilter"/></c></param>
        /// <param name="limit">Limit of returned submissions. Default value is 1000.</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asyncronous operations.</param>
        /// <returns><c><see cref="IEnumerable{T}"/></c> of <c><see cref="ISubmission"/></c></returns>
        Task<IEnumerable<ISubmission>> ListAsync(IEnumerable<int> submissionIds, IEnumerable<int> workflowIds, IFilter filters, int limit = 1000, CancellationToken cancellationToken = default);

        /// <summary>
        /// Method gets certain <c><see cref="ISubmission"/></c>.
        /// </summary>
        /// <param name="submissionId">Submission id.</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asyncronous operations.</param>
        /// <returns><c><see cref="ISubmission"/></c> with provided id.</returns>
        Task<ISubmission> GetAsync(int submissionId, CancellationToken cancellationToken = default);
    }
}
