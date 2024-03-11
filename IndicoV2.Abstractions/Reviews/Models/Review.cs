using Newtonsoft.Json.Linq;
using IndicoV2.Submissions.Models;

namespace IndicoV2.Reviews.Models
{
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


    public class ReviewDetailed : Review
    {
        /// <summary>
        /// Changes made to reviewed predictions
        /// </summary>
        public JObject Changes { get; set; }
    }
}