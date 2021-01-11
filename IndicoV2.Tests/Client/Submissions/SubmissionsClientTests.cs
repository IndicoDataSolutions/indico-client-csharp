using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IndicoV2.Tests.TestUtils;

namespace IndicoV2.Tests.Client.Submissions
{
    [TestClass]
    public class SubmissionsClientTests
    {
        private ClientProvider _clientProvider;

        [TestInitialize]
        public void CreateProvider()
        {
            _clientProvider = new ClientProvider();
        }

        [TestMethod]
        public async Task Create_ShouldReturnListOfIds()
        {
            _clientProvider.Handler.ForRequest(
                req => req.RequestUri.AbsolutePath == "/storage/files/store",
                new[]
                {
                    new
                    {
                        name = "test_file.pdf",
                        path = "test/path/test_file.pdf",
                        upload_type = "test_upload_type",
                    },
                });
            _clientProvider.Handler.ForRequest(req => req.RequestUri.AbsolutePath == "/graph/api/graphql",
                new { data = new { workflowSubmission = new { submissionIds = new[] { 123 } } } });
            var sut = _clientProvider.SubmissionsClient;

            var result = (await sut.CreateAsync(1, new Stream[] { new MemoryStream() })).ToArray();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(123, result.Single());
            Assert.Inconclusive("TODO: Check all properties");
        }


        [TestMethod]
        public void Create_ShouldUploadFiles() => throw new NotImplementedException();


        [TestMethod]
        public async Task Get_ShouldGetSubmissions()
        {
            const int submissionId = 1,
                jobId = 2;
            var sut = _clientProvider.SubmissionsClient;
            _clientProvider.Handler.ForRequest(req =>
                    req.RequestUri.AbsolutePath == "/graph/api/graphql"
                    && req.Content
                .ReadAsStringAsync().Result.Equals(
                    "{\"query\":\"\\n                    query GetSubmission($submissionId: Int!){\\n                        submission(id: $submissionId){\\n                            id\\n                            datasetId\\n                            workflowId\\n                            status\\n                            inputFile\\n                            inputFilename\\n                            resultFile\\n                            retrieved\\n                        }\\n                    }\\n                \",\"operationName\":\"GetSubmission\",\"variables\":{\"submissionId\":1}}"
                ),
                new { data = new { submission = new
                {
                    id = submissionId,
                    datasetId = 12,
                    workflowId = 13,
                    status = "COMPLETE",
                    //inputFile = // string
                    //inputFilename = // string
                    //resultFile // string
                    retrieved = false,
                    errors = (string)null, // string
                } }});
            _clientProvider.Handler.ForRequest(
                req => req.Content.ReadAsStringAsync().Result.Equals(
                    "{\"query\":\"\\n                    mutation CreateSubmissionResults($submissionId: Int!) {\\n                        submissionResults(submissionId: $submissionId) {\\n                            jobId\\n                        }\\n                    }\\n                \",\"operationName\":\"CreateSubmissionResults\",\"variables\":{\"submissionId\":1}}"),
                    new {data = new { submissionResults = new { jobId = jobId}}}
                );

            var result = await sut.GetJobAsync(submissionId);

            Assert.AreEqual(jobId, result.Id);
            Assert.Inconclusive("TODO: Check other properties");
        }
    }
}
