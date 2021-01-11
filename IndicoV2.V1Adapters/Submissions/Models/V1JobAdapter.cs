using IndicoV2.Abstractions.Submissions.Models;

namespace IndicoV2.V1Adapters.Submissions.Models
{
    public class V1JobAdapter : IJob
    {
        private readonly Indico.Jobs.Job _job;

        public V1JobAdapter(Indico.Jobs.Job job)
        {
            _job = job;
        }

        public int Id => int.Parse(_job.Id);
    }
}
