using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.Common.Request;
using Indico.Exception;
using Indico.Storage;
using Newtonsoft.Json.Linq;

namespace Indico.Mutation
{
    /// <summary>
    /// Base for WorkflowSubmission classes.
    /// </summary>
    public class WorkflowSubmissionBase : IMutation<JObject>
    {
        private readonly IndicoClient _client;
        private int? _workflowId;

        /// <summary>
        /// Workflow Id
        /// </summary>
        /// <value>Workflow Id</value>
        public int WorkflowId
        {
            get
            {
                if (!_workflowId.HasValue)
                {
                    throw new ArgumentNullException();
                }

                return _workflowId.Value;
            }

            set => _workflowId = value;
        }

        /// <summary>
        /// Files to submit.
        /// </summary>
        public List<string> Files { get; set; }

        /// <summary>
        /// Streams to submit.
        /// </summary>
        public List<Stream> Streams { get; set; }

        /// <summary>
        /// Uris to submit.
        /// </summary>
        public List<string> Urls { get; set; }

        /// <summary>
        /// If detailed set to false, the <c>Exec</c> method returns simplified result with job and submission ids. If set to true, the result contains detailed information about submitted elements.
        /// </summary>
        protected virtual bool Detailed { get; set; }

        /// <summary>
        /// WorkflowSubmissionBase constructor.
        /// </summary>
        /// <param name="client">Client used to send API requests.</param>
        protected WorkflowSubmissionBase(IndicoClient client) => _client = client;

        /// <summary>
        /// Executes request and returns Job.
        /// </summary>
        public async Task<JObject> Exec(CancellationToken cancellationToken = default)
        {
            if (Convert.ToInt16(Files != null) + Convert.ToInt16(Streams != null) + Convert.ToInt16(Urls != null) != 1)
            {
                throw new InputException("One of 'Files', 'Streams' or 'Urls' must be specified");
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

            var query = $@"
                    mutation WorkflowSubmission($workflowId: Int!, ${arg}: {type}, $recordSubmission: Boolean) {{
                        {mutationName}(workflowId: $workflowId, {arg}: ${arg}, recordSubmission: $recordSubmission) {{
                            jobIds
                            submissionIds
                        }}
                    }}
                ";

            var queryDetailed = $@"
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

            var response = await _client.GraphQLHttpClient.SendMutationAsync(request, cancellationToken);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            JObject data = response.Data;
            return (JObject) data.GetValue(mutationName);
        }
    }
}
