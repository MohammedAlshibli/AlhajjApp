using System.Collections.Generic;
using System.Threading.Tasks;
using ITS.Core.Abstractions.Services;
using Pligrimage.Entities;


namespace Pligrimage.Services.Interface
{
   public  interface ICategoryService :IService<Category>
    {
        Task<IEnumerable<Category>> GetAllCategory(string Desc);

    }

}
