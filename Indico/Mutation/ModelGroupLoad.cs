using GraphQL.Client.Http;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Indico.Entity;
using Indico.Exception;

namespace Indico.Mutation
{
    public class ModelGroupLoad : Mutation<string>
    {
        GraphQLHttpClient _graphQLHttpClient;
        int _id;

        internal ModelGroupLoad(GraphQLHttpClient graphQLHttpClient)
        {
            this._graphQLHttpClient = graphQLHttpClient;
        }

        public ModelGroupLoad ModelGroup(ModelGroup modelGroup)
        {
            this._id = modelGroup.SelectedModel.Id;
            return this;
        }

        public ModelGroupLoad ModelId(int modelId)
        {
            this._id = modelId;
            return this;
        }

        public string Execute()
        {
            string query = @"
                    mutation LoadModel($model_id: Int!) {
                        modelLoad(modelId: $model_id) {
                            status
                        }
                    }
                ";
            GraphQLRequest request = new GraphQLRequest(query)
            {
                OperationName = "LoadModel",
                Variables = new
                {
                    model_id = _id
                }
            };

            GraphQLResponse response = this._graphQLHttpClient.SendMutationAsync(request).Result;
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            var modelLoad = response.Data.modelLoad;
            if (modelLoad == null)
            {
                throw new RuntimeException($"Cannot Load Model id : {_id}");
            }

            string status = (string)modelLoad.status;
            return status;
        }
    }
}