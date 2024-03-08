using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System;
using IndicoV2.Exception;
using System.Linq;

namespace IndicoV2.Storage
{
    public class UploadFile : IRestRequest<JArray>
    {
        private readonly IndicoClient _client;
        private readonly List<string> _files = new List<string>();

        /// <summary>
        /// List of files to upload
        /// </summary>
        public List<string> Files
        {
            get => _files;
            set => CheckFiles(value);
        }

        public UploadFile(IndicoClient client) => _client = client;

        private void CheckFiles(List<string> files)
        {
            foreach (var path in files)
            {
                var filepath = path;
                var seperator = Path.DirectorySeparatorChar;
                var alt = Path.AltDirectorySeparatorChar;

                if (seperator != alt)
                {
                    filepath = filepath.Replace(alt, seperator);
                }

                if (File.Exists(filepath))
                {
                    _files.Add(filepath);
                }
                else
                {
                    throw new ArgumentException($"File ({path}) does not exist.");
                }
            }
        }

        /// <summary>
        /// Upload files and return metadata
        /// </summary>
        /// <returns>JArray</returns>
        public async Task<JArray> Call(CancellationToken cancellationToken = default)
        {
            var fileParameters = new List<FileParameter>();

            foreach (var filepath in Files)
            {
                var filename = Path.GetFileName(filepath);
                var file = File.OpenRead(filepath);
                var param = new FileParameter
                {
                    File = file,
                    FilePath = filepath,
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

            // Dispose FileStreams
            fileParameters.ForEach(param => param.File.Dispose());

            foreach (JObject uploadMeta in uploadResult)
            {
                var error = (string)uploadMeta.GetValue("error");
                if (error != null)
                {
                    var fname = (string)uploadMeta.GetValue("name");
                    var ferror = (string)uploadMeta.GetValue("error");

                    throw new FileUploadException($"File upload failed on {fname} with status {ferror}");
                }
            }

            return uploadResult;
        }
    }
}
