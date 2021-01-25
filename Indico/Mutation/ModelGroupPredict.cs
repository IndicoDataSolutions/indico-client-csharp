using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Client.Http;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Indico.Entity;
using Indico.Exception;
using Indico.Jobs;

namespace Indico.Mutation
{
    /// <summary>
    /// Class to run Model Group predictions
    /// </summary>
    public class ModelGroupPredict : IMutation<Job>
    {
        private readonly GraphQLHttpClient _graphQLHttpClient;
        private List<string> _data;

        /// <summary>
        /// Get/Set the Model ID (often Selected Model ID for a Model Group)
        /// </summary>
        public int ModelId { get; set; }

        /// <summary>
        /// ModelGroupPredict constructor
        /// </summary>
        /// <param name="graphQLHttpClient"></param>
        public ModelGroupPredict(GraphQLHttpClient graphQLHttpClient) => _graphQLHttpClient = graphQLHttpClient;

        /// <summary>
        /// Data to predict
        /// </summary>
        /// <returns>ModelGroupPredict</returns>
        /// <param name="data">Data.</param>
        public ModelGroupPredict Data(List<string> data)
        {
            _data = data;
            return this;
        }

        /// <summary>
        /// Executes request and returns job 
        /// </summary>
        /// <returns>Job</returns>
        public async Task<Job> Exec()
        {
            string query = @"
                    mutation PredictModel($modelId: Int!, $data: [String]!) {
                        modelPredict(modelId: $modelId, data: $data) {
                            jobId
                        }
                    }
                ";

            var request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "PredictModel",
                Variables = new
                {
                    modelId = ModelId,
                    data = _data
                }
            };

            var response = await _graphQLHttpClient.SendMutationAsync(request);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            string jobId = (string)response.Data.modelPredict.jobId;
            var job = new Job(_graphQLHttpClient, jobId);
            return job;
        }
    }
}
