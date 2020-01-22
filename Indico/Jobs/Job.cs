using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Client.Http;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Indico.Exception;
using Indico.Types;

namespace Indico.Jobs
{
    public class Job
    {
        GraphQLHttpClient _graphQLHttpClient;
        public string Id { get; }

        public Job(GraphQLHttpClient graphQLHttpClient, string id)
        {
            this._graphQLHttpClient = graphQLHttpClient;
            this.Id = id;
        }

        public JobStatus Status()
        {
            string query = @"
                    query JobStatus($id: String!) {
                        job(id: $id) {
                            status
                        }
                    }
                ";
            GraphQLRequest request = new GraphQLRequest(query)
            {
                OperationName = "JobStatus",
                Variables = new
                {
                    id = Id
                }
            };

            GraphQLResponse response = this._graphQLHttpClient.SendQueryAsync(request).Result;
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            string status = (string)response.Data.job.status;
            JobStatus jobStatus = (JobStatus)Enum.Parse(typeof(JobStatus), status);
            return jobStatus;
        }

        public JObject Result()
        {
            string result = this.FetchResult().Result;
            JObject json = JsonConvert.DeserializeObject<JObject>(result);
            return json;
        }

        public JArray Results()
        {
            string result = this.FetchResult().Result;
            JArray json = JsonConvert.DeserializeObject<JArray>(result);
            return json;
        }

        async Task<string> FetchResult()
        {
            string query = @"
                    query JobResult($id: String!) {
                        job(id: $id) {
                            status
                            result
                        }
                    }
                ";
            GraphQLRequest request = new GraphQLRequest(query)
            {
                OperationName = "JobResult",
                Variables = new
                {
                    id = Id
                }
            };

            GraphQLResponse response = await _graphQLHttpClient.SendQueryAsync(request);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            string status = (string)response.Data.job.status;
            JobStatus jobStatus = (JobStatus)Enum.Parse(typeof(JobStatus), status);
            if (jobStatus != JobStatus.SUCCESS)
            {
                //TODO:
                Console.WriteLine(response.Data.job.result);
                throw new RuntimeException($"Job finished with status : {status}");
            }

            return (string)response.Data.job.result;
        }

        public List<string> Errors()
        {
            //TODO:
            return new List<string>();
        }
    }
}
