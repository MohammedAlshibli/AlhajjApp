using Pligrimage.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pligrimage.Web.Models
{
    public class DoctorNoteModel
    {
        //public int PligrimageId { get; set; }

        [Display(Name="ServiceNumber")]
        public string ServcieNumber { get; set; }
        [Display(Name = "National ID")]

        public string NationalID { get; set; }

        [Display(Name = "Full  Name")]

        public string Name { get; set; }
        [Display(Name = "Service Name")]

        public string UnitNameAr { get; set; }

        [Display(Name = "Blood Group")]
        public string BloodGroup { get; set; }
        

        [Display(Name = "Doctor Note")]

        [UIHint("TaskTextArea")]

        public string DoctorNote { get; set; }


        [UIHint("FitResultDropDownList")]

        public Parameter parameter { get; set; }



        public class Parameter
        {
            public int? ParameterId { get; set; }

        }




    }
}
