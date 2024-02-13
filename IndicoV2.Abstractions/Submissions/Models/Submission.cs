using System;
using System.Collections.Generic;
using System.Text;
using IndicoV2.StrawberryShake;

namespace IndicoV2.Submissions.Models
{
    public class Submission : ISubmission
    {
         /// <summary>
        /// Submission id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Submission status. See <c><see cref="SubmissionStatus"/></c>.
        /// </summary>
        public SubmissionStatus Status { get; set; }

        /// <summary>
        /// Dataset id.
        /// </summary>
        public int DatasetId { get; set; }

        /// <summary>
        /// Workflow id.
        /// </summary>
        public int WorkflowId { get; set; }

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
        /// Is submission retrieved.
        /// </summary>
        public bool Retrieved { get; set; }

        /// <summary>
        /// Submission errors.
        /// </summary>
        public string Errors { get; set; }
    }

    public class SubmissionSs : Submission
    {
        private readonly IListSubmissions_Submissions_Submissions _ssSubmission;
        public SubmissionSs(IListSubmissions_Submissions_Submissions submission)
        {
            _ssSubmission = submission;
            Id = _ssSubmission.Id ?? 0;
            Status = ConvertFromSs();
            DatasetId = _ssSubmission.DatasetId ?? 0;
            WorkflowId = _ssSubmission.WorkflowId ?? 0;
            InputFile = _ssSubmission.InputFile;
            InputFilename = _ssSubmission.InputFilename;
            ResultFile = _ssSubmission.ResultFile;
            Retrieved = _ssSubmission.Retrieved ?? throw new ArgumentException("Invalid value for retrieved received from call");
            Errors = _ssSubmission.Errors;
        }

        private SubmissionStatus ConvertFromSs()
        {
            var serializer = new SubmissionStatusSerializer();
            string status = serializer.Format(_ssSubmission.Status).ToString();
            if (!Enum.TryParse(status,out SubmissionStatus parsed))
            {
                throw new NotSupportedException($"Cannot read submission status: {_ssSubmission.Status}");
            }

            return parsed;

        }
    }
}
