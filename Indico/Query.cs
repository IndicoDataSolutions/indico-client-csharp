namespace Indico
{
    interface Query<T>
    {
        /**
         * Execute the graphql query and retunrs the results as a specific type
         * @return result of query of type T
         */
        T Query();
        T Refresh(T obj);
    }
}