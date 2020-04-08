using System.Linq;
using GraphQL.Client.Http;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Indico.Entity;
using Indico.Exception;
using Newtonsoft.Json.Linq;

namespace Indico.Query
{
    public class TrainingModelWithProgressQuery : Query<Model>
    {
        IndicoClient _client;
        int _id;
        string _name;

        public TrainingModelWithProgressQuery(IndicoClient client)
        {
            this._client = client;
        }

        /// <summary>
        /// Use to query TrainingModelWithProgress by id
        /// </summary>
        /// <returns>TrainingModelWithProgressQuery</returns>
        /// <param name="id">Identifier.</param>
        public TrainingModelWithProgressQuery Id(int id)
        {
            this._id = id;
            return this;
        }

        /// <summary>
        /// Use to query TrainingModelWithProgress by name
        /// </summary>
        /// <returns>TrainingModelWithProgressQuery</returns>
        /// <param name="name">Name.</param>
        public TrainingModelWithProgressQuery Name(string name)
        {
            this._name = name;
            return this;
        }

        public Model Query()
        {
            GraphQLHttpClient graphQLHttpClient = this._client.GraphQLHttpClient;
            string query = @"
                    query ModelGroupProgressQuery($id: Int) {
                        modelGroups(modelGroupIds: [$id]) {
                            modelGroups {
                                models {
                                    id
                                    status
                                    trainingProgress {
                                        percentComplete
                                    }
                                }
                            }
                        }
                    }
                ";
            GraphQLRequest request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "ModelGroupProgressQuery",
                Variables = new
                {
                    id = _id
                }
            };

            GraphQLResponse response = graphQLHttpClient.SendQueryAsync(request).Result;
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            var modelGroups = response.Data.modelGroups.modelGroups;
            if (modelGroups.Count != 1)
            {
                throw new RuntimeException("Cannot find Model Group");
            }
            JArray models = (JArray)modelGroups[0].models;

            JToken last = models.Select(token => (token.Value<int>("id"), Token: token)).Max().Token;
            if (last == null)
            {
                throw new RuntimeException("Cannot find Training Model");
            }

            JObject model = (JObject)last;
            return new Model(model);
        }

        public Model Refresh(Model model)
        {
            return model;
        }
    }
}
