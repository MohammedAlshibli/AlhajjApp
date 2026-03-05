using ITS.Core.EF.Trackable;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pligrimage.Entities
{
    public class Category : BaseEntity
    {
       
            public int CategoryId { get; set; }

            public string DescArabic { get; set; }

        
            public string DescEnglish { get; set; }


            public DateTime AlhajYear { get; set; }
        
            public int QTY { get; set; }



        //!-------Relation with Pligrimage table-------------------!
        public ICollection<AlhajjMaster> AlhajjMasters { get; set; }





    }
}
