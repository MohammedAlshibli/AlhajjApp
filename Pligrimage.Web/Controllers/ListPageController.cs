using Microsoft.EntityFrameworkCore;
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HrmsHttpClient.HrmsClientApi;
using ITS.Core.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;

using Pligrimage.Services.Interface;

namespace Pligrimage.Web.Controllers
{
    public class ListPageController : Controller
    {
        public readonly IAlHajjMasterServcie _alhajjService;
        public readonly IUnitOfWork _unitOfWork;
        public readonly IUnitServcie _unitRepository;
        public readonly ICategoryService _categoryRepository;
        private readonly IDocumentServcie _documentRepository;
        private readonly IParameterService _parameterRepository;
        private readonly IEmployeeClient _employeeClient;
        public readonly IAdminService _adminService;

        private IWebHostEnvironment _hostingEnvironment;  ///usig to upload file 

        public ListPageController(IAlHajjMasterServcie alhajService,
                                        IUnitOfWork unitOfWork,
                                        IUnitServcie unitRepository,
                                        ICategoryService categoryRepository,
                                        IDocumentServcie documentRepository,
                                        IParameterService parameterRepository,
                                       IEmployeeClient employeeClient,
                                        IWebHostEnvironment hostingEnvironment,
                                         IAdminService adminService
                                        )
        {
            _alhajjService = alhajService;
            _unitOfWork = unitOfWork;
            _unitRepository = unitRepository;
            _categoryRepository = categoryRepository;
            _documentRepository = documentRepository;
            _parameterRepository = parameterRepository;
            _employeeClient = employeeClient;
            _hostingEnvironment = hostingEnvironment;
            _adminService = adminService;

        }



        public IActionResult Index()
        {
            ViewData["CategoryList"] = _categoryRepository.Queryable()
            .Select(c => new
            {
                c.CategoryId,
                c.DescArabic,
            }).ToList();
            return View();
        }


        public IActionResult AlhajjRead()
        {
            //var paramList = new List<int> { 7};
            var result = _alhajjService.Query()
                .Include(c => c.Unit)
                .Include(c => c.Category)
                .Include(c => c.Parameter)
                //.Where(c =>paramList.Contains(c.ParameterId)).SelectAsync().Result.ToDataSourceResult(request);
                .Where(c => c.FitResult == HajjConstants.FitResult.Fit);
            return Ok(result);
        }



    }
}
