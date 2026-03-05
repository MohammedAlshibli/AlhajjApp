using System.Collections.Generic;
using System.Threading.Tasks;
using ITS.Core.Abstractions.Services;
using Pligrimage.Entities;


namespace Pligrimage.Services.Interface
{
  public  interface IPassengerService :IService<Passenger>
    {
        Task<IEnumerable<Passenger>>TotalByYear( string year);

        //Task<Passenger> GetPassByPassId(int passId);
    }
}
