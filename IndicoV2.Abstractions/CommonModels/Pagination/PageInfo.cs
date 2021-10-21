

namespace IndicoV2.CommonModels.Pagination
{
    /// <summary>
    /// Represents graphql pagination cursor.
    /// </summary>
    public class PageInfo
    {
        /// <summary>
        /// Starting number of the page
        /// </summary>
        public int StartCursor { get; set; }
        /// <summary>
        /// Ending number of the page
        /// </summary>
        public int EndCursor { get; set; }
        /// <summary>
        /// If there are additional pages
        /// </summary>
        public bool HasNextPage { get; set; }
        /// <summary>
        /// Total # of results in query.
        /// </summary>
        public int AggregateCount { get; set; }
    }
}
