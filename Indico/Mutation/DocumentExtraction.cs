using System.Collections.Generic;
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
    public class DocumentExtraction : Mutation<List<Job>>
    {
        List<string> _files;
        IndicoClient _client;

        public JObject JsonConfig { get; set; }

        /// <summary>
        /// DocumentExtraction constructor
        /// <param name="client">IndicoClient client</param>
        /// </summary>
        public DocumentExtraction(IndicoClient client)
        {
            this._client = client;
        }

        /// <summary>
        /// Files to extract
        /// <returns>DocumentExtraction</returns>
        /// <param name="files">Files to OCR</param>
        /// </summary>
        public DocumentExtraction Files(List<string> files)
        {
            this._files = files;
            this.JsonConfig = new JObject
            {
                { "preset_config", "standard" }
            };
            return this;
        }

        private JArray Upload(List<string> filePaths)
        {
            UploadFile uploadRequest = new UploadFile(this._client);
            return uploadRequest.FilePaths(filePaths).Call();
        }

        private GraphQLResponse ExecRequest()
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


            GraphQLRequest request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "DocumentExtraction",
                Variables = new
                {
                    files,
                    JsonConfig = this.JsonConfig.ToString()
                }
            };

            return this._client.GraphQLHttpClient.SendMutationAsync(request).Result;
        }

        /// <summary>
        /// Set the JSON configuration for extraction
        /// <param name="jsonConfig">JSON config</param>
        /// <returns>DocumentExtraction for calling in a chain</returns>
        /// </summary>
        public DocumentExtraction SetJsonConfig(JObject jsonConfig)
        {
            this.JsonConfig = jsonConfig;
            return this;
        }

        /// <summary>
        /// Executes OCR and returns Jobs
        /// <returns>List of Jobs</returns>
        /// </summary>
        public List<Job> Exec()
        {
            GraphQLResponse response = this.ExecRequest();
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


        /// <summary>
        /// Executes a single OCR request
        /// <param name="path">pathname of the file to OCR</param>
        /// <returns>Job</returns>
        /// </summary>
        public Job Exec(string path)
        {
            this._files = new List<string>() { path };

            GraphQLResponse response = this.ExecRequest();
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            JArray jobIds = (JArray)response.Data.documentExtraction.jobIds;
           
            return new Job(this._client.GraphQLHttpClient, (string)jobIds[0]);
        }
    }
}