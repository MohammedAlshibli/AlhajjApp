using ITS.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pligrimage.Entities;
using Pligrimage.Services.Interface;
using Pligrimage.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pligrimage.Web.Controllers
{
    /// <summary>
    /// HQ / Central admin screen.
    /// Shows every pilgrim that has been confirmed by their branch commander.
    /// HQ can:
    ///   - Final-approve all (or selected) in one click
    ///   - Return any record back to the branch (resets to Pending)
    /// SysAdmin only — tenant filter is bypassed automatically (TenantId = 0).
    /// </summary>
    public class CentralConfirmController : BaseController
    {
        private readonly IAlHajjMasterServcie _alhajjService;
        private readonly IUnitOfWork          _unitOfWork;
        private readonly IUnitService         _unitService;
        private readonly HajjSettings         _settings;

        public CentralConfirmController(
            IAlHajjMasterServcie alhajjService,
            IUnitOfWork          unitOfWork,
            IUnitService         unitService,
            IOptions<HajjSettings> settings)
        {
            _alhajjService = alhajjService;
            _unitOfWork    = unitOfWork;
            _unitService   = unitService;
            _settings      = settings.Value;
        }

        // ── INDEX ─────────────────────────────────────────────────────────
        public IActionResult Index() => View();

        // ── READ: pilgrims confirmed by branches (ConfirmCode = 51) ──────
        public IActionResult ReadConfirmed()
        {
            int year = _settings.ActiveHajjYear;

            var list = _alhajjService.Queryable()
                .Include(c => c.Unit)
                .Where(c =>
                    c.AlhajYear    == year &&
                    c.ConfirmCode  == HajjConstants.ConfirmCode.Confirmed) // branch-confirmed only
                .Select(c => new
                {
                    c.PligrimageId,
                    c.FullName,
                    c.ServcieNumber,
                    c.NIC,
                    c.RankDesc,
                    c.Region,
                    c.Passport,
                    PassportExpire  = c.PassportExpire.HasValue
                                        ? c.PassportExpire.Value.ToString("yyyy-MM-dd") : "",
                    c.FitResult,
                    c.ConfirmCode,
                    c.ParameterId,
                    UnitNameAr      = c.Unit != null ? c.Unit.UnitNameAr : "",
                    UnitId          = c.Unit != null ? c.Unit.UnitId : 0,
                    c.RegistrationDate,
                    c.BloodGroup
                })
                .ToList();

            return Json(list);
        }

        // ── SUMMARY STATS (per branch + totals) ──────────────────────────
        public IActionResult GetStats()
        {
            int year = _settings.ActiveHajjYear;

            var all = _alhajjService.Queryable()
                .Include(c => c.Unit)
                .Where(c => c.AlhajYear == year)
                .Select(c => new
                {
                    c.ConfirmCode,
                    c.FitResult,
                    c.PassportExpire,
                    UnitNameAr = c.Unit != null ? c.Unit.UnitNameAr : "غير محدد",
                    c.ParameterId
                })
                .ToList();

            var today = DateTime.Today;

            // per-branch breakdown
            var byUnit = all
                .Where(c => c.ConfirmCode == HajjConstants.ConfirmCode.Confirmed)
                .GroupBy(c => c.UnitNameAr)
                .Select(g => new
                {
                    unit      = g.Key,
                    count     = g.Count(),
                    original  = g.Count(x => x.ParameterId == HajjConstants.PilgrimType.Regular),
                    standby   = g.Count(x => x.ParameterId == HajjConstants.PilgrimType.StandBy),
                    passport  = g.Count(x => x.PassportExpire.HasValue &&
                                             (x.PassportExpire.Value - today).TotalDays < 180)
                })
                .OrderByDescending(x => x.count)
                .ToList();

            return Json(new
            {
                total         = all.Count,
                branchConfirmed = all.Count(c => c.ConfirmCode == HajjConstants.ConfirmCode.Confirmed),
                pending       = all.Count(c => c.ConfirmCode == HajjConstants.ConfirmCode.Pending),
                returned      = all.Count(c => c.ConfirmCode == HajjConstants.ConfirmCode.Cancelled),
                passportIssues = all.Count(c =>
                    c.ConfirmCode == HajjConstants.ConfirmCode.Confirmed &&
                    c.PassportExpire.HasValue &&
                    (c.PassportExpire.Value - today).TotalDays < 180),
                byUnit
            });
        }

        // ── FINAL-APPROVE ALL confirmed pilgrims ─────────────────────────
        [HttpPost]
        public async Task<IActionResult> FinalApproveAll()
        {
            int year = _settings.ActiveHajjYear;

            var pilgrims = _alhajjService.Queryable()
                .Where(c => c.AlhajYear   == year &&
                             c.ConfirmCode == HajjConstants.ConfirmCode.Confirmed)
                .ToList();

            if (!pilgrims.Any())
                return BadRequest("لا توجد سجلات مؤكدة من الأسلحة");

            using var tx = await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var p in pilgrims)
                {
                    // Use a dedicated "HQ approved" code: 77
                    // (Keeps it distinguishable from branch-confirmed = 51)
                    p.ConfirmCode = 77;
                    StampUpdate(p);
                    _alhajjService.Update(p);
                }
                await _unitOfWork.SaveChangesAsync();
                await tx.CommitAsync();
                return Ok(new { message = $"تمت الموافقة النهائية على {pilgrims.Count} حاج", count = pilgrims.Count });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return BadRequest($"فشلت العملية: {ex.Message}");
            }
        }

        // ── FINAL-APPROVE SELECTED ────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> FinalApproveSelected([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any()) return BadRequest("لم يتم تحديد سجلات");

            int year = _settings.ActiveHajjYear;
            var pilgrims = _alhajjService.Queryable()
                .Where(c => ids.Contains(c.PligrimageId) &&
                             c.AlhajYear   == year &&
                             c.ConfirmCode == HajjConstants.ConfirmCode.Confirmed)
                .ToList();

            if (!pilgrims.Any()) return BadRequest("لا توجد سجلات مؤكدة من المحددين");

            using var tx = await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var p in pilgrims)
                {
                    p.ConfirmCode = 77; // HQ approved
                    StampUpdate(p);
                    _alhajjService.Update(p);
                }
                await _unitOfWork.SaveChangesAsync();
                await tx.CommitAsync();
                return Ok(new { message = $"تمت الموافقة النهائية على {pilgrims.Count} حاج", count = pilgrims.Count });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return BadRequest($"فشلت العملية: {ex.Message}");
            }
        }

        // ── RETURN TO BRANCH (reset to Pending) ──────────────────────────
        [HttpPost]
        public async Task<IActionResult> ReturnToBranch(int pligrimageId, string returnNote)
        {
            var pilgrim = _alhajjService.Queryable()
                .FirstOrDefault(c => c.PligrimageId == pligrimageId);

            if (pilgrim == null) return NotFound("السجل غير موجود");

            // Reset to Pending + store note in CancelNote so branch sees it
            pilgrim.ConfirmCode = HajjConstants.ConfirmCode.Pending;
            pilgrim.CancelNote  = string.IsNullOrWhiteSpace(returnNote)
                                    ? "أُعيد من الإدارة العليا للمراجعة"
                                    : returnNote;
            StampUpdate(pilgrim);
            _alhajjService.Update(pilgrim);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { message = $"تمت إعادة السجل إلى سلاح {pilgrim.FullName} للمراجعة", pligrimageId });
        }
    }
}
