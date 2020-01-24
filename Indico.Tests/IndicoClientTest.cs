using Indico.Entity;
using Indico.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Indico.Types;
using Indico.Mutation;
using Indico.Jobs;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using GraphQL.Client.Http;

namespace Indico.Tests
{

    [TestClass]
    public class IndicoClientTest
    {
        GraphQLHttpClient _client;

        [TestInitialize]
        public void Initialize()
        {
            _client = this.GetGraphQLHttpClient();
        }

        [TestMethod]
        public void TestModelGroupQuery()
        {
            ModelGroupQuery modelGroupQuery = new ModelGroupQuery(_client);
            ModelGroup modelGroup = modelGroupQuery.Id(1).Query();
            Assert.AreEqual(1, modelGroup.Id);
            Assert.AreEqual("testModelGroup", modelGroup.Name);
            Assert.AreEqual(ModelStatus.COMPLETE, modelGroup.Status);
            Assert.AreEqual(1, modelGroup.SelectedModel.Id);
            Assert.AreEqual("testValue", modelGroup.SelectedModel.ModelInfo.GetValue("testKey"));
        }

        [TestMethod]
        public void TestModelGroupLoad()
        {
            ModelGroupQuery modelGroupQuery = new ModelGroupQuery(_client);
            ModelGroup modelGroup = modelGroupQuery.Id(1).Query();
            ModelGroupLoad modelGroupLoad = new ModelGroupLoad(_client);
            string status = modelGroupLoad.ModelGroup(modelGroup).Execute();
            Assert.AreEqual("loading", status);
        }

        [TestMethod]
        public void TestModelGroupPredict()
        {
            ModelGroupQuery modelGroupQuery = new ModelGroupQuery(_client);
            ModelGroup modelGroup = modelGroupQuery.Id(1).Query();
            ModelGroupPredict modelGroupPredict = new ModelGroupPredict(_client);
            Job job = modelGroupPredict
                .ModelGroup(modelGroup)
                .Data(new List<string>())
                .Execute();
            Assert.AreEqual("jobId_test", job.Id);
            Assert.AreEqual(JobStatus.SUCCESS, job.Status());
            JObject json = (JObject)job.Results().Result[0];
            Assert.AreEqual("testValue", json.GetValue("testKey"));
        }

        [TestMethod]
        public void TestPdfExtraction()
        {
            PdfExtraction pdfExtraction = new PdfExtraction(_client);
            Job job = pdfExtraction
                .Data(new List<string>())
                .Execute();
            Assert.AreEqual("jobId_test", job.Id);
            Assert.AreEqual(JobStatus.SUCCESS, job.Status());
            JObject json = (JObject)job.Results().Result[0];
            Assert.AreEqual("testValue", json.GetValue("testKey"));
        }

        public GraphQLHttpClient GetGraphQLHttpClient()
        {
            GraphQLHttpClientOptions options = new GraphQLHttpClientOptions() {
                EndPoint = new System.Uri("https://app.indico.io/graph/api/graphql"), 
                HttpMessageHandler = new MockHttpHandler()
            };
            GraphQLHttpClient graphQLHttpClient = new GraphQLHttpClient(options);
            return graphQLHttpClient;
        }
    }
}
