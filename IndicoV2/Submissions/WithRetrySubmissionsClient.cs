using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Submissions.Models;

namespace IndicoV2.Submissions
{
    public class WithRetrySubmissionsClient : SubmissionsClient
    {
        private readonly WithRetryClient _client;

        public WithRetrySubmissionsClient(WithRetryClient client) : base(client) => _client = client;

        public override async Task<IEnumerable<ISubmission>> ListAsync(IEnumerable<int> submissionIds, IEnumerable<int> workflowIds, IFilter filters, int limit = 1000, CancellationToken cancellationToken = default)
        {
            var retryCount = 0;
            var success = false;
            IEnumerable<ISubmission> result = null;

            while (!success)
            {
                try
                {
                    result = await _legacy.ListAsync(submissionIds, workflowIds, filters, limit, cancellationToken);
                    success = true;
                }
                catch (Exception)
                {
                    retryCount++;

                    if (retryCount == (_client.MaxRetries - 1))
                    {
                        throw;
                    }
                }
            }

            return result;
        }
    }
}
