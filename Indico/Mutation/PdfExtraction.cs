using System;
using System.Collections.Generic;
using System.IO;
using GraphQL.Client.Http;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using Indico.Entity;
using Indico.Exception;
using Indico.Jobs;

namespace Indico.Mutation
{
    public class PdfExtraction : Mutation<Job>
    {
        GraphQLHttpClient _graphQLHttpClient;
        List<String> _data;
        PdfExtractionOptions _options;
        JobOptions _jobOptions;

        public PdfExtraction(GraphQLHttpClient graphQLHttpClient)
        {
            this._graphQLHttpClient = graphQLHttpClient;
            this._options = new PdfExtractionOptions();
        }

        public PdfExtraction Data(List<string> data)
        {
            this._data = data;
            return this;
        }

        public PdfExtraction PdfExtractionOptions(PdfExtractionOptions options)
        {
            this._options = options;
            return this;
        }

        public PdfExtraction JobOptions(JobOptions jobOptions)
        {
            this._jobOptions = jobOptions;
            return this;
        }

        public Job Execute()
        {
            List<string> files = this.Process(this._data);
            string query = @"
                    mutation PdfExtraction($data: [String]!, $singleColumn: Boolean!, $text: Boolean!, $rawText: Boolean!, $tables: Boolean!, $metadata: Boolean!){
                        pdfExtraction(data: $data, singleColumn: $singleColumn, text: $text, rawText: $rawText, tables: $tables, metadata: $metadata){
                            jobId
                        }
                    }
                ";
            GraphQLRequest request = new GraphQLRequest(query)
            {
                OperationName = "PdfExtraction",
                Variables = new
                {
                    data = files,
                    singleColumn = this._options.SingleColumn,
                    text = this._options.Text,
                    rawText = this._options.RawText,
                    tables = this._options.Tables,
                    metadata = this._options.Metadata
                }
            };

            GraphQLResponse response = this._graphQLHttpClient.SendMutationAsync(request).Result;
            if (response.Errors != null)
            {
                throw new GraphQLException(response.Errors);
            }

            string jobId = (string)response.Data.pdfExtraction.jobId;
            Job job = new Job(this._graphQLHttpClient, jobId);
            return job;
        }

        List<string> Process(List<string> files)
        {
            List<string> pdfList = new List<string>();
            foreach (string url in files)
            {
                if (File.Exists(url))
                {
                    byte[] fileBytes = File.ReadAllBytes(url);
                    string encodedb64 = Convert.ToBase64String(fileBytes);
                    pdfList.Add(encodedb64);
                }
                else
                {
                    pdfList.Add(url);
                }
            }
            return pdfList;
        }
    }
}
