using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Indico.Exception;
using Indico.Jobs;
using Indico.Storage;
using Newtonsoft.Json.Linq;

namespace Indico.Mutation
{
    public class WorkflowSubmission : Mutation<Job>
    {
        IndicoClient _client;
        /// <summary>
        /// Workflow Id
        /// </summary>
        /// <value>Workflow Id</value>
        public int Id { get; set; }
        /// <summary>
        /// Files to submit
        /// </summary>
        /// <value>Files</value>
        public List<string> Files { get; set; }

        public WorkflowSubmission(IndicoClient client)
        {
            this._client = client;
        }

        /// <summary>
        /// Executes request and returns Job
        /// </summary>
        /// <returns>Job</returns>
        async public Task<Job> Exec()
        {
            JArray fileMetadata;
            List<object> files = new List<object>();
            fileMetadata = await this.Upload(this.Files);
            foreach (JObject uploadMeta in fileMetadata)
            {
                JObject meta = new JObject
                {
                    { "name", (string)uploadMeta.GetValue("name") },
                    { "path", (string)uploadMeta.GetValue("path") },
                    { "upload_type", (string)uploadMeta.GetValue("upload_type") }
                };

                var file = new
                {
                    filename = (string)uploadMeta.GetValue("name"),
                    filemeta = meta.ToString()
                };

                files.Add(file);
            }

            string query = @"
                    mutation WorkflowSubmission($workflowId: Int!, $files: [FileInput]!) {
                        workflowSubmission(workflowId: $workflowId, files: $files) {
                            jobIds
                        }
                    }
                ";

            GraphQLRequest request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "WorkflowSubmission",
                Variables = new
                {
                    workflowId = this.Id,
                    files
                }
            };

            GraphQLResponse response = await this._client.GraphQLHttpClient.SendMutationAsync(request);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            string jobId = (string)response.Data.workflowSubmission.jobId;
            if (jobId == null)
            {
                throw new RuntimeException($"Failed to submit to workflow {this.Id}");
            }

            return new Job(this._client.GraphQLHttpClient, jobId);
        }

        async Task<JArray> Upload(List<string> filePaths)
        {
            UploadFile uploadRequest = new UploadFile(this._client)
            {
                Files = filePaths
            };
            return await uploadRequest.Call();
        }
    }
}
