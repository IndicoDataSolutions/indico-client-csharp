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

        public int mgId { get; set; }
        
        /// <summary>
        /// Constructor for Model Group Queries
        /// </summary>
        /// <param name="graphQLHttpClient"></param>
        public ModelGroupQuery(GraphQLHttpClient graphQLHttpClient)
        {
            this._graphQLHttpClient = graphQLHttpClient;
        }

        /// <summary>
        /// Used to query ModelGroup by id
        /// </summary>
        /// <returns>ModelGroupQuery</returns>
        /// <param name="id">Identifier.</param>
        public ModelGroupQuery SetId(int mgId)
        {
            this.mgId = mgId;
            return this;
        }

        /// <summary>
        /// Queries the server and returns ModelGroup
        /// </summary>
        /// <returns>ModelGroup</returns>
        public ModelGroup Exec()
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
                    modelGroupIds = this.mgId
                }
            };

            GraphQLResponse response = this._graphQLHttpClient.SendQueryAsync(request).Result;
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            var modelGroupList = response.Data.modelGroups.modelGroups;
            if (modelGroupList.Count == 0)
            {
                throw new RuntimeException($"Cannot find the default selected model for model group : {this.mgId}");
            }

            JObject modelGroup = (JObject)modelGroupList[0];
            return new ModelGroup(modelGroup);
        }

        /// <summary>
        /// Refreshes the ModelGroup Object
        /// </summary>
        /// <returns>ModelGroup</returns>
        /// <param name="obj">ModelGroup</param>
        public ModelGroup Refresh(ModelGroup obj)
        {
            //TODO:
            throw new RuntimeException("Method Not Implemented");
        }
    }
}