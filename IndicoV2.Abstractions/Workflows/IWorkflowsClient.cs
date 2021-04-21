using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.DataSets.Models;
using IndicoV2.Workflows.Models;

namespace IndicoV2.Workflows
{
    public interface IWorkflowsClient
    {
        /// <summary>
        /// Lists <seealso cref="IWorkflow"/> for a given <seealso cref="IDataSet"/>
        /// </summary>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <param name="dataSetId"><see cref="IDataSet"/>'s Id></param>
        Task<IEnumerable<IWorkflow>> ListAsync(int dataSetId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists all <seealso cref="IWorkflow"/> for given <seealso cref="IDataSet"/>s
        /// </summary>
        /// <param name="dataSetIds">Array of <seealso cref="IDataSet"/> Ids</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        Task<IEnumerable<IWorkflow>> ListAsync(int[] dataSetIds, CancellationToken cancellationToken = default);
    }
}