using IndicoV2.Extensions.Workflows;

namespace IndicoV2
{
    public static partial class IndicoClientExtensions
    {
        public static WorkflowAwaiter WorkflowAwaiter(this IndicoClient client) => new WorkflowAwaiter(client.Workflows());
    }
}
