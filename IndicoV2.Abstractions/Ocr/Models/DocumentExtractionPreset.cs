namespace IndicoV2.Ocr.Models
{
    public enum DocumentExtractionPreset
    {
        /// <summary>
        /// Provides page text and block text/position in a nested format.
        /// </summary>
        Standard,

        /// <summary>
        ///  Provides a simple and fast response for native PDFs (3-5x faster). Will NOT work with scanned PDFs.
        /// </summary>
        Simple,

        /// <summary>
        /// Provided to mimic the behavior of Indico’s older pdf_extraction function. Use this if your model was trained with data from the older pdf_extraction.
        /// </summary>
        Legacy,

        /// <summary>
        /// Provides detailed bounding box information on tokens and characters. Returns data in a nested format at the document level with all metadata included.
        /// </summary>
        Detailed,

        /// <summary>
        /// Provides detailed information at the page-level in an unnested format.
        /// </summary>
        OnDocument,
    }
}
