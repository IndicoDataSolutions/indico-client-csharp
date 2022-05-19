using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.CommonModels.Pagination;
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
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <returns><c><see cref="IEnumerable{T}">IEnumerable</see></c> of submissions ids.</returns>
        [Obsolete("Please provide file name for each stream.", true)]
        Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<Stream> streams, CancellationToken cancellationToken = default);

        /// <summary>
        /// Method creates <c><see cref="ISubmission"/></c>.
        /// </summary>
        /// <param name="workflowId">Workflow Id.</param>
        /// <param name="streams"><c><see cref="Stream">Stream collection</see></c> to create submissions from.</param>
        /// <param name="resultsFileVersion">Optional. Specifies version to use for the results file.</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <returns><c><see cref="IEnumerable{T}">IEnumerable</see></c> of submissions ids.</returns>
        Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<(string Name, Stream Content)> files, CancellationToken cancellationToken = default, SubmissionResultsFileVersion? resultsFileVersion = null);

        /// <summary>
        /// Method creates <c><see cref="ISubmission"/></c>.
        /// </summary>
        /// <param name="workflowId">Workflow Id.</param>
        /// <param name="uris"><c><see cref="Uri">Uri collection</see></c> to create submissions from.</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <returns><c><see cref="IEnumerable{T}">IEnumerable</see></c> of submissions ids.</returns>
        Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<Uri> uris, CancellationToken cancellationToken = default, SubmissionResultsFileVersion? resultsFileVersion = null);

        /// <summary>
        /// Method creates <c><see cref="ISubmission"/></c>.
        /// </summary>
        /// <param name="workflowId">Workflow Id.</param>
        /// <param name="paths">Filepaths collection to create submissions from.</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <returns><c><see cref="IEnumerable{T}"/></c> of submissions ids.</returns>
        Task<IEnumerable<int>> CreateAsync(int workflowId, IEnumerable<string> paths, CancellationToken cancellationToken = default, SubmissionResultsFileVersion? resultsFileVersion = null);

        /// <summary>
        /// Method lists <c><see cref="ISubmission"/></c>.
        /// </summary>
        /// <param name="submissionIds">Submissions ids.</param>
        /// <param name="workflowIds">Workflows ids.</param>
        /// <param name="filters">Submission filter. Use <c><see cref="SubmissionFilter"/></c> or composite filters with <c><see cref="AndFilter"/></c> and <c><see cref="OrFilter"/></c></param>
        /// <param name="limit">Limit of returned submissions. Default value is 1000.</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <returns><c><see cref="IEnumerable{T}"/></c> of <c><see cref="ISubmission"/></c></returns>
        Task<IEnumerable<ISubmission>> ListAsync(IEnumerable<int> submissionIds, IEnumerable<int> workflowIds, IFilter filters, int limit = 1000, CancellationToken cancellationToken = default);

        /// <summary>
        /// Method returns a <c><see cref="IHasCursor{T}"/></c> containing an enumerable of <c><see cref="ISubmission"/></c> 
        /// Set <paramref name="after"/> to the EndCursor of PageInfo for subsequent queries.
        /// </summary>
        /// <param name="submissionIds"></param>
        /// <param name="workflowIds"></param>
        /// <param name="filters"></param>
        /// <param name="after"></param>
        /// <param name="limit"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IHasCursor<IEnumerable<ISubmission>>> ListAsync(IEnumerable<int> submissionIds, IEnumerable<int> workflowIds, IFilter filters, int? after, int limit = 1000, CancellationToken cancellationToken = default);

        /// <summary>
        /// Method gets certain <c><see cref="ISubmission"/></c>.
        /// </summary>
        /// <param name="submissionId">Submission id.</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <returns><c><see cref="ISubmission"/></c> with provided id.</returns>
        Task<ISubmission> GetAsync(int submissionId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Generates <seealso cref="ISubmission"/>'s result
        /// </summary>
        /// <param name="submissionId"><seealso cref="ISubmission"/>'s Id</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <returns>Created Job's Id</returns>
        Task<string> GenerateSubmissionResultAsync(int submissionId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks a submission as retrieved. Defaults retrieved to true.
        /// </summary>
        /// <param name="submissionId"></param>
        /// <param name="retrieved"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Id of the submission updated.</returns>
        Task<ISubmission> MarkSubmissionAsRetrieved(int submissionId, bool retrieved = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Uses the legacy C# client for older frameworks.
        /// </summary>
        /// <param name="workflowId"></param>
        /// <param name="paths"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<IEnumerable<int>> CreateAsyncLegacy(int workflowId, IEnumerable<string> paths,
          CancellationToken cancellationToken = default);
    }
}
