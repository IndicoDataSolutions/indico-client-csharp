using System;
using FluentAssertions;
using Indico.Entity;
using IndicoV2.V1Adapters.Models.Models;
using NUnit.Framework;

namespace IndicoV2.V1Adapters.Tests.Models.Models
{
    public class V1ModelAdapterTests
    {
        [Test]
        public void Constructor_ShouldThrow_WhenModelNull() =>
            this
                .Invoking(_ => new V1ModelAdapter(null))
                .Should()
                .Throw<ArgumentNullException>()
                .WithMessage("*model*");

        [TestCase(13)]
        public void Id_ShouldReturnInnerId(int innerId) =>
            new V1ModelAdapter(new Model { Id = innerId }).Id.Should().Be(innerId);

        [TestCase(null)]
        [TestCase("")]
        [TestCase("test status")]
        public void Status_ShouldReturnInnerStatus(string statusInner) =>
            new V1ModelAdapter(new Model { Status = statusInner }).Status.Should().Be(statusInner);

        [TestCase(0)]
        [TestCase(99f)]
        [TestCase(1.01f)]
        public void TrainingProgress_ShouldReturnInnerProgress(float percentCompleted)
            => new V1ModelAdapter(new Model()
            {
                TrainingProgress = new TrainingProgress() { PercentComplete = percentCompleted },
            }).TrainingProgressPercents.Should().Be(percentCompleted);

        [Test]
        public void TrainingProgress_ShouldReturn0_WhenTrainingProgressNull() =>
            new V1ModelAdapter(new Model()).TrainingProgressPercents.Should().Be(0);
    }
}
