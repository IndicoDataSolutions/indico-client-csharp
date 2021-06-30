using System;

namespace IndicoV2
{
    public class WithRetryClient : IndicoClient
    {
        public int MaxRetries { private set; get; }

        public WithRetryClient(string apiToken, int maxRetries) : base(apiToken) => MaxRetries = maxRetries;
        
        public WithRetryClient(string apiToken, Uri baseUri, int maxRetries) : base(apiToken, baseUri) => MaxRetries = maxRetries;


    }
}
