using System;
using System.Linq;
using IndicoV2.StrawberryShake;

namespace IndicoV2.Submissions.Models
{
    public class SubmissionFile
    {
        public int? Id { get; set; }

        public string FilePath { get; set; }

        public string FileName { get; set; }

        public string? FileType { get; set; }

        public int? SubmissionId { get; set; }

        public int? FileSize { get; set; }

        public int? NumPages { get; set; }
    }

    public class SubmissionOutput
    {
        public int? Id { get; set; }

        public string FilePath { get; set; }

        public int? SubmissionId { get; set; }

        public int? ComponentId { get; set; }

        public DateTimeOffset? CreatedAt { get; set; }
    }

    public class Review
    {
        public int? Id { get; set; }

        public int? SubmissionId { get; set; }

        public string CreatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public string StartedAt { get; set; }

        public string CompletedAt { get; set; }

        public bool? Rejected { get; set; }

        public ReviewType? ReviewType { get; set; }

        public string Notes { get; set; }

    }

    public class SubmissionRetry
    {
        public int? Id { get; set; }

        public int? SubmissionId { get; set; }

        public string PreviousErrors { get; set; }

        public SubmissionStatus? PreviousStatus { get; set; }

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

        public DateTimeOffset? CreatedAt { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTimeOffset? CompletedAt { get; set; }

        /// <summary>
        /// Submission errors.
        /// </summary>
        public string Errors { get; set; }

        /// <summary>
        /// Is submission deleted.
        /// </summary>
        public bool? FilesDeleted { get; set; }

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

        public SubmissionOutput[] OutputFiles { get; set; }

        /// <summary>
        /// Is submission retrieved.
        /// </summary>
        public bool Retrieved { get; set; }

        public Review AutoReview { get; set; }

        public SubmissionRetry[] Retries { get; set; }



        public Review[] Reviews { get; set; }

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
