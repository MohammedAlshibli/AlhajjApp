using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Pligrimage.Services.Interface
{
    public interface IEmployeeService
    {
        Task<HttpResponseMessage> GetEmployees(int pageNumber, int pageSize);
        Task<HttpResponseMessage> GetEmployeeBymilitaryServiceId(string militaryServiceId);
        Task<HttpResponseMessage> GetDriverBymilitaryServiceId(string militaryServiceId);

        Task<HttpResponseMessage> GetEmployeeInfoByOldServiceNo(string militaryServiceId);
        Task<HttpResponseMessage> GetEmployeeByServiceNo(int serviceNo);
        Task<HttpResponseMessage> GetEmployeePhoto(string militaryServiceId);

        Task<HttpResponseMessage> GetEmployeeByNic(int nic);
        Task<HttpResponseMessage> GetAllUnits(int pageNumber, int pageSize);
        Task<HttpResponseMessage> GetAllUnitsById(string unitId);
        Task<HttpResponseMessage> GetAllRankList();

    } 
}
