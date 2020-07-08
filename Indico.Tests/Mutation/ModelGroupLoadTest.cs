using System.Threading.Tasks;
using GraphQL.Client.Http;
using Indico.Mutation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Indico.Tests.Mutation
{
    [TestClass]
    public class ModelGroupQueryTest
    {
        GraphQLHttpClient _client;

        [TestInitialize]
        public void Initialize()
        {
            GraphQLHttpClientOptions options = new GraphQLHttpClientOptions()
            {
                EndPoint = new System.Uri("http://www.example.com/graph/api/graphql"),
                HttpMessageHandler = new MockHttpHandler()
            };
            _client = new GraphQLHttpClient(options);
        }

        [TestMethod]
        async public Task Test()
        {
            ModelGroupLoad modelGroupLoad = new ModelGroupLoad(_client) { ModelId = 1 };
            string status = await modelGroupLoad.Exec();
            Assert.AreEqual("loading", status);
        }
    }
}
