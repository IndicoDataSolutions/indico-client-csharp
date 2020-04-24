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
        int _id;
        string _name;

        /// <summary>
        /// Model Group ID
        /// </summary>
        public int Id
        {
            get => _id;
            set => this._id = value;
        }

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
        public ModelGroupQuery SetId(int id)
        {
            this.Id = id;
            return this;
        }

        /// <summary>
        /// Use to query ModelGroup by name
        /// </summary>
        /// <returns>ModelGroupQuery</returns>
        /// <param name="name">Name.</param>
        public ModelGroupQuery Name(string name)
        {
            this._name = name;
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
                    modelGroupIds = _id
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
                throw new RuntimeException($"Cannot find the default selected model for model group : {_id}");
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