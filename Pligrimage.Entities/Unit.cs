using ITS.Core.EF.Trackable;
using Pligrimage.Entities.IdentityModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pligrimage.Entities
{
    public class Unit : BaseEntity
    {
        public int UnitId { get; set; }

        public int UnitCode { get; set; }


        public string UnitNameEn { get; set; }


        public string UnitNameAr { get; set; }

        //Yes if the staff from Mod N from outSude
        public bool ModFlag { get; set; }


        public DateTime AlhajYear { get; set; }


        public int AllowNumber { get; set; }



        public int StandBy { get; set; }

        public int UnitOrder { get; set; }

        //!-------Relation with Pligrimage table-------------------!
        public ICollection<AlhajjMaster> AlhajjMasters { get; set; }
        public ICollection<UserService> UserService { get; set; }


    }
}
