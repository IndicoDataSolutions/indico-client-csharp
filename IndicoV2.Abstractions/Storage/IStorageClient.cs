using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using IndicoV2.Storage.Models;
using Newtonsoft.Json.Linq;

namespace IndicoV2.Storage
{
    public interface IStorageClient
    {
        Task<Stream> GetAsync(Uri uri, CancellationToken cancellationToken);

        [Obsolete("On this level it's preferable to use streams instead of paths")]
        Task<IEnumerable<IFileMetadata>> UploadAsync(IEnumerable<string> filePaths, CancellationToken cancellationToken);

        Task<(string Name, string Meta)[]> UploadAsync(IEnumerable<(string Path, Stream Content)> files,
            CancellationToken cancellationToken, int batchSize = 20);

        JArray Serialize(IEnumerable<IFileMetadata> filesMetadata);
    }
}
