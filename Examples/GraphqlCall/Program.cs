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
            var client = new IndicoClient(GetToken(), new Uri("https://try.indico.io"));

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
            dynamic variables = new { limit = 1 };
            var graphQLRequestClient = client.GraphQLRequest(query, variables);
            JObject response = await graphQLRequestClient.Call();
            Console.WriteLine(response);
        }
    }
}
