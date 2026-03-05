using AutoMapper;
using ITS.Core.Abstractions;
using ITS.Core.Abstractions.Trackable;
using ITS.Core.EF;
using ITS.Core.EF.Trackable;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pligrimage.Entities;
using Pligrimage.Entities.IdentityModels;
using Pligrimage.Services.Implementation;
using Pligrimage.Services.Interface;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;


namespace Pligrimage.Web.Extensions
{
    public static class IdentityExtendedMethods4Principal
    {
   
        public static User GetUser(this IIdentity _identity)
        {
            User _retVal = null;
            try
            {
                if (_identity != null && _identity.IsAuthenticated)
                {
                    var ci = _identity as ClaimsIdentity;
                    string _userName = ci != null ? ci.FindFirstValue(ClaimTypes.Name) : null;

                    if (!string.IsNullOrEmpty(_userName))
                    {
                        _retVal = GetUserModel(_userName);

                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _retVal;
        }


        public static string GetFullName(this IIdentity identity)
        {
            if (identity != null && identity.IsAuthenticated)
            {
                var fullNameClaim = ((ClaimsIdentity)identity).FindFirst("FullName");

                return (fullNameClaim != null) ? fullNameClaim.Value : string.Empty;
            }
            return string.Empty;
        }


        public static int GetIntUserId(this IIdentity _identity)
        {
            int _retVal = 0;
            try
            {
                if (_identity != null && _identity.IsAuthenticated)
                {
                    var ci = _identity as ClaimsIdentity;
                    string userName = ci != null ? ci.FindFirstValue(ClaimTypes.Name) : null;

                    if (!string.IsNullOrEmpty(userName))
                    {
                        int userId = GetUserModel(userName).UserId;
                        _retVal = userId;//int.Parse(_authenticatedUser.UserId);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _retVal;
        }



        public static bool HasPermission(this IPrincipal _principal, string controllerName, string actionName)
        {
            bool _retVal = false;
            try
            {

                if (_principal != null && _principal.Identity != null && _principal.Identity.IsAuthenticated)
                {
                    var ci = _principal.Identity as ClaimsIdentity;
                    string _userId = ci != null ? ci.FindFirstValue(ClaimTypes.Name) : null;

                    if (!string.IsNullOrEmpty(_userId))
                    {

                        User _authenticatedUser = GetUserModel(_userId);
                        _retVal = _authenticatedUser.IsPermissionInUserRoles(controllerName, actionName);

                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return _retVal;
        }




        public static bool IsSysAdmin(this System.Security.Principal.IPrincipal _principal)
        {
            bool _retVal = false;
            try
            {
                if (_principal != null && _principal.Identity != null && _principal.Identity.IsAuthenticated)
                {
                    var ci = _principal.Identity as ClaimsIdentity;
                    string _userId = ci != null ? ci.FindFirstValue(ClaimTypes.Name) : null;

                    if (!string.IsNullOrEmpty(_userId))
                    {

                        var _authenticatedUser = GetUserModel(_userId).IsSysAdmin;
                        _retVal = _authenticatedUser;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _retVal;
        }


        public static bool IsActive(this System.Security.Principal.IPrincipal _principal)
        {
            bool _retVal = false;
            try
            {
                if (_principal != null && _principal.Identity != null && _principal.Identity.IsAuthenticated)
                {
                    var ci = _principal.Identity as ClaimsIdentity;
                    string _userId = ci != null ? ci.FindFirstValue(ClaimTypes.Name) : null;

                    if (!string.IsNullOrEmpty(_userId))
                    {

                        var _authenticatedUser = GetUserModel(_userId).IsSysAdmin;
                        _retVal = _authenticatedUser;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _retVal;
        }

        private static User GetUserModel(string userName)
        {

            
            var optionBuilder = new DbContextOptionsBuilder<Data.AppDbContext>();
            optionBuilder.UseSqlServer("server=MAMITSSQDBT03; database=PligrimageDB; user id=app; password=P@ssw0rd@123;  MultipleActiveResultSets=True");
           
 

            using (var db = new Data.AppDbContext(optionBuilder.Options))
            {
                return db.Users
                                    .Include(c => c.UserRoles)
                                    .ThenInclude(c => c.Role)
                                    .ThenInclude(c => c.RolePermissions)
                                    .ThenInclude(c => c.Permission).SingleOrDefault(x => x.UserName == userName);
                 
           
                                 
            }
        }



        public static string FindFirstValue(this ClaimsIdentity identity, string claimType)
        {
            string _retVal = string.Empty;
            try
            {
                if (identity != null)
                {
                    var claim = identity.FindFirst(claimType);
                    _retVal = claim != null ? claim.Value : null;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return _retVal;
        }




    }
}


