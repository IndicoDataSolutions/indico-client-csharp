using System.Threading.Tasks;
using IndicoV2.IntegrationTests.Utils;
using IndicoV2.GraphQLRequest;
using NUnit.Framework;
using Unity;

namespace IndicoV2.IntegrationTests.GraphQLRequest
{
    public class GraphQLRequestTests
    {
        private IGraphQLRequestClient _graphQLRequestClient;

        [SetUp]
        public void SetUp()
        {
            var container = new IndicoTestContainerBuilder().Build();
            _graphQLRequestClient = container.Resolve<IGraphQLRequestClient>();
        }

        [Test]
        public async Task RawQueryListDatasets_ShouldReturnResult()
        {
            string query = @"
            query ListDatasets($limit: Int){
                datasetsPage(limit: $limit) {
                    datasets {
                        id
                        name
                        rowCount
                    }
                }
            }";
            string operationName = "ListDatasets";
            dynamic variables = new { limit = 1 };
            var result = await _graphQLRequestClient.Call(query, operationName, variables);
            result.Should().NotBeNull();
        }
    }
}
