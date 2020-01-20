namespace Indico
{
    interface Mutation<T>
    {
        /**
         * Execute the graphql query and returns the results as a specific type
         * @return result of query of type T
         */
        T Execute();
    }
}