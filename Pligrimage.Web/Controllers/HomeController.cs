using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Hajj.Web.Models;
using Pligrimage.Services.Interface;
using ITS.Core.Abstractions;
using Pligrimage.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Pligrimage.Web.Controllers;
using Pligrimage.Entities;
using Pligrimage.Web.Dto;

namespace Hajj.Web.Controllers
{
    
    public class HomeController : BaseController
    {
        public readonly IAlHajjMasterServcie _alhajjRepository;
        public readonly IUnitOfWork _unitOfWork;
        public readonly IUnitServcie _unitRepository;
        public readonly ICategoryService _categoryRepository;
        private readonly IDocumentServcie _documentRepository;
        private readonly IParameterService _parameterRepository;
        //private readonly IEmployeeClient _employeeClient;

        //private IWebHostEnvironment _hostingEnvironment;  ///usig to upload file 

        public HomeController(IAlHajjMasterServcie alhajjRepository,
                                        IUnitOfWork unitOfWork,
                                        IUnitServcie unitRepository,
                                        ICategoryService categoryRepository,
                                        IDocumentServcie documentRepository,
                                        IParameterService parameterRepository

                                        )
        {
            _alhajjRepository = alhajjRepository;
            _unitOfWork = unitOfWork;
            _unitRepository = unitRepository;
            _categoryRepository = categoryRepository;
            _documentRepository = documentRepository;
            _parameterRepository = parameterRepository;


        }
        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                RequestsNumber = 25,
                ApprovedRequestsNumber = 18,
                CanceledRequestsNumber = 5,
                VehiclesNumber = 10
            };

            return View(model);
        }

        public IActionResult Stastic()
        {
            var stats = new
            {
                totalUsers = 1200,
                totalOrders = 450,
                totalRevenue = 25000,
                totalProducts = 80,
                userStats = new
                {
                    dates = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun" },
                    counts = new[] { 50, 70, 90, 120, 150, 180 }
                },
                salesStats = new
                {
                    dates = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun" },
                    counts = new[] { 5, 10, 15, 20, 25, 30 }
                }
            };
            return Ok(stats);
        }
        











    }
}
