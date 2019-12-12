using Amazon.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorAuthTest.Services.Auth
{
    public class BlazorWebAssemblyHttpClientFactory : HttpClientFactory
    {
        private HttpClient _httpClient;

        public BlazorWebAssemblyHttpClientFactory(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public override HttpClient CreateHttpClient(IClientConfig clientConfig)
        {
            return _httpClient;
        }

        public override bool UseSDKHttpClientCaching(IClientConfig clientConfig)
        {
            // return false to indicate that the SDK should not cache clients internally            
            return false;
        }
        public override bool DisposeHttpClientsAfterUse(IClientConfig clientConfig)
        {
            // return false to indicate that the SDK shouldn't dispose httpClients because they're cached in your pool            
            return false;
        }
        public override string GetConfigUniqueString(IClientConfig clientConfig)
        {
            // has no effect because UseSDKHttpClientCaching returns false
            return null;
        }
    }
}
