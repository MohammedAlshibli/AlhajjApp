using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Pligrimage.Web.Extensions
{
    public static class AppControllersExtension
    {
        public static IList<AppController> GetAppControllers()
        {
            var _controllerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t != null
                            && t.IsPublic
                            && t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
                            && !t.IsAbstract
                            //&& typeof(IController).IsAssignableFrom(t));
                            && typeof(Controller).IsAssignableFrom(t));

            var _controllerMethods = _controllerTypes.ToDictionary(controllerType => controllerType,
                controllerType => controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(m => typeof(ActionResult).IsAssignableFrom(m.ReturnType)));

            IList<AppController> controllers = new List<AppController>();

            foreach (var _controller in _controllerMethods)
            {

                string _controllerName = _controller.Key.Name;

                if (_controllerName.EndsWith("Controller"))
                {
                    _controllerName = _controllerName.Substring(0, _controllerName.LastIndexOf("Controller"));
                }

                var myController = new AppController
                {
                    ControllerName = _controllerName
                };

                foreach (var _controllerAction in _controller.Value.Where(x => x.Name == "View" && x.IsPublic))
                {
                    //var sss = _controllerAction.Attributes.ToString();
                    //var sssss = _controllerAction.CustomAttributes.ToString();
                    string _controllerActionName = _controllerAction.Name;
                    myController.ControllerActions.Add(new AppAction { ActionName = _controllerActionName });

                }

                controllers.Add(myController);

            }
            return controllers;
        }

        public static AppController GetControllerWithActions(string controllerName)
        {
            return GetAppControllers().FirstOrDefault(c => c.ControllerName == controllerName);
        }
    }
}
