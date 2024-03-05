using System;
using System.Linq;
using IndicoV2.StrawberryShake;

namespace IndicoV2.Submissions.Models
{
    public class SubmissionFile
    {
        /// <summary>
        /// Unique Id of this file.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Local URL to stored input.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Name of original file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Type of file.
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Id of the submission this file is associated with.
        /// </summary>
        public int? SubmissionId { get; set; }

        /// <summary>
        /// Size of file in bytes.
        /// </summary>
        public int? FileSize { get; set; }

        /// <summary>
        /// Number of pages in file.
        /// </summary>
        public int? NumPages { get; set; }
    }

    public class SubmissionOutput
    {
        /// <summary>
        /// Unique Id of this output.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Local URL to stored input.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Id of the submission this file is associated with.
        /// </summary>
        public int? SubmissionId { get; set; }

        /// <summary>
        /// Id of the workflow component that made this file.
        /// </summary>
        public int? ComponentId { get; set; }

        /// <summary>
        /// Datetime the output file was created.
        /// </summary>
        public DateTimeOffset? CreatedAt { get; set; }
    }

    public class Review
    {
        /// <summary>
        /// Id of this review.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Id of the submission this review is associated with.
        /// </summary>
        public int? SubmissionId { get; set; }

        /// <summary>
        /// When this user first opened the file. See startedAt as well.
        /// </summary>
        public string CreatedAt { get; set; }

        /// <summary>
        /// Reviewer id.
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// When this review was started.
        /// Differs from createdAt because a reviewer may restart their review at any time.
        /// </summary>
        public string StartedAt { get; set; }

        /// <summary>
        /// When this review was completed by the reviewer.
        /// </summary>
        public string CompletedAt { get; set; }

        /// <summary>
        /// Flag for whether the file was rejected (True) or not (False).
        /// </summary>
        public bool? Rejected { get; set; }

        /// <summary>
        /// Type of review.
        /// </summary>
        public ReviewType? ReviewType { get; set; }

        /// <summary>
        /// Reviewer notes.
        /// </summary>
        public string Notes { get; set; }

    }

    public class SubmissionRetry
    {
        /// <summary>
        /// Unique Id of the submission retry.
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Unique Id of the associated submission.
        /// </summary>
        public int? SubmissionId { get; set; }

        /// <summary>
        /// Errors from previous submission.
        /// </summary>
        public string PreviousErrors { get; set; }

        /// <summary>
        /// Status of submission before it was retried.
        /// </summary>
        public SubmissionStatus? PreviousStatus { get; set; }

        /// <summary>
        /// Errors that ocurred during the retrying of this submission.
        /// </summary>
        public string RetryErrors { get; set; }
    }

    public class Submission : ISubmission
    {
        /// <summary>
        /// Submission id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Dataset id.
        /// </summary>
        public int DatasetId { get; set; }

        /// <summary>
        /// Workflow id.
        /// </summary>
        public int WorkflowId { get; set; }

        /// <summary>
        /// Submission status. See <c><see cref="SubmissionStatus"/></c>.
        /// </summary>
        public SubmissionStatus Status { get; set; }

        /// <summary>
        /// Datetime the submission was created.
        /// </summary>
        public DateTimeOffset? CreatedAt { get; set; }

        /// <summary>
        /// Datetime the submission was updated.
        /// </summary>
        public DateTimeOffset? UpdatedAt { get; set; }

        /// <summary>
        /// Id of the user who created the submission.
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// Id of the user who updated the submission.
        /// </summary>
        public int? UpdatedBy { get; set; }

        /// <summary>
        /// Datetime the submission reached a completed state.
        /// </summary>
        public DateTimeOffset? CompletedAt { get; set; }

        /// <summary>
        /// Submission errors.
        /// </summary>
        public string Errors { get; set; }

        /// <summary>
        /// Is submission deleted.
        /// </summary>
        public bool? FilesDeleted { get; set; }

        /// <summary>
        /// Submission input files.
        /// </summary>
        public SubmissionFile[] InputFiles { get; set; }

        /// <summary>
        /// Submission input file.
        /// </summary>
        public string InputFile { get; set; }

        /// <summary>
        /// Submission name of input file.
        /// </summary>
        public string InputFilename { get; set; }

        /// <summary>
        /// Submission result file.
        /// </summary>
        public string ResultFile { get; set; }

        /// <summary>
        /// Output files of the submission.
        /// </summary>
        public SubmissionOutput[] OutputFiles { get; set; }

        /// <summary>
        /// Is submission retrieved.
        /// </summary>
        public bool Retrieved { get; set; }

        /// <summary>
        /// Latest auto review for submission.
        /// </summary>
        public Review AutoReview { get; set; }

        /// <summary>
        /// List of retries for submission.
        /// </summary>
        public SubmissionRetry[] Retries { get; set; }

        /// <summary>
        /// Completed review of this submission, without changes.
        /// </summary>
        public Review[] Reviews { get; set; }

        /// <summary>
        /// True if the submission is being actively reviewed.
        /// </summary>
        public bool? ReviewInProgress { get; set; }
    }

    public class SubmissionSs : Submission
    {
        private readonly IListSubmissions_Submissions_Submissions _ssSubmission;
        public SubmissionSs(IListSubmissions_Submissions_Submissions submission)
        {
            _ssSubmission = submission;
            Id = _ssSubmission.Id ?? 0;
            DatasetId = _ssSubmission.DatasetId ?? 0;
            WorkflowId = _ssSubmission.WorkflowId ?? 0;
            Status = ConvertFromSs();
            CreatedAt = _ssSubmission.CreatedAt;
            UpdatedAt = _ssSubmission.UpdatedAt;
            CreatedBy = _ssSubmission.CreatedBy;
            UpdatedBy = _ssSubmission.UpdatedBy;
            CompletedAt = _ssSubmission.CompletedAt;
            Errors = _ssSubmission.Errors;
            FilesDeleted = _ssSubmission.FilesDeleted;
            InputFiles = _ssSubmission.InputFiles.Select(inputFile => new SubmissionFile
            {
                Id = inputFile.Id,
                FilePath = inputFile.Filepath,
                FileName = inputFile.Filename,
                FileType = inputFile.Filetype.ToString(),
                SubmissionId = inputFile.SubmissionId,
                FileSize = inputFile.FileSize,
                NumPages = inputFile.NumPages
            }).ToArray();
            InputFile = _ssSubmission.InputFile;
            InputFilename = _ssSubmission.InputFilename;
            ResultFile = _ssSubmission.ResultFile;
            OutputFiles = _ssSubmission.OutputFiles.Select(x => new SubmissionOutput() { }).ToArray();
            Retrieved = _ssSubmission.Retrieved ?? throw new ArgumentException("Invalid value for retrieved received from call");
            AutoReview = _ssSubmission.AutoReview != null ? new Review
            {
                Id = _ssSubmission.AutoReview.Id,
                SubmissionId = _ssSubmission.AutoReview.SubmissionId,
                CreatedAt = _ssSubmission.AutoReview.CreatedAt,
                CreatedBy = _ssSubmission.AutoReview.CreatedBy,
                StartedAt = _ssSubmission.AutoReview.StartedAt,
                CompletedAt = _ssSubmission.AutoReview.CompletedAt,
                Rejected = _ssSubmission.AutoReview.Rejected,
                ReviewType = (ReviewType)_ssSubmission.AutoReview.ReviewType,
                Notes = _ssSubmission.AutoReview.Notes,
            } : new Review() { };
            Retries = _ssSubmission.Retries.Select(submissionRetry => new SubmissionRetry
            {
                Id = submissionRetry.Id,
                SubmissionId = submissionRetry.SubmissionId,
                PreviousErrors = submissionRetry.PreviousErrors,
                PreviousStatus = (SubmissionStatus)submissionRetry.PreviousStatus,
                RetryErrors = submissionRetry.RetryErrors
            }).ToArray();
            Reviews = _ssSubmission.Reviews.Select(review => new Review
            {
                Id = review.Id,
                SubmissionId = review.SubmissionId,
                CreatedAt = review.CreatedAt,
                CreatedBy = review.CreatedBy,
                StartedAt = review.StartedAt,
                CompletedAt = review.CompletedAt,
                Rejected = review.Rejected,
                ReviewType = (ReviewType)review.ReviewType,
                Notes = review.Notes,
            }).ToArray();
            ReviewInProgress = _ssSubmission.ReviewInProgress;
        }


        private SubmissionStatus ConvertFromSs()
        {
            switch (_ssSubmission.Status)
            {
                case StrawberryShake.SubmissionStatus.Processing:
                    return SubmissionStatus.PROCESSING;
                case StrawberryShake.SubmissionStatus.PendingAutoReview:
                    return SubmissionStatus.PENDING_AUTO_REVIEW;
                case StrawberryShake.SubmissionStatus.PendingReview:
                    return SubmissionStatus.PENDING_REVIEW;
                case StrawberryShake.SubmissionStatus.PendingAdminReview:
                    return SubmissionStatus.PENDING_ADMIN_REVIEW;
                case StrawberryShake.SubmissionStatus.Complete:
                    return SubmissionStatus.COMPLETE;
                case StrawberryShake.SubmissionStatus.Failed:
                    return SubmissionStatus.FAILED;
                default:
                    throw new NotSupportedException($"Cannot read submission status: {_ssSubmission.Status}");
                    ;
            }
        }
    }
}
