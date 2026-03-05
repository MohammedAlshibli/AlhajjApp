using System.Collections.Generic;
using System.Threading.Tasks;
using ITS.Core.Abstractions.Services;
using Pligrimage.Entities;

namespace Pligrimage.Services.Interface
{
   public  interface IUnitServcie : IService<Unit>
    {

        IEnumerable<Unit> GetByUnitName();

        IEnumerable<Unit> GetNonModUnitName();

        //IEnumerable<Unit> GetNonModUnitName();
        Task<Unit> GetUnitByIdAsync(int UnitId);


        Unit GetUnitByUnitCode(int UnitServiceCode, int EmpStatus);



    }
}
