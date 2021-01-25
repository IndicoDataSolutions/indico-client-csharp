using System.Threading.Tasks;
using GraphQL.Client.Http;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Indico.Entity;
using Indico.Exception;

namespace Indico.Mutation
{
    /// <summary>
    /// Load a Model Group
    /// </summary>
    public class ModelGroupLoad : IMutation<string>
    {
        private readonly GraphQLHttpClient _graphQLHttpClient;
        
        public int ModelId { get; set; }

        /// <summary>
        /// Model Group Load Constructor
        /// </summary>
        /// <param name="graphQLHttpClient"></param>
        public ModelGroupLoad(GraphQLHttpClient graphQLHttpClient) => _graphQLHttpClient = graphQLHttpClient;

        /// <summary>
        /// Use to load ModelGroup
        /// </summary>
        /// <returns>ModelGroupLoad</returns>
        /// <param name="modelGroup">Model group.</param>
        public ModelGroupLoad ModelGroup(ModelGroup modelGroup)
        {
            ModelId = modelGroup.SelectedModel.Id;
            return this;
        }

        /// <summary>
        /// Executes request and returns load status  
        /// </summary>
        /// <returns>Load status</returns>
        public async Task<string> Exec()
        {
            string query = @"
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

            var response = await _graphQLHttpClient.SendMutationAsync(request);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            var modelLoad = response.Data.modelLoad;
            if (modelLoad == null)
            {
                throw new RuntimeException($"Cannot Load Model id : {ModelId}");
            }

            string status = (string)modelLoad.status;
            return status;
        }
    }
}