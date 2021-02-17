using System.Threading.Tasks;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Indico.Query;
using Indico.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Indico.Tests.Query
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
            var modelGroupQuery = new ModelGroupQuery(_client) { MgId = 1 };
            var modelGroup = await modelGroupQuery.Exec();
            Assert.AreEqual(1, modelGroup.Id);
            Assert.AreEqual("testModelGroup", modelGroup.Name);
            Assert.AreEqual(ModelStatus.COMPLETE, modelGroup.Status);
            Assert.AreEqual(1, modelGroup.SelectedModel.Id);
        }
    }
}
