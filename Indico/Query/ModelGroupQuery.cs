using System.Threading.Tasks;
using GraphQL.Client.Http;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Indico.Entity;
using Indico.Exception;
using Newtonsoft.Json.Linq;

namespace Indico.Query
{
    /// <summary>
    /// Get a Model Group
    /// </summary>
    public class ModelGroupQuery : Query<ModelGroup>
    {

        GraphQLHttpClient _graphQLHttpClient;

        /// <summary>
        /// Get/Set the Model Group ID
        /// </summary>
        /// <value>Model Group ID</value>
        public int MgId { get; set; }
        
        /// <summary>
        /// Constructor for Model Group Queries
        /// </summary>
        /// <param name="graphQLHttpClient"></param>
        public ModelGroupQuery(GraphQLHttpClient graphQLHttpClient)
        {
            this._graphQLHttpClient = graphQLHttpClient;
        }

        /// <summary>
        /// Queries the server and returns ModelGroup
        /// </summary>
        /// <returns>ModelGroup</returns>
        async public Task<ModelGroup> Exec()
        {
            string query = @"
                    query ModelGroupQuery($modelGroupIds: [Int]!) {
                        modelGroups(modelGroupIds: $modelGroupIds) {
                            modelGroups {
                                id
                                name
                                status
                                selectedModel {
                                    id
                                    status
                                }
                            }
                        }
                    }
                ";

            GraphQLRequest request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "ModelGroupQuery",
                Variables = new
                {
                    modelGroupIds = this.MgId
                }
            };

            GraphQLResponse response = await this._graphQLHttpClient.SendQueryAsync(request);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            var modelGroupList = response.Data.modelGroups.modelGroups;
            if (modelGroupList.Count == 0)
            {
                throw new RuntimeException($"Cannot find the default selected model for model group : {this.MgId}");
            }

            JObject modelGroup = (JObject)modelGroupList[0];
            return new ModelGroup(modelGroup);
        }

        /// <summary>
        /// Refreshes the ModelGroup Object
        /// </summary>
        /// <returns>ModelGroup</returns>
        /// <param name="obj">ModelGroup</param>
        async public Task<ModelGroup> Refresh(ModelGroup obj)
        {
            //TODO:
            throw new RuntimeException("Method Not Implemented");
        }
    }
}