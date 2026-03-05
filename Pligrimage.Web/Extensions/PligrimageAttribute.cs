using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Pligrimage.Web.Extensions
{
    public class PligrimageFiltter : Attribute, IActionFilter
    {

        public void OnActionExecuting(ActionExecutingContext context)
        {

            var requiredContollerName = context.ActionDescriptor as ControllerActionDescriptor;
            string contollerName = requiredContollerName.ControllerName;
            string actionName = requiredContollerName.ActionName;


            if (requiredContollerName != null)
            {
               

                if (!context.HttpContext.User.HasPermission(contollerName, actionName))
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "Index" }, { "controller", "Unauthorised" } });
                }
            }


        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

        }
    
    }
}
