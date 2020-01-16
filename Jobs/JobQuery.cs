using System.Threading.Tasks;
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

        public JobQuery Id(string id)
        {
            this._id = id;
            return this;
        }

        public Job Query()
        {
            return new Job(this._graphQLHttpClient, this._id);
        }

        public Job Refresh(Job obj)
        {
            //TODO:
            throw new RuntimeException("Method Not Implemented");
        }
    }
}
