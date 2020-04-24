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
    public class ModelGroupQuery : Query<ModelGroup>
    {

        GraphQLHttpClient _graphQLHttpClient;
        int _id;
        string _name;

        public ModelGroupQuery(GraphQLHttpClient graphQLHttpClient)
        {
            this._graphQLHttpClient = graphQLHttpClient;
        }

        /// <summary>
        /// Use to query ModelGroup by id
        /// </summary>
        /// <returns>ModelGroupQuery</returns>
        /// <param name="id">Identifier.</param>
        public ModelGroupQuery Id(int id)
        {
            this._id = id;
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
        public ModelGroup Query()
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

            JToken mg = modelGroupList[0];
            JToken model = mg.Value<JToken>("selectedModel");

            return new ModelGroup(
                id: mg.Value<int>("id"),
                name: mg.Value<string>("name"),
                status: (ModelStatus)Enum.Parse(typeof(ModelStatus), mg.Value<string>("status")),
                selectedModel: new Model(
                    id: model.Value<int>("id"),
                    status: model.Value<string>("status")
                )
            );
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