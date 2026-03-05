using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pligrimage.Web.Common.ViewModel
{
    public class PermissionsViewModel
    {

        public int PermissionId { get; set; }


        [Display(Name = "Area")]
        [Required]
        public string Area { get; set; }



        [Display(Name = "Controller Name")]
        [Required]
        public string ControllerName { get; set; }


        [Display(Name = "Action Name")]
        [Required]
        public string ActionName { get; set; }


        [Display(Name = "ScreenNameEn")]
        [Required]
        public string ScreenNameEn { get; set; }


        [Display(Name = "إسم الشاشة")]
        [Required]
        public string ScreenNameAr { get; set; }



        [Display(Name = "Comment")]
        [Required]
        public string Comment { get; set; }



        public string Icone { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string CreatedBy { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }




        public ICollection<RolePermissionsViewModel> RolePermissionsViewModel { get; set; }
    }
}
