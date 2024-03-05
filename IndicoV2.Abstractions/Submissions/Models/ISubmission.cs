
using System;
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
        /// Dataset id.
        /// </summary>
        int DatasetId { get; }

        /// <summary>
        /// Workflow id.
        /// </summary>
        int WorkflowId { get; }

        /// <summary>
        /// Submission status. See <c><see cref="SubmissionStatus"/></c>.
        /// </summary>
        SubmissionStatus Status { get; }

        DateTimeOffset? CreatedAt { get; }

        DateTimeOffset? UpdatedAt { get; }

        int? CreatedBy { get; }

        int? UpdatedBy { get; }

        DateTimeOffset? CompletedAt { get; }


        /// <summary>
        /// Submission errors.
        /// </summary>
        string Errors { get; }

        bool? FilesDeleted { get; }

        SubmissionFile[] InputFiles { get; }

        /// <summary>
        /// Submission input file.
        /// </summary>
        string InputFile { get; }

        /// <summary>
        /// Submission name of input file.
        /// </summary>
        string InputFilename { get; }

        /// <summary>
        /// Submission result file.
        /// </summary>
        string ResultFile { get; }

        SubmissionOutput[] OutputFiles { get; }

        /// <summary>
        /// Is submission retrieved.
        /// </summary>
        bool Retrieved { get; }

        Review AutoReview { get; }

        SubmissionRetry[] Retries { get; }



        Review[] Reviews { get; }

        bool? ReviewInProgress { get; }

    }
}
