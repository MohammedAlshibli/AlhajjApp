using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITS.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pligrimage.Entities;
using Pligrimage.Services.Interface;

namespace Pligrimage.Web.Controllers
{
    public class TestController : Controller
    {
        public readonly IAlHajjMasterServcie _alHajjRepostory;
        public readonly IParameterService _parameterRepostory;
        public readonly IAdminService _adminService;
        public readonly IUnitOfWork _unitOfWork;

 

        public TestController(IAlHajjMasterServcie alHajjRepostory, IParameterService parameterRepostory, IUnitOfWork unitOfWork, IAdminService adminService)
        {
            _alHajjRepostory = alHajjRepostory;
            _unitOfWork = unitOfWork;
            _parameterRepostory = parameterRepostory;
            _adminService = adminService;
        }

        public IActionResult Index()
        {
            //var FitResultParameter = _parameterRepostory.Queryable().Where(c => c.Code == "FitCode")
            var FitResultParameter = _parameterRepostory.GetFitCodeTypeList()

            .Select(c => new
            {
                c.ParameterId,
                c.DescArabic
            }).ToList();

            ViewData["FitResult"] = FitResultParameter;
            return View();
        }

 
    }
}