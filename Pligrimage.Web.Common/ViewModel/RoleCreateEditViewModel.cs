using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pligrimage.Web.Common.ViewModel
{
   public  class RoleCreateEditViewModel
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }


        public ICollection<SelectListItem> AllPermissions { get; set; }
        public string[] SelectedPermissions { get; set; }
    }
}
