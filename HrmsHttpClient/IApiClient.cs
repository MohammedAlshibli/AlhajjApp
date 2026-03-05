using System.Net.Http;
using System.Threading.Tasks;

namespace HrmsHttpClient
{
    public interface IApiClient
    {
        Task<HttpResponseMessage> Get(string endPointUri);

        Task<HttpResponseMessage> PostWithNoContent(string endPoint);

        Task<HttpResponseMessage> PostJsonEncodedContext<T>(string endPointUri, T content) where T : ApiModel;


    }
}
