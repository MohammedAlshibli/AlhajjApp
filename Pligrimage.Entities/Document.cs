using ITS.Core.EF.Trackable;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pligrimage.Entities
{
    public class Document : BaseEntity
    {
        public int DocumentId { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public string Path { get; set; }

        public string DocumnetType { get; set; }

        public int Year { get; set; }

        public DateTime? VisitDate { get; set; }

        //!-------Relation with Pligrimage table-------------------!
        public ICollection<AlhajjMaster> AlhajjMasters { get; set; }
    }

 }
