using ITS.Core.EF.Trackable;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pligrimage.Entities.IdentityModels
{
    public class ApplicationAuditorLog: Entity
    {
        [Key]
        public int Id { get; set; }

        public string LoggedUserName { get; set; }

        public DateTime LogDateTime { get; set; }

        public string LogType { get; set; }

        public string LogRemarks { get; set; }

        public string EmployeeUser { get; set; }

        public int? RoleId { get; set; }

        public int? UnitId { get; set; }


        public string OsUser { get; set; } = Environment.GetEnvironmentVariable("USERNAME") ?? null;

        public string MachineName { get; set; } = Environment.GetEnvironmentVariable("COMPUTERNAME") ?? null;



    }
}
