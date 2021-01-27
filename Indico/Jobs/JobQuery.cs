using GraphQL.Client.Http;
using Indico.Exception;

namespace Indico.Jobs
{
    public class JobQuery
    {
        private readonly GraphQLHttpClient _graphQLHttpClient;
        
        /// <summary>
        /// Get/Set the Job ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Query a Job
        /// </summary>
        /// <param name="graphQLHttpClient"></param>
        public JobQuery(GraphQLHttpClient graphQLHttpClient) => _graphQLHttpClient = graphQLHttpClient;

        /// <summary>
        /// Returns Job
        /// </summary>
        /// <returns>Job</returns>
        public Job Exec() => new Job(_graphQLHttpClient, Id);

        /// <summary>
        /// Refreshes the Job Object
        /// </summary>
        /// <returns>Job</returns>
        /// <param name="obj">Job</param>
        public Job Refresh() =>
            //TODO:
            throw new RuntimeException("Method Not Implemented");
    }
}
