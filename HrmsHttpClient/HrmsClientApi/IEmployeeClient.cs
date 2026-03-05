using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace HrmsHttpClient.HrmsClientApi
{
    public interface IEmployeeClient
    {

      

        Task<HttpResponseMessage> GetEmployeeByServiceNo(string serviceNo);
 
        
    }
}