using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Indico.Mutation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Indico.Tests.Mutation
{
    [TestClass]
    public class ModelGroupPredictTest
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
            var modelGroupPredict = new ModelGroupPredict(_client) { ModelId = 1 };
            var job = await modelGroupPredict.Data(new List<string>()).Exec();
            Assert.AreEqual("jobId_test", job.Id);
        }
    }
}
