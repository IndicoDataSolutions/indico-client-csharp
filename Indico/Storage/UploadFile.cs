using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Collections.Generic;
using System.IO;
using Indico.Exception;
using System.Net.Http.Headers;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Indico.Storage
{
    public class UploadFile : RestRequest<JArray>
    {
        IndicoClient _client;
        List<string> _files = new List<string>();

        /// <summary>
        /// List of files to upload
        /// </summary>
        public List<string> Files 
        {
            get => this._files;
            set {
                foreach (string path in value)
                {
                    if (File.Exists(path))
                    {
                        this._files.Add(path);
                    }
                    else
                    {
                        throw new RuntimeException($"File ({path}) does not exist");
                    }
                }
            }
        }

        public UploadFile(IndicoClient client)
        {
            this._client = client;
        }

        /// <summary>
        /// Upload files and return metadata
        /// </summary>
        /// <returns>JArray</returns>
        public JArray Call()
        {
            string uploadUrl = this._client.Config.GetAppBaseUrl() + "/storage/files/store";
            Dictionary<string, FileParameter> parameters = new Dictionary<string, FileParameter>();
           
            foreach (string file in this.Files)
            {
                FileStream fileStream = File.OpenRead(file);
                string fileName = Path.GetFileName(file);
                parameters.Add(file, new FileParameter(fileStream, fileName, "application/octet-stream"));
            }

            HttpContent formData = MultipartFormDataContent(parameters).Result;
            HttpClient client = this._client.HttpClient;
            HttpResponseMessage responseMessage = client.PostAsync(uploadUrl, formData).Result;
            string body = responseMessage.Content.ReadAsStringAsync().Result;

            foreach (KeyValuePair<string, FileParameter> entry in parameters)
            {
                entry.Value.Close();
            }

            return JArray.Parse(body);
        }

        async Task<HttpContent> MultipartFormDataContent(Dictionary<string, FileParameter> postParameters)
        {
            Stream formDataStream = new MemoryStream();
            Encoding encoding = Encoding.UTF8;
            bool needsCLRF = false;
            string boundary = string.Format("----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + boundary;

            foreach (var param in postParameters)
            {
                // Skip it on the first parameter, add it to subsequent parameters.
                if (needsCLRF)
                    await formDataStream.WriteAsync(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                needsCLRF = true;

                FileParameter fileToUpload = param.Value;
                string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        param.Key,
                        fileToUpload.FileName,
                        fileToUpload.ContentType);

                await formDataStream.WriteAsync(encoding.GetBytes(header), 0, encoding.GetByteCount(header));
                await fileToUpload.File.CopyToAsync(formDataStream);
            }

            // Add the end of the request.  Start with a newline
            string footer = "\r\n--" + boundary + "--\r\n";
            await formDataStream.WriteAsync(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            await formDataStream.ReadAsync(formData, 0, formData.Length);
            formDataStream.Close();

            // Add Content Headers
            HttpContent httpContent = new ByteArrayContent(formData);
            httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
            httpContent.Headers.ContentLength = formData.Length;
            return httpContent;
        }

        class FileParameter
        {
            public Stream File { get; }
            public string FileName { get; }
            public string ContentType { get; }
            public FileParameter(Stream file, string filename, string contenttype)
            {
                File = file;
                FileName = filename;
                ContentType = contenttype;
            }

            public void Close()
            {
                if (this.File != null)
                {
                    this.File.Close();
                }
            }
        }
    }
}
