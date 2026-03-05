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
using Pligrimage.Web.Models;
namespace Pligrimage.Web.Controllers
{
    public class InjectionController : BaseController
    {
        public readonly IAlHajjMasterServcie _alhajjService;
        public readonly IParameterService _parameterService;
        public readonly IAdminService _adminService;
        public readonly IUnitOfWork _unitOfWork;

        public InjectionController(IAlHajjMasterServcie alHajjRepostory, IParameterService parameterService, IUnitOfWork unitOfWork, IAdminService adminService)
        {
            _alhajjService = alHajjRepostory;
            _unitOfWork = unitOfWork;
            _parameterService = parameterService;
            _adminService = adminService;
        }
        public IActionResult Index()
        {
            var FitResultParameter = _parameterService.GetFitCodeTypeList()

                .Select(c => new
                {
                    c.ParameterId,
                    c.DescArabic
                }).ToList();

            ViewData["FitResult"] = FitResultParameter;
            return View();
        }

        public async Task<IActionResult> Read()
        {

            var list = _alhajjService.Queryable().Include(c => c.Unit).Where(c => c.FitResult == HajjConstants.FitResult.Pending && !c.IsDeleted);
            return View(list);
           
        }

        [HttpPost]
        public async Task<IActionResult> Update(AlhajjMaster alhajjMaster)
        {
            if (alhajjMaster != null && ModelState.IsValid)
            {
                alhajjMaster.UpdatedBy = LoggedUserName();
                alhajjMaster.UpdatedOn = DateTime.Now;
                _alhajjService.Update(alhajjMaster);
                await _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Injection");

        }
    }
}