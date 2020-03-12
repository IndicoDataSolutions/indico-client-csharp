using GraphQL.Client.Http;
using Indico.Jobs;
using Indico.Mutation;
using Indico.Query;

namespace Indico
{
    /// <summary>
    /// Indico client with all available top level query and mutations
    /// </summary>
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
        /// <summary>
        /// Create a new Query for ModelGroup
        /// </summary>
        /// <returns>ModelGroupQuery</returns>
        public ModelGroupQuery ModelGroupQuery()
        {
            return new ModelGroupQuery(this._graphQLHttpClient);
        }

        /// <summary>
        /// Create a new Mutation to load model in ModelGroup
        /// </summary>
        /// <returns>ModelGroupLoad</returns>
        public ModelGroupLoad ModelGroupLoad()
        {
            return new ModelGroupLoad(this._graphQLHttpClient);
        }

        /// <summary>
        /// Create a new Mutation to predict data
        /// </summary>
        /// <returns>ModelGroupPredict</returns>
        public ModelGroupPredict ModelGroupPredict()
        {
            return new ModelGroupPredict(this._graphQLHttpClient);
        }

        /// <summary>
        /// Create a new mutation to submit PDF(s) to process by a PdfExtraction
        /// </summary>
        /// <returns>PdfExtraction</returns>
        public PdfExtraction PdfExtraction()
        {
            return new PdfExtraction(this._graphQLHttpClient);
        }

        /// <summary>
        /// Create a query to retrieve async job info
        /// </summary>
        /// <returns>JobQuery</returns>
        public JobQuery JobQuery()
        {
            return new JobQuery(this._graphQLHttpClient);
        }
    }
}