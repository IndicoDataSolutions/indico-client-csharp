using System;
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
    public class GraphQLRequestClient : IGraphQLRequestClient
    {
        private readonly GraphQLHttpClient _client;

        public GraphQLRequestClient(GraphQLHttpClient client) => _client = client;

        /// <summary>
        /// Run the GraphQL Query
        /// </summary>
        /// <returns></returns>
        [Obsolete("operationName is deprecated. Use call without operationName.")]
        public async Task<JObject> Call(string query, string operationName, dynamic variables = null, CancellationToken cancellationToken = default) => await Call(query, variables, cancellationToken);

        /// <summary>
        /// Run the GraphQL Query
        /// </summary>
        /// <returns></returns>
        public async Task<JObject> Call(string query, dynamic variables = null, CancellationToken cancellationToken = default)
        {
            if (query == null)
            {
                throw new GraphQLException("A query or mutation must be defined.");
            }
            var request = new GraphQLHttpRequest()
            {
                Query = query,
                Variables = variables
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
