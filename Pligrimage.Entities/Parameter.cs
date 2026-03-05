using ITS.Core.EF.Trackable;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pligrimage.Entities
{
    public partial class Parameter : BaseEntity
    {

        public int ParameterId { get; set; }
        public string Code { get; set; }
        public string DescArabic { get; set; }
        public string DescEnglish { get; set; }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }
        public int Value { get; set; }

        public ICollection<AlhajjMaster> AlhajjMasters { get; set; }

        public ICollection<Flight> Flights { get; set; }



    }
}
