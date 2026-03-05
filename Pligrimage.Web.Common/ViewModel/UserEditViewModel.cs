using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pligrimage.Web.Common.ViewModel
{
    public class UserEditViewModel
    {
        public UserEditViewModel()
        {
            AllRoles = new List<SelectListItem>();
            SelectedRoles = new string[] { };
        }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public string FullName { get; set; }

        public string Rank { get; set; }

        public bool IsActive { get; set; }

        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string Serivce { get; set; }

        public int HrmsPfNo { get; set; }
        public bool IsSysAdmin { get; set; }



        public List<SelectListItem> AllRoles { get; set; }
        public string[] SelectedRoles { get; set; }


    }

}
