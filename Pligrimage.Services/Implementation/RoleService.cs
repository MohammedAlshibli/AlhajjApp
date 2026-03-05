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
using System.IO;
using System.Text;




namespace Pligrimage.Services.Implementation
{
    public class RoleService : Service<Role>, IRoleService
    {
        public readonly IUnitOfWork _unitOfWork;
        private readonly IPermissionService _permissionServcie;

        public RoleService(ITrackableRepository<Role> RoleRepository, IUnitOfWork unitOfWork, IPermissionService permissionServcie) : base(RoleRepository)
        {
            _unitOfWork = unitOfWork;
            _permissionServcie = permissionServcie;

        }



        public async Task<IEnumerable<Role>> GetAllRoleAsync()
        {
            try
            {
                return await Repository.Queryable().ToListAsync();


            }
            catch (Exception)
            {

                throw;
            }

        }


        public async Task<bool> AddNewRoleAsync(RoleCreateEditViewModel roleCreateEditViewModel)
        {
            if (roleCreateEditViewModel == null) return false;

            try
            {
                Role newRole = new Role();
                newRole.Name = roleCreateEditViewModel.Name;
                newRole.Description = roleCreateEditViewModel.Description;
                newRole.CreatedBy = "MOHD";
                newRole.CreatedOn = DateTime.Now;
                Insert(newRole);


                var rolePermissions = new List<RolePermissions>();

                foreach (var permissions in roleCreateEditViewModel.SelectedPermissions)
                {

                    //var selectedRole = Queryable().SingleOrDefault(x => x.Id == Int32.Parse(permissions));
                    //if (selectedRole != null)
                    //{
                    var useRolePermission = new RolePermissions()
                    {
                        RoleId = newRole.Id,
                        PermissionId = Int32.Parse(permissions)
                    };
                    rolePermissions.Add(useRolePermission);
                    //}
                }
                newRole.RolePermissions = rolePermissions;

                Insert(newRole); 

                await _unitOfWork.SaveChangesAsync();

                return true;


            }
            catch (Exception)
            {

                return false;
            }
             
        }


        public async Task<bool> RoleEditable(RoleCreateEditViewModel roleEidtModel)
        {
            try
            {

                var role = await Repository.Queryable().Include(x => x.RolePermissions).Where(c => c.Id == roleEidtModel.Id).SingleOrDefaultAsync();




                List<int> permissionList = new List<int>();

                if (roleEidtModel.SelectedPermissions.Count() > 0)
                {
                    foreach (var eelectedPermissions in roleEidtModel.SelectedPermissions)
                    {

                        int selectedPermissionsId = int.Parse(eelectedPermissions.ToString());
                        permissionList.Add(selectedPermissionsId);

                        Permission permission = await _permissionServcie.FindAsync(selectedPermissionsId);
                        if (permission != null)
                        {
                            var isInpermission = role.RolePermissions.Any(r => r.PermissionId == permission.PermissionId);

                            if (!isInpermission)
                            {
                                RolePermissions rolePermissions = new RolePermissions()
                                {
                                    PermissionId = permission.PermissionId,
                                    RoleId = role.Id
                                };

                                role.RolePermissions.Add(rolePermissions);
                            }
                        }
                    }

                }



                var permissionsToBeRemoved = role.RolePermissions.Where(c => !permissionList.Contains(c.PermissionId)).ToList();

                foreach (var item in permissionsToBeRemoved)
                {
                    var x = role.RolePermissions.Remove(item);
                    await _unitOfWork.SaveChangesAsync();
                }




                Update(role);

                await _unitOfWork.SaveChangesAsync();

                return true;
            }

            catch (Exception ex)
            {

                return false;
            }

        }
    }
}
