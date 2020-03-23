using System.Collections.Generic;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Indico.Entity;
using Indico.Exception;
using Indico.Jobs;
using Indico.Storage;
using Newtonsoft.Json.Linq;

namespace Indico.Mutation
{
    public class DocumentExtraction : Mutation<List<Job>>
    {
        List<string> _files;
        DocumentExtractionOptions _options;
        JObject _jsonConfig;
        IndicoClient _client;

        public DocumentExtraction(IndicoClient client)
        {
            this._client = client;
            this._options = new DocumentExtractionOptions();
        }

        /// <summary>
        /// Files to extract
        /// </summary>
        /// <returns>DocumentExtraction</returns>
        /// <param name="files">Files</param>
        public DocumentExtraction Files(List<string> files)
        {
            this._files = files;
            this._jsonConfig = new JObject
            {
                { "preset_config", "simple" }
            };
            return this;
        }

        /// <summary>
        /// JSON configuration for extraction
        /// </summary>
        /// <returns>DocumentExtraction</returns>
        /// <param name="jsonConfig">JSON config</param>
        public DocumentExtraction JsonConfig(JObject jsonConfig)
        {
            this._jsonConfig = jsonConfig;
            return this;
        }

        /// <summary>
        /// Executes request and returns Jobs
        /// </summary>
        /// <returns>Job Array</returns>
        public List<Job> Execute()
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
                    mutation DocumentExtraction($files: [FileInput]!, $jsonConfig: JSONString) {
                        documentExtraction(files: $files, jsonConfig: $jsonConfig) {
                            jobIds
                        }
                    }
                ";


            GraphQLRequest request = new GraphQLRequest(query)
            {
                OperationName = "DocumentExtraction",
                Variables = new
                {
                    files,
                    JsonConfig = this._jsonConfig.ToString()
                }
            };

            GraphQLResponse response = this._client.GraphQLHttpClient.SendMutationAsync(request).Result;
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            JArray jobIds = (JArray)response.Data.documentExtraction.jobIds;
            List<Job> jobs = new List<Job>();
            foreach (string id in jobIds)
            {
                Job job = new Job(this._client.GraphQLHttpClient, id);
                jobs.Add(job);
            }

            return jobs;
        }

        JArray Upload(List<string> filePaths)
        {
            UploadFile uploadRequest = new UploadFile(this._client);
            return uploadRequest.FilePaths(filePaths).Call();
        }
    }
}