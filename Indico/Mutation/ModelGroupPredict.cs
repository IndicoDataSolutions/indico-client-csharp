using System.Collections.Generic;
using System.Threading;
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
    public class ModelGroupPredict : Mutation<Job>
    {
        GraphQLHttpClient _graphQLHttpClient;
        List<string> _data;

        /// <summary>
        /// Get/Set the Model ID (often Selected Model ID for a Model Group)
        /// </summary>
        public int ModelId { get; set; }

        /// <summary>
        /// ModelGroupPredict constructor
        /// </summary>
        /// <param name="graphQLHttpClient"></param>
        public ModelGroupPredict(GraphQLHttpClient graphQLHttpClient)
        {
            this._graphQLHttpClient = graphQLHttpClient;
        }

        /// <summary>
        /// Data to predict
        /// </summary>
        /// <returns>ModelGroupPredict</returns>
        /// <param name="data">Data.</param>
        public ModelGroupPredict Data(List<string> data)
        {
            this._data = data;
            return this;
        }

        /// <summary>
        /// Executes request and returns job 
        /// </summary>
        /// <returns>Job</returns>
        async public Task<Job> Exec(CancellationToken cancellationToken = default)
        {
            string query = @"
                    mutation PredictModel($modelId: Int!, $data: [String]!) {
                        modelPredict(modelId: $modelId, data: $data) {
                            jobId
                        }
                    }
                ";

            GraphQLRequest request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "PredictModel",
                Variables = new
                {
                    modelId = this.ModelId,
                    data = this._data
                }
            };

            GraphQLResponse response = await this._graphQLHttpClient.SendMutationAsync(request, cancellationToken);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            string jobId = (string)response.Data.modelPredict.jobId;
            Job job = new Job(this._graphQLHttpClient, jobId);
            return job;
        }
    }
}
