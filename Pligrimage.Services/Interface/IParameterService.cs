using System.Collections.Generic;
using System.Threading.Tasks;
using ITS.Core.Abstractions.Services;
using Pligrimage.Entities;


namespace Pligrimage.Services.Interface
{
   public  interface IParameterService : IService<Parameter>
    {
       IEnumerable<Parameter> GetFitCodeTypeList();
       IEnumerable<Parameter> GetClassTypeListAsync();
       IEnumerable<Parameter> GetRankCodeListAsync();

       IEnumerable<Parameter> GetRTNCodeListAsync();
        IEnumerable<Parameter> GetConfirmCodeParameter();






    }
}
