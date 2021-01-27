using FluentAssertions;
using IndicoV2.V1Adapters.Converters;
using NUnit.Framework;
using V1Status = Indico.Types.SubmissionStatus;
using V2Status = IndicoV2.Submissions.Models.SubmissionStatus;


namespace IndicoV2.V1Adapters.Tests.Converters
{
    public class SubmissionStatusConverterTests
    {
        [Theory]
        public void Status_ShouldCorrectlyMapV1Status(V1Status v1Status)
        {
            // Arrange
            // Act
            var v2Status = SubmissionStatusConverters.ConvertFromLegacy(v1Status);

            // Assert
            v2Status.ToString().Should().Be(v1Status.ToString());
        }

        [Theory]
        public void Status_ShouldCorrectlyMapV2Status(V2Status v2Status)
        {
            // Arrange
            // Act
            var v1Status = SubmissionStatusConverters.ConvertToLegacy(v2Status);

            // Assert
            v1Status.ToString().Should().Be(v2Status.ToString());
        }

        [Test]
        public void Status_ShouldReturnNullIfNullPassed()
        {
            // Arrange
            // Act
            var v1Status = SubmissionStatusConverters.ConvertToLegacy(null);

            // Assert
            v1Status.Should().BeNull();
        }
    }
}
