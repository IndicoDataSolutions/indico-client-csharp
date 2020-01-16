using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL.Client.Http;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Indico.Entity;
using Indico.Exception;
using Indico.Jobs;

namespace Indico.Mutation
{
    public class ModelGroupPredict : Mutation<Job>
    {
        GraphQLHttpClient _graphQLHttpClient;
        int _id;
        List<string> _data;
        JobOptions _jobOptions;

        internal ModelGroupPredict(GraphQLHttpClient graphQLHttpClient)
        {
            this._graphQLHttpClient = graphQLHttpClient;
        }

        public ModelGroupPredict ModelGroup(ModelGroup modelGroup)
        {
            this._id = modelGroup.SelectedModel.Id;
            return this;
        }

        public ModelGroupPredict ModelId(int modelId)
        {
            this._id = modelId;
            return this;
        }

        public ModelGroupPredict Data(List<string> data)
        {
            this._data = data;
            return this;
        }

        public ModelGroupPredict JobOptions(JobOptions jobOptions)
        {
            this._jobOptions = jobOptions;
            return this;
        }

        public Job Execute()
        {
            string query = @"
                    mutation PredictModel($modelId: Int!, $data: [String]!) {
                        modelPredict(modelId: $modelId, data: $data) {
                            jobId
                        }
                    }
                ";
            GraphQLRequest request = new GraphQLRequest(query)
            {
                OperationName = "PredictModel",
                Variables = new
                {
                    modelId = this._id,
                    data = this._data
                }
            };

            GraphQLResponse response = this._graphQLHttpClient.SendMutationAsync(request).Result;
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            string jobId = (string)response.Data.modelPredict.jobId;
            Job job = new Job(this._graphQLHttpClient, jobId);
            return job;
        }
    }
}
