using ITS.Core.EF.Trackable;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pligrimage.Entities
{
   public  class PassengerSupervisors :BaseEntity
    {
        public int PassengerSuppId { get; set; }
        public int Count { get; set; }
        public DateTime Year { get; set; }

        //!--------relation with Alhajj table-------------------!

        public int PligrimageId { get; set; }
        public AlhajjMaster AlhajjMaster { get; set; }


        //!--------relation with Passenger table-------------------!
        //public ICollection<Passenger> Passenger { set; get; }



    }
}
