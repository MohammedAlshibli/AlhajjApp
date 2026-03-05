using Pligrimage.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Pligrimage.Web.Models
{
    public class AdminNonModVM
    {
        public string FullName { get; set; }
        public string ServcieNumber { get; set; }
        public int NIC { get; set; }
        public DateTime? NICExpire { get; set; }
        public int Passport { get; set; }
        public DateTime? PassportExpire { get; set; }
        public int CategoryId { get; set; }


        public int UnitId { get; set; }

        public int DocumentId { get; set; }

        public int ParameterId { get; set; }

        public Parameter Parameter { set; get; }

        public Unit  Unit { set; get; }
        public Category Category { set; get; }
        public Document Document { set; get; }





    }
}
