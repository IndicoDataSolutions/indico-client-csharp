using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Client.Http;
using GraphQL.Common.Request;
using Indico.Exception;
using Indico.Jobs;

namespace Indico.Mutation
{
    /// <summary>
    /// Predicts on a Model Group.
    /// </summary>
    public class ModelGroupPredict : IMutation<Job>
    {
        private readonly GraphQLHttpClient _graphQLHttpClient;
        private int? _modelId;
        private List<string> _data;

        /// <summary>
        /// Get/Set the Model ID (often Selected Model ID for a Model Group).
        /// </summary>
        public int ModelId
        {
            get
            {
                if (!_modelId.HasValue)
                {
                    throw new ArgumentNullException();
                }

                return _modelId.Value;
            }

            set => _modelId = value;
        }

        /// <summary>
        /// ModelGroupPredict constructor.
        /// </summary>
        /// <param name="graphQLHttpClient">Client used to send API requests.</param>
        public ModelGroupPredict(GraphQLHttpClient graphQLHttpClient) => _graphQLHttpClient = graphQLHttpClient;

        /// <summary>
        /// Data to predict.
        /// </summary>
        /// <returns>ModelGroupPredict</returns>
        public ModelGroupPredict Data(List<string> data)
        {
            _data = data;
            return this;
        }

        /// <summary>
        /// Executes request and returns job. 
        /// </summary>
        public async Task<Job> Exec(CancellationToken cancellationToken = default)
        {
            var query = @"
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

            var response = await _graphQLHttpClient.SendMutationAsync(request, cancellationToken);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            var jobId = (string)response.Data.modelPredict.jobId;
            var job = new Job(_graphQLHttpClient, jobId);

            return job;
        }
    }
}
