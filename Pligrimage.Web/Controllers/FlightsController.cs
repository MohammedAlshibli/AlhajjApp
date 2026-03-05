using System;
using System.Linq;
using System.Threading.Tasks;
using ITS.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pligrimage.Data;
using Pligrimage.Entities;
using Pligrimage.Services.Interface;
using Pligrimage.Web.Extensions;

namespace Pligrimage.Web.Controllers
{
    public class FlightsController : BaseController
    {

        private readonly IFlightServcie _flightService;

        private readonly IParameterService _parameterService;
        private readonly IUnitOfWork _unitOfWork;

        public FlightsController(IFlightServcie flightServcie, IUnitOfWork unitOfWork,IParameterService parameterService)
        {
            _flightService = flightServcie;
            _unitOfWork = unitOfWork;
            _parameterService = parameterService;
        }


        [PligrimageFiltter]
        public IActionResult Index()
        {
        
            ViewData["FlightType"] = _parameterService.Queryable().Where(c => c.Code == "FlightType")
               .Select(c => new
               {
                   c.ParameterId,
                   c.DescArabic
               }).ToList();

            return View();
        }

        public async Task<IActionResult> FlightRead()
        {
            var result = _flightService.Queryable()
                .Select(c => new
                {
                    c.FlightId,
                    c.FlightNo,
                    FlightDate = c.FlightDate.ToString("dd/MM/yyyy"),
                    ArriveDate = c.ArriveDate.ToString("dd/MM/yyyy"),
                    c.FlightCapacity,
                    c.Direction,
                    FlightType = c.Parameter != null ? c.Parameter.DescArabic : ""
                }).ToList();
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateFlight( Flight flight)
        {
            if (flight != null && ModelState.IsValid)
            {
                flight.CreateBy = LoggedUserName();
                flight.CreateOn = DateTime.Now;          
                flight.FlightYear = DateTime.Now.Year;
                _flightService.Insert(flight);
                await _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Flights");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateFlight(Flight flight)
        {
            if (flight != null && ModelState.IsValid)
            {
                flight.UpdatedBy = LoggedUserName();
                flight.UpdatedOn = DateTime.Now;
                _flightService.Update(flight);
                await _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Categories");
        }



        [HttpPost]
        public async Task<IActionResult> DeleteFlight(Flight flight)
        {
            if (flight != null && ModelState.IsValid)
            {
                _flightService.Delete(flight);
                await _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Flights");
        }
















        //private readonly AppDbContext _context;

        //public FlightsController(AppDbContext context)
        //{
        //    _context = context;
        //}

        //// GET: Flights
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Flights.ToListAsync());
        //}

        //// GET: Flights/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var flight = await _context.Flights
        //        .FirstOrDefaultAsync(m => m.FlightId == id);
        //    if (flight == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(flight);
        //}

        //// GET: Flights/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Flights/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("FlightId,FlightNo,FlightDate,ArriveDate,FlightYear,FlightCapacity,Direction")] Flight flight)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(flight);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(flight);
        //}

        //// GET: Flights/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var flight = await _context.Flights.FindAsync(id);
        //    if (flight == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(flight);
        //}

        //// POST: Flights/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("FlightId,FlightNo,FlightDate,ArriveDate,FlightYear,FlightCapacity,Direction")] Flight flight)
        //{
        //    if (id != flight.FlightId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(flight);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!FlightExists(flight.FlightId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(flight);
        //}

        //// GET: Flights/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var flight = await _context.Flights
        //        .FirstOrDefaultAsync(m => m.FlightId == id);
        //    if (flight == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(flight);
        //}

        //// POST: Flights/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var flight = await _context.Flights.FindAsync(id);
        //    _context.Flights.Remove(flight);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool FlightExists(int id)
        //{
        //    return _context.Flights.Any(e => e.FlightId == id);
        //}
    }
}
