using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pligrimage.Services.Interface;
using Pligrimage.Web.Models;

using System.Net.Http;
using HrmsHttpClient.HrmsClientApi;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pligrimage.Web.Common.ViewModel;
using Pligrimage.Web.Extensions;
using Pligrimage.Entities.IdentityModels;
using ITS.Core.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Pligrimage.Web.Controllers
{
    public class AdminController : BaseController
    {
        public readonly IAdminService _adminService;
        private readonly IMapper _mapper;
        private readonly IEmployeeClient _employeeClient;
        private readonly IRoleService _roleService;
        private readonly IUnitServcie _unitServcie;
        private readonly IPermissionService _permissionServcie;
        public readonly IUnitOfWork _unitOfWork;



        public AdminController(IAdminService adminRepository, IMapper mapper, IEmployeeClient employeeClient, IRoleService roleService, IUnitServcie unitServcie,
            IPermissionService permissionServcie, IUnitOfWork unitOfWork)
        {
            _adminService = adminRepository;
            _mapper = mapper;
            _employeeClient = employeeClient;
            _roleService = roleService;
            _unitServcie = unitServcie;
            _permissionServcie = permissionServcie;
            _unitOfWork = unitOfWork;
        }


        #region Users
        //[PligrimageFiltter]
        public IActionResult UserIndex()
        {

            return View();

        }




        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var userList = await _adminService.GetAllUsersAsync();

                var result = _mapper.Map<IEnumerable<UserViewModel>>(userList);

                return View(result);

            }
            catch (Exception)
            {

                throw;
            }


        }

        public IActionResult CreateUser()
        {

            return View();

        }



        [HttpGet]
        public async Task<IActionResult> UserCreateation(string username)
        {
            HttpResponseMessage response = new HttpResponseMessage();


            try
            {
                response = await _employeeClient.GetEmployeeByServiceNo(username.ToUpper());

            }
            catch (Exception ex)
            {

                return BadRequest(string.Format("There is an error in web service api connection!!{0}, contact with Admin", System.Environment.NewLine));
            }

            if (!response.IsSuccessStatusCode)
            {
                var responError = (int)response.StatusCode;
                return BadRequest(string.Format("There is Error with No {0},{1} ", responError, response.StatusCode));
            };
            string json = await response.Content.ReadAsStringAsync();
            EmployeeDetailsDto _fromHrms = JsonConvert.DeserializeObject<EmployeeDetailsDto>(json);


            if (_fromHrms.SERVICE_NUMBER == null) return BadRequest("الرقم العسكري غير متوفر ");


            var isRegistered = await _adminService.GetUserByUserName(_fromHrms.SERVICE_NUMBER.Trim());
            if (isRegistered != null) return BadRequest("المستخدم مسجل");



            var roleList = await _roleService.GetAllRoleAsync();

            var allRoles = roleList.Select(r => new SelectListItem()
            {
                Text = r.Name,
                Value = r.Id.ToString()
            }).ToList();



            var serviceList = _unitServcie.Queryable()/*.Where(c => c.ModFlag == true)*/;

            var allServices = serviceList.Select(r => new SelectListItem()
            {
                Text = r.UnitNameAr,
                Value = r.UnitId.ToString()
            }).ToList();


            UserCreateViewModel userCreateViewModel = new UserCreateViewModel();
            var newuser = _mapper.Map<UserCreateViewModel>(_fromHrms);
            newuser.Unit = _unitServcie.Queryable().Where(c => c.UnitCode == int.Parse(newuser.Unit)).Select(c=>c.UnitNameAr).FirstOrDefault();
            newuser.AllRoles = allRoles;
            newuser.AllServices = allServices;

            return PartialView("_UserCreateation", newuser);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserCreateation(UserCreateViewModel userCreateViewModel )
        {
            if (ModelState.IsValid)
            {

                var isRegistered = await _adminService.GetUserByUserName(userCreateViewModel.UserName.ToUpper());
                if (isRegistered != null) return BadRequest("المستخدم مسجل");

                bool adduser = await _adminService.AddUserAsync(userCreateViewModel,LoggedUserName());

                if (adduser) return RedirectToAction(nameof(UserIndex));

                return View(userCreateViewModel);

            }

            return PartialView("_UserCreateation", userCreateViewModel);

        }


        [HttpGet]
        public async Task<IActionResult> UserEditable(int userId)
        {
          

             var _user = await _adminService.GetUserByUserId(userId);


            if (_user == null) return BadRequest("The user not registered");

            var roleList = _roleService.Queryable();

            var AllRoles = roleList.Select(r => new SelectListItem()
            {
                Text = r.Name,
                Value = r.Id.ToString()
            }).ToList();

            var serviceList = _unitServcie.Queryable()/*.Where(c => c.ModFlag == true)*/;


            var allServices = serviceList.Select(r => new SelectListItem()
            {
                Text = r.UnitNameAr,
                Value = r.UnitId.ToString()
            }).ToList();




            UserViewModel userViewModel = new UserViewModel();
            userViewModel = _user;


            userViewModel.AllRoles = AllRoles;
            userViewModel.AllServices = allServices;


            return View(userViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserEditable(UserViewModel userViewModel )
        {
            if (ModelState.IsValid)
            {
                var isRegistered = _adminService.GetUserByUserId(userViewModel.UserId);
                if (isRegistered == null) return BadRequest("المستخدم غير مسجل");
                bool updateUser = await _adminService.UpdateUserAsync(userViewModel,LoggedUserName());
                if (!updateUser) return View(userViewModel);
                return RedirectToAction("UserIndex");

            }
            return View(userViewModel);
        }

        #endregion





        #region Permission
     //[PligrimageFiltter]
        public IActionResult PermissionIndex()
        {

            return View();

        }


        public async Task<IActionResult> GetAllPermissions()
        {
            try
            {
                var permissionList = _permissionServcie.Queryable().ToList();

                var result = _mapper.Map<IEnumerable<PermissionsViewModel>>(permissionList);

                return View(result);

            }
            catch (Exception)
            {

                throw;
            }


        }



        [HttpPost]
        public ActionResult UpdatePermission(Permission permission)
        {
            if (permission != null && ModelState.IsValid)
            {
                _permissionServcie.Update(permission);
                _unitOfWork.SaveChangesAsync();
            }
            return View(permission);
        }

        [HttpPost]
        public ActionResult DeletePermission(Permission permission)
        {
            if (permission != null && ModelState.IsValid)
            {
                _permissionServcie.Delete(permission);
                _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("UserIndex", "Admin");

        }


        [HttpGet]
        public IActionResult AddEditPermission()
        {

            PermissionsCreateEditViewModel model = new PermissionsCreateEditViewModel();


            return PartialView("_AddEditPermission", model);

        }


        [HttpGet]
        public IActionResult GetController()
        {
            var controllers = AppControllersExtension.GetAppControllers();
            return View(controllers);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditPermission(PermissionsCreateEditViewModel permissionsCreateEditViewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    var isRegistered = _permissionServcie.Queryable().SingleOrDefault(x => x.ControllerName == permissionsCreateEditViewModel.ControllerName && x.ActionName == permissionsCreateEditViewModel.ActionName);


                    if (isRegistered != null) return BadRequest("was Registered ");


                    var newpermissions = new Permission();

                    //var ControllerList = AppControllersExtension.GetAppControllers(); ;

                    //var allController = ControllerList.Select(r => new SelectListItem()
                    //{
                    //    Text = r.ControllerName,
                    //    Value = r.ControllerName
                    //}).ToList();

                    newpermissions.ControllerName = permissionsCreateEditViewModel.ControllerName;
                    newpermissions.ActionName = permissionsCreateEditViewModel.ActionName;
                    newpermissions.Comment = permissionsCreateEditViewModel.Comment;
                    newpermissions.Icone = permissionsCreateEditViewModel.Icone;
                    newpermissions.ScreenNameAr = permissionsCreateEditViewModel.ScreenNameAr;
                    newpermissions.ScreenNameEn = permissionsCreateEditViewModel.ScreenNameEn;
                    newpermissions.CreatedBy = LoggedUserName();
                    newpermissions.CreatedOn = DateTime.Now;

                    _permissionServcie.Insert(newpermissions);
                    await _unitOfWork.SaveChangesAsync();
                    return RedirectToAction(nameof(PermissionIndex));

                    //   return RedirectToAction("PermissionIndex", "Admin");
                    ///    return RedirectToAction("PermissionIndex");


                }
                return RedirectToAction(nameof(PermissionIndex));
                //  return PartialView("_AddEditPermission", permissionsCreateEditViewModel);

            }
            catch (Exception)
            {

                throw;
            }





        }




        #endregion



        #region Role

       [PligrimageFiltter]
        public IActionResult RoleIndex()
        {

            return View();

        }


        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var RoleList = _roleService.Queryable().ToList();

                var result = _mapper.Map<IEnumerable<RoleViewModel>>(RoleList);
                return View(result);

            }
            catch (Exception)
            {

                throw;
            }


        }


        //[HttpGet]
        public async Task<IActionResult> AddEditRole(int roleId)
        {

            RoleCreateEditViewModel model = new RoleCreateEditViewModel();

            if (roleId > 0)
            {
                var role =await  _roleService.Queryable().Include(x => x.RolePermissions).Where(c => c.Id == roleId).SingleOrDefaultAsync();
              
                if (role != null)
                {
                    model = _mapper.Map<RoleCreateEditViewModel>(role);
                }

            }

            var permissionList = _permissionServcie.Queryable();
            var AllPermission = permissionList.Select(r => new SelectListItem()
            {
                Text =r.ScreenNameAr,           
                Value = r.PermissionId.ToString()
            }).ToList();

            model.AllPermissions = AllPermission;

            return View("_AddEditRole", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditRole(RoleCreateEditViewModel roleCreateEditViewModel)
        {
            if (ModelState.IsValid)
            {
                bool addRole = false;
                bool updateRole = false;

                bool isExist = (roleCreateEditViewModel.Id >= 1);
                if (!isExist)
                {
                    var isRegistered = _roleService.Queryable().SingleOrDefault(x => x.Name == roleCreateEditViewModel.Name);
                    if (isRegistered != null) return BadRequest("هذه الصلاحية مسجلة");
                     addRole = await _roleService.AddNewRoleAsync(roleCreateEditViewModel);
                    if (addRole) return RedirectToAction(nameof(RoleIndex));
                }
                else
                {
                    updateRole= await _roleService.RoleEditable(roleCreateEditViewModel);
                    if (updateRole) return RedirectToAction(nameof(RoleIndex));
                }

              
               

                return View(roleCreateEditViewModel);

            }

            return  View("AddEditRole", roleCreateEditViewModel);

        }








        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddEditRole(RoleCreateEditViewModel roleCreateEditViewModel)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {


        //            var isRegistered = _roleService.Queryable().SingleOrDefault(x => x.Name == roleCreateEditViewModel.Name && x.Name == roleCreateEditViewModel.Name);


        //            if (isRegistered != null) return BadRequest("was Registered ");


        //            var newRole = new Role();

        //            newRole.Name = roleCreateEditViewModel.Name;
        //            newRole.Description = roleCreateEditViewModel.Description;
        //            newRole.CreatedBy = "MOHD";
        //            newRole.CreatedOn = DateTime.Now;
        //            newRole.ModifiedBy = roleCreateEditViewModel.ModifiedBy;
        //            newRole.ModifiedOn = roleCreateEditViewModel.ModifiedOn;
        //            newRole.CreatedBy = "MOHD";
        //            newRole.CreatedOn = DateTime.Now;

        //            _roleService.Insert(newRole);
        //            _unitOfWork.SaveChangesAsync();
        //            return RedirectToAction(nameof(PermissionIndex));

        //            //   return RedirectToAction("PermissionIndex", "Admin");
        //            ///    return RedirectToAction("PermissionIndex");


        //        }
        //        return RedirectToAction(nameof(RoleIndex));
        //        //  return PartialView("_AddEditPermission", permissionsCreateEditViewModel);

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //}

        #endregion

        public async Task<IActionResult> OnlineUser()
        {
            try
            {
                return View(await _adminService.GetOnlineUserList(LoggedUserName()));
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}