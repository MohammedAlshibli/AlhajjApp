using ITS.Core.EF.Trackable;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pligrimage.Entities
{
   public  class Flight:BaseEntity
    {
        public int FlightId { get; set; }
        public string FlightNo { get; set; }
      
        public DateTime FlightDate { get; set; }

        public DateTime ArriveDate { get; set; }

        public int FlightYear { get; set; }

        public int FlightCapacity { get; set; }

        public string Direction { get; set; }

        //public int? FlightType { get; set; }

        //!--------relation with Parameter table-------------------!
        //[Display(Name ="FlightType")]
        public int ParameterId { get; set; }
        public Parameter Parameter { get; set; }


        //----------------------
        public ICollection<Buses> buses { set; get; }

        public ICollection<Passenger> Passenger { set; get; }

    }
}
