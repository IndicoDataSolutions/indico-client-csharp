using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Client.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Indico.Exception;
using Indico.Types;
using GraphQL;

namespace Indico.Jobs
{
    /// <summary>
    /// Async Job information
    /// </summary>
    public class Job
    {
        private readonly GraphQLHttpClient _graphQLHttpClient;
        
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
            _graphQLHttpClient = graphQLHttpClient;
            Id = id;
        }

        private async Task<string> FetchResult(CancellationToken cancellationToken)
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

            var request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "JobStatus",
                Variables = new
                {
                    id = Id
                }
            };

            var response = await _graphQLHttpClient.SendQueryAsync<dynamic>(request, cancellationToken);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            var job = response.Data.job;
            string status = (string)job.status;
            var jobStatus = (JobStatus)Enum.Parse(typeof(JobStatus), status);
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
        public async Task<JobStatus> Status()
        {
            string query = @"
                    query JobStatus($id: String!) {
                        job(id: $id) {
                            status
                        }
                    }
                ";
            var request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "JobStatus",
                Variables = new
                {
                    id = Id
                }
            };

            var response = await _graphQLHttpClient.SendQueryAsync<dynamic>(request);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            string status = (string)response.Data.job.status;
            var jobStatus = (JobStatus)Enum.Parse(typeof(JobStatus), status);
            return jobStatus;
        }

        /// <summary>
        /// Retrieve result. Status must be success or an error will be thrown.
        /// </summary>
        /// <returns>JSON Object</returns>
        public async Task<JObject> Result(CancellationToken cancellationToken = default)
        {
            while (await Status() == JobStatus.PENDING)
            {
                await Task.Delay(1000, cancellationToken);
            }
            string result = await FetchResult(cancellationToken);
            var json = JsonConvert.DeserializeObject<JObject>(result);
            return json;
        }

        /// <summary>
        /// Retrieve results. Status must be success or an error will be thrown.
        /// </summary>
        /// <returns>JSON Array</returns>
        public async Task<JArray> Results(CancellationToken cancellationToken = default)
        {
            while (await Status() == JobStatus.PENDING)
            {
               await Task.Delay(1000, cancellationToken);
            }
            string result = await FetchResult(cancellationToken);
            var json = JsonConvert.DeserializeObject<JArray>(result);
            return json;
        }

        /// <summary>
        /// If job status is FAILURE returns the list of errors encoutered
        /// </summary>
        /// <returns>List of errors</returns>
        public List<string> Errors() =>
            //TODO:
            new List<string>();
    }
}
