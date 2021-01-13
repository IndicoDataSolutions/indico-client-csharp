using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Indico.Mutation
{
    public class WorkflowSubmission : WorkflowSubmissionBase, IMutation<List<int>>
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
        protected override bool Detailed { get; set; } = false;

        public WorkflowSubmission(IndicoClient client) : base(client) { }

        public async Task<List<int>> Exec()
        {
            var response = await base.Exec();
            var ids = (JArray)response.GetValue("submissionIds");

            var submissionIds = new List<int>();
            foreach (var submissionId in ids)
            {
                submissionIds.Add((int)submissionId);
            }

            return submissionIds;
        }
    }
}
