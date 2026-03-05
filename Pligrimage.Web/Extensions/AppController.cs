using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pligrimage.Web.Extensions
{
    public class AppController
    {
        public AppController()
        {
            ControllerActions = new List<AppAction>();
        }
        public string ControllerName { get; set; }
        public ICollection<AppAction> ControllerActions { get; set; }
    }
}
