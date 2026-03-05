using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pligrimage.Web.Models
{
    public class StaticServiceViewModel
    {
        public string ServiceName { get; set; }
        public int AllowNumber { get; set; }

        public int StandBy { get; set; }

        public int Count { get; set; }

        public int AllowNumberRemming { get; set; }

        public int StandByRemming { get; set; }
        public int Remining { get; set; }
    }
}
