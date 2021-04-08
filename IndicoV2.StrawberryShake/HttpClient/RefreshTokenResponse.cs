using System.Text.Json.Serialization;

namespace IndicoV2.StrawberryShake.HttpClient
{
    internal class RefreshTokenResponse
    {
        [JsonInclude, JsonPropertyName("auth_token")]
        public string AuthToken { get; internal set; }
    }
}
