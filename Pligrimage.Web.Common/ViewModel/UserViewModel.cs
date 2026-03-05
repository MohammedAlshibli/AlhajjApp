using ITS.Core.EF.Trackable;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pligrimage.Web.Common.ViewModel
{
    public class UserViewModel 
    {

        public int UserId { get; set; }
        public int HrmsPfNo { get; internal set; }

        public string UserName { get; set; }


        public string FullName { get; set; }


        public string Rank { get; set; }


        public bool Active  { get; set; }
        public bool IsSysAdmin { get; set; }



        public string UnitName { get; set; }
        public int ServiceId { get; set; }



        public string Serivce { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LastLoginDate { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOn { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }

        [Display(Name = "عدالصلاحيات")]
        public int RoleCount { get; set; }

        [Display(Name = "الاسلحة")]
        public int SerivceCount { get; set; }


        


       public ICollection<UserRoleViewModel> UserRoleViewModel { get; set; }

        public List<SelectListItem> AllRoles { get; set; }
        public string[] SelectedRoles { get; set; }
      


        public List<SelectListItem> AllServices { get; set; }
        public string[] SelectedServices { get; set; }
    }
}
