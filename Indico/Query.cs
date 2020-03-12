namespace Indico
{
    interface Query<T>
    {
        /// <summary>
        /// Execute the graphql query and retunrs the results as a specific type
        /// </summary>
        /// <returns>result of query of type T</returns>
        T Query();
        T Refresh(T obj);
    }
}