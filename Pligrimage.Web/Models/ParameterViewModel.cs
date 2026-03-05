using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pligrimage.Web.Models
{
    public class ParameterViewModel
    {
        public int ParameterID { get; set; }
        public string Code { get; set; }
        public string DescArabic { get; set; }
        public string DescEnglish { get; set; }
        public int MaxValue { get; set; }
        public int MinValue { get; set; }
        public int Value { get; set; }
    }
}
