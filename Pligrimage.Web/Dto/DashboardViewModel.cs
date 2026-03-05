using System;

namespace Pligrimage.Web.Dto
{
        public class DashboardViewModel
        {
            public int RequestsNumber { get; set; }
            public int ApprovedRequestsNumber { get; set; }
            public int CanceledRequestsNumber { get; set; }
            public int VehiclesNumber { get; set; }
        }
    
}