using GraphQL.Client.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using IndicoV2.Exception;

namespace IndicoV2.GraphQLRequest
{
    /// <summary>
    /// Class to send GraphQL Queries to the Indico Platform
    /// </summary>
    public class GraphQLRequest : IRestRequest<JObject>
    {
        private readonly GraphQLHttpClient _client;

        /// <summary>
        /// Get/Set the GraphQL Query String
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Get/Set the GraphQL Query Variables
        /// </summary>
        public dynamic Variables { get; set; }

        public GraphQLRequest(GraphQLHttpClient client) => _client = client;

        /// <summary>
        /// Run the GraphQL Query
        /// </summary>
        /// <returns></returns>
        public async Task<JObject> Call(CancellationToken cancellationToken = default)
        {
            var request = new GraphQLHttpRequest()
            {
                Query = Query,
                Variables = Variables
            };

            var response = await _client.SendQueryAsync<dynamic>(request, cancellationToken);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            var data = (JObject)response.Data;
            return data;
        }
    }
}
