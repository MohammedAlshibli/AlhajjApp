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
    /// Two-flight distribution with linked return rule:
    ///   • Departure F1 → Return F2 (mandatory)
    ///   • Departure F2 → Return F1 (mandatory)
    ///   • Admins (ParameterId=3) must always be in Departure F1
    ///   • Admin can override individual assignments
    /// </summary>
    public class FlightDistributionController : BaseController
    {
        private readonly IAlHajjMasterServcie _alhajjService;
        private readonly IFlightServcie        _flightService;
        private readonly IPassengerService     _passengerService;
        private readonly IUnitOfWork           _unitOfWork;
        private readonly HajjSettings          _settings;

        public FlightDistributionController(
            IAlHajjMasterServcie alhajjService,
            IFlightServcie        flightService,
            IPassengerService     passengerService,
            IUnitOfWork           unitOfWork,
            IOptions<HajjSettings> settings)
        {
            _alhajjService    = alhajjService;
            _flightService    = flightService;
            _passengerService = passengerService;
            _unitOfWork       = unitOfWork;
            _settings         = settings.Value;
        }

        // ── INDEX ─────────────────────────────────────────────────────────
        public IActionResult Index()
        {
            int year = _settings.ActiveHajjYear;

            // Expect exactly 2 departure flights and 2 return flights for the year
            var flights = _flightService.Queryable()
                .Where(f => f.FlightYear == year)
                .OrderBy(f => f.ParameterId) // departure=34 before return=35
                .ThenBy(f => f.FlightDate)
                .Select(f => new {
                    f.FlightId, f.FlightNo, f.FlightDate,
                    f.FlightCapacity, f.Direction, f.ParameterId
                })
                .ToList();

            ViewData["Flights"] = flights;
            return View();
        }

        // ── ELIGIBLE: medically approved, not yet assigned ────────────────
        public IActionResult GetEligible()
        {
            int year = _settings.ActiveHajjYear;

            var assigned = _passengerService.Queryable()
                .Include(p => p.Flight)
                .Where(p => p.AlhajYear == year &&
                             p.Flight.ParameterId == HajjConstants.FlightDirection.Departure)
                .Select(p => p.PligrimageId)
                .Distinct()
                .ToList();

            var pilgrims = _alhajjService.Queryable()
                .Include(c => c.Unit)
                .Where(c => c.AlhajYear   == year &&
                             c.FitResult   == HajjConstants.FitResult.DoctorApproved &&
                             c.ConfirmCode == HajjConstants.ConfirmCode.HQApproved)
                .Select(c => new {
                    c.PligrimageId,
                    c.FullName,
                    c.ServcieNumber,
                    c.RankDesc,
                    c.ParameterId,               // 1=regular,2=standby,3=admin
                    UnitNameAr = c.Unit != null ? c.Unit.UnitNameAr : "",
                    c.BloodGroup,
                    IsAdmin = c.ParameterId == HajjConstants.PilgrimType.Admin,
                    AlreadyAssigned = assigned.Contains(c.PligrimageId)
                })
                .ToList();

            return Json(pilgrims);
        }

        // ── CURRENT DISTRIBUTION (what's saved in DB) ─────────────────────
        public IActionResult GetDistribution()
        {
            int year = _settings.ActiveHajjYear;

            var passengers = _passengerService.Queryable()
                .Include(p => p.Flight)
                .Include(p => p.AlhajjMaster).ThenInclude(m => m.Unit)
                .Where(p => p.AlhajYear == year)
                .Select(p => new {
                    p.PassengerId,
                    p.PligrimageId,
                    Name       = p.AlhajjMaster.FullName,
                    Rank       = p.AlhajjMaster.RankDesc,
                    Unit       = p.AlhajjMaster.Unit != null ? p.AlhajjMaster.Unit.UnitNameAr : "",
                    IsAdmin    = p.AlhajjMaster.ParameterId == HajjConstants.PilgrimType.Admin,
                    p.FlightId,
                    FlightNo   = p.Flight.FlightNo,
                    Direction  = p.Flight.ParameterId   // 34=depart,35=return
                })
                .ToList();

            return Json(passengers);
        }

        // ── FLIGHT STATS ──────────────────────────────────────────────────
        public IActionResult GetStats()
        {
            int year = _settings.ActiveHajjYear;

            var flights = _flightService.Queryable()
                .Where(f => f.FlightYear == year)
                .Select(f => new { f.FlightId, f.FlightNo, f.FlightCapacity, f.ParameterId })
                .ToList();

            var counts = _passengerService.Queryable()
                .Where(p => p.AlhajYear == year)
                .GroupBy(p => p.FlightId)
                .Select(g => new { flightId = g.Key, count = g.Count() })
                .ToList();

            int eligible = _alhajjService.Queryable()
                .Count(c => c.AlhajYear   == year &&
                             c.FitResult   == HajjConstants.FitResult.DoctorApproved &&
                             c.ConfirmCode == HajjConstants.ConfirmCode.HQApproved);

            int assigned = _passengerService.Queryable()
                .Where(p => p.AlhajYear == year)
                .Select(p => p.PligrimageId)
                .Distinct()
                .Count();

            return Json(new {
                eligible,
                assigned,
                unassigned = eligible - assigned,
                flights = flights.Select(f => new {
                    f.FlightId, f.FlightNo, f.FlightCapacity, f.ParameterId,
                    used      = counts.FirstOrDefault(c => c.flightId == f.FlightId)?.count ?? 0,
                    remaining = f.FlightCapacity - (counts.FirstOrDefault(c => c.flightId == f.FlightId)?.count ?? 0)
                })
            });
        }

        // ── AUTO-DISTRIBUTE ───────────────────────────────────────────────
        /// <summary>
        /// Rules:
        ///  1. Admins → always Departure F1
        ///  2. Fill F1 departure to half capacity (or less), rest to F2
        ///  3. Link return: dep-F1 → ret-F2, dep-F2 → ret-F1
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AutoDistribute()
        {
            int year = _settings.ActiveHajjYear;

            var flights = _flightService.Queryable()
                .Where(f => f.FlightYear == year)
                .OrderBy(f => f.FlightDate)
                .ToList();

            var depFlights = flights.Where(f => f.ParameterId == HajjConstants.FlightDirection.Departure)
                .OrderBy(f => f.FlightDate).ToList();
            var retFlights = flights.Where(f => f.ParameterId == HajjConstants.FlightDirection.Return)
                .OrderByDescending(f => f.FlightDate).ToList(); // F1-dep → F2-ret (later return)

            if (depFlights.Count < 2 || retFlights.Count < 2)
                return BadRequest("يجب تسجيل رحلتي ذهاب ورحلتي عودة في النظام أولاً");

            var dep1 = depFlights[0]; // Departure Flight 1 (earlier)
            var dep2 = depFlights[1]; // Departure Flight 2 (later)
            var ret1 = retFlights[0]; // Return  Flight 1 (earlier — for dep2 passengers)
            var ret2 = retFlights[1]; // Return  Flight 2 (later  — for dep1 passengers)

            // Remove existing assignments for this year
            var existing = _passengerService.Queryable()
                .Where(p => p.AlhajYear == year).ToList();
            foreach (var p in existing) _passengerService.Delete(p);

            // Eligible pilgrims
            var pilgrims = _alhajjService.Queryable()
                .Include(c => c.Unit)
                .Where(c => c.AlhajYear   == year &&
                             c.FitResult   == HajjConstants.FitResult.DoctorApproved &&
                             c.ConfirmCode == HajjConstants.ConfirmCode.HQApproved)
                .OrderByDescending(c => c.ParameterId == HajjConstants.PilgrimType.Admin) // admins first
                .ThenBy(c => c.RegistrationDate)
                .ToList();

            if (!pilgrims.Any()) return BadRequest("لا يوجد حجاج لائقون طبياً");

            int half      = (int)Math.Ceiling(dep1.FlightCapacity / 2.0);
            var dep1List  = new List<AlhajjMaster>();
            var dep2List  = new List<AlhajjMaster>();

            foreach (var pilgrim in pilgrims)
            {
                bool isAdmin = pilgrim.ParameterId == HajjConstants.PilgrimType.Admin;

                if (isAdmin || dep1List.Count < half)
                    dep1List.Add(pilgrim);  // Admins + first-half → dep1
                else
                    dep2List.Add(pilgrim);  // Rest → dep2
            }

            using var tx = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Departure F1 + Return F2
                foreach (var p in dep1List)
                {
                    var depP = new Passenger { PligrimageId=p.PligrimageId, FlightId=dep1.FlightId, BusId=1, ResidencesId=1, AlhajYear=year };
                    var retP = new Passenger { PligrimageId=p.PligrimageId, FlightId=ret2.FlightId, BusId=1, ResidencesId=1, AlhajYear=year };
                    StampNew(depP); StampNew(retP);
                    _passengerService.Insert(depP);
                    _passengerService.Insert(retP);
                }
                // Departure F2 + Return F1
                foreach (var p in dep2List)
                {
                    var depP = new Passenger { PligrimageId=p.PligrimageId, FlightId=dep2.FlightId, BusId=1, ResidencesId=1, AlhajYear=year };
                    var retP = new Passenger { PligrimageId=p.PligrimageId, FlightId=ret1.FlightId, BusId=1, ResidencesId=1, AlhajYear=year };
                    StampNew(depP); StampNew(retP);
                    _passengerService.Insert(depP);
                    _passengerService.Insert(retP);
                }

                await _unitOfWork.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(new {
                    message = $"تم التوزيع: {dep1List.Count} في الرحلة الأولى، {dep2List.Count} في الرحلة الثانية",
                    f1Count = dep1List.Count,
                    f2Count = dep2List.Count
                });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return BadRequest($"فشل التوزيع: {ex.Message}");
            }
        }

        // ── MOVE PILGRIM BETWEEN FLIGHTS (manual override) ────────────────
        /// <summary>
        /// Move a pilgrim from one departure flight to the other.
        /// Automatically flips their return flight too (linkage rule).
        /// Validates: admin cannot leave F1, capacity must not be exceeded.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> MovePilgrim(int pligrimageId, int targetDepFlightId)
        {
            int year = _settings.ActiveHajjYear;

            var pilgrim = _alhajjService.Queryable()
                .FirstOrDefault(c => c.PligrimageId == pligrimageId);
            if (pilgrim == null) return NotFound("الحاج غير موجود");

            // Block moving admins off F1
            if (pilgrim.ParameterId == HajjConstants.PilgrimType.Admin)
            {
                var depFlights = _flightService.Queryable()
                    .Where(f => f.FlightYear == year && f.ParameterId == HajjConstants.FlightDirection.Departure)
                    .OrderBy(f => f.FlightDate).ToList();

                if (depFlights.Any() && targetDepFlightId != depFlights[0].FlightId)
                    return BadRequest("الحجاج الإداريون يجب أن يكونوا دائماً في الرحلة الأولى");
            }

            // Check capacity
            var targetFlight = _flightService.FindAsync(targetDepFlightId);
            if (targetFlight == null) return NotFound("الرحلة غير موجودة");

            int currentCount = _passengerService.Queryable()
                .Count(p => p.FlightId == targetDepFlightId && p.AlhajYear == year);
            if (currentCount >= targetFlight.Result.FlightCapacity)
                return BadRequest($"الرحلة ممتلئة — السعة الكاملة {targetFlight.Result.FlightCapacity}");

            // Find linked return flight (opposite)
            var allFlights = _flightService.Queryable()
                .Where(f => f.FlightYear == year).OrderBy(f => f.FlightDate).ToList();
            var depFlights2 = allFlights.Where(f => f.ParameterId == HajjConstants.FlightDirection.Departure)
                .OrderBy(f => f.FlightDate).ToList();
            var retFlights2 = allFlights.Where(f => f.ParameterId == HajjConstants.FlightDirection.Return)
                .OrderBy(f => f.FlightDate).ToList();

            // dep[0]→ret[1], dep[1]→ret[0]
            int targetRetFlightId = targetDepFlightId == depFlights2[0].FlightId
                ? retFlights2[1].FlightId
                : retFlights2[0].FlightId;

            // Update departure + return passengers
            var depPassenger = _passengerService.Queryable()
                .FirstOrDefault(p => p.PligrimageId == pligrimageId &&
                                      p.AlhajYear    == year &&
                                      depFlights2.Select(d => d.FlightId).Contains(p.FlightId));

            var retPassenger = _passengerService.Queryable()
                .FirstOrDefault(p => p.PligrimageId == pligrimageId &&
                                      p.AlhajYear    == year &&
                                      retFlights2.Select(r => r.FlightId).Contains(p.FlightId));

            if (depPassenger == null) return BadRequest("السجل غير موجود في قائمة الرحلات");

            depPassenger.FlightId = targetDepFlightId;
            StampUpdate(depPassenger);
            _passengerService.Update(depPassenger);

            if (retPassenger != null)
            {
                retPassenger.FlightId = targetRetFlightId;
                StampUpdate(retPassenger);
                _passengerService.Update(retPassenger);
            }

            await _unitOfWork.SaveChangesAsync();

            return Ok(new {
                message = $"تم نقل {pilgrim.FullName} — الذهاب والعودة تم تعديلهما تلقائياً",
                pligrimageId,
                newDepFlightId = targetDepFlightId,
                newRetFlightId = targetRetFlightId
            });
        }

        // ── CLEAR ALL ─────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> ClearAll()
        {
            int year = _settings.ActiveHajjYear;
            var all = _passengerService.Queryable()
                .Where(p => p.AlhajYear == year).ToList();
            foreach (var p in all) _passengerService.Delete(p);
            await _unitOfWork.SaveChangesAsync();
            return Ok(new { message = $"تم مسح توزيع {all.Count} سجل", count = all.Count });
        }
    }
}
