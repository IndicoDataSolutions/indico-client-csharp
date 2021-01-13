using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Indico.Storage
{
    internal class MultipartFormUpload : RestRequest<JArray>
    {
        private readonly IndicoClient _client;
        public List<FileParameter> FileParameters { get; set; }

        public MultipartFormUpload(IndicoClient client) => _client = client;

        public async Task<JArray> Call()
        {
            string uploadUrl = _client.Config.GetAppBaseUrl() + "/storage/files/store";
            var formData = await MultipartFormDataContent(FileParameters);
            var client = _client.HttpClient;
            var responseMessage = await client.PostAsync(uploadUrl, formData);
            string body = await responseMessage.Content.ReadAsStringAsync();
            var uploadResult = JArray.Parse(body);
            return uploadResult;
        }

        private async Task<HttpContent> MultipartFormDataContent(List<FileParameter> parameters)
        {
            Stream formDataStream = new MemoryStream();
            var encoding = Encoding.UTF8;
            bool needsCLRF = false;
            string boundary = string.Format("----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + boundary;

            foreach (var fileToUpload in parameters)
            {
                // Skip it on the first parameter, add it to subsequent parameters.
                if (needsCLRF)
                {
                    await formDataStream.WriteAsync(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));
                }

                needsCLRF = true;

                string header = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                        boundary,
                        fileToUpload.FilePath,
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
    }
}
