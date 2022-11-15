using IdentityModel.Client;

namespace ProductGrpcClientConsole
{
    internal class AuthenticationHandler: DelegatingHandler
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthenticationHandler(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string? token = await GetTokenFromIS4();

            if(token != null)
            {
                request.Headers.Add("authorization", $"Bearer {token}");
            }

            return await base.SendAsync(request, cancellationToken);
        }

        internal async Task<string?> GetTokenFromIS4()
        {
            var httpClient = _httpClientFactory.CreateClient();
            var disco = await httpClient.GetDiscoveryDocumentAsync("https://localhost:5005");

            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return null;
            }

            var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "ShoppingCartClient",
                    ClientSecret = "secret",
                    Scope = "ShoppingCartAPI"
                });

            return tokenResponse.AccessToken;
        }
    }
}
