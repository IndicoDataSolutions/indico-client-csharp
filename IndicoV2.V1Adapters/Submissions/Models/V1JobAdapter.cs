using IndicoV2.Submissions.Models;

namespace IndicoV2.V1Adapters.Submissions.Models
{
    internal class V1JobAdapter : IJob
    {
        private readonly Indico.Jobs.Job _job;

        public V1JobAdapter(Indico.Jobs.Job job) => _job = job;

        public int Id => int.Parse(_job.Id);
    }
}
