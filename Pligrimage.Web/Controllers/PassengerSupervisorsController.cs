using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITS.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pligrimage.Data;
using Pligrimage.Entities;
using Pligrimage.Services.Interface;

namespace Pligrimage.Web.Controllers
{
    public class PassengerSupervisorsController : Controller
    {
        public readonly IPassengerSupervisorService _SupervisorRepository;
        public readonly IAlHajjMasterServcie _alhajjRepository;
        public readonly IUnitOfWork _unitOfWork;


        public PassengerSupervisorsController(IPassengerSupervisorService supervisorRepository,IAlHajjMasterServcie alhajjRepository, IUnitOfWork unitOfWork)
        {
            _SupervisorRepository = supervisorRepository;
            _alhajjRepository = alhajjRepository;
            _unitOfWork = unitOfWork;
        }  


        public IActionResult Index()
        {
            AlhajjList();
            return View();
        }

        public void AlhajjList()
        {
            var HajjList = _alhajjRepository.Queryable()
                .Select(c => new
                {
                    c.PligrimageId,
                    c.ServcieNumber,
                }).ToList();
            ViewData["AlhajjList"] = HajjList;
        }


        public IActionResult PassSupRead()
        {

            return View(_SupervisorRepository.Queryable());
        }


        [HttpPost]
        public ActionResult CreatePassSup(PassengerSupervisors supervisors)
        {
            if (supervisors != null && ModelState.IsValid)
            {
                try
                {
                    var alahaj = _alhajjRepository.FindAsync(supervisors.PligrimageId);
                    supervisors.AlhajjMaster = alahaj.Result;
                    _SupervisorRepository.Insert(supervisors);
                    _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                    throw;
                }
            }
            return RedirectToAction("Index", "PassengerSupervisors");

        }


        [HttpPost]
        public ActionResult UpdatePassSup(PassengerSupervisors supervisors)
        {
            if (supervisors != null && ModelState.IsValid)
            {
                _SupervisorRepository.Update(supervisors);
                _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "PassengerSupervisors");

        }

        [HttpPost]
        public ActionResult DeletePassSup(PassengerSupervisors supervisors)
        {
            if (supervisors != null && ModelState.IsValid)
            {
                _SupervisorRepository.Delete(supervisors);
                _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "PassengerSupervisors");
        }




        //private readonly AppDbContext _context;

        //public PassengerSupervisorsController(AppDbContext context)
        //{
        //    _context = context;
        //}

        //// GET: PassengerSupervisors
        //public async Task<IActionResult> Index()
        //{
        //    var appDbContext = _context.PassengerSupervisors.Include(p => p.AlhajjMaster);
        //    return View(await appDbContext.ToListAsync());
        //}

        //// GET: PassengerSupervisors/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var passengerSupervisors = await _context.PassengerSupervisors
        //        .Include(p => p.AlhajjMaster)
        //        .FirstOrDefaultAsync(m => m.PassengerSuppId == id);
        //    if (passengerSupervisors == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(passengerSupervisors);
        //}

        //// GET: PassengerSupervisors/Create
        //public IActionResult Create()
        //{
        //    ViewData["PassengerSuppId"] = new SelectList(_context.AlhajjMasters, "PligrimageId", "PligrimageId");
        //    return View();
        //}

        //// POST: PassengerSupervisors/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("PassengerSuppId,Count,Year,PligrimageId")] PassengerSupervisors passengerSupervisors)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(passengerSupervisors);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["PassengerSuppId"] = new SelectList(_context.AlhajjMasters, "PligrimageId", "PligrimageId", passengerSupervisors.PassengerSuppId);
        //    return View(passengerSupervisors);
        //}

        //// GET: PassengerSupervisors/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var passengerSupervisors = await _context.PassengerSupervisors.FindAsync(id);
        //    if (passengerSupervisors == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["PassengerSuppId"] = new SelectList(_context.AlhajjMasters, "PligrimageId", "PligrimageId", passengerSupervisors.PassengerSuppId);
        //    return View(passengerSupervisors);
        //}

        //// POST: PassengerSupervisors/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("PassengerSuppId,Count,Year,PligrimageId")] PassengerSupervisors passengerSupervisors)
        //{
        //    if (id != passengerSupervisors.PassengerSuppId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(passengerSupervisors);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!PassengerSupervisorsExists(passengerSupervisors.PassengerSuppId))
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
        //    ViewData["PassengerSuppId"] = new SelectList(_context.AlhajjMasters, "PligrimageId", "PligrimageId", passengerSupervisors.PassengerSuppId);
        //    return View(passengerSupervisors);
        //}

        //// GET: PassengerSupervisors/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var passengerSupervisors = await _context.PassengerSupervisors
        //        .Include(p => p.AlhajjMaster)
        //        .FirstOrDefaultAsync(m => m.PassengerSuppId == id);
        //    if (passengerSupervisors == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(passengerSupervisors);
        //}

        //// POST: PassengerSupervisors/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var passengerSupervisors = await _context.PassengerSupervisors.FindAsync(id);
        //    _context.PassengerSupervisors.Remove(passengerSupervisors);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool PassengerSupervisorsExists(int id)
        //{
        //    return _context.PassengerSupervisors.Any(e => e.PassengerSuppId == id);
        //}
    }
}
