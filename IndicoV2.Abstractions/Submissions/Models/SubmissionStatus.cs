namespace IndicoV2.Submissions.Models
{
    public enum SubmissionStatus
    {
        PROCESSING,
        FAILED,
        COMPLETE,
        PENDING_ADMIN_REVIEW,
        PENDING_REVIEW,
        PENDING_AUTO_REVIEW,
        POST_PROCESSING
    }
}
