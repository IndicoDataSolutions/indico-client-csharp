using System;
using System.Threading;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Client.Http;
using Indico.Entity;
using Indico.Exception;
using Indico.Types;
using Newtonsoft.Json.Linq;

namespace Indico.Query
{
    /// <summary>
    /// Gets a Model Group.
    /// </summary>
    public class ModelGroupQuery : IQuery<ModelGroup>
    {
        private readonly GraphQLHttpClient _graphQLHttpClient;
        private int? _mgId;

        /// <summary>
        /// Get/Set the Model Group ID.
        /// </summary>
        /// <value>Model Group ID</value>
        public int MgId 
        {
            get
            {
                if (!_mgId.HasValue)
                {
                    throw new ArgumentNullException();
                }

                return _mgId.Value;
            }

            set => _mgId = value;
        }

        /// <summary>
        /// ModelGroupQuery constructor.
        /// </summary>
        /// <param name="graphQLHttpClient">Client used to send API requests.</param>
        public ModelGroupQuery(GraphQLHttpClient graphQLHttpClient) => _graphQLHttpClient = graphQLHttpClient;

        /// <summary>
        /// Queries the server and returns ModelGroup
        /// </summary>
        public async Task<ModelGroup> Exec(CancellationToken cancellationToken = default)
        {
            var query = @"
                    query ModelGroupQuery($modelGroupIds: [Int]!) {
                        modelGroups(modelGroupIds: $modelGroupIds) {
                            modelGroups {
                                id
                                name
                                status
                                selectedModel {
                                    id
                                    status
                                }
                            }
                        }
                    }
                ";

            var request = new GraphQLRequest()
            {
                Query = query,
                OperationName = "ModelGroupQuery",
                Variables = new
                {
                    modelGroupIds = MgId
                }
            };

            var response = await _graphQLHttpClient.SendQueryAsync<dynamic>(request, cancellationToken);
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            var modelGroupList = response.Data.modelGroups.modelGroups;
            if (modelGroupList.Count == 0)
            {
                throw new RuntimeException($"Cannot find the default selected model for model group : {MgId}");
            }

            JToken mg = modelGroupList[0];
            var model = mg.Value<JToken>("selectedModel");

            return new ModelGroup()
            {
                Id = mg.Value<int>("id"),
                Name = mg.Value<string>("name"),
                Status = (ModelStatus)Enum.Parse(typeof(ModelStatus), mg.Value<string>("status")),
                SelectedModel = new Model()
                {
                    Id = model.Value<int>("id"),
                    Status = model.Value<string>("status")
                }
            };
        }
    }
}