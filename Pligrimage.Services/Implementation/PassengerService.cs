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
   public  class PassengerService :Service<Passenger>,IPassengerService
    {
        public PassengerService(ITrackableRepository<Passenger> passengerRepository ) : base(passengerRepository)
        {

        }

        public Task<IEnumerable<Passenger>> TotalByYear(string year)
        {
            throw new System.NotImplementedException();
        }

        //public async Task<IEnumerable<Passenger>> GetPassByPassId(int passId)
        //{
        //    return await Repository.Queryable().Where(c => c.PassengerId == passId).ToListAsync();
        //}

      
    }
}
