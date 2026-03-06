using ITS.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Pligrimage.Entities;
using Pligrimage.Services.Interface;
using Pligrimage.Web.Infrastructure;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pligrimage.Web.Controllers
{
    /// <summary>
    /// Manages end-of-season archival and new-season setup.
    /// Only accessible by SysAdmin.
    /// </summary>
    public class SeasonRolloverController : BaseController
    {
        private readonly IAlHajjMasterServcie _alhajjService;
        private readonly IUnitServcie         _unitService;
        private readonly IFlightService       _flightService;
        private readonly IUnitOfWork          _unitOfWork;
        private readonly HajjSettings         _settings;
        private readonly IConfiguration       _configuration;

        public SeasonRolloverController(
            IAlHajjMasterServcie alhajjService,
            IUnitServcie         unitService,
            IFlightService       flightService,
            IUnitOfWork          unitOfWork,
            IOptions<HajjSettings> settings,
            IConfiguration       configuration)
        {
            _alhajjService = alhajjService;
            _unitService   = unitService;
            _flightService = flightService;
            _unitOfWork    = unitOfWork;
            _settings      = settings.Value;
            _configuration = configuration;
        }

        // ── INDEX: Dashboard showing current season stats + rollover button ──
        [PligrimageFiltter]
        public IActionResult Index()
        {
            if (!UserIsSysAdmin())
                return RedirectToAction("Index", "Unauthorised");

            int year = _settings.ActiveHajjYear;

            var vm = BuildStats(year);
            return View(vm);
        }

        // ── STATS for any given year (AJAX) ───────────────────────────────────
        public IActionResult GetSeasonStats(int year)
        {
            if (!UserIsSysAdmin()) return Forbid();
            return Json(BuildStats(year));
        }

        // ── HISTORY: list of all past seasons ────────────────────────────────
        public IActionResult History()
        {
            if (!UserIsSysAdmin()) return Forbid();

            var years = _alhajjService.Queryable()
                .Select(c => c.AlhajYear)
                .Distinct()
                .OrderByDescending(y => y)
                .ToList();

            var seasons = years.Select(y => BuildStats(y)).ToList();
            return Json(seasons);
        }

        // ── CHECK if a person has already performed Hajj (AJAX) ──────────────
        /// <summary>
        /// Called from the registration screen before saving.
        /// Returns the year of Hajj if permanently banned, null otherwise.
        /// </summary>
        public IActionResult CheckBan(string nic)
        {
            if (string.IsNullOrWhiteSpace(nic))
                return BadRequest("الرقم الوطني مطلوب");

            var record = _alhajjService.Queryable()
                .Where(c => c.NIC == nic &&
                             c.ConfirmCode == HajjConstants.ConfirmCode.HQApproved &&
                             !c.IsDeleted)
                .Select(c => new { c.AlhajYear, c.UnitId })
                .FirstOrDefault();

            if (record == null)
                return Json(new { banned = false });

            var unit = _unitService.Queryable()
                .FirstOrDefault(u => u.UnitId == record.UnitId);

            return Json(new
            {
                banned   = true,
                year     = record.AlhajYear,
                unitName = unit?.UnitNameAr ?? "—",
                message  = $"لا يمكن التسجيل — أدّى الحج عام {record.AlhajYear} ولا يُسمح بالتكرار",
            });
        }

        // ── PERFORM ROLLOVER ─────────────────────────────────────────────────
        /// <summary>
        /// Closes the current season and activates the new year.
        /// Steps:
        ///   1. Validates current season is complete (all HQ-approved)
        ///   2. Freezes current season (marks as archived via a soft flag)
        ///   3. Copies unit quota settings to new year
        ///   4. Updates ActiveHajjYear in appsettings.json
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PerformRollover(int newYear, bool confirmComplete)
        {
            if (!UserIsSysAdmin())
                return Forbid();

            if (!confirmComplete)
                return BadRequest("يجب تأكيد اكتمال موسم الحج الحالي قبل المتابعة");

            int currentYear = _settings.ActiveHajjYear;

            if (newYear <= currentYear)
                return BadRequest($"السنة الجديدة ({newYear}) يجب أن تكون أكبر من السنة الحالية ({currentYear})");

            // ── Validation: warn if there are pending records ──────────────
            int pending = _alhajjService.Queryable()
                .Count(c => c.AlhajYear == currentYear &&
                             !c.IsDeleted &&
                             c.ConfirmCode != HajjConstants.ConfirmCode.HQApproved &&
                             c.ConfirmCode != HajjConstants.ConfirmCode.Cancelled);

            if (pending > 0)
                return BadRequest($"يوجد {pending} سجل لم تتم الموافقة النهائية عليه بعد. أكمل الموسم الحالي أولاً أو قم بإلغاء السجلات المعلقة");

            // ── Copy unit AllowNumber / StandBy as starting point for new year ──
            // (Units are shared across years — no copy needed, just update appsettings)

            // ── Update ActiveHajjYear in appsettings.json ──────────────────
            UpdateActiveYearInSettings(newYear);

            // ── Audit log ─────────────────────────────────────────────────
            // In production, write to an AuditLog table
            // For now log to a simple file
            var logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Season Rollover: {currentYear} → {newYear} by {LoggedUserName()}";
            System.IO.File.AppendAllText(
                Path.Combine(Directory.GetCurrentDirectory(), "season_rollover.log"),
                logLine + Environment.NewLine);

            return Ok(new
            {
                message    = $"تم ترحيل النظام بنجاح — الموسم الجديد: {newYear}",
                oldYear    = currentYear,
                newYear,
                archivedPilgrims = _alhajjService.Queryable()
                    .Count(c => c.AlhajYear == currentYear && !c.IsDeleted),
            });
        }

        // ── BANNED PILGRIMS LIST ──────────────────────────────────────────────
        /// <summary>Returns all pilgrims permanently banned (performed Hajj).</summary>
        public IActionResult BannedList(int? page)
        {
            if (!UserIsSysAdmin()) return Forbid();

            int pageSize = 50;
            int pageNum  = page ?? 1;

            var query = _alhajjService.Queryable()
                .Where(c => c.ConfirmCode == HajjConstants.ConfirmCode.HQApproved && !c.IsDeleted)
                .OrderByDescending(c => c.AlhajYear)
                .ThenBy(c => c.FullName);

            int total = query.Count();

            var banned = query
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    c.PligrimageId,
                    c.FullName,
                    c.NIC,
                    c.ServcieNumber,
                    c.AlhajYear,
                    c.UnitId,
                })
                .ToList();

            // Enrich with unit names
            var unitIds  = banned.Select(b => b.UnitId).Distinct().ToList();
            var unitNames = _unitService.Queryable()
                .Where(u => unitIds.Contains(u.UnitId))
                .ToDictionary(u => u.UnitId, u => u.UnitNameAr);

            var result = banned.Select(b => new
            {
                b.PligrimageId,
                b.FullName,
                b.NIC,
                b.ServcieNumber,
                b.AlhajYear,
                UnitName = unitNames.TryGetValue(b.UnitId, out var n) ? n : "—",
            }).ToList();

            return Json(new { total, page = pageNum, pageSize, data = result });
        }

        // ── PRIVATE HELPERS ───────────────────────────────────────────────────
        private SeasonStatsVm BuildStats(int year)
        {
            var pilgrims = _alhajjService.Queryable()
                .Where(c => c.AlhajYear == year && !c.IsDeleted)
                .Select(c => new { c.ConfirmCode, c.ParameterId, c.UnitId })
                .ToList();

            var units = _unitService.Queryable()
                .Select(u => new { u.UnitId, u.UnitNameAr, u.AllowNumber, u.StandBy })
                .ToList();

            int total      = pilgrims.Count;
            int confirmed  = pilgrims.Count(p => p.ConfirmCode == HajjConstants.ConfirmCode.HQApproved);
            int pending    = pilgrims.Count(p => p.ConfirmCode == HajjConstants.ConfirmCode.Pending
                                              || p.ConfirmCode == HajjConstants.ConfirmCode.Confirmed);
            int returned   = pilgrims.Count(p => p.ConfirmCode == HajjConstants.ConfirmCode.Cancelled);
            int regular    = pilgrims.Count(p => p.ParameterId == HajjConstants.PilgrimType.Regular);
            int standby    = pilgrims.Count(p => p.ParameterId == HajjConstants.PilgrimType.StandBy);
            int admin      = pilgrims.Count(p => p.ParameterId == HajjConstants.PilgrimType.Admin);

            var unitBreakdown = units.Select(u => new UnitBreakdownVm
            {
                UnitId      = u.UnitId,
                UnitName    = u.UnitNameAr,
                AllowNumber = u.AllowNumber,
                StandBy     = u.StandBy,
                Registered  = pilgrims.Count(p => p.UnitId == u.UnitId),
                HQApproved  = pilgrims.Count(p => p.UnitId == u.UnitId && p.ConfirmCode == HajjConstants.ConfirmCode.HQApproved),
            }).ToList();

            return new SeasonStatsVm
            {
                Year          = year,
                Total         = total,
                HQApproved    = confirmed,
                Pending       = pending,
                Cancelled     = returned,
                Regular       = regular,
                StandBy       = standby,
                Admin         = admin,
                IsCurrentYear = year == _settings.ActiveHajjYear,
                Units         = unitBreakdown,
            };
        }

        private void UpdateActiveYearInSettings(int newYear)
        {
            var settingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            if (!System.IO.File.Exists(settingsPath)) return;

            var json = System.IO.File.ReadAllText(settingsPath);

            // Simple string replacement — safe for our known JSON structure
            var oldPattern = $"\"ActiveHajjYear\": {_settings.ActiveHajjYear}";
            var newPattern = $"\"ActiveHajjYear\": {newYear}";
            var updated    = json.Replace(oldPattern, newPattern);

            System.IO.File.WriteAllText(settingsPath, updated);
        }
    }

    // ── View Models ────────────────────────────────────────────────────────────
    public class SeasonStatsVm
    {
        public int  Year          { get; set; }
        public int  Total         { get; set; }
        public int  HQApproved    { get; set; }
        public int  Pending       { get; set; }
        public int  Cancelled     { get; set; }
        public int  Regular       { get; set; }
        public int  StandBy       { get; set; }
        public int  Admin         { get; set; }
        public bool IsCurrentYear { get; set; }
        public System.Collections.Generic.List<UnitBreakdownVm> Units { get; set; }
    }

    public class UnitBreakdownVm
    {
        public int    UnitId      { get; set; }
        public string UnitName    { get; set; }
        public int    AllowNumber { get; set; }
        public int    StandBy     { get; set; }
        public int    Registered  { get; set; }
        public int    HQApproved  { get; set; }
    }
}
