using System;
using System.Collections.Generic;
using System.Text;

namespace IndicoV2.Submissions.Models
{
    /// <summary>
    /// Information about files in a submission.
    /// </summary>
    public class SubmissionFiles
    {
        /// <summary>
        /// Id of the file.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Number of pages in file.
        /// </summary>
        public int NumPages { get; set; }
        /// <summary>
        /// Filename of the file.
        /// </summary>
        public string Filename { get; set; }
    }
}
