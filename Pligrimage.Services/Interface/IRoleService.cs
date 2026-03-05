using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ITS.Core.Abstractions.Services;
using Pligrimage.Entities;
using Pligrimage.Entities.IdentityModels;
using Pligrimage.Web.Common.ViewModel;

namespace Pligrimage.Services.Interface
{
   public  interface IRoleService :IService<Role>
    {
        Task<IEnumerable<Role>> GetAllRoleAsync();



        Task<bool>  AddNewRoleAsync(RoleCreateEditViewModel roleCreateEditViewModel);

        Task<bool> RoleEditable(RoleCreateEditViewModel roleEditable);
    }
}
