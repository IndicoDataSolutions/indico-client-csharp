namespace IndicoV2.Abstractions.Submissions.Models
{
    public enum SubmissionStatus
    {
        PROCESSING,
        FAILED,
        COMPLETE,
        PENDING_ADMIN_REVIEW,
        PENDING_REVIEW,
    }
}
