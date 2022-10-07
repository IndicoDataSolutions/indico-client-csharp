using System;
using System.Threading;
using System.Threading.Tasks;
using Indico;
using Indico.Request;
using IndicoV2.GraphQLRequest;
using Newtonsoft.Json.Linq;

namespace IndicoV2.V1Adapters.GraphQLRequest
{
    public class GraphQLRequestV1Adapter : IGraphQLRequestClient
    {
        private readonly IndicoClient _indicoClient;

        public GraphQLRequestV1Adapter(IndicoClient indicoClient)
        {
            _indicoClient = indicoClient;
        }

        public async Task<JObject> Call(string query, string operationName, dynamic variables, CancellationToken cancellationToken = default)
        {
            var graphQLRequest = _indicoClient.GraphQLRequest();
            graphQLRequest.Query = query;
            graphQLRequest.OperationName = operationName;
            graphQLRequest.Variables = variables;
            return await graphQLRequest.Call();
        }
    }
}

