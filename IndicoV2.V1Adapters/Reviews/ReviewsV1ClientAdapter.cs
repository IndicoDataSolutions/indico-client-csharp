﻿using System.Threading;
using System.Threading.Tasks;
using Indico;
using Indico.Mutation;
using IndicoV2.Reviews;
using Newtonsoft.Json.Linq;

namespace IndicoV2.V1Adapters.Reviews
{
    public class ReviewsV1ClientAdapter : IReviewsClient
    {
        private readonly IndicoClient _indicoClientLegacy;

        public ReviewsV1ClientAdapter(IndicoClient indicoClientLegacy) => _indicoClientLegacy = indicoClientLegacy;

        public async Task<string> SubmitReviewAsync(int submissionId, JObject changes, CancellationToken cancellationToken)
        {
            var job = await new SubmitReview(_indicoClientLegacy)
            {
                SubmissionId = submissionId,
                Changes = changes,
            }.Exec(cancellationToken);

            return job.Id;
        }

        public async Task<string> RejectAsync(int submissionId, CancellationToken cancellationToken)
        {
            var job = await new SubmitReview(_indicoClientLegacy) { Rejected = true }.Exec(cancellationToken);

            return job.Id;
        }
    }
}