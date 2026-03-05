using Pligrimage.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pligrimage.Web.Models
{
    public class DoctorNoteViewModels
    {
        public AlhajjMaster alhajjMaster { get; set; }

        public IEnumerable<Parameter> Parameters { get; set; }


    }
}
