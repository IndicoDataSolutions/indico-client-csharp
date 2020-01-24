using System.Collections.Generic;
using GraphQL.Client.Http;
using Indico.Jobs;
using Indico.Mutation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Indico.Tests.Mutation
{
    [TestClass]
    public class PdfExtractionTest
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
            PdfExtraction pdfExtraction = new PdfExtraction(_client);
            Job job = pdfExtraction
                .Data(new List<string>())
                .Execute();
            Assert.AreEqual("jobId_test", job.Id);
        }
    }
}
