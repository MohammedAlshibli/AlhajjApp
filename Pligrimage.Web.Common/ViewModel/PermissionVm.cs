using System;
using System.Collections.Generic;
using System.Text;

namespace Pligrimage.Web.Common.ViewModel
{
    public class PermissionVm
    {  public string RoleName { get; set; }
        public int PermissionId { get; set; }
        public string ControllerName { get; set; }

        public string ActionName { get; set; }
        public string Description { get; set; }
        
        public string Icon { get; set; }
    }
}
