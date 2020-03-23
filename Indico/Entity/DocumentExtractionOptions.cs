namespace Indico.Entity
{
    public class DocumentExtractionOptions
    {
        /// <summary>
        /// assumes the input is a single column of text.
        /// </summary>
        /// <value><c>true</c> if single column; otherwise, <c>false</c>.</value>
        public bool SingleColumn { get; }
        /// <summary>
        /// returns body text as part of the results for each page.
        /// </summary>
        /// <value><c>true</c> if text; otherwise, <c>false</c>.</value>
        public bool Text { get; }
        /// <summary>
        /// returns all body text for the PDF document in a single block.
        /// </summary>
        /// <remarks>
        /// This text matches what you would see in Indico Teach.
        /// </remarks>
        /// <value><c>true</c> if raw text; otherwise, <c>false</c>.</value>
        public bool RawText { get; }
        /// <summary>
        /// returns the contents of tables in the document, separately from body text.
        /// </summary>
        /// <value><c>true</c> if tables; otherwise, <c>false</c>.</value>
        public bool Tables { get; }
        /// <summary>
        /// returns the following: tagged, form, producer, author, encryption status, program used to create the file, file size, PDF version, optimized, modification date, title, creation date, number of pages, page size
        /// </summary>
        /// <value><c>true</c> if metadata; otherwise, <c>false</c>.</value>
        public bool Metadata { get; }

        public bool ForceRender { get; }
        public bool Detailed { get; }

        public DocumentExtractionOptions(
            bool singleColumn = false,
            bool text = false,
            bool rawText = false,
            bool tables = false,
            bool metadata = false,
            bool forceRender = false,
            bool detailed = false
        )
        {
            this.SingleColumn = singleColumn;
            this.Text = text;
            this.RawText = rawText;
            this.Tables = tables;
            this.Metadata = metadata;
            this.ForceRender = forceRender;
            this.Detailed = detailed;
        }
    }
}