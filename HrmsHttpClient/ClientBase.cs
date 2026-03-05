using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HrmsHttpClient
{
    public abstract class ClientBase
    {

        private readonly IApiClient _apiClient;

        protected ClientBase(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        protected async Task<HttpResponseMessage> Get(string uri)
        {
            return await _apiClient.Get(uri);
        }

        protected async Task<HttpResponseMessage> PostWithNoContent(string endPoint)
        {
            return await _apiClient.PostWithNoContent(endPoint);

        }

        protected async Task<HttpResponseMessage> PostJsonEncodedContent<TModel>(string uri, TModel model) where TModel : ApiModel
        {
            var apiResposne = await _apiClient.PostJsonEncodedContext(uri, model);
            return apiResposne;

        }
    }
}
