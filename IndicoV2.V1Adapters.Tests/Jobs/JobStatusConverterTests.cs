using System;
using System.Linq;
using FluentAssertions;
using V2JobStatus = IndicoV2.Jobs.Models.JobStatus;
using IndicoV2.V1Adapters.Jobs;
using V1JobStatus = Indico.Types.JobStatus;
using NUnit.Framework;

namespace IndicoV2.V1Adapters.Tests.Jobs
{
    public class JobStatusConverterTests
    {
        private static readonly V1JobStatus[] _jobStatuses = Enum.GetValues(typeof(V1JobStatus)).Cast<V1JobStatus>().ToArray();

        [TestCaseSource(nameof(_jobStatuses))]
        public void ShouldConvertTestStatus(V1JobStatus v1JobStatus) =>
            new JobStatusConverter().Map(v1JobStatus).Should()
                .Be((V2JobStatus)Enum.Parse(typeof(V2JobStatus), v1JobStatus.ToString()));
    }
}
