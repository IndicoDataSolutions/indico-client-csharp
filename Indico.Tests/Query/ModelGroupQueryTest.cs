using System.Threading.Tasks;
using GraphQL.Client.Http;
using Indico.Query;
using Indico.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Indico.Tests.Query
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
            ModelGroupQuery modelGroupQuery = new ModelGroupQuery(_client) { MgId = 1 };
            Entity.ModelGroup modelGroup = await modelGroupQuery.Exec();
            Assert.AreEqual(1, modelGroup.Id);
            Assert.AreEqual("testModelGroup", modelGroup.Name);
            Assert.AreEqual(ModelStatus.COMPLETE, modelGroup.Status);
            Assert.AreEqual(1, modelGroup.SelectedModel.Id);
        }
    }
}
