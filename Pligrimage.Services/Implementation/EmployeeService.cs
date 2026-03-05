using HrmsHttpClient;
using Pligrimage.Services.Interface;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Pligrimage.Services.Implementation
{
    public class EmployeeService : ClientBase, IEmployeeService
    {
        private const string ApiBaseUri = "https://jundbe/api/app/integration/";

        public EmployeeService(IApiClient client) : base(client)
        {

        }

        public async Task<HttpResponseMessage> GetEmployees(int pageNumber, int pageSize)
        {
            return await Get(ApiBaseUri + "people");
        }

        public async Task<HttpResponseMessage> GetEmployeeBymilitaryServiceId(string militaryServiceId)
        {
            return await Get(ApiBaseUri + $"person-by-military-service-id/{militaryServiceId}");
        }


        public async Task<HttpResponseMessage> GetEmployeeByServiceNo(int serviceNo)
        {
            return await Get(ApiBaseUri + $"person-by-service-no?serviceNumber={serviceNo}");

        }


        public async Task<HttpResponseMessage> GetEmployeeByNic(int nationalId)
        {
            return await Get(ApiBaseUri + $"person-by-nic" + nationalId);
        }

        public async Task<HttpResponseMessage> GetEmployeeInfoByOldServiceNo(string oldMilitaryServiceId)
        {
            return await Get(ApiBaseUri + $"person-by-old-military-service-id/{oldMilitaryServiceId}");
        }

        public async Task<HttpResponseMessage> GetEmployeePhoto(string militaryServiceId)
        {
            HttpResponseMessage response = await Get(ApiBaseUri + $"person-photo/{militaryServiceId}");
            return response;
        }

        public async Task<HttpResponseMessage> GetAllUnits(int pageNumber, int pageSize)
        {

            var resp = await Get(ApiBaseUri + $"units-list?PageNumber={pageNumber}&PageSize={pageSize}");
            return resp;

        }

        public async Task<HttpResponseMessage> GetAllRankList()
        {
            return await Get(ApiBaseUri + "ranks-list");
        }

        public async Task<HttpResponseMessage> GetAllUnitsById(string unitId)
        {
            return await Get(ApiBaseUri + $"unit-by-id/{unitId}");
        }

        public async Task<HttpResponseMessage> GetDriverBymilitaryServiceId(string militaryServiceId)
        {
            var driverDetails = await Get(ApiBaseUri + $"person-driver-trade-by-military-service-id/{militaryServiceId}");
            return driverDetails;
        }
    }
}
