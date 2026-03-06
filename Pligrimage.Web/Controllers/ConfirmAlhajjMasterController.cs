using ITS.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pligrimage.Entities;
using Pligrimage.Services.Interface;
using Pligrimage.Web.Extensions;
using Pligrimage.Web.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pligrimage.Web.Controllers
{
    public class ConfirmAlhajjMasterController : BaseController
    {
        private readonly IAlHajjMasterServcie _alhajjService;
        private readonly IUnitOfWork          _unitOfWork;
        private readonly IParameterService    _parameterServcie;
        private readonly IAdminService        _adminService;
        private readonly HajjSettings         _settings;

        public ConfirmAlhajjMasterController(
            IAlHajjMasterServcie alhajService,
            IUnitOfWork          unitOfWork,
            IParameterService    parameterService,
            IAdminService        adminService,
            IOptions<HajjSettings> settings)
        {
            _alhajjService    = alhajService;
            _unitOfWork       = unitOfWork;
            _parameterServcie = parameterService;
            _adminService     = adminService;
            _settings         = settings.Value;
        }

        // ── INDEX ─────────────────────────────────────────────────────────
        [PligrimageFiltter]
        public IActionResult Index() => View();

        // ── READ ALL (for DataTable) ──────────────────────────────────────
        public IActionResult ConfirmAlhajjRead()
        {
            int activeYear = _settings.ActiveHajjYear;

            // TenantId filter is automatic via Global Query Filter in DbContext
            var list = _alhajjService.Queryable()
                .Include(c => c.Unit)
                .Where(c => c.AlhajYear == activeYear)
                .Select(c => new
                {
                    c.PligrimageId,
                    c.FullName,
                    c.ServcieNumber,
                    c.NIC,
                    c.RankDesc,
                    c.Region,
                    c.Passport,
                    PassportExpire  = c.PassportExpire.HasValue ? c.PassportExpire.Value.ToString("yyyy-MM-dd") : "",
                    NICExpire       = c.NICExpire.HasValue       ? c.NICExpire.Value.ToString("yyyy-MM-dd")      : "",
                    c.FitResult,
                    c.ConfirmCode,
                    c.CancelNote,
                    c.RegistrationDate,
                    UnitNameAr      = c.Unit != null ? c.Unit.UnitNameAr : "",
                    ParameterTypeId = c.ParameterId
                })
                .ToList();

            return Json(list);
        }

        // ── SUMMARY STATS ─────────────────────────────────────────────────
        public IActionResult GetSummary()
        {
            int activeYear = _settings.ActiveHajjYear;
            var all = _alhajjService.Queryable()
                .Where(c => c.AlhajYear == activeYear)
                .Select(c => new { c.ConfirmCode, c.FitResult, c.PassportExpire })
                .ToList();

            var today = DateTime.Today;
            return Json(new
            {
                total     = all.Count,
                pending   = all.Count(c => c.ConfirmCode == HajjConstants.ConfirmCode.Pending),
                confirmed = all.Count(c => c.ConfirmCode == HajjConstants.ConfirmCode.Confirmed),
                cancelled = all.Count(c => c.ConfirmCode == HajjConstants.ConfirmCode.Cancelled),
                passportIssues = all.Count(c =>
                    c.PassportExpire.HasValue &&
                    (c.PassportExpire.Value - today).TotalDays < 180)
            });
        }

        // ── CONFIRM SINGLE ────────────────────────────────────────────────
        [HttpPost]
        public async Task<ActionResult> Confirm(int pligrimageId)
        {
            var pilgrim = _alhajjService.Queryable()
                .FirstOrDefault(c => c.PligrimageId == pligrimageId);

            if (pilgrim == null) return NotFound("السجل غير موجود");

            if (pilgrim.ConfirmCode == HajjConstants.ConfirmCode.Confirmed)
                return BadRequest("تم تأكيد هذا الحاج مسبقاً");

            pilgrim.ConfirmCode = HajjConstants.ConfirmCode.Confirmed;
            StampUpdate(pilgrim);
            _alhajjService.Update(pilgrim);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { message = "تم التأكيد بنجاح", pligrimageId });
        }

        // ── CONFIRM BULK ──────────────────────────────────────────────────
        [HttpPost]
        public async Task<ActionResult> ConfirmBulk([FromBody] List<int> ids)
        {
            if (ids == null || !ids.Any())
                return BadRequest("لم يتم تحديد أي سجلات");

            int activeYear = _settings.ActiveHajjYear;
            var pilgrims   = _alhajjService.Queryable()
                .Where(c => ids.Contains(c.PligrimageId) &&
                             c.AlhajYear == activeYear &&
                             c.ConfirmCode == HajjConstants.ConfirmCode.Pending)
                .ToList();

            if (!pilgrims.Any())
                return BadRequest("لا توجد سجلات في انتظار التأكيد");

            using var tx = await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var p in pilgrims)
                {
                    p.ConfirmCode = HajjConstants.ConfirmCode.Confirmed;
                    StampUpdate(p);
                    _alhajjService.Update(p);
                }
                await _unitOfWork.SaveChangesAsync();
                await tx.CommitAsync();
                return Ok(new { message = $"تم تأكيد {pilgrims.Count} حاج بنجاح", count = pilgrims.Count });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return BadRequest($"فشل التأكيد الجماعي: {ex.Message}");
            }
        }

        // ── CANCEL ────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<ActionResult> Cancel(int pligrimageId, string cancelNote)
        {
            var pilgrim = _alhajjService.Queryable()
                .FirstOrDefault(c => c.PligrimageId == pligrimageId);

            if (pilgrim == null) return NotFound("السجل غير موجود");

            pilgrim.ConfirmCode = HajjConstants.ConfirmCode.Cancelled;
            pilgrim.CancelNote  = cancelNote;
            StampUpdate(pilgrim);
            _alhajjService.Update(pilgrim);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { message = "تم الإلغاء", pligrimageId });
        }

        // ── RESTORE CANCELLED → PENDING ──────────────────────────────────
        [HttpPost]
        public async Task<ActionResult> Restore(int pligrimageId)
        {
            var pilgrim = _alhajjService.Queryable()
                .FirstOrDefault(c => c.PligrimageId == pligrimageId);

            if (pilgrim == null) return NotFound();

            pilgrim.ConfirmCode = HajjConstants.ConfirmCode.Pending;
            pilgrim.CancelNote  = null;
            StampUpdate(pilgrim);
            _alhajjService.Update(pilgrim);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { message = "تمت إعادة الطلب للمراجعة", pligrimageId });
        }
    }
}
