using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ITS.Core.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pligrimage.Entities.Enum;
using Pligrimage.Services.Interface;
using Pligrimage.Web.Common.ViewModel;
using Pligrimage.Web.Models;

namespace Pligrimage.Web.Controllers
{
    public class StatisticsController : BaseController
    {
        public readonly IAlHajjMasterServcie _alhajjRepository;
        public readonly IUnitOfWork _unitOfWork;
        public readonly IUnitServcie _unitRepository;
        public readonly ICategoryService _categoryRepository;
        private readonly IDocumentServcie _documentRepository;
        private readonly IParameterService _parameterRepository;
        public readonly IAdminService _adminService;
        private readonly IMapper _mapper;


        public StatisticsController(IAlHajjMasterServcie alhajjRepository,
                                        IUnitOfWork unitOfWork,
                                        IUnitServcie unitRepository,
                                        ICategoryService categoryRepository,
                                        IDocumentServcie documentRepository,
                                        IParameterService parameterRepository,
                                         IAdminService adminService,
                                          IMapper mapper)
        {
            _alhajjRepository = alhajjRepository;
            _unitOfWork = unitOfWork;
            _unitRepository = unitRepository;
            _categoryRepository = categoryRepository;
            _documentRepository = documentRepository;
            _parameterRepository = parameterRepository;
            _adminService = adminService;
            _mapper = mapper;
        }


        public IActionResult Index()
        {
            return View();
        }


        public IActionResult IndexCanceledStatus()
        {
          
            return View();
        }

        public IActionResult ReadCanceledStatus()
        {
            //var CanceledStatus = _alhajjRepository.Queryable().Where(c => c.ParameterId == 4).Select(c => new MedicalViewModel()
            //{
            //    Name = c.FullName,
            //    ServcieNumber = c.ServcieNumber,
            //    NationalID = c.NIC,
            //    DoctorNote = c.DoctorNote,
            //    UnitNameAr = c.Unit.UnitNameAr,
            //    AlhajjCancelNote=c.CancelNote
                
            //}).ToList().ToDataSourceResult(request);

            var xx = _alhajjRepository.Query().Where(c => c.ParameterId == 4);

            var x = _mapper.Map<MedicalViewModel>(xx);

            return Json(x);
        }

        public async Task<MedicalViewModel> CancelView()
        {
            try
            {
                var xx = _alhajjRepository.Query().Where(c => c.ParameterId == 4);

               return _mapper.Map<MedicalViewModel>(xx);
                
            }
            catch (Exception xe)
            {

                throw;
            }
        }

        public IActionResult IndexStandByStatus()
        {
            
            return View();
        }

        public IActionResult StandByStatusRead()
        {
            var StandByStatus = _alhajjRepository.Queryable().Where(c => c.ParameterId == 2).Select(c => new MedicalViewModel()
            {
                Name = c.FullName,
                ServcieNumber = c.ServcieNumber,
                NationalID = c.NIC,
                DoctorNote = c.DoctorNote,
                UnitNameAr = c.Unit.UnitNameAr
            }).ToList();
            return View(StandByStatus);
        }
        public IActionResult IndexAlhajjNonFit()
        {
            return View();
        }
        public IActionResult ReadAlhajjNonFit()
        {
            var alhajjNonfit = _alhajjRepository.Queryable().Where(c => c.FitResult == 6).Select(c => new MedicalViewModel()
            {
                Name = c.FullName,
                ServcieNumber = c.ServcieNumber,
                NationalID = c.NIC,
                DoctorNote = c.DoctorNote,
                UnitNameAr = c.Unit.UnitNameAr
            }).ToList();
            return View(alhajjNonfit);
        }

        public IActionResult IndexAlhajjFitResult()
        {
            return View();
        }

        public IActionResult AlhajjFitResult()
        {
            var alhajjfitResult = _alhajjRepository.Queryable().Where(c => c.FitResult == 5 && c.ParameterId !=2 ).Select(c => new MedicalViewModel()
            {
                Name = c.FullName,
                ServcieNumber = c.ServcieNumber,
                NationalID = c.NIC,
                DoctorNote = c.DoctorNote,
                UnitNameAr = c.Unit.UnitNameAr,
                AdminType=c.Category.DescArabic,
                EmployeePension=c.EmployeeStatus.ToString()

                
            }).ToList();
            return View(alhajjfitResult);


        }
        public IActionResult IndexAdminList()
        {
            return View();
        }

        public IActionResult ReadAdminList()
        {

            var alhajjAdminlist = _alhajjRepository.Queryable().Where(c => c.FitResult == 7 && c.ParameterId == 3).Select(c => new MedicalViewModel()
            {
                Name = c.FullName,
                ServcieNumber = c.ServcieNumber,
                NationalID = c.NIC,
                DoctorNote = c.DoctorNote,
                UnitNameAr = c.Unit.UnitNameAr

            }).ToList();
            return View(alhajjAdminlist);


        }

        public IActionResult IndexPensions()
        {
            return View();
        }

        public IActionResult ReadPensionsList()
        {

            var PensionList = _alhajjRepository.Queryable().Where(c =>c.EmployeeStatus !=0)
                .Select(c => new MedicalViewModel()
            {
                Name = c.FullName,
                ServcieNumber = c.ServcieNumber,
                NationalID = c.NIC,
                DoctorNote = c.DoctorNote,
                UnitNameAr = c.Unit.UnitNameAr

            }).ToList();
            return RedirectToAction("Index", "Residences");


        }



        public IActionResult ByService()
        {
            var data = _unitRepository.Queryable().Select(c => new StaticServiceViewModel()
            {
                ServiceName = c.UnitNameAr,
                AllowNumber = c.AllowNumber,
                StandBy = c.StandBy,
                Count = c.AlhajjMasters.Where(m => m.UnitId == c.UnitId).Count(),
                AllowNumberRemming = c.AllowNumber - c.AlhajjMasters.Where(m => m.UnitId == c.UnitId).Where(cx => cx.ParameterId == 1).Count(),
                StandByRemming = c.StandBy - c.AlhajjMasters.Where(m => m.UnitId == c.UnitId).Where(cx => cx.ParameterId == 2).Count()

            }).ToList();
            return View(data);

        }
        public IActionResult ChartByServicesData()
        {
            var xdata = _unitRepository.Queryable()
                .Select(c => new ServiceStatisticsCountVm
                {
                    Status = c.UnitNameAr,
                    Count = c.AllowNumber - c.AlhajjMasters.Where(m => m.UnitId == c.UnitId).Where(cx => cx.ParameterId == 1).Count(),
                    Total = c.AllowNumber
                });

            return Json(xdata);
        }



        public IActionResult AlhajjTotal()
        {
            var alhajjTotal = _alhajjRepository.Queryable()/*.Where(c => c.ParameterId == 1)*/.Select(c => new ServiceStatisticsCountVm()
            {
                Count = c.ParameterId
            }).Count();
            return View(alhajjTotal);
        }

        public IActionResult StaticALlService()
        {

            int[] parmList = new int[] { 1,2,3 };
            int[] fitresultList = new int[] { 5,6,7 };
            var ConsumedAllowedNumberService = _alhajjRepository.Queryable().Where(c => parmList.Contains(c.ParameterId) && fitresultList.Contains(c.FitResult)).Count();
            var statcisStatusList = Enumerable.Empty<object>().Select(r => new { total = 0, }).ToList();
            var items = new[]{
                new { Type = "total", Consumed=ConsumedAllowedNumberService },
            };
            return Json(items);

        }




        public IActionResult NotFoundPage()
        {
            return View();
        }



        public IActionResult StaticService()
        {
            List<int> userServiceList = _adminService.GetUserServiceListByUnitCode(LoggedUserName()).ToList();

            var StaticService = _unitRepository.Queryable().Where(x => userServiceList.Contains(x.UnitCode)).ToList();

            List<ServiceStatcisVM> serviceStatcis = new List<ServiceStatcisVM>();

            foreach (var item in StaticService)
            {
                var statcisVM = new ServiceStatcisVM();

                statcisVM.ServiceName = item.UnitNameAr.ToString();
                statcisVM.AllowNumber = _alhajjRepository.Queryable().Where(c => c.Unit.UnitCode == item.UnitCode && c.ParameterId == 1 && c.EmployeeStatus == EmployeeStatus.Employee && c.FitResult != 6).Count();
                statcisVM.FromAllowNumber = item.AllowNumber;
                statcisVM.SatndByNumber = _alhajjRepository.Queryable().Where(c => c.Unit.UnitCode == item.UnitCode && c.ParameterId == 2).Count();
                statcisVM.FromSatndByNumber = item.StandBy;
                serviceStatcis.Add(statcisVM);

            }

            return PartialView("_staticService", serviceStatcis);
        }


    }
}