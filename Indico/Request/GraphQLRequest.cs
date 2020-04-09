using Indico.Exception;
using Newtonsoft.Json.Linq;
using GraphQLHttpRequest = GraphQL.Common.Request.GraphQLRequest;
using GraphQLHttpResponse = GraphQL.Common.Response.GraphQLResponse;

namespace Indico.Request
{
    public class GraphQLRequest : RestRequest<JObject>
    {
        IndicoClient _client;
        string _query;
        string _operationName;
        dynamic _variables;

        public GraphQLRequest(IndicoClient client)
        {
            this._client = client;
        }

        public GraphQLRequest Query(string query)
        {
            this._query = query;
            return this;
        }

        public GraphQLRequest OperationName(string operationName)
        {
            this._operationName = operationName;
            return this;
        }

        public GraphQLRequest Variables(dynamic variables)
        {
            this._variables = variables;
            return this;
        }

        public JObject Call()
        {
            GraphQLHttpRequest request = new GraphQLHttpRequest()
            {
                Query = this._query,
                OperationName = this._operationName,
                Variables = this._variables
            };

            GraphQLHttpResponse response = this._client.GraphQLHttpClient.SendQueryAsync(request).Result;
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            JObject data = (JObject)response.Data;
            return data;
        }
    }
}
