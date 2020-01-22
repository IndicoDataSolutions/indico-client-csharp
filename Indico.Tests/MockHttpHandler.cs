using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Indico.Tests
{
    class MockHttpHandler : DelegatingHandler
    {
        public MockHttpHandler()
        {
            this.InnerHandler = new HttpClientHandler();
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await Task.Delay(1000);
            string json = "";
            if (request.RequestUri.AbsolutePath == "/graph/api/graphql")
            {
                string httpContent = await request.Content.ReadAsStringAsync();
                JObject jsonObject = JsonConvert.DeserializeObject<JObject>(httpContent);
                string queryName = (string)jsonObject.GetValue("operationName");
                if (queryName == "ModelGroupQuery")
                {
                    json = this.GetModelGroupQuery();
                }
                else if (queryName == "LoadModel")
                {
                    json = this.GetModelGroupLoad();
                }
                else if (queryName == "PredictModel")
                {
                    json = this.GetModelGroupPredict();
                }
                else if (queryName == "PdfExtraction")
                {
                    json = this.GetPdfExtraction();
                }
                else if (queryName == "JobResult" || queryName == "JobStatus")
                {
                    json = this.GetJob();
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

        string GetModelGroupQuery()
        {
            return @"
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
        }

        string GetModelGroupLoad()
        {
            return @"
            {
                data: {
                    modelLoad: {
                        __typename: ""modelLoad"",
                        status: ""loading""
                    }
                }
            }";
        }

        string GetModelGroupPredict()
        {
            return @"
            {
                data: {
                    modelPredict: {
                        __typename: ""ModelPredict"",
                        jobId: ""jobId_test""
                    }
                }
            }";
        }

        string GetPdfExtraction()
        {
            return @"
            {
                data: {
                    pdfExtraction: {
                        __typename: ""PDFExtraction"",
                        jobId: ""jobId_test""
                    }
                }
            }";
        }

        string GetJob()
        {
            return @"
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
}
