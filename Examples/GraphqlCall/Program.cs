using System;
using System.IO;
using System.Threading.Tasks;
using Indico;
using Indico.Request;
using Newtonsoft.Json.Linq;

namespace Examples
{
    /// <summary>
    /// Example for raw GraphQL call. Uses Indico V1 client.
    /// </summary>
    internal class GraphQLCall
    {
        private static string GetTokenPath() =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "indico_api_token.txt");

        public static async Task Main()
        {
            var config = new IndicoConfig(GetTokenPath(), "app.indico.io");
            var client = new IndicoClient(config);

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
            var request = new GraphQLRequest(client.GraphQLHttpClient)
            {
                Query = query,
                OperationName = "ListDatasets",
                Variables = new
                {
                    limit = 1,
                }
            };
            JObject response = await request.Call();
            Console.WriteLine(response);
        }
    }
}
