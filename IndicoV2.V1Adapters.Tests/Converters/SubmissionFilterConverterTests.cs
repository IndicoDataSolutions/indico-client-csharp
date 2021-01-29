using System;
using System.Collections.Generic;

using FluentAssertions;

using IndicoV2.Submissions.Models;
using IndicoV2.V1Adapters.Converters;

using NUnit.Framework;


namespace IndicoV2.V1Adapters.Tests.Converters
{
    public class SubmissionFilterConverterTests
    {
        [Test]
        public void SubmissionFilter_ShouldCorrectlyMapV1SubmissionFilter()
        {
            // Arrange
            var v2SubmissionFilter = new SubmissionFilter
            {
                InputFilename = "testvalue",
                Retrieved = true,
                Status = SubmissionStatus.COMPLETE
            };

            var v1ExpectedStatus = SubmissionStatusConverters.ConvertToLegacy(v2SubmissionFilter.Status);

            // Act
            var v1SubmissionFilter = SubmissionFilterConverters.ConvertToLegacy(v2SubmissionFilter);

            // Assert
            v1SubmissionFilter.InputFilename.Should().Be(v2SubmissionFilter.InputFilename);
            v1SubmissionFilter.Retrieved.Should().Be(v2SubmissionFilter.Retrieved);
            v1SubmissionFilter.Status.Should().Be(v1ExpectedStatus);
            v1SubmissionFilter.AND.Should().BeNull();
            v1SubmissionFilter.OR.Should().BeNull();
        }

        [Test]
        public void AndFilter_ShouldCorrectlyMapV1SubmissionFilter()
        {
            // Arrange
            GetTestSubmissionFilters(out SubmissionFilter v2SubmissionFilter1, out SubmissionFilter v2SubmissionFilter2);

            var v2AndFilter = new AndFilter
            {
                And = new List<IFilter> { v2SubmissionFilter1, v2SubmissionFilter2 }
            };

            // Act
            var v1SubmissionFilter = SubmissionFilterConverters.ConvertToLegacy(v2AndFilter);

            // Assert
            v1SubmissionFilter.AND.Should().NotBeNullOrEmpty();
            v1SubmissionFilter.AND.Should().HaveCount(2);
            v1SubmissionFilter.AND.Should().Contain(s => s.InputFilename == v2SubmissionFilter1.InputFilename);
            v1SubmissionFilter.AND.Should().Contain(s => s.InputFilename == v2SubmissionFilter2.InputFilename);
            v1SubmissionFilter.OR.Should().BeNull();
            v1SubmissionFilter.InputFilename.Should().BeNull();
            v1SubmissionFilter.Status.Should().BeNull();
            v1SubmissionFilter.Retrieved.Should().BeNull();
        }

        [Test]
        public void OrFilter_ShouldCorrectlyMapV1SubmissionFilter()
        {
            // Arrange
            GetTestSubmissionFilters(out SubmissionFilter v2SubmissionFilter1, out SubmissionFilter v2SubmissionFilter2);

            var v2OrFilter = new OrFilter
            {
                Or = new List<IFilter> { v2SubmissionFilter1, v2SubmissionFilter2 }
            };

            // Act
            var v1SubmissionFilter = SubmissionFilterConverters.ConvertToLegacy(v2OrFilter);

            // Assert
            v1SubmissionFilter.OR.Should().NotBeNullOrEmpty();
            v1SubmissionFilter.OR.Should().HaveCount(2);
            v1SubmissionFilter.OR.Should().Contain(s => s.InputFilename == v2SubmissionFilter1.InputFilename);
            v1SubmissionFilter.OR.Should().Contain(s => s.InputFilename == v2SubmissionFilter2.InputFilename);
            v1SubmissionFilter.AND.Should().BeNull();
            v1SubmissionFilter.InputFilename.Should().BeNull();
            v1SubmissionFilter.Status.Should().BeNull();
            v1SubmissionFilter.Retrieved.Should().BeNull();
        }

        [Test]
        public void EmptySubmissionFilter_ShouldBeEmptyAfterMapping()
        {
            // Arrange
            // Act
            var v1SubmissionFilter = SubmissionFilterConverters.ConvertToLegacy(new SubmissionFilter());

            // Assert
            v1SubmissionFilter.OR.Should().BeNull();
            v1SubmissionFilter.AND.Should().BeNull();
            v1SubmissionFilter.InputFilename.Should().BeNull();
            v1SubmissionFilter.Status.Should().BeNull();
            v1SubmissionFilter.Retrieved.Should().BeNull();
        }

        private static void GetTestSubmissionFilters(out SubmissionFilter v2SubmissionFilter1, out SubmissionFilter v2SubmissionFilter2)
        {
            // Arrange
            v2SubmissionFilter1 = new SubmissionFilter
            {
                InputFilename = "testvalue",
                Retrieved = true,
                Status = SubmissionStatus.COMPLETE
            };
            v2SubmissionFilter2 = new SubmissionFilter
            {
                InputFilename = "testvalue2",
                Retrieved = false,
                Status = SubmissionStatus.FAILED
            };
        }
    }
}
