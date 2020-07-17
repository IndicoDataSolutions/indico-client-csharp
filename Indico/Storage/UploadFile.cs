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
            set
            {
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
        async public Task<JArray> Call()
        {
            string uploadUrl = this._client.Config.GetAppBaseUrl() + "/storage/files/store";
            List<FileParameter> parameters = new List<FileParameter>();

            foreach (string filepath in this.Files)
            {
                string filename = Path.GetFileName(filepath);
                parameters.Add(new FileParameter(filepath, filename, "application/octet-stream"));
            }

            HttpContent formData = await MultipartFormDataContent(parameters);
            HttpClient client = this._client.HttpClient;
            HttpResponseMessage responseMessage = await client.PostAsync(uploadUrl, formData);
            string body = await responseMessage.Content.ReadAsStringAsync();
            return JArray.Parse(body);
        }

        async Task<HttpContent> MultipartFormDataContent(List<FileParameter> parameters)
        {
            Stream formDataStream = new MemoryStream();
            Encoding encoding = Encoding.UTF8;
            bool needsCLRF = false;
            string boundary = string.Format("----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + boundary;

            foreach (FileParameter fileToUpload in parameters)
            {
                // Skip it on the first parameter, add it to subsequent parameters.
                if (needsCLRF)
                    await formDataStream.WriteAsync(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                needsCLRF = true;

                string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        fileToUpload.FilePath,
                        fileToUpload.FileName,
                        fileToUpload.ContentType);

                await formDataStream.WriteAsync(encoding.GetBytes(header), 0, encoding.GetByteCount(header));
                using (FileStream fs = File.OpenRead(fileToUpload.FilePath))
                {
                    await fs.CopyToAsync(formDataStream);
                }
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
            public string FilePath { get; }
            public string FileName { get; }
            public string ContentType { get; }
            public FileParameter(string filepath, string filename, string contenttype)
            {
                FilePath = filepath;
                FileName = Path.GetFileName(filepath);
                ContentType = contenttype;
            }
        }
    }
}
