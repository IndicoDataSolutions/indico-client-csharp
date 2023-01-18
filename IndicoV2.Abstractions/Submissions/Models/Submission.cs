using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IndicoV2.StrawberryShake;

namespace IndicoV2.Submissions.Models
{
    public class SubmissionSs : ISubmission
    {
        private readonly IListSubmissions_Submissions_Submissions _ssSubmission;
        public SubmissionSs(IListSubmissions_Submissions_Submissions submission) => _ssSubmission = submission;
        public int Id => _ssSubmission.Id ?? 0;

        public SubmissionStatus Status => ConvertFromSs();

        public int DatasetId => _ssSubmission.DatasetId ?? 0;

        public int WorkflowId => _ssSubmission.WorkflowId ?? 0;

        public string InputFile => _ssSubmission.InputFile;

        public string InputFilename => _ssSubmission.InputFilename;

        public string ResultFile => _ssSubmission.ResultFile;

        public bool Retrieved => _ssSubmission.Retrieved ?? throw new ArgumentException("Invalid value for retrieved received from call");

        public string Errors => _ssSubmission.Errors;

        public IList<SubmissionFiles> InputFiles => GetSubmissionFiles();

        private SubmissionStatus ConvertFromSs()
        {
                if (!Enum.TryParse(_ssSubmission.Status.ToString().ToUpper(),out SubmissionStatus parsed))
                {
                    throw new NotSupportedException($"Cannot read submission status: {_ssSubmission.Status}");
                }

                return parsed;
            
        }

        private IList<SubmissionFiles> GetSubmissionFiles()
        {
            if (_ssSubmission.InputFiles.Any())
            {
                return _ssSubmission.InputFiles.Select(x => new SubmissionFiles() { Filename = x.Filename, Id = x.Id.Value, NumPages = x.NumPages.Value }).ToList();
            }
            else
            {
                return new List<SubmissionFiles>();
            } 
        }
    }
}
