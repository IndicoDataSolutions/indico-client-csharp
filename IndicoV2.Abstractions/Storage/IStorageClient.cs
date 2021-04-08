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
        Task<IEnumerable<IFileMetadata>> UploadAsync(IEnumerable<string> filePaths, CancellationToken cancellationToken);

        JArray Serialize(IEnumerable<IFileMetadata> filesMetadata);
    }
}
