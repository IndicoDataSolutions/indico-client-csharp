using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Indico.Exception;

namespace Indico
{
    public class IndicoConfig
    {
        public string Host { get; }
        public string Protocol { get; }
        public string ApiToken { get; }

        public IndicoConfig(
            [Optional] string apiToken,
            [Optional] string tokenPath,
            string host = "app.indico.io",
            string protocol = "https"
        )
        {
            this.Host = host;
            this.Protocol = protocol;
            if (apiToken == null)
            {
                if (tokenPath == null)
                {
                    throw new RuntimeException("apiToken or tokenPath is missing");
                }
                this.ApiToken = this.ResolveApiToken(tokenPath).Result;
            }
            else
            {
                this.ApiToken = apiToken;
            }
        }

        private async Task<string> ResolveApiToken(string path)
        {
            // This Might Not Work On Windows Since Path contains "/" Instead of "\"
            string apiToken;
            if (File.Exists($"{path}/indico_api_token.txt"))
            {
                FileStream fileStream = new FileStream($"{path}/indico_api_token.txt", FileMode.Open, FileAccess.Read);
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    apiToken = await streamReader.ReadLineAsync();
                }
                return apiToken;
            }
            else
            {
                throw new FileNotFoundException("Invalid TokenPath Provided");
            }
        }
    }
}