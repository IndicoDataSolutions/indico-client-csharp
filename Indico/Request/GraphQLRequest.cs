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
    public class GraphQLRequest : RestRequest<JObject>
    {
        GraphQLHttpClient _client;

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

        public GraphQLRequest(GraphQLHttpClient client)
        {
            this._client = client;
        }

        /// <summary>
        /// Run the GraphQL Query
        /// </summary>
        /// <returns></returns>
        async public Task<JObject> Call()
        {
            GraphQLHttpRequest request = new GraphQLHttpRequest()
            {
                Query = this.Query,
                OperationName = this.OperationName,
                Variables = this.Variables
            };

            GraphQLHttpResponse response = await this._client.SendQueryAsync(request);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            JObject data = (JObject)response.Data;
            return data;
        }
    }
}
