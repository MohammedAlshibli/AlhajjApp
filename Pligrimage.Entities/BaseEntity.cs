using ITS.Core.EF.Trackable;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pligrimage.Entities
{
    public abstract class BaseEntity : Entity
    {

        public DateTime? CreateOn { get; set; } = DateTime.Now;
        public string CreateBy { get; set; } = PligrimageConstants.UserName;
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
    }
}
