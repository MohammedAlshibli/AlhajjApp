using ITS.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Pligrimage.Entities;
using Pligrimage.Services.Interface;
using Pligrimage.Web.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pligrimage.Web.Controllers
{
    public class UnitsController : BaseController
    {
        private readonly IUnitServcie        _unitService;
        private readonly IAlHajjMasterServcie _alhajjService;
        private readonly IAdminService       _adminService;
        private readonly IUnitOfWork         _unitOfWork;
        private readonly HajjSettings        _settings;

        public UnitsController(
            IUnitServcie        unitService,
            IAlHajjMasterServcie alhajjService,
            IAdminService       adminService,
            IUnitOfWork         unitOfWork,
            IOptions<HajjSettings> settings)
        {
            _unitService   = unitService;
            _alhajjService = alhajjService;
            _adminService  = adminService;
            _unitOfWork    = unitOfWork;
            _settings      = settings.Value;
        }

        // ── INDEX ──────────────────────────────────────────────────────────
        [PligrimageFiltter]
        public IActionResult Index() => View();

        // ── READ all units with live quota counts ──────────────────────────
        public IActionResult UnitRead()
        {
            int year = _settings.ActiveHajjYear;

            var units = _unitService.Queryable().ToList();

            // Count registered pilgrims per unit per type (non-deleted)
            var counts = _alhajjService.Queryable()
                .Where(c => c.AlhajYear == year && !c.IsDeleted)
                .GroupBy(c => new { c.UnitId, c.ParameterId })
                .Select(g => new { g.Key.UnitId, g.Key.ParameterId, Count = g.Count() })
                .ToList();

            var result = units.Select(u =>
            {
                int regularUsed  = counts.FirstOrDefault(c => c.UnitId == u.UnitId && c.ParameterId == HajjConstants.PilgrimType.Regular)?.Count  ?? 0;
                int standbyUsed  = counts.FirstOrDefault(c => c.UnitId == u.UnitId && c.ParameterId == HajjConstants.PilgrimType.StandBy)?.Count  ?? 0;
                int adminUsed    = counts.FirstOrDefault(c => c.UnitId == u.UnitId && c.ParameterId == HajjConstants.PilgrimType.Admin)?.Count    ?? 0;

                return new
                {
                    u.UnitId,
                    u.UnitNameAr,
                    u.UnitNameEn,
                    u.UnitCode,
                    u.UnitOrder,
                    u.ModFlag,
                    u.AllowNumber,
                    u.StandBy,
                    RegularUsed     = regularUsed,
                    StandByUsed     = standbyUsed,
                    AdminUsed       = adminUsed,
                    TotalUsed       = regularUsed + standbyUsed + adminUsed,
                    RegularRemain   = Math.Max(0, u.AllowNumber - regularUsed),
                    StandByRemain   = Math.Max(0, u.StandBy    - standbyUsed),
                    RegularPct      = u.AllowNumber > 0 ? (int)((double)regularUsed / u.AllowNumber * 100) : 0,
                    StandByPct      = u.StandBy     > 0 ? (int)((double)standbyUsed / u.StandBy     * 100) : 0,
                    RegularFull     = regularUsed >= u.AllowNumber,
                    StandByFull     = standbyUsed >= u.StandBy,
                };
            }).OrderBy(u => u.UnitOrder).ToList();

            return Json(result);
        }

        // ── QUOTA endpoint — called from registration screen ───────────────
        public IActionResult GetQuota(int unitId)
        {
            int year = _settings.ActiveHajjYear;

            var unit = _unitService.Queryable().FirstOrDefault(u => u.UnitId == unitId);
            if (unit == null) return NotFound();

            int regularUsed = _alhajjService.Queryable()
                .Count(c => c.UnitId == unitId && c.AlhajYear == year &&
                             c.ParameterId == HajjConstants.PilgrimType.Regular && !c.IsDeleted);

            int standbyUsed = _alhajjService.Queryable()
                .Count(c => c.UnitId == unitId && c.AlhajYear == year &&
                             c.ParameterId == HajjConstants.PilgrimType.StandBy && !c.IsDeleted);

            return Json(new
            {
                unitId,
                unitNameAr    = unit.UnitNameAr,
                allowNumber   = unit.AllowNumber,
                standBy       = unit.StandBy,
                regularUsed,
                standbyUsed,
                regularRemain = Math.Max(0, unit.AllowNumber - regularUsed),
                standbyRemain = Math.Max(0, unit.StandBy     - standbyUsed),
                regularFull   = regularUsed >= unit.AllowNumber,
                standbyFull   = standbyUsed >= unit.StandBy,
                regularPct    = unit.AllowNumber > 0 ? (int)((double)regularUsed / unit.AllowNumber * 100) : 0,
                standbyPct    = unit.StandBy     > 0 ? (int)((double)standbyUsed / unit.StandBy     * 100) : 0,
            });
        }

        // ── MY UNIT QUOTA — only the unit(s) the logged-in user belongs to ─
        /// <summary>
        /// Called from the registration screen.
        /// Returns quota info for the unit(s) the current officer manages.
        /// A unit-level officer sees only their own unit.
        /// SysAdmin sees all units.
        /// </summary>
        public IActionResult GetMyQuota()
        {
            int year = _settings.ActiveHajjYear;

            IQueryable<Unit> query;

            if (UserIsSysAdmin())
            {
                // SysAdmin sees everything
                query = _unitService.Queryable();
            }
            else
            {
                // Get unit codes the logged-in user is associated with
                var unitCodes = _adminService
                    .GetUserServiceListByUnitCode(LoggedUserName())
                    .ToList();

                query = _unitService.Queryable()
                    .Where(u => unitCodes.Contains(u.UnitCode));
            }

            var units = query.OrderBy(u => u.UnitOrder).ToList();

            var counts = _alhajjService.Queryable()
                .Where(c => c.AlhajYear == year && !c.IsDeleted)
                .GroupBy(c => new { c.UnitId, c.ParameterId })
                .Select(g => new { g.Key.UnitId, g.Key.ParameterId, Count = g.Count() })
                .ToList();

            var result = units.Select(u =>
            {
                int rUsed = counts.FirstOrDefault(c => c.UnitId == u.UnitId && c.ParameterId == HajjConstants.PilgrimType.Regular)?.Count ?? 0;
                int sUsed = counts.FirstOrDefault(c => c.UnitId == u.UnitId && c.ParameterId == HajjConstants.PilgrimType.StandBy)?.Count ?? 0;

                return new
                {
                    u.UnitId,
                    u.UnitNameAr,
                    u.AllowNumber,
                    u.StandBy,
                    RegularUsed     = rUsed,
                    StandByUsed     = sUsed,
                    RegularRemain   = Math.Max(0, u.AllowNumber - rUsed),
                    StandByRemain   = Math.Max(0, u.StandBy    - sUsed),
                    RegularPct      = u.AllowNumber > 0 ? (int)((double)rUsed / u.AllowNumber * 100) : 0,
                    StandByPct      = u.StandBy     > 0 ? (int)((double)sUsed / u.StandBy     * 100) : 0,
                    RegularFull     = rUsed >= u.AllowNumber,
                    StandByFull     = sUsed >= u.StandBy,
                };
            }).ToList();

            return Json(result);
        }

        // ── CREATE ─────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> CreateUnit(Unit unit)
        {
            if (unit == null || !ModelState.IsValid)
                return BadRequest("البيانات غير مكتملة");

            if (_unitService.Queryable().Any(u => u.UnitCode == unit.UnitCode))
                return BadRequest("كود الوحدة مستخدم مسبقاً");

            unit.CreateBy  = LoggedUserName();
            unit.CreateOn  = DateTime.Now;
            unit.AlhajYear = new DateTime(_settings.ActiveHajjYear, 1, 1);
            _unitService.Insert(unit);
            await _unitOfWork.SaveChangesAsync();
            return Ok(new { message = $"تم إضافة {unit.UnitNameAr} بنجاح", unit });
        }

        // ── UPDATE ─────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> UpdateUnit(Unit unit)
        {
            if (unit == null || !ModelState.IsValid)
                return BadRequest("البيانات غير صحيحة");

            var existing = _unitService.Queryable().FirstOrDefault(u => u.UnitId == unit.UnitId);
            if (existing == null) return NotFound("الوحدة غير موجودة");

            existing.UnitNameAr  = unit.UnitNameAr;
            existing.UnitNameEn  = unit.UnitNameEn;
            existing.UnitCode    = unit.UnitCode;
            existing.UnitOrder   = unit.UnitOrder;
            existing.ModFlag     = unit.ModFlag;
            existing.AllowNumber = unit.AllowNumber;
            existing.StandBy     = unit.StandBy;
            existing.UpdatedBy   = LoggedUserName();
            existing.UpdatedOn   = DateTime.Now;

            _unitService.Update(existing);
            await _unitOfWork.SaveChangesAsync();
            return Ok(new { message = $"تم تحديث {existing.UnitNameAr} بنجاح" });
        }

        // ── DELETE ─────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> DeleteUnit(int unitId)
        {
            var unit = _unitService.Queryable().FirstOrDefault(u => u.UnitId == unitId);
            if (unit == null) return NotFound();

            // Block delete if unit has registered pilgrims
            int count = _alhajjService.Queryable()
                .Count(c => c.UnitId == unitId && !c.IsDeleted);
            if (count > 0)
                return BadRequest($"لا يمكن حذف الوحدة — لديها {count} حاج مسجل");

            _unitService.Delete(unit);
            await _unitOfWork.SaveChangesAsync();
            return Ok(new { message = "تم الحذف بنجاح" });
        }
    }
}
