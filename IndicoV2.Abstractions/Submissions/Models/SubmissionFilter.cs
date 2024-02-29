namespace IndicoV2.Submissions.Models
{
    public enum FileType
    {
        Csv,
        Doc,
        Docx,
        Eml,
        Excel,
        Jpg,
        Msg,
        Pdf,
        Png,
        Ppt,
        Pptx,
        Rtf,
        Tiff,
        Txt,
        Unknown,
        Xls,
        Xlsx
    }

    public enum ReviewType
    {
        Admin,
        Auto,
        Manual
    }

    public class SubmissionReviewFilter : IFilter
    {
        public bool? Rejected { get; set; }
        public int? CreatedBy { get; set; }
        public ReviewType ReviewType { get; set; }
    }

    public class DateRangeFilter : IFilter
    {
        public string From { get; set; }
        public string To { get; set; }
    }

    public class SubmissionFilter : IFilter
    {
        public FileType[] FileType { get; set; }
        public string InputFilename { get; set; }
        public SubmissionStatus? Status { get; set; }
        public bool? Retrieved { get; set; }
        public SubmissionReviewFilter Reviews { get; set; }
        public bool? ReviewInProgress { get; set; }
        public bool? FilesDeleted { get; set; }
        public DateRangeFilter CreatedAt { get; set; }
        public DateRangeFilter UpdatedAt { get; set; }
    }
}
