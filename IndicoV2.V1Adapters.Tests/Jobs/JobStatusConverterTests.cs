using System;
using System.Linq;
using FluentAssertions;
using IndicoV2.V1Adapters.Jobs;
using V1=Indico.Types;
using NUnit.Framework;

namespace IndicoV2.V1Adapters.Tests.Jobs
{
    public class JobStatusConverterTests
    {
        private static readonly V1.JobStatus[] _jobStatuses = Enum.GetValues(typeof(V1.JobStatus)).Cast<V1.JobStatus>().ToArray();

        [TestCaseSource(nameof(_jobStatuses))]
        public void ShouldConvertTestStatus(V1.JobStatus v1JobStatus) =>
            new JobStatusConverter().Map(v1JobStatus).Should().Should().NotBeNull();
    }
}
