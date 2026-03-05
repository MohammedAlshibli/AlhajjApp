using Pligrimage.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pligrimage.Web.Models
{
    public class PassengerViewModel
    {

        public ICollection<AlhajjMaster>  AlhajjsList { get; set; }

        public string Fullname { get; set; }

        public string ServcieNumber { get; set; }

        public string NIC { get; set; }

        public int PligrimageId { get; set; }

        public Flight Flight { get; set; }
        public int FlightId { get; set; }

        ////BusRelation
        public Buses Buses { get; set; }
        public int BusId { get; set; }

        public Residences Residences { get; set; }
        public int ResidencesId { get; set; }

        public int SupervisorID { get; set; }


    }
}
