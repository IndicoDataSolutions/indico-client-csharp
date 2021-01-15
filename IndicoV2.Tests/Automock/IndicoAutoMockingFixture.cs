using AutoFixture;
using AutoFixture.AutoMoq;

namespace IndicoV2.Tests.Automock
{
    public class IndicoAutoMockingFixture : Fixture
    {
        public IndicoAutoMockingFixture()
        {
            Customize(new AutoMoqCustomization());
        }
    }
}
