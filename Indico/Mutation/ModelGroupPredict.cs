using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Http;
using Indico.Entity;
using Indico.Exception;
using Indico.Jobs;

namespace Indico.Mutation
{

    [Obsolete("This is the V1 Version of the object. Please use V2 where possible.")]
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
                    throw new ArgumentNullException(nameof(ModelId));
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

            var response = await _graphQLHttpClient.SendMutationAsync<dynamic>(request, cancellationToken);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            string jobId = (string)response.Data.modelPredict.jobId;
            
            return new Job(_graphQLHttpClient, jobId);
        }
    }
}
