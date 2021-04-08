using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.DataSets.Models;
using IndicoV2.StrawberryShake;

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

        /// <summary>
        /// Get the status of dataset file upload
        /// </summary>
        /// <param name="dataSetId">id of the dataset to query</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <returns></returns>
        Task<IDatasetUploadStatusResult> FileUploadStatusAsync(int dataSetId, CancellationToken cancellationToken);

        /// <summary>
        /// Process CSV associated with a dataset and add corresponding data to the dataset
        /// </summary>
        /// <param name="dataSetId">ID of the dataset</param>
        /// <param name="fileIds">IDs of the CSV datafiles to process</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <returns></returns>
        Task<IProcessCsvResult> ProcessCsvAsync(int dataSetId, IEnumerable<int> fileIds, CancellationToken cancellationToken);

        /// <summary>
        /// Process files associated with a dataset and add corresponding data to the dataset
        /// </summary>
        /// <param name="dataSetId">ID of the dataset</param>
        /// <param name="fileIds">IDs of the datafiles to process</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <returns></returns>
        Task<IProcessFilesResult> ProcessFileAsync(int dataSetId, IEnumerable<int> fileIds, CancellationToken cancellationToken);
    }
}
