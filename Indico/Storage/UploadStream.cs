using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using Indico.Exception;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace Indico.Storage
{
    public class UploadStream : IRestRequest<JArray>
    {
        private readonly IndicoClient _client;

        /// <summary>
        /// List of streams to upload
        /// </summary>
        public List<Stream> Streams { get; set; }

        public UploadStream(IndicoClient client) => _client = client;

        /// <summary>
        /// Upload streams and return metadata
        /// </summary>
        /// <returns>JArray</returns>
        public async Task<JArray> Call(CancellationToken cancellationToken = default)
        {
            var fileParameters = new List<FileParameter>();

            foreach (var stream in Streams)
            {
                string filename = Guid.NewGuid().ToString();
                var param = new FileParameter
                {
                    File = stream,
                    FilePath = $"/tmp/{filename}",
                    FileName = filename,
                    ContentType = "application/octet-stream"
                };
                fileParameters.Add(param);
            }

            var formUpload = new MultipartFormUpload(_client)
            {
                FileParameters = fileParameters
            };
            var uploadResult = await formUpload.Call(cancellationToken);

            foreach (JObject uploadMeta in uploadResult)
            {
                string error = (string)uploadMeta.GetValue("error");
                if (error != null)
                {
                    string fname = (string)uploadMeta.GetValue("name");
                    string ferror = (string)uploadMeta.GetValue("error");
                    throw new FileUploadException($"File upload failed on {fname} with status {ferror}");
                }
            }

            return uploadResult;
        }
    }
}

