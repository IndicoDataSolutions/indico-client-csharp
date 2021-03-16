using FluentAssertions;
using Indico.Entity;
using Indico.Types;
using IndicoV2.V1Adapters.Models.Models;
using NUnit.Framework;

namespace IndicoV2.V1Adapters.Tests.Models.Models
{
    public class ModelGroupAdapterTests
    {
        [TestCase(0)]
        public void Id_ShouldReturnInnerId(int innerId) =>
            new V1ModelGroupAdapter(new ModelGroup { Id = innerId }).Id.Should().Be(innerId);

        [TestCase(null)]
        [TestCase("")]
        [TestCase("test name")]
        public void Name_ShouldReturnInnerName(string name) =>
            new V1ModelGroupAdapter(new ModelGroup { Name = name }).Name.Should().Be(name);

        [Theory]
        public void Status_ShouldReturnInnerStatusMapped(ModelStatus status)
            => new V1ModelGroupAdapter(new ModelGroup { Status = status }).Status.ToString().Should()
                .Be(status.ToString());

        [TestCase(13)]
        public void Model_ShouldReturnMapOfInner(int innerId)
            => new V1ModelGroupAdapter(new ModelGroup { SelectedModel = new Model { Id = 13 } }).SelectedModel.Id.Should()
                .Be(innerId);
    }
}
