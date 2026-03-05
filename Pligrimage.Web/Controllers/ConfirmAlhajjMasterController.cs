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
    public class ConfirmAlhajjMasterController : BaseController
    {

        public readonly IAlHajjMasterServcie _alhajjService;
        public readonly IUnitOfWork _unitOfWork;
        private readonly IParameterService _parameterServcie;
        public readonly IAdminService _adminService;



        public ConfirmAlhajjMasterController(IAlHajjMasterServcie alhajService,
                                        IUnitOfWork unitOfWork,
                                        IParameterService parameterService,IAdminService adminService

                                        )
        {
            _alhajjService = alhajService;
            _unitOfWork = unitOfWork;
            _parameterServcie = parameterService;
            _adminService = adminService;
        }
        public IActionResult Index()
        {
            var ConfirmCodeParameter = _parameterServcie.GetConfirmCodeParameter()

            .Select(c => new
            {
                c.ParameterId,
                c.DescArabic
            }).ToList();

            ViewData["ConfirmCode"] = ConfirmCodeParameter;
            return View();
        }

        public IActionResult ConfirmAlhajjRead()
        {
            //List<int> userServiceList = _adminService.GetUserServiceListByUnitCode(LoggedUserName()).ToList();

            //var list = _alhajjService.Queryable().Where(x => x.ConfirmCode == 51 && userServiceList.Contains(x.Unit.UnitCode)).ToDataSourceResult(request);
            var list = _alhajjService.Queryable().Where(x => x.ConfirmCode == 51).ToList();

            return View(list);
        }


        [HttpPost]
        public ActionResult Update(AlhajjMaster alhajjMaster)
        {
            if (alhajjMaster != null && ModelState.IsValid)
            {
                alhajjMaster.UpdatedBy = LoggedUserName();
                alhajjMaster.UpdatedOn = DateTime.Now;
                _alhajjService.Update(alhajjMaster);
                _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "ConfirmAlhajj"); 
        }


    }
}