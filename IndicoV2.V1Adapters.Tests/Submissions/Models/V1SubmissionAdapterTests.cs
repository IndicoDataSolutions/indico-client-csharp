using System;
using System.Linq;
using FluentAssertions;
using Indico.Entity;
using IndicoV2.V1Adapters.Submissions.Models;
using NUnit.Framework;
using V1Status = Indico.Types.SubmissionStatus;
using V2Status = IndicoV2.Submissions.Models.SubmissionStatus;

namespace IndicoV2.V1Adapters.Tests.Submissions.Models
{
    public class V1SubmissionAdapterTests
    {
        private static object[][] _v1StatusToV2StatusMap = Enum.GetValues(typeof(V1Status))
            .OfType<V1Status>()
            .Select(s => new object[] {s, Enum.Parse<V2Status>(s.ToString())})
            .ToArray();

        [TestCaseSource(nameof(_v1StatusToV2StatusMap))]
        public void Status_ShouldCorrectlyMapV1Status(V1Status v1Status, V2Status expectedV2Status)
        {
            // Arrange
            var sut = new V1SubmissionAdapter(new Submission{ Status = v1Status });

            // Act
            var v2Status = sut.Status;

            // Assert
            v2Status.Should().Be(expectedV2Status);
        }
    }
}
