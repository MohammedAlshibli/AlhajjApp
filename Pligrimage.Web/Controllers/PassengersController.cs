using ITS.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Pligrimage.Entities;
using Pligrimage.Services.Interface;
using Pligrimage.Web.Extensions;
using Pligrimage.Web.Infrastructure;
using Pligrimage.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pligrimage.Web.Controllers
{
    public class PassengersController : BaseController
    {
        private readonly IPassengerService _passengerRepository;
        private readonly IFlightServcie _flightRepository;
        private readonly IBusServcie _busRepository;
        private readonly IAlHajjMasterServcie _alHajjRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IParameterService _parameterRepository;
        private readonly IResidenceService _residenceRepository;
        private readonly HajjSettings _settings;

        public PassengersController(
            IPassengerService passengerRepository,
            IParameterService parameterRepository,
            IAlHajjMasterServcie alHajjMasterRepository,
            IFlightServcie flightRepository,
            IBusServcie busRepository,
            IUnitOfWork unitOfWork,
            IResidenceService residenceRepository,
            IOptions<HajjSettings> settings)
        {
            _passengerRepository = passengerRepository;
            _alHajjRepository    = alHajjMasterRepository;
            _busRepository       = busRepository;
            _flightRepository    = flightRepository;
            _unitOfWork          = unitOfWork;
            _parameterRepository = parameterRepository;
            _residenceRepository = residenceRepository;
            _settings            = settings.Value;
        }

        [PligrimageFiltter]
        public IActionResult Index()
        {
            ViewData["FlightType"] = _flightRepository.Queryable()
                .Select(c => new { c.FlightId, c.FlightNo }).ToList();

            ViewData["BusList"] = _busRepository.Queryable()
                .Select(c => new { c.BusId, c.BusNo }).ToList();

            return View();
        }

        [PligrimageFiltter]
        public IActionResult SwapFlight() => View();

        // ────────────────────────────────────────────────────────────────────
        // FLIGHT AUTO-ASSIGNMENT (DEPARTURE)
        // BUG-FIX #3: no more hardcoded flight/bus IDs – uses year + ParameterId
        // BUG-FIX #7: fixed bus overflow index crash
        // BUG-FIX #23: single transaction wrapping entire assignment
        // BUG-FIX #13: capacity enforcement
        // ────────────────────────────────────────────────────────────────────
        public async Task<ActionResult> AlhajjFlightDepart()
        {
            int activeYear = _settings.ActiveHajjYear;

            // Eligible pilgrims: regular or admin type, doctor-approved, not deleted, not yet assigned
            int[] eligibleTypes = { HajjConstants.PilgrimType.Regular, HajjConstants.PilgrimType.Admin };

            var eligiblePilgrims = _alHajjRepository.Queryable()
                .Where(c => eligibleTypes.Contains(c.ParameterId) &&
                             c.FitResult == HajjConstants.FitResult.DoctorApproved &&
                             c.AlhajYear == activeYear &&
                             !c.IsDeleted)
                .ToList();

            if (!eligiblePilgrims.Any())
                return BadRequest("لا يوجد حجاج مؤهلون للتسجيل في الرحلات");

            // Departure flights for the active year - BUG-FIX #3: no hardcoded IDs
            var departureFlights = _flightRepository.Queryable()
                .Include(c => c.buses)
                .Where(c => c.ParameterId == HajjConstants.FlightDirection.Departure &&
                             c.FlightYear == activeYear)
                .OrderBy(c => c.FlightDate)
                .ToList();

            if (!departureFlights.Any())
                return BadRequest($"لا توجد رحلات ذهاب مسجلة لعام {activeYear}");

            // Already-assigned pilgrim IDs for departure flights
            var alreadyAssignedIds = _passengerRepository.Queryable()
                .Include(c => c.Flight)
                .Where(c => c.Flight.ParameterId == HajjConstants.FlightDirection.Departure &&
                             c.Flight.FlightYear == activeYear)
                .Select(c => c.PligrimageId)
                .ToHashSet();

            var unassigned = eligiblePilgrims
                .Where(c => !alreadyAssignedIds.Contains(c.PligrimageId))
                .OrderBy(_ => Guid.NewGuid()) // random assignment
                .ToList();

            if (!unassigned.Any())
                return Ok("جميع الحجاج المؤهلين تم تعيينهم مسبقاً");

            using var tx = await _unitOfWork.BeginTransactionAsync();
            try
            {
                int pilgrimIndex = 0;

                foreach (var flight in departureFlights)
                {
                    if (pilgrimIndex >= unassigned.Count) break;

                    var buses = flight.buses?.OrderBy(b => b.BusNo).ToList() ?? new List<Buses>();
                    if (!buses.Any())
                        continue;

                    // Count how many seats this flight still has
                    int flightSeatsUsed = _passengerRepository.Queryable()
                        .Count(c => c.FlightId == flight.FlightId);

                    int flightSeatsAvail = flight.FlightCapacity - flightSeatsUsed;
                    if (flightSeatsAvail <= 0) continue;

                    int busIndex = 0;

                    while (pilgrimIndex < unassigned.Count && flightSeatsAvail > 0)
                    {
                        // BUG-FIX #7: proper bounds check before array access
                        if (busIndex >= buses.Count)
                            break; // all buses on this flight are full

                        var currentBus = buses[busIndex];

                        int busSeatsUsed = _passengerRepository.Queryable()
                            .Count(c => c.BusId == currentBus.BusId);

                        if (busSeatsUsed >= currentBus.BusCapacity)
                        {
                            busIndex++;
                            continue;
                        }

                        var pilgrim = unassigned[pilgrimIndex];

                        var passenger = new Passenger
                        {
                            CreateBy     = LoggedUserName(),
                            CreateOn     = DateTime.Now,
                            PligrimageId = pilgrim.PligrimageId,
                            FlightId     = flight.FlightId,
                            BusId        = currentBus.BusId,
                            ResidencesId = 1, // TODO: replace with real accommodation assignment screen
                            AlhajYear    = activeYear
                        };

                        _passengerRepository.Insert(passenger);
                        pilgrimIndex++;
                        flightSeatsAvail--;
                    }
                }

                // BUG-FIX #1 + #23: single awaited save inside transaction
                await _unitOfWork.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok($"تم تعيين {pilgrimIndex} حاج على رحلات الذهاب");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return BadRequest($"فشل التعيين: {ex.Message}");
            }
        }

        // ────────────────────────────────────────────────────────────────────
        // FLIGHT AUTO-ASSIGNMENT (RETURN)
        // BUG-FIX #3, #8: same fixes as departure, bus assignment now works
        // ────────────────────────────────────────────────────────────────────
        public async Task<ActionResult> AlhajjFlightReturn()
        {
            int activeYear = _settings.ActiveHajjYear;

            int[] eligibleTypes = { HajjConstants.PilgrimType.Regular, HajjConstants.PilgrimType.Admin };

            var eligiblePilgrims = _alHajjRepository.Queryable()
                .Where(c => eligibleTypes.Contains(c.ParameterId) &&
                             c.FitResult == HajjConstants.FitResult.DoctorApproved &&
                             c.AlhajYear == activeYear &&
                             !c.IsDeleted)
                .ToList();

            var returnFlights = _flightRepository.Queryable()
                .Include(c => c.buses)
                .Where(c => c.ParameterId == HajjConstants.FlightDirection.Return &&
                             c.FlightYear == activeYear)
                .OrderBy(c => c.FlightDate)
                .ToList();

            if (!returnFlights.Any())
                return BadRequest($"لا توجد رحلات عودة مسجلة لعام {activeYear}");

            var alreadyAssignedIds = _passengerRepository.Queryable()
                .Include(c => c.Flight)
                .Where(c => c.Flight.ParameterId == HajjConstants.FlightDirection.Return &&
                             c.Flight.FlightYear == activeYear)
                .Select(c => c.PligrimageId)
                .ToHashSet();

            var unassigned = eligiblePilgrims
                .Where(c => !alreadyAssignedIds.Contains(c.PligrimageId))
                .OrderBy(_ => Guid.NewGuid())
                .ToList();

            if (!unassigned.Any())
                return Ok("جميع الحجاج المؤهلين تم تعيينهم على رحلات العودة");

            using var tx = await _unitOfWork.BeginTransactionAsync();
            try
            {
                int pilgrimIndex = 0;

                foreach (var flight in returnFlights)
                {
                    if (pilgrimIndex >= unassigned.Count) break;

                    var buses = flight.buses?.OrderBy(b => b.BusNo).ToList() ?? new List<Buses>();
                    if (!buses.Any()) continue;

                    int flightSeatsUsed  = _passengerRepository.Queryable().Count(c => c.FlightId == flight.FlightId);
                    int flightSeatsAvail = flight.FlightCapacity - flightSeatsUsed;
                    if (flightSeatsAvail <= 0) continue;

                    int busIndex = 0;

                    while (pilgrimIndex < unassigned.Count && flightSeatsAvail > 0)
                    {
                        if (busIndex >= buses.Count) break;

                        var currentBus   = buses[busIndex];
                        int busSeatsUsed = _passengerRepository.Queryable().Count(c => c.BusId == currentBus.BusId);

                        if (busSeatsUsed >= currentBus.BusCapacity) { busIndex++; continue; }

                        var passenger = new Passenger
                        {
                            CreateBy     = LoggedUserName(),
                            CreateOn     = DateTime.Now,
                            PligrimageId = unassigned[pilgrimIndex].PligrimageId,
                            FlightId     = flight.FlightId,
                            BusId        = currentBus.BusId,
                            ResidencesId = 1,
                            AlhajYear    = activeYear
                        };

                        _passengerRepository.Insert(passenger);
                        pilgrimIndex++;
                        flightSeatsAvail--;
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await tx.CommitAsync();
                return Ok($"تم تعيين {pilgrimIndex} حاج على رحلات العودة");
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return BadRequest($"فشل التعيين: {ex.Message}");
            }
        }

        // ────────────────────────────────────────────────────────────────────
        // MANUAL ASSIGNMENT – BUG-FIX #13: enforce capacity
        // ────────────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<ActionResult> PassengerPost(PassengerViewModel passengerViewModel)
        {
            var alhajjMasterList = JsonConvert.DeserializeObject<List<AlhajjMaster>>(
                Request.Form["AlhajjsList"]);

            passengerViewModel.AlhajjsList = alhajjMasterList;

            // BUG-FIX #13: check flight capacity before inserting
            var flight = _flightRepository.Queryable()
                .FirstOrDefault(f => f.FlightId == passengerViewModel.FlightId);

            if (flight == null)
                return BadRequest("الرحلة غير موجودة");

            int currentCount = _passengerRepository.Queryable()
                .Count(p => p.FlightId == passengerViewModel.FlightId);

            int newCount = alhajjMasterList?.Count ?? 0;

            if (currentCount + newCount > flight.FlightCapacity)
                return BadRequest($"الرحلة ممتلئة. السعة: {flight.FlightCapacity}, المسجلون: {currentCount}, الإضافة المطلوبة: {newCount}");

            using var tx = await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var item in passengerViewModel.AlhajjsList)
                {
                    _passengerRepository.Insert(new Passenger
                    {
                        PligrimageId = item.PligrimageId,
                        BusId        = passengerViewModel.BusId,
                        FlightId     = passengerViewModel.FlightId,
                        ResidencesId = passengerViewModel.ResidencesId,
                        CreateBy     = LoggedUserName(),
                        CreateOn     = DateTime.Now,
                        AlhajYear    = _settings.ActiveHajjYear
                    });
                }

                await _unitOfWork.SaveChangesAsync(); // BUG-FIX #1
                await tx.CommitAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }

        // ────────────────────────────────────────────────────────────────────
        // SWAP – BUG-FIX #9: was returning null
        // ────────────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> PassengerUpdateSwap(string swapListParam)
        {
            var swapList = JsonConvert.DeserializeObject<List<SwapVM>>(swapListParam);

            if (swapList == null || !swapList.Any())
                return BadRequest("بيانات الإبدال فارغة");

            using var tx = await _unitOfWork.BeginTransactionAsync();
            try
            {
                foreach (var item in swapList)
                {
                    var passenger = _passengerRepository.Queryable()
                        .FirstOrDefault(p => p.PassengerId == item.PassengerId);

                    if (passenger == null) continue;

                    passenger.FlightId  = item.FlightId;
                    passenger.UpdatedBy = LoggedUserName();
                    passenger.UpdatedOn = DateTime.Now;
                    _passengerRepository.Update(passenger);
                }

                await _unitOfWork.SaveChangesAsync(); // BUG-FIX #1
                await tx.CommitAsync();
                return Ok(new { success = true }); // BUG-FIX #9: no longer null
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateSwap(SwapVM p1, SwapVM p2)
        {
            if (!ModelState.IsValid)
                return BadRequest("بيانات الإبدال غير صحيحة");

            using var tx = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var person1 = _passengerRepository.Queryable()
                    .FirstOrDefault(c => c.PassengerId == p1.PassengerId);
                var person2 = _passengerRepository.Queryable()
                    .FirstOrDefault(c => c.PassengerId == p2.PassengerId);

                if (person1 == null || person2 == null)
                    return NotFound("أحد المسافرين غير موجود");

                (person1.FlightId, person2.FlightId) = (person2.FlightId, person1.FlightId);
                (person1.BusId,    person2.BusId)    = (person2.BusId,    person1.BusId);

                person1.UpdatedBy = person2.UpdatedBy = LoggedUserName();
                person1.UpdatedOn = person2.UpdatedOn = DateTime.Now;

                _passengerRepository.Update(person1);
                _passengerRepository.Update(person2);

                await _unitOfWork.SaveChangesAsync(); // BUG-FIX #1
                await tx.CommitAsync();

                // Reflect swapped values back for client
                (p1.FlightNo, p2.FlightNo) = (p2.FlightNo, p1.FlightNo);
                (p1.BusNo,    p2.BusNo)    = (p2.BusNo,    p1.BusNo);

                return Ok(new { p1, p2 });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> UpdatePassenger(Passenger passenger)
        {
            if (passenger == null || !ModelState.IsValid)
                return BadRequest();
            passenger.UpdatedBy = LoggedUserName();
            passenger.UpdatedOn = DateTime.Now;
            _passengerRepository.Update(passenger);
            await _unitOfWork.SaveChangesAsync(); // BUG-FIX #1
            return RedirectToAction("Index");
        }

        // ── Read endpoints ─────────────────────────────────────────────────
        public IActionResult PassengerRead()
        {
            int activeYear = _settings.ActiveHajjYear;
            var result = _passengerRepository.Queryable()
                .Include(c => c.AlhajjMaster)
                .Include(c => c.Flight.Parameter)
                .Include(c => c.Buses)
                .Where(c => c.AlhajYear == activeYear) // BUG-FIX #11
                .Select(c => new {
                    c.PassengerId,
                    c.PligrimageId,
                    FullName       = c.AlhajjMaster.FullName,
                    ServcieNumber  = c.AlhajjMaster.ServcieNumber,
                    FlightNo       = c.Flight.FlightNo,
                    FlightType     = c.Flight.Parameter.DescArabic,
                    BusNo          = c.Buses.BusNo,
                    c.FlightId,
                    c.BusId
                })
                .ToList();
            return Json(result);
        }

        public IActionResult PassengerReadSwap(int flightID)
        {
            var result = _passengerRepository.Queryable()
                .Include(c => c.AlhajjMaster)
                .Include(c => c.Flight.Parameter)
                .Include(c => c.Buses)
                .Where(c => c.FlightId == flightID)
                .Select(c => new SwapVM {
                    PassengerId   = c.PassengerId,
                    PligrimageId  = c.PligrimageId,
                    ServcieNumber = c.AlhajjMaster.ServcieNumber,
                    FullName      = c.AlhajjMaster.FullName,
                    FlightId      = c.FlightId,
                    FlightNo      = c.Flight.FlightNo
                }).ToList();
            return Json(result);
        }

        public IActionResult FlightList()
        {
            int activeYear = _settings.ActiveHajjYear;
            var list = _passengerRepository.Queryable()
                .Include(c => c.Flight)
                .Where(c => c.Flight.ParameterId == HajjConstants.FlightDirection.Departure &&
                             c.Flight.FlightYear == activeYear)
                .Select(c => new { c.Flight.FlightNo, c.Flight.FlightId })
                .Distinct().ToList();
            return Json(list);
        }

        public IActionResult CascadingFlightList(int flightID)
        {
            int activeYear = _settings.ActiveHajjYear;
            var list = _passengerRepository.Queryable()
                .Include(c => c.Flight)
                .Where(c => c.Flight.ParameterId == HajjConstants.FlightDirection.Departure &&
                             c.Flight.FlightYear == activeYear &&
                             c.FlightId != flightID)
                .Select(c => new { c.Flight.FlightNo, c.Flight.FlightId })
                .Distinct().ToList();
            return Json(list);
        }

        public IActionResult FlightCategory()
        {
            var list = _flightRepository.Queryable()
                .Select(c => new { flightId = c.FlightId, FlightType = c.FlightNo.TrimStart() })
                .OrderBy(x => x.FlightType).ToList();
            return Json(list);
        }

        public IActionResult SwapFlightBus()
        {
            var list = _passengerRepository.Queryable()
                .Include(c => c.Flight)
                .Select(c => new { passangerId = c.PassengerId, flightNo = c.Flight.FlightNo })
                .OrderBy(x => x.flightNo).ToList();
            return Json(list);
        }

        public async Task<IActionResult> SwapTransfer(int passId)
        {
            var details = await _passengerRepository.Queryable()
                .Where(c => c.PassengerId == passId)
                .Select(c => new SwapVM {
                    PassengerId   = passId,
                    FullName      = c.AlhajjMaster.FullName,
                    ServcieNumber = c.AlhajjMaster.ServcieNumber,
                    NIC           = c.AlhajjMaster.NIC,
                    FlightNo      = c.Flight.FlightNo,
                    BusNo         = c.Buses.BusNo,
                    FlightId      = c.Flight.FlightId,
                    BusId         = c.Buses.BusId
                }).SingleOrDefaultAsync();
            return View(details);
        }

        public async Task<IActionResult> SwapTransfer2(int passId)
        {
            var details = await _passengerRepository.Queryable()
                .Where(c => c.PassengerId == passId)
                .Select(c => new SwapVM {
                    PassengerId   = passId,
                    FullName      = c.AlhajjMaster.FullName,
                    ServcieNumber = c.AlhajjMaster.ServcieNumber,
                    NIC           = c.AlhajjMaster.NIC,
                    FlightNo      = c.Flight.FlightNo,
                    BusNo         = c.Buses.BusNo,
                    FlightId      = c.Flight.FlightId,
                    BusId         = c.Buses.BusId
                }).SingleOrDefaultAsync();
            return Json(details);
        }

        public IActionResult Swap1() => View();

        [HttpPost]
        public ActionResult PDFExportSave(string contentType, string base64, string fileName)
        {
            var fileContent = Convert.FromBase64String(base64);
            return File(fileContent, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContent = Convert.FromBase64String(base64);
            return File(fileContent, contentType, fileName);
        }

        public JsonResult BusList(int FlightId)
        {
            var list = _busRepository.Queryable()
                .Where(c => c.FlightId == FlightId)
                .Select(c => new { c.BusId, BusNo = $"{c.BusNo} رقم الباص" })
                .ToList();
            return Json(list);
        }

        public IActionResult IndexWithList() => View();
    }
}
