using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ITS.Core.Abstractions;
using ITS.Core.Abstractions.Services;
using ITS.Core.Abstractions.Trackable;
using ITS.Core.Services;
using Microsoft.EntityFrameworkCore;
using Pligrimage.Entities;
using Pligrimage.Entities.IdentityModels;
using Pligrimage.Services.Interface;
using Pligrimage.Web.Common.ViewModel;

namespace Pligrimage.Services.Implementation
{

    public class AdminService : Service<User>, IAdminService
    {


        private readonly IRoleService _roleService;
        public readonly IUnitOfWork _unitOfWork;
        private readonly IUnitServcie _unitServcie;
        private readonly IMapper _mapper;





        public AdminService(ITrackableRepository<User> UserRepository, IRoleService roleService, IUnitOfWork unitOfWork, IUnitServcie unitServcie, IMapper mapper) : base(UserRepository)
        {
            _roleService = roleService;
            _unitOfWork = unitOfWork;
            _unitServcie = unitServcie;
            _mapper = mapper;

        }

        public async Task<bool> AddUserAsync(UserCreateViewModel userCreateViewModel, string loggedUserName)
        {
            if (userCreateViewModel == null) return false;

            try
            {
                User newUser = new User();
                newUser.UserName = userCreateViewModel.UserName.ToUpper();
                newUser.FullName = userCreateViewModel.FullName;
                newUser.Rank = userCreateViewModel.Rank;
                newUser.Active = true;
                newUser.IsSysAdmin = false;
                //newUser.HrmsPfNo = userCreateViewModel.HrmsPfNo;

                newUser.CreatedOn = DateTime.Now;
                newUser.CreatedBy = loggedUserName.ToString();
                newUser.MainUnitId = userCreateViewModel.UnitId;
                Insert(newUser);
                //  Insert(newUser);

                var userRolesNew = new List<UserRoles>();
                foreach (var role in userCreateViewModel.SelectedRoles)
                {
                    var selectedRole = _roleService.Queryable().SingleOrDefault(x => x.Id == Int32.Parse(role));
                    if (selectedRole != null)
                    {
                        var useRole = new UserRoles()
                        {
                            RoleId = selectedRole.Id,
                            UserId = newUser.UserId,
                        };
                        userRolesNew.Add(useRole);
                    }
                }
                newUser.UserRoles = userRolesNew;

                Insert(newUser);



                var userServiceNew = new List<UserService>();
                foreach (var srv in userCreateViewModel.SelectedServices)
                {
                    var selectedService = _unitServcie.Queryable().SingleOrDefault(x => x.UnitId == Int32.Parse(srv));

                    if (selectedService != null)
                    {
                        var useService = new UserService()
                        {
                            ServiceId = selectedService.UnitId,
                            UserId = newUser.UserId,
                        };
                        userServiceNew.Add(useService);
                    }
                }
                newUser.UserServices = userServiceNew;

                Insert(newUser);

                await _unitOfWork.SaveChangesAsync();

                return true;


            }
            catch (Exception ex)
            {

                return false;
            }


        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                return await Repository.Queryable().Include(x => x.UserRoles).Include(v => v.UserServices).ToListAsync();


            }
            catch (Exception)
            {

                throw;
            }

        }


        public async Task<User> GetUserAsync()
        {
            return await Repository.Queryable().SingleOrDefaultAsync();

        }

        public async Task<User> GetUserByUserName(string username)
        {
            return await Repository.Queryable().Where(c => c.UserName == username).SingleOrDefaultAsync();
        }




        public async Task<UserViewModel> GetUserByUserId(int userId)
        {

            try
            {
                var user = await Repository.Queryable().Include(x => x.UserRoles).Include(v => v.UserServices).Where(c => c.UserId == userId).SingleOrDefaultAsync();

                return _mapper.Map<UserViewModel>(user);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        async Task<bool> IAdminService.UpdateUserAsync(UserViewModel userViewModel, string loggedUserName)
        {
            //if (userEditViewModel == null) return false;

            try
            {

                User user = await Repository.Queryable().Include(x => x.UserRoles).Include(v => v.UserServices).Where(c => c.UserId == userViewModel.UserId).SingleOrDefaultAsync();



                List<int> rolesList = new List<int>();

                if (userViewModel.SelectedRoles.Count() > 0)
                {
                    foreach (var selectedRole in userViewModel.SelectedRoles)
                    {

                        int selectedRoleId = int.Parse(selectedRole.ToString());
                        rolesList.Add(selectedRoleId);

                        Role role = await _roleService.FindAsync(selectedRoleId);
                        if (role != null)
                        {
                            var isInRole = user.UserRoles.Any(r => r.RoleId == role.Id);

                            if (!isInRole)
                            {
                                UserRoles userRoles = new UserRoles()
                                {
                                    UserId = user.UserId,
                                    RoleId = role.Id
                                };

                                user.UserRoles.Add(userRoles);
                            }
                        }
                    }

                }


                var rolesToBeRemoved = user.UserRoles.Where(c => !rolesList.Contains(c.RoleId)).ToList();

                foreach (var item in rolesToBeRemoved)
                {
                    var x = user.UserRoles.Remove(item);
                    await _unitOfWork.SaveChangesAsync();
                }





                List<int> serviceList = new List<int>();

                if (userViewModel.SelectedServices.Count() > 0)
                {
                    foreach (var selectedService in userViewModel.SelectedServices)
                    {

                        int selectedServiceId = int.Parse(selectedService.ToString());
                        serviceList.Add(selectedServiceId);

                        var unit = await _unitServcie.FindAsync(selectedServiceId);
                        if (unit != null)
                        {
                            var isInUnit = user.UserServices.Any(r => r.ServiceId == unit.UnitId);

                            if (!isInUnit)
                            {
                                UserService userService = new UserService()
                                {
                                    UserId = user.UserId,

                                    ServiceId = unit.UnitId
                                };

                                user.UserServices.Add(userService);


                            }
                        }
                    }

                }



                var serviceToBeRemoved = user.UserServices.Where(c => !serviceList.Contains(c.ServiceId)).ToList();

                foreach (var item in serviceToBeRemoved)
                {
                    var x = user.UserServices.Remove(item);
                    await _unitOfWork.SaveChangesAsync();
                }

                //user.Active = userViewModel.Active;
                user.ModifiedOn = DateTime.Now;
                user.ModifiedBy = loggedUserName.ToString();

                Update(user);

                await _unitOfWork.SaveChangesAsync();

                return true;
            }

            catch (Exception ex)
            {

                return false;
            }
        }


        public async Task<IEnumerable<PermissionVm>> GetPermissionsByUser(string userName)
        {

            var userPermission = await Repository.Queryable()
                        .Include(c => c.UserRoles)
                        .ThenInclude(c => c.Role)
                        .ThenInclude(c => c.RolePermissions)
                        .ThenInclude(c => c.Permission)
                        .Where(u => u.UserName == userName.ToUpper()
                        )
                        .SelectMany(u => u.UserRoles.SelectMany(ur => ur.Role.RolePermissions.Select(p => new PermissionVm
                        {
                            RoleName = p.Role.Name,
                            ControllerName = p.Permission.ControllerName,
                            ActionName = p.Permission.ActionName,
                            Icon = p.Permission.Icone,

                            Description = p.Permission.ScreenNameAr
                        }))).ToListAsync();


            return userPermission;

        }

        public User GetUserByName(string userName)
        {
            return Repository.Queryable().Where(c => c.UserName == userName).SingleOrDefault();
        }



        public ICollection<int> GetUserServiceList(string userName)
        {
            try
            {
                return Repository.Queryable().Include(c => c.UserServices).Where(c => c.UserName == userName).Select(t => t.UserServices.Select(z => z.ServiceId).ToList()).FirstOrDefault();
            }
            catch (Exception)
            {

                throw;
            }
           
        }


        public IList<int> GetUserServiceListByUnitCode(string userName)
        {
            try
            {
                return Repository.Queryable().Include(c => c.UserServices).Where(c => c.UserName == userName).Select(t => t.UserServices.Select(z => z.Unit.UnitCode).ToList()).FirstOrDefault();
            }
            catch (Exception)
            {

                throw;
            }

        }



        public async Task<IEnumerable<User>> GetOnlineUserList(string LoggedUserName)
        {

            var loggedUser = await GetLoggedUserName(LoggedUserName);
            var usersService = loggedUser.UserServices.Select(x => x.ServiceId).ToList();
            bool IsSysAdmin = loggedUser.IsSysAdmin;


            if (IsSysAdmin)
            {
                return await Repository.Queryable().Include(x => x.UserServices).ToListAsync(); ;


            }
            else
            {

                return await Repository.Queryable().Include(x => x.UserServices).ToListAsync();

            }


        }

        private async Task<User> GetLoggedUserName(string loggedUserName)
        {
            try
            {
                return await Repository.Queryable().Include(c => c.UserServices).FirstOrDefaultAsync(c => c.UserName == loggedUserName);
            }
            catch (Exception)
            {

                throw;
            }


        }


    }

}
