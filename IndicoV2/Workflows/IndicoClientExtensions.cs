using IndicoV2.V1Adapters.Workflows;
using IndicoV2.Workflows;

namespace IndicoV2
{
    public static partial class IndicoClientExtensions
    {
        /// <summary>
        /// Gets <seealso cref="IWorkflowsClient"/>
        /// </summary>
        /// <param name="indicoClient"></param>
        /// <returns></returns>
        public static IWorkflowsClient Workflows(this IndicoClient indicoClient) => new WorkflowsV1ClientAdapter(indicoClient.LegacyClient);
    }
}
