using ITS.Core.EF.Trackable;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pligrimage.Entities
{
   public  class Residences: BaseEntity
    {
        public int ResidencesId { get; set; }


      
        public string Building { get; set; }


       
        public int Room { get; set; }


     
        public int RoomCapacity { get; set; }



        public int Floor { get; set; }


  
        public DateTime Year { get; set; }


        //!--------relation with Passenger table-------------------!
        public ICollection<Passenger> Passenger { set; get; }
    }
}
