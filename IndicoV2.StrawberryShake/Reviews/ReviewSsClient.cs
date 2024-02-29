using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using IndicoV2.StrawberryShake.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using Newtonsoft.Json.Linq;

namespace IndicoV2.StrawberryShake.Reviews
{
    public class ReviewSsClient : ErrorHandlingWrapper
    {
        private readonly ServiceProvider _services;

        public ReviewSsClient(ServiceProvider services) => _services = services;

        public async Task<string> SubmitReview(int submissionId, string changes, bool rejected, bool? forceComplete, CancellationToken cancellationToken)
        {
            if (forceComplete == null)
            {
                var response = await _services.GetRequiredService<SubmitReviewMutation>().ExecuteAsync(submissionId, changes, rejected, cancellationToken);
                return response.Data.SubmitAutoReview.JobId;
            }
            else
            {
                var response = await _services.GetRequiredService<SubmitReviewForceCompleteMutation>().ExecuteAsync(submissionId, changes, rejected, forceComplete, cancellationToken);
                return response.Data.SubmitAutoReview.JobId;
            }
        }
    }
}