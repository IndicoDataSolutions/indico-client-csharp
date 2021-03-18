using System.Threading.Tasks;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Indico.Mutation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Indico.Tests.Mutation
{
    [TestClass]
    public class ModelGroupQueryTest
    {
        private GraphQLHttpClient _client;

        [TestInitialize]
        public void Initialize()
        {
            var options = new GraphQLHttpClientOptions()
            {
                EndPoint = new System.Uri("http://www.example.com/graph/api/graphql"),
                HttpMessageHandler = new MockHttpHandler()
            };
            _client = new GraphQLHttpClient(options, new NewtonsoftJsonSerializer());
        }

        [TestMethod]
        public async Task Test()
        {
            var modelGroupLoad = new ModelGroupLoad(_client) { ModelId = 1 };
            string status = await modelGroupLoad.Exec();
            Assert.AreEqual("loading", status);
        }
    }
}
