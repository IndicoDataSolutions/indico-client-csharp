using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace IndicoV2.GraphQLRequest
{
    public interface IGraphQLRequestClient
    {
        /// <summary>
        /// Run the GraphQL Query
        /// </summary>
        /// <returns></returns>
        Task<JObject> Call(string query, string operationName, dynamic variables, CancellationToken cancellationToken = default);
    }
}

