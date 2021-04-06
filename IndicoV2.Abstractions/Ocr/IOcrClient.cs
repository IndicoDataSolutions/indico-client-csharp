using System;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Ocr.Models;

namespace IndicoV2.Ocr
{
    public interface IOcrClient
    {
        /// <summary>
        /// Performs OCR on PDF, TIF, JPG and PNG files.
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="filePath">Path to the document</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <returns>Job's Is</returns>
        Task<string> ExtractDocumentAsync(string filePath, DocumentExtractionPreset preset = DocumentExtractionPreset.Standard, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets result of the OCR
        /// </summary>
        /// <param name="documentUri">Url of the document (returned in Job's result).</param>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <returns>Content of the document</returns>
        Task<string> GetExtractionResultAsync(Uri documentUri, CancellationToken cancellationToken);

        /// <inheritdoc cref="GetExtractionResultAsync"/>
        /// <param name="cancellationToken"><c><see cref="CancellationToken"/></c> for handling cancellation of asynchronous operations.</param>
        /// <typeparam name="TResult">OCR result model.</typeparam>
        Task<TResult> GetExtractionResultAsync<TResult>(Uri documentUri, CancellationToken cancellationToken);
    }
}
