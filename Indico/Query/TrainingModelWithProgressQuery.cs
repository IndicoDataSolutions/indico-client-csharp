using System.Linq;
using GraphQL.Client.Http;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Indico.Entity;
using Indico.Exception;
using Newtonsoft.Json.Linq;

namespace Indico.Query
{
    /// <summary>
    /// Find the % complete of a training Model Group
    /// </summary>
    public class TrainingModelWithProgressQuery : Query<JArray>
    {
        IndicoClient _client;
        int _id;
        string _name;

        /// <summary>
        /// The model ID to query. Often this is the selected model Id for a Model Group
        /// </summary>
        public int Id
        {
            get => this._id;
            set => this._id = value;
        }

        /// <summary>
        /// Find the % complete of a training Model Group
        /// </summary>
        /// <param name="client">Indico Client</param>
        public TrainingModelWithProgressQuery(IndicoClient client)
        {
            this._client = client;
        }

        /// <summary>
        /// Use to query TrainingModelWithProgress by id
        /// </summary>
        /// <returns>TrainingModelWithProgressQuery</returns>
        /// <param name="id">Identifier.</param>
        public TrainingModelWithProgressQuery SetId(int id)
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

        /// <summary>
        /// Query a Model Group for training % complete
        /// </summary>
        /// <returns>JObject with % training complete</returns>
        public JArray Exec()
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
            
            return (JArray)modelGroups[0].models;
        }

        public JArray Refresh(JArray obj)
        {
            return obj;
        }
    }
}
