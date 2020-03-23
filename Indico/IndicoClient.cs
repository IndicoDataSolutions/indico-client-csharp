using GraphQL.Client.Http;
using Indico.Jobs;
using Indico.Mutation;
using Indico.Query;
using Indico.Storage;
using System.Net.Http;

namespace Indico
{
    /// <summary>
    /// Indico client with all available top level query and mutations
    /// </summary>
    public class IndicoClient
    {
        public IndicoConfig Config { get; }
        public HttpClient HttpClient { get; }
        public GraphQLHttpClient GraphQLHttpClient { get; }

        public IndicoClient(IndicoConfig indicoConfig)
        {
            this.Config = indicoConfig;
            TokenHandler tokenHandler = new TokenHandler(indicoConfig.ApiToken);
            this.HttpClient = new HttpClient(tokenHandler);
            string endpoint = $"{indicoConfig.Protocol}://{indicoConfig.Host}";
            GraphQLHttpClientOptions options = new GraphQLHttpClientOptions();
            options.EndPoint = new System.Uri($"{endpoint}/graph/api/graphql");
            options.HttpMessageHandler = tokenHandler;
            this.GraphQLHttpClient = this.HttpClient.AsGraphQLClient(options);
        }

        /// <summary>
        /// Create a new Query for ModelGroup
        /// </summary>
        /// <returns>ModelGroupQuery</returns>
        public ModelGroupQuery ModelGroupQuery()
        {
            return new ModelGroupQuery(this.GraphQLHttpClient);
        }

        /// <summary>
        /// Create a new Mutation to load model in ModelGroup
        /// </summary>
        /// <returns>ModelGroupLoad</returns>
        public ModelGroupLoad ModelGroupLoad()
        {
            return new ModelGroupLoad(this.GraphQLHttpClient);
        }

        /// <summary>
        /// Create a new Mutation to predict data
        /// </summary>
        /// <returns>ModelGroupPredict</returns>
        public ModelGroupPredict ModelGroupPredict()
        {
            return new ModelGroupPredict(this.GraphQLHttpClient);
        }

        /// <summary>
        /// Create a new mutation to submit PDF(s) to process by a PdfExtraction
        /// </summary>
        /// <returns>PdfExtraction</returns>
        public PdfExtraction PdfExtraction()
        {
            return new PdfExtraction(this.GraphQLHttpClient);
        }

        public DocumentExtraction DocumentExtraction()
        {
            return new DocumentExtraction(this);
        }

        /// <summary>
        /// Create a query to retrieve async job info
        /// </summary>
        /// <returns>JobQuery</returns>
        public JobQuery JobQuery()
        {
            return new JobQuery(this.GraphQLHttpClient);
        }

        public RetrieveBlob RetrieveBlob()
        {
            return new RetrieveBlob(this);
        }

        public UploadFile UploadFile()
        {
            return new UploadFile(this);
        }
    }
}