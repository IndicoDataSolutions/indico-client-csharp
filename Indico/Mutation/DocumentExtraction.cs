using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Indico.Exception;
using Indico.Jobs;
using Indico.Storage;
using Newtonsoft.Json.Linq;

namespace Indico.Mutation
{
    /// <summary>
    /// OCR PDF, TIF, JPG and PNG files
    /// </summary>
    public class DocumentExtraction : IMutation<List<Job>>
    {
        private readonly IndicoClient _client;

        /// <summary>
        /// List of files to process
        /// </summary>
        public List<string> Files { get; set; }

        /// <summary>
        /// Get/Set the JSON Configuration for DocumentExtraction
        /// </summary>
        public JObject JsonConfig { get; set; }

        /// <summary>
        /// DocumentExtraction constructor
        /// <param name="client">IndicoClient client</param>
        /// </summary>
        public DocumentExtraction(IndicoClient client) => _client = client;

        private async Task<JArray> Upload(List<string> filePaths)
        {
            var uploadRequest = new UploadFile(_client) { Files = filePaths };
            var arr = await uploadRequest.Call();
            return arr;
        }

        private async Task<GraphQLResponse> ExecRequest(CancellationToken cancellationToken = default)
        {
            JArray fileMetadata;
            var files = new List<object>();
            fileMetadata = await Upload(Files);
            foreach (JObject uploadMeta in fileMetadata)
            {
                var meta = new JObject
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
                    mutation DocumentExtraction($files: [FileInput]!, $jsonConfig: JSONString) {
                        documentExtraction(files: $files, jsonConfig: $jsonConfig) {
                            jobIds
                        }
                    }
                ";


            var request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "DocumentExtraction",
                Variables = new
                {
                    files,
                    JsonConfig = JsonConfig.ToString()
                }
            };

            var response = await _client.GraphQLHttpClient.SendMutationAsync(request, cancellationToken);
            return response;
        }

        /// <summary>
        /// Executes OCR and returns Jobs
        /// <returns>List of Jobs</returns>
        /// </summary>
        public async Task<List<Job>> Exec(CancellationToken cancellationToken = default)
        {
            var response = await ExecRequest(cancellationToken);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            var jobIds = (JArray)response.Data.documentExtraction.jobIds;
            var jobs = new List<Job>();
            foreach (string id in jobIds)
            {
                var job = new Job(_client.GraphQLHttpClient, id);
                jobs.Add(job);
            }

            return jobs;
        }


        /// <summary>
        /// Executes a single OCR request
        /// <param name="path">pathname of the file to OCR</param>
        /// <returns>Job</returns>
        /// </summary>
        public async Task<Job> Exec(string path)
        {
            Files = new List<string>() { path };

            var response = await ExecRequest();
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            var jobIds = (JArray)response.Data.documentExtraction.jobIds;

            return new Job(_client.GraphQLHttpClient, (string)jobIds[0]);
        }
    }
}