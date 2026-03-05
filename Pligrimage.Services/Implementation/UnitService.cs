using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITS.Core.Abstractions.Services;
using ITS.Core.Abstractions.Trackable;
using ITS.Core.Services;
using Microsoft.EntityFrameworkCore;
using Pligrimage.Entities;
using Pligrimage.Services.Interface;


namespace Pligrimage.Services.Implementation
{
    public class UnitService : Service<Unit>, IUnitServcie
    {
        public UnitService(ITrackableRepository<Unit> unitRepository):base(unitRepository)
        {

        }

        public IEnumerable<Unit> GetByUnitName()
        {
            return Repository.Queryable().Where(c => c.ModFlag == true);
        }

        public IEnumerable<Unit> GetNonModUnitName()
        {
            return Repository.Queryable().Where(c => c.ModFlag == false);

        }

        public async Task<Unit> GetUnitByIdAsync(int UnitId)
        {
            return await Repository.Queryable().Where(c => c.UnitCode == UnitId).SingleOrDefaultAsync();
        }

        public Unit GetUnitByUnitCode(int UnitServiceCode, int EmpStatus)
        {
          
                return Repository.Queryable().Where(c => c.UnitCode == UnitServiceCode).FirstOrDefault();

           
        }




    }
}
