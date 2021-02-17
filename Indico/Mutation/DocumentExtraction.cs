using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
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

        private async Task<GraphQLResponse<dynamic>> ExecRequest(CancellationToken cancellationToken = default)
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

            return await _client.GraphQLHttpClient.SendMutationAsync<dynamic>(request, cancellationToken);
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
            return jobIds.Select(id => new Job(_client.GraphQLHttpClient, (string)id)).ToList();
        }


        /// <summary>
        /// Executes a single OCR request
        /// <param name="path">pathname of the file to OCR</param>
        /// <param name="cancellationToken">Cancellation token to stop execution.</param>
        /// <returns>Job</returns>
        /// </summary>
        public async Task<Job> Exec(string path, CancellationToken cancellationToken = default)
        {
            Files = new List<string>() { path };

            var response = await ExecRequest(cancellationToken);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            var jobIds = (JArray)response.Data.documentExtraction.jobIds;

            return new Job(_client.GraphQLHttpClient, (string)jobIds[0]);
        }
    }
}