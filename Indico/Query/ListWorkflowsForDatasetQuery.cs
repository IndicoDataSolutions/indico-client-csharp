using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Indico.Entity;
using Indico.Exception;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Indico.Query
{
    public class ListWorkflowsForDatasetQuery : Query<List<Workflow>>
    {
        IndicoClient _client;
        /// <summary>
        /// Use to query workflows by datasetId
        /// </summary>
        /// <value>Dataset Id</value>
        public int Id { get; set; }

        public ListWorkflowsForDatasetQuery(IndicoClient client)
        {
            this._client = client;

        }

        /// <summary>
        /// Queries the server and returns Workflow List
        /// </summary>
        /// <returns>Workflow List</returns>
        public List<Workflow> Exec()
        {
            string query = @"
                    query ListWorkflows($datasetId: Int){
                        workflows(datasetIds: [$datasetId]){
                            workflows {
                                id
                                name
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
                    datasetId = this.Id
                }
            };

            GraphQLResponse response = this._client.GraphQLHttpClient.SendQueryAsync(request).Result;
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            JArray wf = (JArray)response.Data.workflows.workflows;
            List<Workflow> workflows = wf.Select(workflow => new Workflow()
            {
                Id = workflow.Value<int>("id"),
                Name = workflow.Value<string>("name")
            }).ToList();

            return workflows;
        }

        public List<Workflow> Refresh(List<Workflow> obj)
        {
            //TODO:
            throw new RuntimeException("Method Not Implemented");
        }
    }
}