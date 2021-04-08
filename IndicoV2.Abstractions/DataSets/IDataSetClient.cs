using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.DataSets.Models;
using IndicoV2.StrawberryShake.IndicoGqlClient;

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

        /// <summary>
        /// Add files to a dataset
        /// </summary>
        /// <param name="dataSetId">ID of the dataset</param>
        /// <param name="filePaths">List of pathnames to the dataset files</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <returns></returns>
        Task<IAddFilesResult> AddFilesAsync(int dataSetId, IEnumerable<string> filePaths, CancellationToken cancellationToken);

        Task<IDatasetUploadStatusResult> FileUploadStatus(int dataSetId, CancellationToken cancellationToken);
    }
}
