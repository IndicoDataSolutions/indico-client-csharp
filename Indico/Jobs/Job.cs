using System;
using System.Collections.Generic;
using System.Threading;
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
    /// <summary>
    /// Async Job information
    /// </summary>
    public class Job
    {
        GraphQLHttpClient _graphQLHttpClient;
        
        /// <summary>
        /// The Job ID
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Job constructor
        /// </summary>
        /// <param name="graphQLHttpClient">GraphQL Client</param>
        /// <param name="id">Job id</param>
        public Job(GraphQLHttpClient graphQLHttpClient, string id)
        {
            this._graphQLHttpClient = graphQLHttpClient;
            this.Id = id;
        }

        async private Task<string> FetchResult()
        {
            string query = @"
                    query JobStatus($id: String!) {
                        job(id: $id) {
                            id
                            ready
                            status
                            result
                        }
                    }
                ";

            GraphQLRequest request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "JobStatus",
                Variables = new
                {
                    id = Id
                }
            };

            GraphQLResponse response = await this._graphQLHttpClient.SendQueryAsync(request);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            var job = response.Data.job;
            string status = (string)job.status;
            JobStatus jobStatus = (JobStatus)Enum.Parse(typeof(JobStatus), status);
            if (jobStatus != JobStatus.SUCCESS)
            {
                throw new RuntimeException($"Job finished with status : {status}");
            }

            var result = job.result;
            if (result == null)
            {
                throw new RuntimeException("Job has finished with no results");
            }

            string output = (string)result;
            return output;
        }

        /// <summary>
        /// Retrieve job status
        /// </summary>
        /// <returns>JobStatus</returns>
        async public Task<JobStatus> Status()
        {
            string query = @"
                    query JobStatus($id: String!) {
                        job(id: $id) {
                            status
                        }
                    }
                ";
            GraphQLRequest request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "JobStatus",
                Variables = new
                {
                    id = Id
                }
            };

            GraphQLResponse response = await this._graphQLHttpClient.SendQueryAsync(request);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            string status = (string)response.Data.job.status;
            JobStatus jobStatus = (JobStatus)Enum.Parse(typeof(JobStatus), status);
            return jobStatus;
        }

        /// <summary>
        /// Retrieve result. Status must be success or an error will be thrown.
        /// </summary>
        /// <returns>JSON Object</returns>
        async public Task<JObject> Result()
        {
            while (await this.Status() == JobStatus.PENDING)
            {
                Thread.Sleep(1000);
            }
            string result = await this.FetchResult();
            JObject json = JsonConvert.DeserializeObject<JObject>(result);
            return json;
        }

        /// <summary>
        /// Retrieve results. Status must be success or an error will be thrown.
        /// </summary>
        /// <returns>JSON Array</returns>
        async public Task<JArray> Results()
        {
            while (await this.Status() == JobStatus.PENDING)
            {
               Thread.Sleep(1000);
            }
            string result = await this.FetchResult();
            JArray json = JsonConvert.DeserializeObject<JArray>(result);
            return json;
        }

        /// <summary>
        /// If job status is FAILURE returns the list of errors encoutered
        /// </summary>
        /// <returns>List of errors</returns>
        public List<string> Errors()
        {
            //TODO:
            return new List<string>();
        }
    }
}
