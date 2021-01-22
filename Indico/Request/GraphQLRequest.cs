using System.Threading.Tasks;
using GraphQL.Client.Http;
using Indico.Exception;
using Newtonsoft.Json.Linq;
using GraphQLHttpRequest = GraphQL.Common.Request.GraphQLRequest;
using GraphQLHttpResponse = GraphQL.Common.Response.GraphQLResponse;

namespace Indico.Request
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
        /// Get/Set the Operation Name
        /// </summary>
        public string OperationName { get; set; }

        /// <summary>
        /// Get/Set the GraphQL Query Variables
        /// </summary>
        public dynamic Variables { get; set; }

        public GraphQLRequest(GraphQLHttpClient client) => _client = client;

        /// <summary>
        /// Run the GraphQL Query
        /// </summary>
        /// <returns></returns>
        public async Task<JObject> Call()
        {
            var request = new GraphQLHttpRequest()
            {
                Query = Query,
                OperationName = OperationName,
                Variables = Variables
            };

            var response = await _client.SendQueryAsync(request);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            var data = (JObject)response.Data;
            return data;
        }
    }
}
