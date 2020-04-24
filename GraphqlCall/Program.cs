using System;
using Indico;
using Indico.Request;
using Indico.Entity;
using Newtonsoft.Json.Linq;

namespace Examples
{
    class GraphQL
    {
        static void Main(string[] args)
        {
            IndicoConfig config = new IndicoConfig(
                host: "app.indico.io"
            );

            IndicoClient client = new IndicoClient(config);
            GraphQLRequest request = client.GraphQLRequest();
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

            JObject response = request.Query(query).OperationName("GetDatasets").Call();
            Console.WriteLine(response);
        }
    }
}