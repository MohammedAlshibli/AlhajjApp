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
    public class BusService : Service<Buses>, IBusServcie
    {

        public BusService(ITrackableRepository<Buses> busRepository) : base(busRepository)
        {

        }

        public async Task<IEnumerable<Buses>> ByBusNumberAsync(string BusNumber)
        {
            return await Repository.Queryable().Where(c => c.BusNo == BusNumber).ToListAsync();
        }
    }
}
