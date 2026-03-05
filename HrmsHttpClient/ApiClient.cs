using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace HrmsHttpClient
{
    public class ApiClient : IApiClient
    {
        //private const string ApiBaseUri = "http://10.20.19.106:8085/api/";
        private readonly HttpClient _httpClient;

        public ApiClient(HttpClient httpClient)
        {
            HttpClient authClient = new HttpClient();
            var discoveryDoc = authClient.GetDiscoveryDocumentAsync("https://jundauth").Result;

            if (discoveryDoc.IsError)
            {
                throw new System.Exception("Invalid Disco Document");
            }

            var tokenResponse = authClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoveryDoc.TokenEndpoint,
                ClientId = "ITS_Apps",
                ClientSecret = "ITS@123",
                Scope = "Jund"
            }).Result;

            _httpClient = httpClient;
            _httpClient.SetBearerToken(tokenResponse.AccessToken);

        }
        public async Task<HttpResponseMessage> Get(string endPointUri)
        {

            HttpResponseMessage apiResponse = await _httpClient.GetAsync(endPointUri);
            return apiResponse;
        }

        public async Task<HttpResponseMessage> PostWithNoContent(string endpoint)
        {
            HttpResponseMessage apiResponse = await _httpClient.PostAsync(endpoint, null);
            return apiResponse;
        }

        public async Task<HttpResponseMessage> PostJsonEncodedContext<T>(string endPointUri, T content) where T : ApiModel
        {
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            var apiResponse = await _httpClient.PostAsJsonAsync(endPointUri, content);
            return apiResponse;
        }

   
    }
}