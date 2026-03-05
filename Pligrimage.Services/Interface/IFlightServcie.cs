using System.Collections.Generic;
using System.Threading.Tasks;
using ITS.Core.Abstractions.Services;
using Pligrimage.Entities;

namespace Pligrimage.Services.Interface
{
   public  interface IFlightServcie : IService<Flight>
    {
        Task<IEnumerable<Flight>> ByFlightNo(string FlightNo);
    }
}
