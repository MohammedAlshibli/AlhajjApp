using System.Collections.Generic;
using System.Threading.Tasks;
using ITS.Core.Abstractions.Services;
using Pligrimage.Entities;

namespace Pligrimage.Services.Interface
{
  public   interface IResidenceService :IService<Residences>
    {
        Task<IEnumerable<Residences>> ByBuilding(string Building);
    }
}
