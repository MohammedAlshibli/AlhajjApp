using ITS.Core.EF.Trackable;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Pligrimage.Entities
{
    public class Buses : BaseEntity
    {
        public int BusId { get; set; }
        
        public string BusNo { get; set; }

      
        public int BusCapacity { get; set; }


        public int Year { get; set; }

        public DateTime Date { get; set; }



        //!-------Relation with Flight table-------------------!
        public int FlightId { get; set; }
        public Flight flight { get; set; }

        //!--------relation with Passenger table-------------------!
        public ICollection<Passenger> Passenger { set; get; }

        
    }
}
