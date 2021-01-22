using System;
using System.Linq;

using FluentAssertions;

using IndicoV2.V1Adapters.Converters;

using NUnit.Framework;

using V1Status = Indico.Types.SubmissionStatus;
using V2Status = IndicoV2.Submissions.Models.SubmissionStatus;


namespace IndicoV2.V1Adapters.Tests.Converters
{
    public class SubmissionStatusConverterTests
    {
        private static object[][] _v1StatusToV2StatusMap = Enum.GetValues(typeof(V1Status))
            .OfType<V1Status>()
            .Select(s => new object[] { s, Enum.Parse<V2Status>(s.ToString()) })
            .ToArray();

        [TestCaseSource(nameof(_v1StatusToV2StatusMap))]
        public void Status_ShouldCorrectlyMapV1Status(V1Status v1Status, V2Status expectedV2Status)
        {
            // Arrange
            // Act
            var v2Status = SubmissionStatusConverters.ConvertFromLegacy(v1Status);

            // Assert
            v2Status.Should().Be(expectedV2Status);
        }

        [TestCaseSource(nameof(_v1StatusToV2StatusMap))]
        public void Status_ShouldCorrectlyMapV2Status(V1Status expectedV1Status, V2Status v2Status)
        {
            // Arrange
            // Act
            var v1Status = SubmissionStatusConverters.ConvertToLegacy(v2Status);

            // Assert
            v1Status.Should().Be(expectedV1Status);
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
