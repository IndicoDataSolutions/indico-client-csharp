using FluentAssertions;
using Indico.Entity;
using IndicoV2.V1Adapters.Workflows.Models;
using NUnit.Framework;

namespace IndicoV2.V1Adapters.Tests.Workflows.Model
{
    public class V1WorkflowAdapterTests
    {
        [Test]
        public void Id_ReturnsInnerId() => new V1WorkflowAdapter(new Workflow {Id = 1}).Id.Should().Be(1);

        [Test]
        public void Name_ReturnsInnerName() =>
            new V1WorkflowAdapter(new Workflow {Name = "test"}).Name.Should().Be("test");
    }
}
