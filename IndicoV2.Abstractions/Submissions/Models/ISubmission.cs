
using System.Collections;
using System.Collections.Generic;
using IndicoV2.CommonModels.Pagination;

namespace IndicoV2.Submissions.Models
{
    /// <summary>
    /// Submission interface.
    /// </summary>
    public interface ISubmission
    {
        /// <summary>
        /// Submission id.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Submission status. See <c><see cref="SubmissionStatus"/></c>.
        /// </summary>
        SubmissionStatus Status { get; }

        /// <summary>
        /// Dataset id.
        /// </summary>
        int DatasetId { get; }

        /// <summary>
        /// Workflow id.
        /// </summary>
        int WorkflowId { get; }

        /// <summary>
        /// Submission input file.
        /// </summary>
        string InputFile { get; }

        /// <summary>
        /// Information about input files.
        /// </summary>
        IList<SubmissionFiles> InputFiles {get; }

        /// <summary>
        /// Submission name of input file.
        /// </summary>
        string InputFilename { get; }

        /// <summary>
        /// Submission result file.
        /// </summary>
        string ResultFile { get; }

        /// <summary>
        /// Is submission retrieved.
        /// </summary>
        bool Retrieved { get; }

        /// <summary>
        /// Submission errors.
        /// </summary>
        string Errors { get; }
    }
}
