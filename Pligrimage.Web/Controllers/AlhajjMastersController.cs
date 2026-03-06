using AutoMapper;
using HrmsHttpClient.HrmsClientApi;
using ITS.Core.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pligrimage.Entities;
using Pligrimage.Entities.Enum;
using Pligrimage.Services.Interface;
using Pligrimage.Web.Dto;
using Pligrimage.Web.Extensions;
using Pligrimage.Web.Infrastructure;
using Pligrimage.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Pligrimage.Web.Controllers
{
    public class AlhajjMastersController : BaseController
    {
        private readonly IAlHajjMasterServcie _alhajjService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUnitServcie _unitRepository;
        private readonly ICategoryService _categoryRepository;
        private readonly IParameterService _parameterRepository;
        private readonly IEmployeeService _employeeService;
        private readonly IAdminService _adminService;
        private readonly IMapper _mapper;
        private readonly HajjSettings _settings;

        public AlhajjMastersController(
            IAlHajjMasterServcie alhajService,
            IUnitOfWork unitOfWork,
            IUnitServcie unitRepository,
            ICategoryService categoryRepository,
            IParameterService parameterRepository,
            IEmployeeService employeeService,
            IAdminService adminService,
            IMapper mapper,
            IOptions<HajjSettings> settings)
        {
            _alhajjService    = alhajService;
            _unitOfWork       = unitOfWork;
            _unitRepository   = unitRepository;
            _categoryRepository = categoryRepository;
            _parameterRepository = parameterRepository;
            _employeeService  = employeeService;
            _adminService     = adminService;
            _mapper           = mapper;
            _settings         = settings.Value;
        }

        // ────────────────────────────────────────────────────────────────────
        // HRMS LOOKUP – validate service number before calling API
        // ────────────────────────────────────────────────────────────────────
        public async Task<IActionResult> GetAlhajjDataFormWebService(string ServiceNumber)
        {
            // BUG-FIX #26: validate format before hitting external API
            if (string.IsNullOrWhiteSpace(ServiceNumber) ||
                !Regex.IsMatch(ServiceNumber.Trim().ToUpper(), _settings.ServiceNumberPattern ?? @"^[A-Z0-9]{4,12}$"))
            {
                return BadRequest("رقم الخدمة العسكرية غير صالح");
            }

            HttpResponseMessage response;
            try
            {
                response = await _employeeService.GetEmployeeBymilitaryServiceId(ServiceNumber.Trim().ToUpper());
            }
            catch (Exception)
            {
                return BadRequest("يوجد خطأ في الاتصال بنظام الموارد البشرية، الرجاء التواصل مع الإدارة");
            }

            if (!response.IsSuccessStatusCode)
                return BadRequest($"خطأ في نظام HRMS: {(int)response.StatusCode}");

            string json = await response.Content.ReadAsStringAsync();
            JundEmployeeDto fromHrms = JsonConvert.DeserializeObject<JundEmployeeDto>(json);
            if (fromHrms == null)
                return BadRequest("لم يتم العثور على الموظف في نظام الموارد البشرية");

            ViewData["ClassTypeList"] = _parameterRepository.Queryable()
                .Where(c => c.Code == HajjConstants.ParamCode.ClassType && c.ParameterId != HajjConstants.PilgrimType.Admin)
                .Select(c => new { c.ParameterId, c.DescArabic })
                .ToList();

            var newPilgrim = _mapper.Map<AlhajjMaster>(fromHrms);
            return PartialView("_Create", newPilgrim);
        }

        // ────────────────────────────────────────────────────────────────────
        // INDEX – active year only, unit-scoped
        // ────────────────────────────────────────────────────────────────────
        [PligrimageFiltter]
        public IActionResult Index()
        {
            ViewData["ClassTypeList"] = _parameterRepository.GetClassTypeListAsync()
                .Where(c => c.ParameterId != HajjConstants.PilgrimType.Admin)
                .Select(c => new { c.ParameterId, c.DescArabic })
                .ToList();

            return View();
        }

        // TenantId filter + IsDeleted filter now automatic via DbContext Global Query Filter.
        // No manual .Where(unit filter) needed.
        public IActionResult AlhajjRead()
        {
            int activeYear = _settings.ActiveHajjYear;

            var result = _alhajjService.Queryable()
                .Include(c => c.Unit)
                .Where(c =>
                    c.AlhajYear == activeYear &&
                    c.EmployeeStatus == EmployeeStatus.Employee &&
                    c.ParameterId != HajjConstants.PilgrimType.Admin)
                .Select(c => new {
                    c.PligrimageId,
                    c.FullName,
                    c.ServcieNumber,
                    c.NIC,
                    c.RankDesc,
                    c.Region,
                    UnitNameAr = c.Unit != null ? c.Unit.UnitNameAr : "",
                    c.ParameterId,
                    c.FitResult,
                    c.ConfirmCode
                })
                .ToList();

            return Json(result);
        }

        // ────────────────────────────────────────────────────────────────────
        // QUOTA HELPER – returns remaining slots (thread-safe read)
        // ────────────────────────────────────────────────────────────────────
        public IActionResult StaticService()
        {
            List<int> userServiceList = _adminService.GetUserServiceListByUnitCode(LoggedUserName()).ToList();
            int activeYear = _settings.ActiveHajjYear;

            var units = _unitRepository.Queryable().Where(x => userServiceList.Contains(x.UnitCode)).ToList();
            var statList = new List<ServiceStatcisVM>();

            foreach (var unit in units)
            {
                statList.Add(new ServiceStatcisVM
                {
                    ServiceName       = unit.UnitNameAr,
                    AllowNumber       = _alhajjService.Queryable().Count(c =>
                                            c.UnitId == unit.UnitId &&
                                            c.ParameterId == HajjConstants.PilgrimType.Regular &&
                                            c.AlhajYear == activeYear && !c.IsDeleted),
                    FromAllowNumber   = unit.AllowNumber,
                    SatndByNumber     = _alhajjService.Queryable().Count(c =>
                                            c.UnitId == unit.UnitId &&
                                            c.ParameterId == HajjConstants.PilgrimType.StandBy &&
                                            c.AlhajYear == activeYear && !c.IsDeleted),
                    FromSatndByNumber = unit.StandBy
                });
            }

            return PartialView("_staticService", statList);
        }

        public IActionResult AlhajjType()
        {
            var types = _parameterRepository.Queryable()
                .Where(x => x.Code == HajjConstants.ParamCode.ClassType)
                .Select(c => new { parameterId = c.ParameterId, alhajjType = c.DescArabic.TrimStart() })
                .OrderBy(x => x.alhajjType)
                .ToList();
            return Json(types);
        }

        // ────────────────────────────────────────────────────────────────────
        // CREATE – BUG-FIX #1 (await), #2 (race→transaction), #4 (year),
        //          #5 (fake passport removed), #6 (ConfirmCode set)
        // ────────────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<ActionResult> CreateAlhajjFromHrms(AlhajjMaster alhajjMaster)
        {
            if (!ModelState.IsValid)
                return BadRequest("البيانات غير مكتملة، يرجى مراجعة الحقول المطلوبة");

            int activeYear = _settings.ActiveHajjYear;

            var unit = _unitRepository.Queryable()
                .FirstOrDefault(x => x.UnitId == alhajjMaster.UnitId);

            if (unit == null)
                return BadRequest("الوحدة غير موجودة");

            // ── PERMANENT BAN: already performed Hajj in any previous year ──
            var previousHajj = _alhajjService.Queryable()
                .Where(c => c.NIC == alhajjMaster.NIC &&
                             c.ConfirmCode == HajjConstants.ConfirmCode.HQApproved &&
                             !c.IsDeleted)
                .Select(c => c.AlhajYear)
                .FirstOrDefault();

            if (previousHajj > 0)
                return BadRequest($"لا يمكن تسجيل هذا الموظف — لقد أدّى فريضة الحج عام {previousHajj} ولا يُسمح له بالحج مرة ثانية");

            // ── DUPLICATE: already registered this season ──────────────────
            bool alreadyRegistered = _alhajjService.Queryable()
                .Any(c => c.NIC == alhajjMaster.NIC &&
                           c.AlhajYear == activeYear &&
                           !c.IsDeleted);

            if (alreadyRegistered)
                return BadRequest("الموظف مسجل مسبقاً في هذه الدورة");

            // BUG-FIX #2: quota check + insert inside a DB transaction
            using var tx = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Re-count inside transaction to prevent race condition
                int consumed = _alhajjService.Queryable().Count(c =>
                    c.UnitId == alhajjMaster.UnitId &&
                    c.ParameterId == alhajjMaster.ParameterId &&
                    c.AlhajYear == activeYear &&
                    !c.IsDeleted);

                int allowed = alhajjMaster.ParameterId == HajjConstants.PilgrimType.Regular
                    ? unit.AllowNumber
                    : unit.StandBy;

                if (consumed >= allowed)
                    return BadRequest($"تجاوزت الحد المسموح به ({allowed}) لهذه الوحدة");

                // StampNew sets: TenantId, CreateBy, CreateOn, IsDeleted=false
                StampNew(alhajjMaster);
                alhajjMaster.AlhajYear        = activeYear;
                alhajjMaster.RegistrationDate = DateTime.Now;
                alhajjMaster.FitResult        = HajjConstants.FitResult.Pending;
                alhajjMaster.ConfirmCode      = HajjConstants.ConfirmCode.Pending;

                _alhajjService.Insert(alhajjMaster);

                // BUG-FIX #1: await the save
                await _unitOfWork.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok("تمت الإضافة بنجاح");
            }
            catch (Exception)
            {
                await tx.RollbackAsync();
                return BadRequest("لم يتم الحفظ، يوجد نقص في البيانات");
            }
        }

        // ────────────────────────────────────────────────────────────────────
        // UPDATE / SOFT DELETE
        // ────────────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<ActionResult> UpdateAlhajj(AlhajjMaster alhajjMaster)
        {
            if (alhajjMaster == null || !ModelState.IsValid)
                return BadRequest("البيانات غير صحيحة");

            StampUpdate(alhajjMaster);
            _alhajjService.Update(alhajjMaster);
            await _unitOfWork.SaveChangesAsync(); // BUG-FIX #1
            return RedirectToAction("Index");
        }

        // BUG-FIX #24: soft delete instead of hard delete
        [HttpPost]
        public async Task<ActionResult> DeleteAlhajjMaster(int pligrimageId)
        {
            var pilgrim = _alhajjService.Queryable()
                .FirstOrDefault(c => c.PligrimageId == pligrimageId);

            if (pilgrim == null)
                return NotFound();

            pilgrim.IsDeleted  = true;
            pilgrim.DeletedBy  = LoggedUserName();
            pilgrim.DeletedOn  = DateTime.Now;
            pilgrim.UpdatedBy  = LoggedUserName();
            pilgrim.UpdatedOn  = DateTime.Now;

            _alhajjService.Update(pilgrim);
            await _unitOfWork.SaveChangesAsync(); // BUG-FIX #1

            return RedirectToAction("Index");
        }

        // ────────────────────────────────────────────────────────────────────
        // BULK EXCEL IMPORT (Non-Mod external units)
        // ────────────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> CreateAlhajjNonMod([FromBody] IEnumerable<AlhajjMaster> AlhajjMasterList)
        {
            int activeYear = _settings.ActiveHajjYear;
            var items = AlhajjMasterList?.ToList() ?? new List<AlhajjMaster>();

            if (!items.Any())
                return BadRequest("القائمة فارغة");

            var selectedUnit = items.Select(x => x.Unit).FirstOrDefault();
            if (selectedUnit == null)
                return BadRequest("بيانات الوحدة مفقودة في الملف");

            int regularCount = items.Count(c => c.Parameter?.ParameterId == HajjConstants.PilgrimType.Regular);
            int standByCount = items.Count(c => c.Parameter?.ParameterId == HajjConstants.PilgrimType.StandBy);

            if (regularCount > selectedUnit.AllowNumber)
                return BadRequest($"تجاوزت الحد المسموح به ({selectedUnit.AllowNumber}) للحجاج الأصليين");

            if (standByCount > selectedUnit.StandBy)
                return BadRequest($"تجاوزت الحد المسموح به ({selectedUnit.StandBy}) للحجاج الاحتياط");

            using var tx = await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var item in items)
                {
                    // BUG-FIX #4: year-scoped duplicate check
                    bool exists = _alhajjService.Queryable()
                        .Any(c => c.NIC == item.NIC && c.AlhajYear == activeYear && !c.IsDeleted);

                    if (exists)
                        return BadRequest($"الموظف برقم الهوية {item.NIC} مسجل مسبقاً في هذه الدورة");

                    StampNew(item);
                    item.AlhajYear        = activeYear;
                    item.RegistrationDate = DateTime.Now;
                    item.FitResult        = HajjConstants.FitResult.Pending;
                    item.ConfirmCode      = HajjConstants.ConfirmCode.Pending;
                    _alhajjService.Insert(item);
                }

                await _unitOfWork.SaveChangesAsync(); // BUG-FIX #1
                await tx.CommitAsync();
                return Ok("تم حفظ البيانات بنجاح");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return BadRequest($"خطأ أثناء الحفظ: {ex.Message}");
            }
        }

        public IActionResult Create() => View();

        public IActionResult MasterDetails(int pligrimageId) =>
            RedirectToAction("Index", "AlhajjMasters");
    }
}
