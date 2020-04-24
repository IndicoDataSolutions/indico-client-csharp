using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Indico.Exception;

namespace Indico
{
    /// <summary>
    /// Indico client configuration
    /// </summary>
    /// <remarks>
    /// Use the builder to modify the config and pass this object to the IndicoClient constructor
    /// </remarks>
    public class IndicoConfig
    {
        /// <summary>
        /// Gets the host.
        /// </summary>
        /// <value>The host.</value>
        public string Host { get; }
        /// <summary>
        /// Gets the protocol.
        /// </summary>
        /// <value>The protocol.</value>
        public string Protocol { get; }
        /// <summary>
        /// Gets the API token.
        /// </summary>
        /// <value>The API token.</value>
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
                    tokenPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
                }
                this.ApiToken = this.ResolveApiToken(tokenPath).Result;
            }
            else
            {
                this.ApiToken = apiToken;
            }
        }

        public string GetAppBaseUrl()
        {
            return this.Protocol + "://" + this.Host;
        }

        private async Task<string> ResolveApiToken(string path)
        {
            string apiToken;
            string absolutePath;
            const string ApiTokenFile = "indico_api_token.txt";

            if (path.EndsWith(ApiTokenFile, System.StringComparison.Ordinal))
            {
                absolutePath = path;
            }
            else
            {
                absolutePath = Path.Combine(path, ApiTokenFile);
            }
            
            if (File.Exists(absolutePath))
            {
                FileStream fileStream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read);
                using (StreamReader streamReader = new StreamReader(fileStream))
                {
                    apiToken = await streamReader.ReadLineAsync();
                }
                return apiToken;
            }
            throw new FileNotFoundException("Invalid TokenPath Provided");
        }
    }
}