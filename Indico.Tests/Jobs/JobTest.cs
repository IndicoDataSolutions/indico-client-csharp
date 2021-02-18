using System;
using System.Threading.Tasks;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Indico.Jobs;
using Indico.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Indico.Tests.Mutation
{
    [TestClass]
    public class JobTest
    {
        private GraphQLHttpClient _client;

        [TestInitialize]
        public void Initialize()
        {
            var options = new GraphQLHttpClientOptions()
            {
                EndPoint = new Uri("http://www.example.com/graph/api/graphql"),
                HttpMessageHandler = new MockHttpHandler()
            };
            _client = new GraphQLHttpClient(options, new NewtonsoftJsonSerializer());
        }

        [TestMethod]
        public async Task Test()
        {
            var jobQuery = new JobQuery(_client) { Id = "jobId_test" };
            var job = jobQuery.Exec();
            Assert.AreEqual(JobStatus.SUCCESS, await job.Status());
            //JObject json = (JObject)job.Results().Result[0];
            var jsonResults = await job.Results();
            foreach (JObject item in jsonResults)
            {
                Assert.AreEqual("testValue", item.GetValue("testKey"));
            }
        }
    }
}
