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
    }
}
