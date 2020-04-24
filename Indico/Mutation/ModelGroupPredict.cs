using System.Collections.Generic;
using GraphQL.Client.Http;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Indico.Entity;
using Indico.Exception;
using Indico.Jobs;

namespace Indico.Mutation
{
    /// <summary>
    /// Class to run Model Group predictions
    /// </summary>
    public class ModelGroupPredict : Mutation<Job>
    {
        GraphQLHttpClient _graphQLHttpClient;
        int _id;
        List<string> _data;

        public int Id
        {
            get => this._id;
            set => this._id = value;
        }

        /// <summary>
        /// ModelGroupPredict constructor
        /// </summary>
        /// <param name="graphQLHttpClient"></param>
        public ModelGroupPredict(GraphQLHttpClient graphQLHttpClient)
        {
            this._graphQLHttpClient = graphQLHttpClient;
        }

        /// <summary>
        /// Use to predict ModelGroup
        /// </summary>
        /// <returns>ModelGroupPredict</returns>
        /// <param name="modelGroup">Model group.</param>
        public ModelGroupPredict ModelGroup(ModelGroup modelGroup)
        {
            this._id = modelGroup.SelectedModel.Id;
            return this;
        }

        /// <summary>
        /// Use to predict ModelGroup by id
        /// </summary>
        /// <returns>ModelGroupPredict</returns>
        /// <param name="modelId">Model identifier.</param>
        public ModelGroupPredict SetId(int modelId)
        {
            this._id = modelId;
            return this;
        }

        /// <summary>
        /// Data to predict
        /// </summary>
        /// <returns>ModelGroupPredict</returns>
        /// <param name="data">Data.</param>
        public ModelGroupPredict Data(List<string> data)
        {
            this._data = data;
            return this;
        }

        /// <summary>
        /// Executes request and returns job 
        /// </summary>
        /// <returns>Job</returns>
        public Job Exec()
        {
            string query = @"
                    mutation PredictModel($modelId: Int!, $data: [String]!) {
                        modelPredict(modelId: $modelId, data: $data) {
                            jobId
                        }
                    }
                ";

            GraphQLRequest request = new GraphQLRequest()
            {
                Query = query,
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
