using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITS.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pligrimage.Data;
using Pligrimage.Entities;
using Pligrimage.Services.Interface;
using Pligrimage.Web.Extensions;

namespace Pligrimage.Web.Controllers
{
    public class ParametersController : BaseController
    {
        private readonly IParameterService _parameterRepo;
        private readonly IUnitOfWork _unitOfWork;

        public ParametersController(IParameterService parameterRepo, IUnitOfWork unitOfWork)
        {
            _parameterRepo = parameterRepo;
            _unitOfWork = unitOfWork;
        }

        ///[PligrimageFiltter]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ParameterRead()
        {
            
            return View(_parameterRepo.Queryable());
        }
               
      

        [HttpPost]
        public ActionResult CreateParameter(Parameter parameter)
        {
            if(parameter !=null && ModelState.IsValid)
            {
                parameter.CreateBy = LoggedUserName();
                parameter.CreateOn = DateTime.Now;
                _parameterRepo.Insert(parameter);
                _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Parameters");
        }


        [HttpPost]
        public ActionResult UpdateParameter(Parameter parameter)
        {
            if (parameter != null && ModelState.IsValid)
            {
                parameter.UpdatedBy = LoggedUserName();
                parameter.UpdatedOn = DateTime.Now;
                _parameterRepo.Update(parameter);
                _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Parameters");

        }

        [HttpPost]
        public ActionResult DeleteParameter(Parameter parameter)
        {
            if (parameter != null && ModelState.IsValid)
            {
                _parameterRepo.Delete(parameter);
                _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Parameters");
        }


    }
}
