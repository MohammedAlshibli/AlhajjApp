using ITS.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pligrimage.Entities;
using Pligrimage.Services.Interface;
using Pligrimage.Web.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pligrimage.Web.Controllers
{
    public class MedicalController : BaseController
    {
        private readonly IAlHajjMasterServcie _alhajjService;
        private readonly IParameterService    _parameterService;
        private readonly IUnitOfWork          _unitOfWork;
        private readonly HajjSettings         _settings;

        public MedicalController(
            IAlHajjMasterServcie alhajjService,
            IParameterService    parameterService,
            IUnitOfWork          unitOfWork,
            IOptions<HajjSettings> settings)
        {
            _alhajjService    = alhajjService;
            _parameterService = parameterService;
            _unitOfWork       = unitOfWork;
            _settings         = settings.Value;
        }

        // ── INDEX ─────────────────────────────────────────────────────────
        [PligrimageFiltter]
        public IActionResult Index()
        {
            ViewData["FitResult"] = _parameterService.GetFitCodeTypeList()
                .Select(c => new { c.ParameterId, c.DescArabic }).ToList();
            return View();
        }

        // ── READ: HQ-approved pilgrims awaiting medical exam ─────────────
        // Shows pilgrims with ConfirmCode == 77 (HQ approved) only
        public IActionResult ReadMedical()
        {
            int year = _settings.ActiveHajjYear;

            var list = _alhajjService.Queryable()
                .Include(c => c.Unit)
                .Where(c =>
                    c.AlhajYear   == year &&
                    c.ConfirmCode == HajjConstants.ConfirmCode.HQApproved)
                .Select(c => new
                {
                    c.PligrimageId,
                    c.FullName,
                    c.ServcieNumber,
                    c.NIC,
                    c.RankDesc,
                    c.BloodGroup,
                    c.FitResult,
                    c.DoctorNote,
                    c.ParameterId,
                    InjectionDate   = c.InjectionDate != default
                                        ? c.InjectionDate.ToString("yyyy-MM-dd") : "",
                    PassportExpire  = c.PassportExpire.HasValue
                                        ? c.PassportExpire.Value.ToString("yyyy-MM-dd") : "",
                    UnitNameAr      = c.Unit != null ? c.Unit.UnitNameAr : ""
                })
                .ToList();

            return Json(list);
        }

        // ── STATS ─────────────────────────────────────────────────────────
        public IActionResult GetMedicalStats()
        {
            int year = _settings.ActiveHajjYear;
            var all = _alhajjService.Queryable()
                .Where(c => c.AlhajYear == year && c.ConfirmCode == HajjConstants.ConfirmCode.HQApproved)
                .Select(c => new { c.FitResult, c.InjectionDate })
                .ToList();

            return Json(new
            {
                total         = all.Count,
                pending       = all.Count(c => c.FitResult == HajjConstants.FitResult.Pending),
                fit           = all.Count(c => c.FitResult == HajjConstants.FitResult.Fit ||
                                               c.FitResult == HajjConstants.FitResult.DoctorApproved),
                conditionally = all.Count(c => c.FitResult == HajjConstants.FitResult.ConditionallyFit),
                notFit        = all.Count(c => c.FitResult == HajjConstants.FitResult.NotFit),
                vaccinated    = all.Count(c => c.InjectionDate != default(DateTime))
            });
        }

        // ── UPDATE FIT RESULT + DOCTOR NOTE ──────────────────────────────
        [HttpPost]
        public async Task<IActionResult> UpdateMedical(
            int pligrimageId, int fitResult,
            string doctorNote, string injectionDate)
        {
            var pilgrim = _alhajjService.Queryable()
                .FirstOrDefault(c => c.PligrimageId == pligrimageId);

            if (pilgrim == null) return NotFound("السجل غير موجود");

            pilgrim.FitResult  = fitResult;
            pilgrim.DoctorNote = doctorNote;

            if (DateTime.TryParse(injectionDate, out var injDate))
                pilgrim.InjectionDate = injDate;

            StampUpdate(pilgrim);
            _alhajjService.Update(pilgrim);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new
            {
                message = $"تم تحديث نتيجة الفحص الطبي لـ {pilgrim.FullName}",
                pligrimageId,
                fitResult
            });
        }

        // ── BULK UPDATE FIT: mark many as Fit at once ────────────────────
        [HttpPost]
        public async Task<IActionResult> BulkMarkFit([FromBody] System.Collections.Generic.List<int> ids)
        {
            if (ids == null || !ids.Any()) return BadRequest("لم يتم تحديد سجلات");

            int year = _settings.ActiveHajjYear;
            var pilgrims = _alhajjService.Queryable()
                .Where(c => ids.Contains(c.PligrimageId) && c.AlhajYear == year)
                .ToList();

            if (!pilgrims.Any()) return BadRequest("لم يتم العثور على السجلات");

            using var tx = await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var p in pilgrims)
                {
                    p.FitResult = HajjConstants.FitResult.DoctorApproved;
                    StampUpdate(p);
                    _alhajjService.Update(p);
                }
                await _unitOfWork.SaveChangesAsync();
                await tx.CommitAsync();
                return Ok(new { message = $"تم تحديث {pilgrims.Count} سجل كـ لائق", count = pilgrims.Count });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return BadRequest($"فشلت العملية: {ex.Message}");
            }
        }
    }
}
