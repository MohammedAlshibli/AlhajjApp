using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ITS.Core.EF.Trackable;

namespace ITS.Core.EF.Tests.Models
{
    public partial class Category : Entity
    {
        [Key]
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public List<Product> Products { get; set; }
    }
}
