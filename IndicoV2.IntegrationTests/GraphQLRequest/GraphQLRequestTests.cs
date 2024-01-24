using System;
using System.Threading.Tasks;
using FluentAssertions;
using IndicoV2.IntegrationTests.Utils;
using NUnit.Framework;
using Unity;

namespace IndicoV2.IntegrationTests.GraphQLRequest
{
    public class GraphQLRequestTests
    {
        private IndicoClient _indicoClient;

        [SetUp]
        public void SetUp()
        {
            var container = new IndicoTestContainerBuilder().Build();
            _indicoClient = container.Resolve<IndicoClient>();
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
            var request = _indicoClient.GraphQLRequest();
            var result = await request.Call(query, operationName, variables);
            result.Should().NotBeNull();
        }
    }
}
