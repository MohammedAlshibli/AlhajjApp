using AutoMapper;
using ITS.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pligrimage.Entities;
using Pligrimage.Entities.Enum;
using Pligrimage.Services.Interface;
using Pligrimage.Web.Infrastructure;
using Pligrimage.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pligrimage.Web.Controllers
{
    public class StatisticsController : BaseController
    {
        private readonly IAlHajjMasterServcie _alhajjRepository;
        private readonly IUnitServcie _unitRepository;
        private readonly IAdminService _adminService;
        private readonly HajjSettings _settings;

        public StatisticsController(
            IAlHajjMasterServcie alhajjRepository,
            IUnitOfWork unitOfWork,
            IUnitServcie unitRepository,
            ICategoryService categoryRepository,
            IDocumentServcie documentRepository,
            IParameterService parameterRepository,
            IAdminService adminService,
            IMapper mapper,
            IOptions<HajjSettings> settings)
        {
            _alhajjRepository = alhajjRepository;
            _unitRepository   = unitRepository;
            _adminService     = adminService;
            _settings         = settings.Value;
        }

        public IActionResult Index() => View();

        // ── Cancelled ────────────────────────────────────────────────────────
        public IActionResult IndexCanceledStatus() => View();

        // BUG-FIX #18: was referencing undefined variable x
        public IActionResult ReadCanceledStatus()
        {
            int activeYear = _settings.ActiveHajjYear;
            var result = _alhajjRepository.Queryable()
                .Include(c => c.Unit)
                .Where(c => c.ConfirmCode == HajjConstants.ConfirmCode.Cancelled &&
                             c.AlhajYear == activeYear) // BUG-FIX #11: year filter
                .Select(c => new MedicalViewModel
                {
                    Name              = c.FullName,
                    ServcieNumber     = c.ServcieNumber,
                    NationalID        = c.NIC,
                    DoctorNote        = c.DoctorNote,
                    UnitNameAr        = c.Unit != null ? c.Unit.UnitNameAr : "",
                    AlhajjCancelNote  = c.CancelNote
                })
                .ToList();
            return Json(result);
        }

        // ── Standby ──────────────────────────────────────────────────────────
        public IActionResult IndexStandByStatus() => View();

        public IActionResult StandByStatusRead()
        {
            int activeYear = _settings.ActiveHajjYear;
            var result = _alhajjRepository.Queryable()
                .Include(c => c.Unit)
                .Where(c => c.ParameterId == HajjConstants.PilgrimType.StandBy &&
                             c.AlhajYear == activeYear && !c.IsDeleted) // BUG-FIX #11
                .Select(c => new MedicalViewModel
                {
                    Name          = c.FullName,
                    ServcieNumber = c.ServcieNumber,
                    NationalID    = c.NIC,
                    DoctorNote    = c.DoctorNote,
                    UnitNameAr    = c.Unit != null ? c.Unit.UnitNameAr : ""
                }).ToList();
            return Json(result);
        }

        // ── Non-Fit ─────────────────────────────────────────────────────────
        public IActionResult IndexAlhajjNonFit() => View();

        public IActionResult ReadAlhajjNonFit()
        {
            int activeYear = _settings.ActiveHajjYear;
            var result = _alhajjRepository.Queryable()
                .Include(c => c.Unit)
                .Where(c => c.FitResult == HajjConstants.FitResult.NotFit &&
                             c.AlhajYear == activeYear && !c.IsDeleted)
                .Select(c => new MedicalViewModel
                {
                    Name          = c.FullName,
                    ServcieNumber = c.ServcieNumber,
                    NationalID    = c.NIC,
                    DoctorNote    = c.DoctorNote,
                    UnitNameAr    = c.Unit != null ? c.Unit.UnitNameAr : ""
                }).ToList();
            return Json(result);
        }

        // ── Fit results ──────────────────────────────────────────────────────
        public IActionResult IndexAlhajjFitResult() => View();

        public IActionResult ReadAlhajjFitResult()
        {
            int activeYear = _settings.ActiveHajjYear;
            var result = _alhajjRepository.Queryable()
                .Include(c => c.Unit)
                .Include(c => c.Category)
                .Where(c => c.FitResult == HajjConstants.FitResult.Fit &&
                             c.ParameterId != HajjConstants.PilgrimType.StandBy &&
                             c.AlhajYear == activeYear && !c.IsDeleted)
                .Select(c => new MedicalViewModel
                {
                    Name             = c.FullName,
                    ServcieNumber    = c.ServcieNumber,
                    NationalID       = c.NIC,
                    DoctorNote       = c.DoctorNote,
                    UnitNameAr       = c.Unit != null ? c.Unit.UnitNameAr : "",
                    AdminType        = c.Category != null ? c.Category.DescArabic : "",
                    EmployeePension  = c.EmployeeStatus.ToString()
                }).ToList();
            return Json(result);
        }

        // ── Admin list ───────────────────────────────────────────────────────
        public IActionResult IndexAdminList() => View();

        public IActionResult ReadAdminList()
        {
            int activeYear = _settings.ActiveHajjYear;
            var result = _alhajjRepository.Queryable()
                .Include(c => c.Unit)
                .Where(c => c.FitResult == HajjConstants.FitResult.Pending &&
                             c.ParameterId == HajjConstants.PilgrimType.Admin &&
                             c.AlhajYear == activeYear && !c.IsDeleted)
                .Select(c => new MedicalViewModel
                {
                    Name          = c.FullName,
                    ServcieNumber = c.ServcieNumber,
                    NationalID    = c.NIC,
                    DoctorNote    = c.DoctorNote,
                    UnitNameAr    = c.Unit != null ? c.Unit.UnitNameAr : ""
                }).ToList();
            return Json(result);
        }

        // ── Pensions ─────────────────────────────────────────────────────────
        public IActionResult IndexPensions() => View();

        public IActionResult ReadPensionsList()
        {
            int activeYear = _settings.ActiveHajjYear;
            var result = _alhajjRepository.Queryable()
                .Include(c => c.Unit)
                .Where(c => c.EmployeeStatus != EmployeeStatus.Employee &&
                             c.AlhajYear == activeYear && !c.IsDeleted)
                .Select(c => new MedicalViewModel
                {
                    Name          = c.FullName,
                    ServcieNumber = c.ServcieNumber,
                    NationalID    = c.NIC,
                    DoctorNote    = c.DoctorNote,
                    UnitNameAr    = c.Unit != null ? c.Unit.UnitNameAr : ""
                }).ToList();
            return Json(result);
        }

        // ── Charts & totals ──────────────────────────────────────────────────
        public IActionResult ByService()
        {
            int activeYear = _settings.ActiveHajjYear;
            var data = _unitRepository.Queryable()
                .Select(c => new
                {
                    ServiceName        = c.UnitNameAr,
                    AllowNumber        = c.AllowNumber,
                    StandBy            = c.StandBy,
                    Count              = c.AlhajjMasters.Count(m => m.AlhajYear == activeYear && !m.IsDeleted),
                    AllowNumberRemming = c.AllowNumber - c.AlhajjMasters.Count(m => m.AlhajYear == activeYear && !m.IsDeleted && m.ParameterId == HajjConstants.PilgrimType.Regular),
                    StandByRemming     = c.StandBy    - c.AlhajjMasters.Count(m => m.AlhajYear == activeYear && !m.IsDeleted && m.ParameterId == HajjConstants.PilgrimType.StandBy)
                }).ToList();
            return View(data);
        }

        public IActionResult ChartByServicesData()
        {
            int activeYear = _settings.ActiveHajjYear;
            var data = _unitRepository.Queryable()
                .Select(c => new
                {
                    Status = c.UnitNameAr,
                    Count  = c.AllowNumber - c.AlhajjMasters.Count(m => m.AlhajYear == activeYear && !m.IsDeleted && m.ParameterId == HajjConstants.PilgrimType.Regular),
                    Total  = c.AllowNumber
                });
            return Json(data);
        }

        public IActionResult AlhajjTotal()
        {
            int activeYear = _settings.ActiveHajjYear;
            var total = _alhajjRepository.Queryable()
                .Count(c => c.AlhajYear == activeYear && !c.IsDeleted);
            return View(total);
        }

        public IActionResult StaticService()
        {
            int activeYear = _settings.ActiveHajjYear;
            List<int> userServiceList = _adminService.GetUserServiceListByUnitCode(LoggedUserName()).ToList();
            var units = _unitRepository.Queryable().Where(x => userServiceList.Contains(x.UnitCode)).ToList();
            var statList = new List<ServiceStatcisVM>();

            foreach (var unit in units)
            {
                statList.Add(new ServiceStatcisVM
                {
                    ServiceName       = unit.UnitNameAr,
                    AllowNumber       = _alhajjRepository.Queryable().Count(c =>
                                            c.UnitId == unit.UnitId &&
                                            c.ParameterId == HajjConstants.PilgrimType.Regular &&
                                            c.EmployeeStatus == EmployeeStatus.Employee &&
                                            c.FitResult != HajjConstants.FitResult.NotFit &&
                                            c.AlhajYear == activeYear && !c.IsDeleted),
                    FromAllowNumber   = unit.AllowNumber,
                    SatndByNumber     = _alhajjRepository.Queryable().Count(c =>
                                            c.UnitId == unit.UnitId &&
                                            c.ParameterId == HajjConstants.PilgrimType.StandBy &&
                                            c.AlhajYear == activeYear && !c.IsDeleted),
                    FromSatndByNumber = unit.StandBy
                });
            }

            return PartialView("_staticService", statList);
        }

        public IActionResult StaticALlService()
        {
            int activeYear = _settings.ActiveHajjYear;
            int[] activeTypes    = { HajjConstants.PilgrimType.Regular, HajjConstants.PilgrimType.StandBy, HajjConstants.PilgrimType.Admin };
            int[] activeFitCodes = { HajjConstants.FitResult.Fit, HajjConstants.FitResult.NotFit, HajjConstants.FitResult.Pending };

            int total = _alhajjRepository.Queryable()
                .Count(c => activeTypes.Contains(c.ParameterId) &&
                             activeFitCodes.Contains(c.FitResult) &&
                             c.AlhajYear == activeYear && !c.IsDeleted);

            return Json(new[] { new { Type = "total", Consumed = total } });
        }

        public IActionResult NotFoundPage() => View();
    }
}
