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
    public class FlightServcie : Service<Flight>, IFlightServcie
    {

        public FlightServcie(ITrackableRepository<Flight> flightRepository) : base(flightRepository)
        {

        }
        public Task<IEnumerable<Flight>> ByFlightNo(string FlightNo)
        {
            throw new System.NotImplementedException();
        }
    }
}
