namespace Indico.Entity
{
    public class PdfExtractionOptions
    {
        public bool SingleColumn { get; }
        public bool Text { get; }
        public bool RawText { get; }
        public bool Tables { get; }
        public bool Metadata { get; }

        public PdfExtractionOptions(
            bool singleColumn = false,
            bool text = false,
            bool rawText = false,
            bool tables = false,
            bool metadata = false
        )
        {
            this.SingleColumn = singleColumn;
            this.Text = text;
            this.RawText = rawText;
            this.Tables = tables;
            this.Metadata = metadata;
        }
    }
}