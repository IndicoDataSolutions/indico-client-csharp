using System;
using System.IO;
using System.Threading.Tasks;
using IndicoV2;
using Newtonsoft.Json.Linq;

namespace Examples
{
    /// <summary>
    /// Example of raw GraphQL call.
    /// </summary>
    public class GraphQLCall
    {
        private static string GetToken() =>
            File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "indico_api_token.txt"));

        public static async Task Main()
        {
            var client = new IndicoClient(GetToken(), new Uri("https://app.indico.io"));

            var graphQLRequestClient = client.GraphQLRequest();

            string query = @"
            query ListDatasets($limit: Int){
                datasetsPage(limit: $limit) {
                    datasets {
                        id
                        name
                        rowCount
                    }
                }
            }
            ";
            string operationName = "ListDatasets";
            dynamic variables = new { limit = 1 };

            JObject response = await graphQLRequestClient.Call(query, operationName, variables);
            Console.WriteLine(response);
        }
    }
}
