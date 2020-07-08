using GraphQL.Client.Http;
using Indico.Exception;

namespace Indico.Jobs
{
    public class JobQuery
    {
        GraphQLHttpClient _graphQLHttpClient;
        
        /// <summary>
        /// Get/Set the Job ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Query a Job
        /// </summary>
        /// <param name="graphQLHttpClient"></param>
        public JobQuery(GraphQLHttpClient graphQLHttpClient)
        {
            this._graphQLHttpClient = graphQLHttpClient;
        }

        /// <summary>
        /// Returns Job
        /// </summary>
        /// <returns>Job</returns>
        public Job Exec()
        {
            return new Job(this._graphQLHttpClient, this.Id);
        }

        /// <summary>
        /// Refreshes the Job Object
        /// </summary>
        /// <returns>Job</returns>
        /// <param name="obj">Job</param>
        public Job Refresh(Job obj)
        {
            //TODO:
            throw new RuntimeException("Method Not Implemented");
        }
    }
}
