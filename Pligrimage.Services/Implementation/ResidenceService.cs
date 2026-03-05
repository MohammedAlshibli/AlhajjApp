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
   public  class ResidenceService :Service<Residences>,IResidenceService
    {
        public ResidenceService(ITrackableRepository<Residences> residenceRepository):base(residenceRepository)
        {

        }

        public Task<IEnumerable<Residences>> ByBuilding(string Building)
        {
            throw new System.NotImplementedException();
        }
    }
}
