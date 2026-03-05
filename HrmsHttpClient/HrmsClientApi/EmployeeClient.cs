using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Threading.Tasks;
using HrmsHttpClient;

namespace HrmsHttpClient.HrmsClientApi
{
    public class EmployeeClient : IEmployeeClient
    {
   

        private readonly IApiClient _client;
        public EmployeeClient(IApiClient client)
        {
            _client = client;
        }

  
        public async Task<HttpResponseMessage> GetEmployeeByServiceNo(string serviceNo)
        {        
            return await _client.Get("Employees/GetEmployeeByServiceNo?snumber=" + serviceNo);
        }
         
    }
}