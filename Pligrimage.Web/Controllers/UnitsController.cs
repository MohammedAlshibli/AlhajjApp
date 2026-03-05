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
using Pligrimage.Web.Extensions;

namespace Pligrimage.Web.Controllers
{
    public class UnitsController : BaseController
    {
      
        private readonly IUnitServcie _unitService ;
        private readonly IUnitOfWork _unitOfWork;

        public UnitsController(IUnitServcie unitService, IUnitOfWork unitOfWork)
        {
            _unitService = unitService;
            _unitOfWork = unitOfWork;
        }

        // GET: Units
        [PligrimageFiltter]
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult UnitRead()
        {

            return Json(_unitService.Queryable().ToList());
        }



        [HttpPost]
        public ActionResult CreateUnit(Unit unit)
        {
            if (unit != null && ModelState.IsValid)
            {
                unit.CreateBy = LoggedUserName();
                unit.CreateOn = DateTime.Now;
                _unitService.Insert(unit);
                _unitOfWork.SaveChangesAsync();
            }
            return Json(unit);
        }


        [HttpPost]
        public ActionResult UpdateUnit(Unit unit)
        {
            if (unit != null && ModelState.IsValid)
            {
                unit.CreateBy = LoggedUserName();
                unit.CreateOn = DateTime.Now;
                _unitService.Update(unit);
                _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Units");

        }


        [HttpPost]
        public ActionResult DeleteUnit(Unit unit)
        {
            if (unit != null && ModelState.IsValid)
            {
                _unitService.Delete(unit);
                _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Units");
        }





























        // GET: Units/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var unit = await _context.units
        //        .FirstOrDefaultAsync(m => m.UnitId == id);
        //    if (unit == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(unit);
        //}

        //// GET: Units/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Units/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("UnitId,UnitNameEn,UnitNameAr,ModFlag,AlhajYear,AllowNumber,StandBy")] Unit unit)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(unit);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(unit);
        //}

        //// GET: Units/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var unit = await _context.units.FindAsync(id);
        //    if (unit == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(unit);
        //}

        //// POST: Units/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("UnitId,UnitNameEn,UnitNameAr,ModFlag,AlhajYear,AllowNumber,StandBy")] Unit unit)
        //{
        //    if (id != unit.UnitId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(unit);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!UnitExists(unit.UnitId))
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
        //    return View(unit);
        //}

        //// GET: Units/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var unit = await _context.units
        //        .FirstOrDefaultAsync(m => m.UnitId == id);
        //    if (unit == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(unit);
        //}

        //// POST: Units/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var unit = await _context.units.FindAsync(id);
        //    _context.units.Remove(unit);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool UnitExists(int id)
        //{
        //    return _context.units.Any(e => e.UnitId == id);
        //}
    }
}
