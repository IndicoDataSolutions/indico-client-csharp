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

        public bool Verify { get; }

        /// <summary>
        /// Indico Client config constructor
        /// </summary>
        /// <param name="apiToken">The actual text of the API Token</param>
        /// <param name="tokenPath">Path to the API Token file</param>
        /// <param name="host">Indico Platform host. Defaults to app.indico.io</param>
        /// <param name="protocol">Defaults to https</param>
        public IndicoConfig(
            [Optional] string apiToken,
            [Optional] string tokenPath,
            string host = "app.indico.io",
            string protocol = "https",
            bool verify = true
        )
        {
            this.Host = host;
            this.Protocol = protocol;
            this.Verify = verify;
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

        /// <summary>
        /// Get the base URL for the Indico Platform host, including protocol
        /// </summary>
        /// <returns>base URL string</returns>
        public string GetAppBaseUrl()
        {
            return this.Protocol + "://" + this.Host;
        }
    }
}