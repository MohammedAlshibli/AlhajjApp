using ITS.Core.EF.Trackable;
using System;
using System.Collections.Generic;
using System.Text;


namespace Pligrimage.Entities
{
    public class Passenger : BaseEntity
    {
        public Passenger()
        {
            AlhajYear = DateTime.Now.Year;

        }
        public int PassengerId { get; set; }
        public int AlhajYear { get; set; }



        //FlightRelation
        public Flight Flight { get; set; }
        public int FlightId { get; set; }

        ////BusRelation
        public Buses Buses { get; set; }
        public int BusId { get; set; }

        ////ResidentRelation

        public Residences Residences { get; set; }
        public int ResidencesId { get; set; }

        ////PassengerSuperRelation
        //public PassengerSupervisors PassengerSupervisors { get; set; }
        //public int PassengerSuppId { get; set; }


        //public int AlhajjForeignKey { get; set; }


        public int PligrimageId { get; set; }

        public AlhajjMaster AlhajjMaster { get; set; }



    }
}
