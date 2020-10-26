using Indico.Types;

namespace Indico.Entity
{
    public class Submission
    {
        public int Id { get; set; }
        public int DatasetId { get; set; }
        public int WorkflowId { get; set; }
        public SubmissionStatus Status { get; set; }
        public string InputFile { get; set; }
        public string InputFilename { get; set; }
        public string ResultFile { get; set; }
        public bool Retrieved { get; set; } = false;
        public string Errors { get; set; }
    }
}
