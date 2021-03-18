using IndicoV2.Submissions.Models;
using IndicoV2.V1Adapters.Converters;

namespace IndicoV2.V1Adapters.Submissions.Models
{
    internal class V1SubmissionAdapter : ISubmission
    {
        private readonly Indico.Entity.Submission _submissionLegacy;

        public V1SubmissionAdapter(Indico.Entity.Submission submissionLegacy) => _submissionLegacy = submissionLegacy;

        public int Id => _submissionLegacy.Id;

        public SubmissionStatus Status => _submissionLegacy.Status.ConvertFromLegacy();

        public int DatasetId => _submissionLegacy.DatasetId;

        public int WorkflowId => _submissionLegacy.WorkflowId;

        public string InputFile => _submissionLegacy.InputFile;

        public string InputFilename => _submissionLegacy.InputFilename;

        public string ResultFile => _submissionLegacy.ResultFile;

        public bool Retrieved => _submissionLegacy.Retrieved;

        public string Errors => _submissionLegacy.Errors;
    }
}