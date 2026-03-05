using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ITS.Core.Abstractions.Services;
using Pligrimage.Entities;
using Pligrimage.Entities.IdentityModels;
using Pligrimage.Web.Common.ViewModel;

namespace Pligrimage.Services.Interface
{
    public interface IAdminService : IService<User>
    {
        Task<User> GetUserByUserName(string userName);

        Task<UserViewModel> GetUserByUserId(int userId);

        Task<IEnumerable<User>> GetAllUsersAsync();

        Task<User> GetUserAsync();

        Task<bool> AddUserAsync(UserCreateViewModel userCreateViewModel, string loggedUserName);
        Task<bool> UpdateUserAsync(UserViewModel userEditViewModel, string loggedUserName);


        Task<IEnumerable<PermissionVm>> GetPermissionsByUser(string userName);

        User GetUserByName(string userName);


       ICollection<int> GetUserServiceList(string userName);

        IList<int> GetUserServiceListByUnitCode(string userName);


        Task<IEnumerable<User>> GetOnlineUserList(string LoggedUserName);

        // List<int> GetUserServiceList(string userName);
    }



}
