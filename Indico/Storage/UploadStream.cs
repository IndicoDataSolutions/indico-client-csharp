using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using Indico.Exception;
using System;
using System.Threading.Tasks;

namespace Indico.Storage
{
    public class UploadStream : RestRequest<JArray>
    {
        IndicoClient _client;

        /// <summary>
        /// List of streams to upload
        /// </summary>
        public List<Stream> Streams { get; set; }

        public UploadStream(IndicoClient client)
        {
            this._client = client;
        }

        /// <summary>
        /// Upload streams and return metadata
        /// </summary>
        /// <returns>JArray</returns>
        async public Task<JArray> Call()
        {
            List<FileParameter> fileParameters = new List<FileParameter>();

            foreach (Stream stream in this.Streams)
            {
                string filename = Guid.NewGuid().ToString();
                FileParameter param = new FileParameter
                {
                    File = stream,
                    FilePath = $"/tmp/{filename}",
                    FileName = filename,
                    ContentType = "application/octet-stream"
                };
                fileParameters.Add(param);
            }

            MultipartFormUpload formUpload = new MultipartFormUpload(this._client)
            {
                FileParameters = fileParameters
            };
            JArray uploadResult = await formUpload.Call();

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

