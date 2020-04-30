using GraphQL.Client.Http;
using Indico.Jobs;
using Indico.Mutation;
using Indico.Query;
using Indico.Entity;
using Indico.Request;
using Indico.Storage;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Indico
{
    /// <summary>
    /// Indico Client to send all GraphQL requests to the platform
    /// </summary>
    public class IndicoClient
    {
        /// <summary>
        /// Gets the client config.
        /// </summary>
        public IndicoConfig Config { get; }

        /// <summary>
        /// Gets the underlying http client.
        /// </summary>
        public HttpClient HttpClient { get; }

        /// <summary>
        /// Gets the underlying GraphQL client.
        /// </summary>
        public GraphQLHttpClient GraphQLHttpClient { get; }

        /// <summary>
        /// IndicoClient constructor
        /// </summary>
        /// <param name="indicoConfig">Client configuration with platform hostname, etc</param>
        public IndicoClient(IndicoConfig indicoConfig=null)
        {
            if (indicoConfig != null)
            {
                this.Config = indicoConfig;
            }
            else
            {
                this.Config = new IndicoConfig();
            }
            
            TokenHandler tokenHandler = new TokenHandler(this.Config.ApiToken);
            this.HttpClient = new HttpClient(tokenHandler);
            string endpoint = $"{this.Config.Protocol}://{this.Config.Host}";
            GraphQLHttpClientOptions options = new GraphQLHttpClientOptions();
            options.EndPoint = new System.Uri($"{endpoint}/graph/api/graphql");
            options.HttpMessageHandler = tokenHandler;
            this.GraphQLHttpClient = this.HttpClient.AsGraphQLClient(options);
        }

        /// <summary>
        /// Create a new GraphQL request
        /// </summary>
        /// <returns>GraphQLRequest</returns>
        public GraphQLRequest GraphQLRequest(string query=null, string operationName=null)
        {
            GraphQLRequest request = new GraphQLRequest(this.GraphQLHttpClient);
            if (query != null)
            {
                request.Query = query;
            }

            if (operationName != null)
            {
                request.OperationName = operationName;
            }
            return request;
        }

        /// <summary>
        /// Create a new Query for a ModelGroup
        /// </summary>
        /// <returns>ModelGroupQuery</returns>
        public ModelGroupQuery ModelGroupQuery(int mgId=-1)
        {
            ModelGroupQuery mgQuery = new ModelGroupQuery(this.GraphQLHttpClient);
            if (mgId != -1)
            {
                mgQuery.MgId = mgId;
            }
            return mgQuery;
        }

        /// <summary>
        /// Create a new Query to retrieve TrainingModelWithProgress.
        /// </summary>
        /// <returns>TrainingModelWithProgressQuery</returns>
        public TrainingModelWithProgressQuery TrainingModelWithProgressQuery(ModelGroup mg=null)
        {
            TrainingModelWithProgressQuery mgTraining = new TrainingModelWithProgressQuery(this);
            if (mg != null)
            {
                mgTraining.ModelId = mg.Id;
            }
            return mgTraining;
        }

        /// <summary>
        /// Create a new request to load a ModelGroup.
        /// </summary>
        /// <returns>ModelGroupLoad</returns>
        public ModelGroupLoad ModelGroupLoad(ModelGroup mg=null)
        {
            ModelGroupLoad mgLoad = new ModelGroupLoad(this.GraphQLHttpClient);
            if (mg != null)
            {
                mgLoad.ModelId = mg.SelectedModel.Id;
            }
            return mgLoad;
        }

        /// <summary>
        /// Create a new request to fetch model predictions.
        /// </summary>
        /// <returns>ModelGroupPredict</returns>
        public ModelGroupPredict ModelGroupPredict(ModelGroup mg=null)
        {
            ModelGroupPredict mgPredict = new ModelGroupPredict(this.GraphQLHttpClient);
            if (mg != null)
            {
                mgPredict.ModelId = mg.SelectedModel.Id;
            }
            return mgPredict;
        }

        /// <summary>
        /// Create a new DocumentExtraction client to OCR files
        /// </summary>
        /// <param name="jsonConfig">DocumentExtraction passed in as a JSON Object. Defaults to null</param>
        /// <returns>DocumentExtraction</returns>
        public DocumentExtraction DocumentExtraction(JObject jsonConfig=null)
        {
            DocumentExtraction ocr = new DocumentExtraction(this);
            if (jsonConfig != null)
            {
                ocr.JsonConfig = jsonConfig;
            }
            return ocr;
        }

        /// <summary>
        /// Create a query to retrieve async job info
        /// </summary>
        /// <returns>JobQuery</returns>
        public JobQuery JobQuery()
        {
            return new JobQuery(this.GraphQLHttpClient);
        }

        /// <summary>
        /// Retrieve a blob from indico blob storage
        /// </summary>
        /// <param name="url">URL to retrieve. Defaults to null.</param>
        /// <returns>RetrieveBlob</returns>
        public RetrieveBlob RetrieveBlob(string url=null)
        {
            RetrieveBlob blob = new RetrieveBlob(this);
            if (url != null)
            {
                blob.Url = url;
            }
            return blob;
        }

        /// <summary>
        /// Upload files
        /// </summary>
        /// <returns>UploadFile</returns>
        public UploadFile UploadFile()
        {
            return new UploadFile(this);
        }
    }
}