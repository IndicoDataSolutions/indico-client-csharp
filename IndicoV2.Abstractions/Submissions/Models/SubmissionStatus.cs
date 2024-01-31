namespace IndicoV2.Submissions.Models
{
    public enum SubmissionStatus
    {
        PROCESSING,
        PENDING_AUTO_REVIEW,
        PENDING_REVIEW,
        PENDING_ADMIN_REVIEW,
        COMPLETE,
        FAILED
    }
}
