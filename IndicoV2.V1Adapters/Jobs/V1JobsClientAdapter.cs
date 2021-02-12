using System.Threading;
using System.Threading.Tasks;
using Indico;
using Indico.Jobs;
using IndicoV2.Jobs;
using IndicoV2.Jobs.Models;
using Newtonsoft.Json.Linq;

namespace IndicoV2.V1Adapters.Jobs
{
    public class V1JobsClientAdapter : IJobsClient
    {
        private readonly IndicoClient _indicoClient;
        private readonly JobStatusConverter _jobStatusConverter;

        public V1JobsClientAdapter(IndicoClient indicoClient, JobStatusConverter jobStatusConverter)
        {
            _indicoClient = indicoClient;
            _jobStatusConverter = jobStatusConverter;
        }

        public async Task<JToken> GetResultAsync(string jobId) => await new Job(_indicoClient.GraphQLHttpClient, jobId).Result();

        public async Task<JobStatus> GetStatusAsync(string jobId, CancellationToken cancellationToken) =>
            _jobStatusConverter.Map(await new Job(_indicoClient.GraphQLHttpClient, jobId).Status());
    }
}
