using GraphQL.Client.Http;
using Indico.Jobs;
using Indico.Mutation;
using Indico.Query;

namespace Indico
{
    public class IndicoClient
    {

        GraphQLHttpClient _graphQLHttpClient;

        public IndicoClient(IndicoConfig indicoConfig)
        {
            string endpoint = $"{indicoConfig.Protocol}://{indicoConfig.Host}";
            GraphQLHttpClientOptions options = new GraphQLHttpClientOptions();
            options.EndPoint = new System.Uri($"{endpoint}/graph/api/graphql");
            options.HttpMessageHandler = new TokenHandler(indicoConfig.ApiToken);
            this._graphQLHttpClient = new GraphQLHttpClient(options);
        }

        public ModelGroupQuery ModelGroupQuery()
        {
            return new ModelGroupQuery(this._graphQLHttpClient);
        }

        public ModelGroupLoad ModelGroupLoad()
        {
            return new ModelGroupLoad(this._graphQLHttpClient);
        }

        public ModelGroupPredict ModelGroupPredict()
        {
            return new ModelGroupPredict(this._graphQLHttpClient);
        }

        public PdfExtraction PdfExtraction()
        {
            return new PdfExtraction(this._graphQLHttpClient);
        }

        public JobQuery JobQuery()
        {
            return new JobQuery(this._graphQLHttpClient);
        }
    }
}