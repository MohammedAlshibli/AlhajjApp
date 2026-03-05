using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pligrimage.Web.Models
{
    public class LoginModel
    {
        [Display(Name = "Domain")]
        //[Required(ErrorMessage = "Please enter your domain.")]
        public string Domain { get; set; }

        [Display(Name = "Username")]
        [Required(ErrorMessage = "يرجى إدخال إسم المستخدم")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [Required(ErrorMessage = "يرجى إدخال كلمة المرور")]
        public string Password { get; set; }


        [Display(Name = "FullName")]
        public string FullName { get; set; }



        public IList<SelectListItem> AvailableDomains { get; set; }

        public LoginModel()
        {
            AvailableDomains = new List<SelectListItem>();
        }


    }
}
