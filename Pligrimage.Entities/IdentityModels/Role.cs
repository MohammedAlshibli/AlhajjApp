using ITS.Core.EF.Trackable;
using System;
using System.Collections.Generic;

namespace Pligrimage.Entities.IdentityModels
{
    public class Role : Entity
    {
        public Role()
        {
            UserRoles = new List<UserRoles>();
            RolePermissions = new List<RolePermissions>();

        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }

        public ICollection<UserRoles> UserRoles { get; set; }
        public ICollection<RolePermissions> RolePermissions { get; set; }

        public bool IsPermissionInRole(string controllerName, string actionName)
        {
            bool _retVal = false;
            try
            {
                foreach (RolePermissions rolePerm in this.RolePermissions)
                {
                    var _perm = rolePerm.Permission;
                    if (_perm.ControllerName == controllerName && _perm.ActionName == actionName)
                    {
                        _retVal = true;
                        break;
                    }
                }
            }
            catch (Exception)
            {
            }
            return _retVal;
        }

    }
}

