using Indico.Entity;
using Indico.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Indico.Mutation
{
    public class WorkflowSubmissionDetailed : WorkflowSubmissionBase, Mutation<List<Submission>>
    {
        /// <summary>
        /// Workflow Id
        /// </summary>
        /// <value>Workflow Id</value>
        public override int WorkflowId { get; set; }
        /// <summary>
        /// Files to submit
        /// </summary>
        /// <value>Files</value>
        public override List<string> Files { get; set; }
        public override List<Stream> Streams { get; set; }
        public override List<string> Urls { get; set; }
        protected override bool Detailed { get; set; } = true;

        public WorkflowSubmissionDetailed(IndicoClient client) : base(client) { }

        public async Task<List<Submission>> Exec()
        {
            JObject response = await base.Exec();
            JArray subs = (JArray)response.GetValue("submissions");

            List<Submission> submissions = new List<Submission>();
            foreach (JToken submission in subs)
                submissions.Add(new Submission()
                {
                    Id = submission.Value<int>("id"),
                    DatasetId = submission.Value<int>("datasetId"),
                    WorkflowId = submission.Value<int>("workflowId"),
                    Status = (SubmissionStatus)Enum.Parse(typeof(SubmissionStatus), submission.Value<string>("status")),
                    InputFile = submission.Value<string>("inputFile"),
                    InputFilename = submission.Value<string>("inputFilename"),
                    ResultFile = submission.Value<string>("resultFile"),
                    Retrieved = submission.Value<bool>("retrieved"),
                    Errors = submission.Value<string>("errors")
                });

            return submissions;
        }
    }
}
