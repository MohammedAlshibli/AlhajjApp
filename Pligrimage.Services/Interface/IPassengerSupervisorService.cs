using System.Collections.Generic;
using System.Threading.Tasks;
using ITS.Core.Abstractions.Services;
using Pligrimage.Entities;

namespace Pligrimage.Services.Interface
{
  public interface IPassengerSupervisorService : IService<PassengerSupervisors>
    {
        Task<IEnumerable<PassengerSupervisors>> ByName (string ByName);
    }
}
