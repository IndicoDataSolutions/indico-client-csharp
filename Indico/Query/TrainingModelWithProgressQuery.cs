using System.Linq;
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
    /// Find the % complete of a training Model Group
    /// </summary>
    public class TrainingModelWithProgressQuery : Query<JArray>
    {
        IndicoClient _client;

        /// <summary>
        /// Get/Set the Model ID (Often, the Selected Model ID for a Model Group)
        /// </summary>
        public int ModelId { get; set; }

        /// <summary>
        /// Find the % complete of a training Model Group
        /// </summary>
        /// <param name="client">Indico Client</param>
        public TrainingModelWithProgressQuery(IndicoClient client)
        {
            this._client = client;
        }

        /// <summary>
        /// Query a Model Group for training % complete
        /// </summary>
        /// <returns>JObject with % training complete</returns>
        async public Task<JArray> Exec()
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
                    id = this.ModelId
                }
            };

            GraphQLResponse response = await graphQLHttpClient.SendQueryAsync(request);
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

        public Task<JArray> Refresh(JArray obj)
        {
            //TODO:
            throw new RuntimeException("Method Not Implemented");
        }
    }
}
