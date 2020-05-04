using System;
using GraphQL.Client.Http;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Indico.Entity;
using Indico.Exception;
using Indico.Types;
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
                    modelGroupIds = this.MgId
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
                throw new RuntimeException($"Cannot find the default selected model for model group : {this.MgId}");
            }

            JToken mg = modelGroupList[0];
            JToken model = mg.Value<JToken>("selectedModel");

            return new ModelGroup()
            {
                Id = mg.Value<int>("id"),
                Name = mg.Value<string>("name"),
                Status = (ModelStatus)Enum.Parse(typeof(ModelStatus), mg.Value<string>("status")),
                SelectedModel = new Model()
                {
                    Id = model.Value<int>("id"),
                    Status = model.Value<string>("status")
                }
            };
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