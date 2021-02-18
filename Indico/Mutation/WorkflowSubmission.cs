using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace Indico.Mutation
{
    /// <summary>
    /// Submits to workflow.
    /// </summary>
    public class WorkflowSubmission : WorkflowSubmissionBase, IMutation<List<int>>
    {
        /// <inheritdoc/>
        protected override bool Detailed { get; set; } = false;

        /// <summary>
        /// WorkflowSubmission constructor.
        /// </summary>
        /// <param name="client"></param>
        public WorkflowSubmission(IndicoClient client) : base(client) { }

        /// <summary>
        /// Executes the query and returns list of submissions ids.
        /// </summary>
        public new async Task<List<int>> Exec(CancellationToken cancellationToken = default)
        {
            var response = await base.Exec(cancellationToken);
            var ids = (JArray)response.GetValue("submissionIds");

            return ids.Select(submissionId => (int)submissionId).ToList();
        }
    }
}
