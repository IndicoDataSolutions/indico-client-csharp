using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Indico.Tests
{
    internal class MockHttpHandler : DelegatingHandler
    {
        public MockHttpHandler() => InnerHandler = new HttpClientHandler();

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await Task.Delay(0);
            string json = "";
            if (request.RequestUri.AbsolutePath == "/graph/api/graphql")
            {
                string httpContent = await request.Content.ReadAsStringAsync();
                var jsonObject = JsonConvert.DeserializeObject<JObject>(httpContent);
                string queryName = (string)jsonObject.GetValue("operationName");

                switch (queryName)
                {
                    case "ModelGroupQuery":
                        json = GetModelGroupQuery();
                        break;
                    case "LoadModel":
                        json = GetModelGroupLoad();
                        break;
                    case "PredictModel":
                        json = GetModelGroupPredict();
                        break;
                    case "PdfExtraction":
                        json = GetPdfExtraction();
                        break;
                    case "JobResult":
                    case "JobStatus":
                        json = GetJob();
                        break;
                    default:
                        return new HttpResponseMessage()
                        {
                            StatusCode = HttpStatusCode.NotImplemented
                        };
                }

                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                    StatusCode = HttpStatusCode.OK
                };
            }

            return new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.Forbidden
            };
        }

        private string GetModelGroupQuery() => @"
            {
                data: {
                    modelGroups: {
                        __typename: ""ModelGroupPage"",
                        modelGroups: [
                            {
                                __typename: ""ModelGroup"",
                                id: 1,
                                name: ""testModelGroup"",
                                status: ""COMPLETE"",
                                selectedModel: {
                                    __typename: ""Model"",
                                    id: 1,
                                    modelInfo: ""{\""testKey\"":\""testValue\""}""
                                }
                            }
                        ]
                    }
                }
            }";

        private string GetModelGroupLoad() => @"
            {
                data: {
                    modelLoad: {
                        __typename: ""modelLoad"",
                        status: ""loading""
                    }
                }
            }";

        private string GetModelGroupPredict() => @"
            {
                data: {
                    modelPredict: {
                        __typename: ""ModelPredict"",
                        jobId: ""jobId_test""
                    }
                }
            }";

        private string GetPdfExtraction() => @"
            {
                data: {
                    pdfExtraction: {
                        __typename: ""PDFExtraction"",
                        jobId: ""jobId_test""
                    }
                }
            }";

        private string GetJob() => @"
            {
                data: {
                    job: {
                        __typename: ""Job"",
                        status: ""SUCCESS"",
                        result: ""[
                            {\""testKey\"":\""testValue\""}
                        ]""
                    }
                }
            }";
    }
}
