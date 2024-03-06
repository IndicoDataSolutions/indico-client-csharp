
using System;

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

        /// <summary>
        /// Datetime the submission was created.
        /// </summary>
        DateTimeOffset? CreatedAt { get; }

        /// <summary>
        /// Datetime the submission was updated.
        /// </summary>
        DateTimeOffset? UpdatedAt { get; }

        /// <summary>
        /// Id of the user who created the submission.
        /// </summary>
        int? CreatedBy { get; }

        /// <summary>
        /// Id of the user who updated the submission.
        /// </summary>
        int? UpdatedBy { get; }

        /// <summary>
        /// Datetime the submission reached a completed state.
        /// </summary>
        DateTimeOffset? CompletedAt { get; }

        /// <summary>
        /// Submission errors.
        /// </summary>
        string Errors { get; }

        /// <summary>
        /// Submission files have been deleted from file store.
        /// </summary>
        bool? FilesDeleted { get; }

        /// <summary>
        /// List of submission input files.
        /// </summary>
        SubmissionFile[] InputFiles { get; }

        /// <summary>
        /// Local URL to first stored input.
        /// </summary>
        string InputFile { get; }

        /// <summary>
        /// Original name of first file.
        /// </summary>
        string InputFilename { get; }

        /// <summary>
        /// Local URL to most recently stored output.
        /// </summary>
        string ResultFile { get; }

        /// <summary>
        /// List of submission output files.
        /// </summary>
        SubmissionOutput[] OutputFiles { get; }

        /// <summary>
        /// Is submission retrieved.
        /// </summary>
        bool Retrieved { get; }

        /// <summary>
        /// Latest auto review for submission.
        /// </summary>
        Review AutoReview { get; }

        /// <summary>
        /// List of submission retries.
        /// </summary>
        SubmissionRetry[] Retries { get; }

        /// <summary>
        /// Completed reviews of this submission, without changes.
        /// </summary>
        Review[] Reviews { get; }

        /// <summary>
        /// True if the submission is being actively reviewed.
        /// </summary>
        bool? ReviewInProgress { get; }

    }
}
