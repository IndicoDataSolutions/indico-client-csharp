using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using Indico.Exception;
using System.Threading.Tasks;

namespace Indico.Storage
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
            foreach (string path in files)
            {
                string filepath = path;
                char seperator = Path.DirectorySeparatorChar;
                char alt = Path.AltDirectorySeparatorChar;
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
                    throw new RuntimeException($"File ({path}) does not exist");
                }
            }
        }

        /// <summary>
        /// Upload files and return metadata
        /// </summary>
        /// <returns>JArray</returns>
        public async Task<JArray> Call()
        {
            var fileParameters = new List<FileParameter>();

            foreach (string filepath in Files)
            {
                string filename = Path.GetFileName(filepath);
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
            var uploadResult = await formUpload.Call();

            // Dispose FileStreams
            fileParameters.ForEach(param => param.File.Dispose());

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
