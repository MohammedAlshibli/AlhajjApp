using Pligrimage.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Pligrimage.Web.Models
{
    public class SwapVM
    {
        public int PassengerId { get; set; }
        public string ServcieNumber { get; set; }
        public string FullName { get; set; }
        public string FlightNo { get; set; }
        public int FlightId { get; set; }
        public int PligrimageId { get; set; }
        public string NIC { get; set; }
        public string BusNo { get; set; }
        public int BusId { get; set; }







    }
}
