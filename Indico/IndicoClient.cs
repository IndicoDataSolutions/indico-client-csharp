using GraphQL.Client.Http;
using Indico.Jobs;
using Indico.Mutation;
using Indico.Query;
using Indico.Entity;
using Indico.Request;
using Indico.Storage;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using GraphQL.Client.Serializer.Newtonsoft;
using System;

namespace Indico
{
    
    /// <summary>
    /// Indico Client to send all GraphQL requests to the platform
    /// </summary>
    /// 
    [Obsolete("This is the V1 Version of the indico client. Please use V2.")]
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
                Config = indicoConfig;
            }
            else
            {
                Config = new IndicoConfig();
            }

            var handler = GetHandler();
            HttpClient = new HttpClient(handler);
            string endpoint = Config.GetAppBaseUrl();
            var options = new GraphQLHttpClientOptions
            {
                EndPoint = new System.Uri($"{endpoint}/graph/api/graphql"),
                HttpMessageHandler = handler
            };
            GraphQLHttpClient = new GraphQLHttpClient(options, new NewtonsoftJsonSerializer(), HttpClient);
        }

        private HttpMessageHandler GetHandler()
        {
            var innerHandler = new HttpClientHandler();
            if (!Config.Verify)
            {
                innerHandler.ServerCertificateCustomValidationCallback = (httpRequestMessage, x509Certificate2, x509Chain, sslPolicyError) => true;
            }
            var tokenHandler = new TokenHandler(Config.ApiToken, innerHandler);
            return tokenHandler;
        }

        /// <summary>
        /// Create a new GraphQL request
        /// </summary>
        /// <returns>GraphQLRequest</returns>
        public GraphQLRequest GraphQLRequest(string query=null, string operationName=null)
        {
            var request = new GraphQLRequest(GraphQLHttpClient);
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
            var mgQuery = new ModelGroupQuery(GraphQLHttpClient);
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
            var mgTraining = new TrainingModelWithProgressQuery(this);
            if (mg != null)
            {
                mgTraining.ModelId = mg.Id;
            }
            return mgTraining;
        }

        /// <summary>
        /// Create a new mutation to submit documents to process by a workflow
        /// </summary>
        /// <returns>WorkflowSubmission</returns>
        public WorkflowSubmission WorkflowSubmission() => new WorkflowSubmission(this);

  
        

        /// <summary>
        /// Create a new request to fetch model predictions.
        /// </summary>
        /// <returns>ModelGroupPredict</returns>
        public ModelGroupPredict ModelGroupPredict(ModelGroup mg=null)
        {
            var mgPredict = new ModelGroupPredict(GraphQLHttpClient);
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
            var ocr = new DocumentExtraction(this);
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
        public JobQuery JobQuery() => new JobQuery(GraphQLHttpClient);

        /// <summary>
        /// Retrieve a blob from indico blob storage
        /// </summary>
        /// <param name="url">URL to retrieve. Defaults to null.</param>
        /// <returns>RetrieveBlob</returns>
        public RetrieveBlob RetrieveBlob(string url=null)
        {
            var blob = new RetrieveBlob(this);
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
        public UploadFile UploadFile() => new UploadFile(this);
    }
}