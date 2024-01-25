using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using IndicoV2.StrawberryShake.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data;
using Newtonsoft.Json.Linq;

namespace IndicoV2.StrawberryShake.Ocr
{
    public class OcrSsClient : ErrorHandlingWrapper
    {
        private readonly ServiceProvider _services;

        public OcrSsClient(ServiceProvider services) => _services = services;

        public Task<IDocumentExtractionResult> DocumentExtraction(IReadOnlyList<FileInput> files, JObject config, CancellationToken cancellationToken) =>
            ExecuteAsync(async () => await _services
                .GetRequiredService<DocumentExtractionMutation>().ExecuteAsync(files, config.ToString(), cancellationToken));
    }
}