using System;
using Indico;
using Indico.Request;
using Indico.Entity;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Examples
{
    internal class GraphQL
    {
        private static async Task Main(string[] args)
        {
            var config = new IndicoConfig(
                host: "app.indico.io"
            );

            var client = new IndicoClient(config);
            
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

            var request = client.GraphQLRequest(query, "GetDatasets");            
            var response = await request.Call();
            Console.WriteLine(response);
        }
    }
}