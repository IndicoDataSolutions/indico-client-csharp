using GraphQL.Client.Http;
using Indico.Exception;

namespace Indico.Jobs
{
    public class JobQuery : Query<Job>
    {
        GraphQLHttpClient _graphQLHttpClient;
        string _id;

        public JobQuery(GraphQLHttpClient graphQLHttpClient)
        {
            this._graphQLHttpClient = graphQLHttpClient;
        }

        /// <summary>
        /// Use to query job by id
        /// </summary>
        /// <returns>JobQuery</returns>
        /// <param name="id">Identifier.</param>
        public JobQuery Id(string id)
        {
            this._id = id;
            return this;
        }

        /// <summary>
        /// Returns Job
        /// </summary>
        /// <returns>Job</returns>
        public Job Query()
        {
            return new Job(this._graphQLHttpClient, this._id);
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
