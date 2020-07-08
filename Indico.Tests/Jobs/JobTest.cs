using System;
using System.Threading.Tasks;
using GraphQL.Client.Http;
using Indico.Jobs;
using Indico.Types;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Indico.Tests.Mutation
{
    [TestClass]
    public class JobTest
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
            JobQuery jobQuery = new JobQuery(_client) { Id = "jobId_test" };
            Job job = jobQuery.Exec();
            Assert.AreEqual(JobStatus.SUCCESS, await job.Status());
            //JObject json = (JObject)job.Results().Result[0];
            JArray jsonResults = await job.Results();
            foreach (JObject item in jsonResults)
            {
                Assert.AreEqual("testValue", item.GetValue("testKey"));
            }
        }
    }
}
