namespace IndicoV2.Submissions.Models
{
    public interface ISubmission
    {
        int Id { get; }
        SubmissionStatus Status { get; }
    }
}
