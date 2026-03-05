using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pligrimage.Web.Common.ViewModel
{
   public class UserCreateViewModel
    {
        public UserCreateViewModel()
        {
            AllRoles = new List<SelectListItem>();
            AllServices = new List<SelectListItem>();
            SelectedRoles = new string[] { };
        }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string Rank { get; set; }
        public string Unit { get; set; }

        public int UnitId { get; set; }
      //  public int HrmsPfNo { get; set; }


        public ICollection<SelectListItem> AllRoles { get; set; }
        public ICollection<SelectListItem> AllServices { get; set; }
        public string[] SelectedRoles { get; set; }
        public string[] SelectedServices { get; set; }
    }
}
