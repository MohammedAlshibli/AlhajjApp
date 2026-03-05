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
    public class CategoryService : Service<Category>, ICategoryService
    {

        public CategoryService(ITrackableRepository<Category> categoryRepository):base( categoryRepository)
        {

        }

        public async Task<IEnumerable<Category>> GetAllCategory(string Desc)
        {
            return await Repository.Queryable().Where(c => c.DescArabic == Desc).ToListAsync();
        }

      


    }
}
