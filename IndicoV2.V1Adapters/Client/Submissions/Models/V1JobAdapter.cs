using IndicoV2.Client.Submissions.Models;

namespace IndicoV2.V1Adapters.Client.Submissions.Models
{
    public class V1JobAdapter : Job
    {
        private readonly Indico.Jobs.Job _job;

        public V1JobAdapter(Indico.Jobs.Job job)
        {
            _job = job;
        }

        public override int Id => int.Parse(_job.Id);
    }
}
