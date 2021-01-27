using FluentAssertions;
using Indico.Entity;
using IndicoV2.V1Adapters.Submissions.Models;
using NUnit.Framework;
using V1Status = Indico.Types.SubmissionStatus;

namespace IndicoV2.V1Adapters.Tests.Submissions.Models
{
    public class V1SubmissionAdapterTests
    {
        [Theory]
        public void Status_ShouldCorrectlyMapV1Status(V1Status v1Status)
        {
            // Arrange
            var sut = new V1SubmissionAdapter(new Submission { Status = v1Status });

            // Act
            var v2Status = sut.Status;

            // Assert
            v2Status.ToString().Should().Be(v1Status.ToString());
        }
    }
}
