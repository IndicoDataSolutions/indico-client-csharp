using System;
using Indico;
using Indico.Request;
using Indico.Entity;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Examples
{
    class GraphQL
    {
        async static Task Main(string[] args)
        {
            IndicoConfig config = new IndicoConfig(
                host: "app.indico.io"
            );

            IndicoClient client = new IndicoClient(config);
            
            string query = @"
              query GetDatasets {
                datasets {
                  id
                  name
                  status
                  rowCount
                  numModelGroups
                  modelGroups {
                    id
                  }
                }
              }
            ";

            GraphQLRequest request = client.GraphQLRequest(query, "GetDatasets");            
            JObject response = await request.Call();
            Console.WriteLine(response);
        }
    }
}