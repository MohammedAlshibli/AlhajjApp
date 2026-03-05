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
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> ParameterRead()
        {
            
            return View(_parameterRepo.Queryable());
        }
               
      

        [HttpPost]
        public async Task<IActionResult> CreateParameter(Parameter parameter)
        {
            if(parameter !=null && ModelState.IsValid)
            {
                parameter.CreateBy = LoggedUserName();
                parameter.CreateOn = DateTime.Now;
                _parameterRepo.Insert(parameter);
                await _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Parameters");
        }


        [HttpPost]
        public async Task<IActionResult> UpdateParameter(Parameter parameter)
        {
            if (parameter != null && ModelState.IsValid)
            {
                parameter.UpdatedBy = LoggedUserName();
                parameter.UpdatedOn = DateTime.Now;
                _parameterRepo.Update(parameter);
                await _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Parameters");

        }

        [HttpPost]
        public async Task<IActionResult> DeleteParameter(Parameter parameter)
        {
            if (parameter != null && ModelState.IsValid)
            {
                _parameterRepo.Delete(parameter);
                await _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Parameters");
        }


    }
}
