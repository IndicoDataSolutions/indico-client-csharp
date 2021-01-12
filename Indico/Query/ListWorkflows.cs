using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Indico.Entity;
using Indico.Exception;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Indico.Query
{
    public class ListWorkflows : Query<List<Workflow>>
    {
        IndicoClient _client;
        public List<int> DatasetIds { get; set; }
        public List<int> WorkflowIds { get; set; }

        public ListWorkflows(IndicoClient client) => this._client = client;

        public async Task<List<Workflow>> Exec(CancellationToken cancellationToken = default)
        {
            string query = @"
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
            GraphQLRequest request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "ListWorkflows",
                Variables = new
                {
                    datasetIds = this.DatasetIds,
                    workflowIds = this.WorkflowIds
                }
            };

            GraphQLResponse response = await this._client.GraphQLHttpClient.SendQueryAsync(request);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            JArray wfs = response.Data.workflows.workflows;
            List<Workflow> workflows = wfs.Select(workflow => new Workflow()
            {
                Id = workflow.Value<int>("id"),
                Name = workflow.Value<string>("name"),
                ReviewEnabled = workflow.Value<bool>("reviewEnabled")
            }).ToList();

            return workflows;
        }
    }
}
