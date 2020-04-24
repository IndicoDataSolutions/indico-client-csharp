using System.Collections.Generic;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Indico.Exception;
using Indico.Jobs;
using Indico.Storage;
using Newtonsoft.Json.Linq;

namespace Indico.Mutation
{
    public class WorkflowSubmisson : Mutation<Job>
    {
        IndicoClient _client;
        int _id;
        List<string> _files;

        public WorkflowSubmisson(IndicoClient client)
        {
            this._client = client;
        }

        /// <summary>
        /// Files to submit
        /// </summary>
        /// <returns>WorkflowSubmission</returns>
        /// <param name="files">Files</param>
        public WorkflowSubmisson Files(List<string> files)
        {
            this._files = files;
            return this;
        }

        /// <summary>
        /// Workflow Id
        /// </summary>
        /// <returns>WorkflowSubmission</returns>
        /// <param name="id">Workflow Id</param>
        public WorkflowSubmisson WorkflowId(int id)
        {
            this._id = id;
            return this;
        }

        /// <summary>
        /// Executes request and returns Job
        /// </summary>
        /// <returns>Job</returns>
        public Job Execute()
        {
            JArray fileMetadata;
            List<object> files = new List<object>();
            fileMetadata = this.Upload(this._files);
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
                            jobId
                        }
                    }
                ";

            GraphQLRequest request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "WorkflowSubmission",
                Variables = new
                {
                    workflowId = this._id,
                    files
                }
            };

            GraphQLResponse response = this._client.GraphQLHttpClient.SendMutationAsync(request).Result;
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            string jobId = (string)response.Data.workflowSubmission.jobId;
            if (jobId == null)
            {
                throw new RuntimeException($"Failed to submit to workflow {this._id}");
            }

            return new Job(this._client.GraphQLHttpClient, jobId);
        }

        JArray Upload(List<string> filePaths)
        {
            UploadFile uploadRequest = new UploadFile(this._client);
            return uploadRequest.FilePaths(filePaths).Call();
        }
    }
}
