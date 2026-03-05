using Microsoft.AspNetCore.Http;
using Pligrimage.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Pligrimage.Web.Models
{
    public class AlhajjPhotoViewModel
    {

        public int PligrimageId { get; set; }

        [Required]
        public string ServcieNumber { get; set; }

        [Required]
        public string NIC { get; set; }
        [Required]
        public DateTime? NICExpire { get; set; }
        [Required]
        public string Passport { get; set; }

        public DateTime? PassportExpire { get; set; }
        [Required]
        //[StringLength(maximumLength: 100, MinimumLength = 10, ErrorMessage = " يجب ادخال العدد المسموح من الاحرف")]
        public string FullName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [NotMapped]
        public int Age { get; set; }

        public int RankCode { get; set; }

        public string RankDesc { get; set; }

        public DateTime? RegistrationDate { get; set; }


        public DateTime? Snapshote { get; set; }

        public int HrmsUnitCode { get; set; }

        public string HrmsUnitDesc { get; set; }

        public string WorkLocation { get; set; }

        public string Region { get; set; }

        public int? WilayaCode { get; set; }

        public string WilayaDesc { get; set; }

        public int? VillageCode { get; set; }

        public string VillageDesc { get; set; }

        public string WorkPhone { get; set; }

        public string GSM { get; set; }

        public string ReletiveName1 { get; set; }


        public int ReletiveCode1 { get; set; }

        public int RelativeGsm1 { get; set; }

        public string ReletiveName2 { get; set; }

        public int ReletiveCode2 { get; set; }

        public int RelativeGsm2 { get; set; }

        public bool FitFlag { get; set; }
        public int FitResult { get; set; }

        public string DoctorNote { get; set; }

        public string Notes { get; set; }


        public int AlhajYear { get; set; }

        public string BloodGroup { get; set; }


        public DateTime? DateOfEnlistment { get; set; }

        public IFormFile Photo { get; set; }


        //!-------Relation with Category table-------------------!

        public int? CategoryId { get; set; }
        public Category Category { get; set; }

        //!--------relation with service table-------------------!
        public int? UnitId { get; set; }
        public Unit Unit { get; set; }

        //!--------relation with Document table-------------------!

        public int? DocumentId { get; set; }
        public Document Document { get; set; }

        //!--------relation with Parameter table-------------------!

        public int ParameterId { get; set; }
        public Parameter Parameter { get; set; }

        //!--------relation with Passenger table-------------------!



        //!--------relation with PassSupervisors table-------------------!
        public ICollection<PassengerSupervisors> PassengerSupervisors { get; set; }

        public Passenger Passenger { get; set; }



    }
}
