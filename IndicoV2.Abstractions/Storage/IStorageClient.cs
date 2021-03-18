using System;
using System.IO;
using System.Threading.Tasks;

namespace IndicoV2.Storage
{
    public interface IStorageClient
    {
        Task<Stream> GetAsync(Uri uri);
    }
}
