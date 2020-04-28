using System.Collections.Generic;
using GraphQL.Client.Http;
using Indico.Jobs;
using Indico.Mutation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Indico.Tests.Mutation
{
    [TestClass]
    public class ModelGroupPredictTest
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
        public void Test()
        {
            ModelGroupPredict modelGroupPredict = new ModelGroupPredict(_client) { ModelId = 1 };
            Job job = modelGroupPredict.Data(new List<string>()).Exec();
            Assert.AreEqual("jobId_test", job.Id);
        }
    }
}
