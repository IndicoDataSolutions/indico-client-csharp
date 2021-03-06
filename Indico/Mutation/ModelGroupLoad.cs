using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Client.Http;
using GraphQL.Common.Request;
using Indico.Entity;
using Indico.Exception;

namespace Indico.Mutation
{
    /// <summary>
    /// Loads a Model Group.
    /// </summary>
    public class ModelGroupLoad : IMutation<string>
    {
        private readonly GraphQLHttpClient _graphQLHttpClient;

        private int? _modelId;

        /// <summary>
        /// Model Id.
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
        /// Model Group Load Constructor.
        /// </summary>
        /// <param name="graphQLHttpClient">Client used to send API requests.</param>
        public ModelGroupLoad(GraphQLHttpClient graphQLHttpClient) => _graphQLHttpClient = graphQLHttpClient;

        /// <summary>
        /// Used to load ModelGroup.
        /// </summary>
        /// <param name="modelGroup">Model group.</param>
        public ModelGroupLoad ModelGroup(ModelGroup modelGroup)
        {
            ModelId = modelGroup.SelectedModel.Id;
            return this;
        }

        /// <summary>
        /// Executes request and returns load status.  
        /// </summary>
        /// <returns>Load status.</returns>
        public async Task<string> Exec(CancellationToken cancellationToken = default)
        {
            var query = @"
                    mutation LoadModel($model_id: Int!) {
                        modelLoad(modelId: $model_id) {
                            status
                        }
                    }
                ";

            var request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "LoadModel",
                Variables = new
                {
                    model_id = ModelId
                }
            };

            var response = await _graphQLHttpClient.SendMutationAsync(request, cancellationToken);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            var modelLoad = response.Data.modelLoad;
            if (modelLoad == null)
            {
                throw new RuntimeException($"Cannot Load Model id : {ModelId}");
            }

            var status = (string)modelLoad.status;
            return status;
        }
    }
}