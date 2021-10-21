using Indico.Entity;
using Indico.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Indico.Mutation
{


    [Obsolete("This is the V1 Version of the object. Please use V2 where possible.")]
    /// <summary>
    /// Submits detailed to workflow.
    /// </summary>
    public class WorkflowSubmissionDetailed : WorkflowSubmissionBase, IMutation<List<Submission>>
    {
        /// <inheritdoc/>
        protected override bool Detailed { get; set; } = true;

        /// <summary>
        /// WorkflowSubmissionDetailed constuctor.
        /// </summary>
        /// <param name="client"></param>
        public WorkflowSubmissionDetailed(IndicoClient client) : base(client) { }

        /// <summary>
        /// Executes query and returns list of submissions.
        /// </summary>
        public new async Task<List<Submission>> Exec(CancellationToken cancellationToken = default)
        {
            var response = await base.Exec(cancellationToken);
            var subs = (JArray)response.GetValue("submissions");

            return subs.Select(submission => new Submission()
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
            }).ToList();
        }
    }
}
