using System;
using GraphQL.Client.Http;
using Indico.Exception;

namespace Indico.Jobs
{

    [Obsolete("This is the V1 Version. Please use V2 where possible.")]
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
