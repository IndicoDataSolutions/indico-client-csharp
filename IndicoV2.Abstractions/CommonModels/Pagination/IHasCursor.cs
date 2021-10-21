


namespace IndicoV2.CommonModels.Pagination
{
    /// <summary>
    /// A wrapper for paged queries -- returns the data and a <see cref="PageInfo"/> cursor of type <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHasCursor<T>
    {
        /// <summary>
        /// The <see cref="PageInfo"/> cursor
        /// </summary>
        PageInfo PageInfo {get;}
        /// <summary>
        /// <typeparamref name="T"/> data returned from the API.
        /// </summary>
        T Data { get; }
    }
}
