using Pligrimage.Entities.IdentityModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pligrimage.Web.Common.ViewModel
{
   public  class RoleViewModel
    {
         

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }

        public ICollection<UserRoleViewModel> UserRoleViewModel { get; set; }
        public ICollection<RolePermissionsViewModel> RolePermissionsViewModel { get; set; }
     

    }
}

