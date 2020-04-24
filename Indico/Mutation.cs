namespace Indico
{
    interface Mutation<T>
    {
        /// <summary>
        /// Execute the graphql query and returns the results as a specific type
        /// </summary>
        /// <returns>result of query of type T</returns>
        T Exec();
    }
}