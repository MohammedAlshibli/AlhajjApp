using ITS.Core.EF.Trackable;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pligrimage.Entities.IdentityModels
{
    public class AdminLogin: Entity
    {
        [Key]
        public int Id { get; set; }
        public string PasswordHash { get; set; }

        public DateTime ModifiedOn { get; set; }

    }
}
