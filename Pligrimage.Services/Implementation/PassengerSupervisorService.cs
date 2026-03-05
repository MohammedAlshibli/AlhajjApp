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
  public   class PassengerSupervisorService : Service<PassengerSupervisors>, IPassengerSupervisorService
    {

        public PassengerSupervisorService(ITrackableRepository<PassengerSupervisors> passengerSuperRepository) : base(passengerSuperRepository)
        {

        }

        public Task<IEnumerable<PassengerSupervisors>> ByName(string ByName)
        {
            throw new System.NotImplementedException();
        }
    }
}
