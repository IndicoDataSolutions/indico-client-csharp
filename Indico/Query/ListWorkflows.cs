using GraphQL.Common.Request;
using Indico.Entity;
using Indico.Exception;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Indico.Query
{
    /// <summary>
    /// Lists workflows.
    /// </summary>
    public class ListWorkflows : IQuery<List<Workflow>>
    {
        private readonly IndicoClient _client;

        /// <summary>
        /// Datasets ids to list workflows from.
        /// </summary>
        public List<int> DatasetIds { get; set; }

        /// <summary>
        /// Workflows ids to list.
        /// </summary>
        public List<int> WorkflowIds { get; set; }

        /// <summary>
        /// ListWorkflows constructor.
        /// </summary>
        /// <param name="client">Client used to send API requests.</param>
        public ListWorkflows(IndicoClient client) => _client = client;

        /// <summary>
        /// Executes query and returns list of workflows.
        /// </summary>
        public async Task<List<Workflow>> Exec(CancellationToken cancellationToken = default)
        {
            var query = @"
                    query ListWorkflows($datasetIds: [Int], $workflowIds:[Int]){
                        workflows(datasetIds: $datasetIds, workflowIds: $workflowIds){
                            workflows {
                                id
                                name
                                reviewEnabled
                            }
                        }
                    }
                ";
            var request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "ListWorkflows",
                Variables = new
                {
                    datasetIds = DatasetIds,
                    workflowIds = WorkflowIds
                }
            };

            var response = await _client.GraphQLHttpClient.SendQueryAsync(request);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            JArray wfs = response.Data.workflows.workflows;
            var workflows = wfs.Select(workflow => new Workflow()
            {
                Id = workflow.Value<int>("id"),
                Name = workflow.Value<string>("name"),
                ReviewEnabled = workflow.Value<bool>("reviewEnabled")
            }).ToList();

            return workflows;
        }
    }
}
