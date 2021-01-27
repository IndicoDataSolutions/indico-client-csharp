using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Indico.Exception;
using Indico.Storage;
using Newtonsoft.Json.Linq;

namespace Indico.Mutation
{
    public class WorkflowSubmissionBase : IMutation<JObject>
    {
        private readonly IndicoClient _client;
        /// <summary>
        /// Workflow Id
        /// </summary>
        /// <value>Workflow Id</value>
        public virtual int WorkflowId { get; set; }
        /// <summary>
        /// Files to submit
        /// </summary>
        /// <value>Files</value>
        public virtual List<string> Files { get; set; }
        public virtual List<Stream> Streams { get; set; }
        public virtual List<string> Urls { get; set; }
        protected virtual bool Detailed { get; set; }

        protected WorkflowSubmissionBase(IndicoClient client) => _client = client;

        /// <summary>
        /// Executes request and returns Job
        /// </summary>
        /// <returns>Job</returns>
        public async Task<JObject> Exec(CancellationToken cancellationToken = default)
        {
            if (Files == null && Streams == null && Urls == null)
            {
                throw new InputException("One of 'Files', 'Streams' or 'Urls' must be specified");
            }
            else if (Files != null && Streams != null && Urls != null)
            {
                throw new InputException("Only one of 'Files', 'Streams' or 'Urls' must be specified");
            }

            var files = new List<object>();
            string arg, type, mutationName;

            if(Files != null || Streams != null)
            {
                arg = "files";
                type = "[FileInput]!";
                mutationName = "workflowSubmission";

                JArray fileMetadata;

                if (Files != null)
                {
                    var uploadRequest = new UploadFile(_client)
                    {
                        Files = Files
                    };
                    fileMetadata = await uploadRequest.Call();
                }
                else
                {
                    var uploadRequest = new UploadStream(_client)
                    {
                        Streams = Streams
                    };
                    fileMetadata = await uploadRequest.Call();
                }

                foreach (JObject uploadMeta in fileMetadata)
                {
                    var meta = new JObject
                    {
                        { "name", uploadMeta.Value<string>("name") },
                        { "path", uploadMeta.Value<string>("path") },
                        { "upload_type", uploadMeta.Value<string>("upload_type") }
                    };

                    var file = new
                    {
                        filename = uploadMeta.Value<string>("name"),
                        filemeta = meta.ToString()
                    };

                    files.Add(file);
                }
            }
            else
            {
                arg = "urls";
                type = "[String]!";
                mutationName = "workflowUrlSubmission";
            }

            string query = $@"
                    mutation WorkflowSubmission($workflowId: Int!, ${arg}: {type}, $recordSubmission: Boolean) {{
                        {mutationName}(workflowId: $workflowId, {arg}: ${arg}, recordSubmission: $recordSubmission) {{
                            jobIds
                            submissionIds
                        }}
                    }}
                ";

            string queryDetailed = $@"
                    mutation workflowSubmissionMutation($workflowId: Int!, ${arg}: {type}, $recordSubmission: Boolean) {{
                        {mutationName}(workflowId: $workflowId, {arg}: ${arg}, recordSubmission: $recordSubmission) {{
                            submissionIds
                            submissions {{
                                id
                                datasetId
                                workflowId
                                status
                                inputFile
                                inputFilename
                                resultFile
                                retrieved
                                errors
                            }}
                        }}
                    }}
                ";

            var request = new GraphQLRequest()
            {
                Query = Detailed ? queryDetailed : query,
                OperationName = "WorkflowSubmission",
                Variables = new
                {
                    workflowId = WorkflowId,
                    files = files,
                    urls = Urls
                }
            };

            var response = await this._client.GraphQLHttpClient.SendMutationAsync(request, cancellationToken);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            JObject data = response.Data;
            return (JObject) data.GetValue(mutationName);
        }
    }
}
