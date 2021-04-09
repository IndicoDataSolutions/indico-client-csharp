using IndicoV2.Workflows;

namespace IndicoV2
{
    public static partial class IndicoClientExtensions
    {
        /// <summary>
        /// Gets <seealso cref="IWorkflowsClient"/>
        /// </summary>
        /// <param name="indicoClient">Instance of <seealso cref="IndicoClient"/></param>
        /// <returns>Instance of <seealso cref="IWorkflowsClient"/></returns>
        public static IWorkflowsClient Workflows(this IndicoClient indicoClient) => new WorkflowsClient(indicoClient);
    }
}
