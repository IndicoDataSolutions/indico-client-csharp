using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.DataSets.Models;

namespace IndicoV2.DataSets
{
    public interface IDataSetClient
    {
        /// <summary>
        /// Lists <seealso cref="IDataSet"/>s
        /// </summary>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        Task<IEnumerable<IDataSet>> ListAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Lists <seealso cref="IDataSetFull"/>s
        /// </summary>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        Task<IEnumerable<IDataSetFull>> ListFullAsync(int? limit = null, CancellationToken cancellationToken = default);
    }
}
