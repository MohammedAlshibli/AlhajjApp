
using ITS.Core.EF.Trackable;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Pligrimage.Entities.IdentityModels
{
    public partial class User : Entity
    {
        public User()
        {
            UserRoles = new List<UserRoles>();
            //UserOrgUnits = new List<UserOrgUnits>();

        }


        public int UserId { get; set; }
        public string UserName { get; set; }

        public string Rank { get; set; }

        public string FullName { get; set; }

        public bool Active { get; set; }

        public bool IsSysAdmin { get; set; }

        public int? HrmsPfNo { get; set; }

        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }

        public DateTime? LastLoginDate { get; set; }


        public  Unit unit { get; set; }
        [ForeignKey("Unit")]
        public int? MainUnitId { get; set; }


        public ICollection<UserRoles> UserRoles { get; set; }
        public ICollection<UserService>  UserServices { get; set; }

        public bool IsPermissionInUserRoles(string controllerName, string actionName)
        {
            bool _retVal = false;
            try
            {
                foreach (UserRoles userRole in this.UserRoles)
                {
                    var role = userRole.Role;
                    if (role.IsPermissionInRole(controllerName, actionName))
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
