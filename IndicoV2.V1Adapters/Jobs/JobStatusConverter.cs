using System;
using IndicoV2.Jobs.Models;

namespace IndicoV2.V1Adapters.Jobs
{
    public class JobStatusConverter
    {
        public JobStatus Map(Indico.Types.JobStatus status) => (JobStatus)Enum.Parse(typeof(JobStatus), status.ToString());
    }
}
