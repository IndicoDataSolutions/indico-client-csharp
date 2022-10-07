using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Indico.Entity;
using IndicoV2.V1Adapters.GraphQLRequest;
using Newtonsoft.Json.Linq;

namespace IndicoV2.GraphQLRequest
{
    public class GraphQLRequestClient : IGraphQLRequestClient
    {
        private readonly GraphQLRequestV1Adapter _legacyAdapter;

        public GraphQLRequestClient(GraphQLRequestV1Adapter legacyAdapter)
        {
            _legacyAdapter = legacyAdapter;
        }

        public async Task<JObject> Call(string query, string operationName, dynamic variables, CancellationToken cancellationToken = default)
        {
            return await _legacyAdapter.Call(query, operationName, variables);
        }
    }
}

