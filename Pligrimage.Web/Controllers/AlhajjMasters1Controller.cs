using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pligrimage.Data;
using Pligrimage.Entities;

namespace Pligrimage.Web.Controllers
{
    /// <summary>
    /// DEPRECATED: Duplicate test controller. Do not use. Use AlhajjMastersController instead.
    /// </summary>
    [Obsolete("Use AlhajjMastersController")]
    public class AlhajjMasters1Controller : Controller
    {
        private readonly AppDbContext _context;

        public AlhajjMasters1Controller(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            _context = context;
        }

        // GET: AlhajjMasters1
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.AlhajjMasters.Include(a => a.Category).Include(a => a.Document).Include(a => a.Parameter).Include(a => a.Unit);
            return View(await appDbContext.ToListAsync());
        }

        // GET: AlhajjMasters1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alhajjMaster = await _context.AlhajjMasters
                .Include(a => a.Category)
                .Include(a => a.Document)
                .Include(a => a.Parameter)
                .Include(a => a.Unit)
                .FirstOrDefaultAsync(m => m.PligrimageId == id);
            if (alhajjMaster == null)
            {
                return NotFound();
            }

            return View(alhajjMaster);
        }

        // GET: AlhajjMasters1/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.categories, "CategoryId", "CategoryId");
            ViewData["DocumentId"] = new SelectList(_context.Documents, "DocumentId", "DocumentId");
            ViewData["ParameterId"] = new SelectList(_context.parameters, "ParameterId", "ParameterId");
            ViewData["UnitId"] = new SelectList(_context.units, "UnitId", "UnitId");
            return View();
        }

        // POST: AlhajjMasters1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PligrimageId,ServcieNumber,EmployeeStatus,NIC,NICExpire,Passport,PassportExpire,FullName,DateOfBirth,RankCode,RankDesc,RegistrationDate,Snapshote,HrmsUnitCode,HrmsUnitDesc,WorkLocation,Region,WilayaCode,WilayaDesc,VillageCode,VillageDesc,WorkPhone,GSM,ReletiveName1,ReletiveRelation1,RelativeGsm1,ReletiveName2,ReletiveRelation2,RelativeGsm2,SheikhName,SheikhGsm,FitFlag,FitResult,DoctorNote,CancelNote,Notes,AlhajYear,BloodGroup,DateOfEnlistment,PhotoPath,ConfirmCode,InjectionDate,CategoryId,UnitId,DocumentId,ParameterId,CreateOn,CreateBy,UpdatedOn,UpdatedBy")] AlhajjMaster alhajjMaster)
        {
            if (ModelState.IsValid)
            {
                _context.Add(alhajjMaster);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.categories, "CategoryId", "CategoryId", alhajjMaster.CategoryId);
            ViewData["DocumentId"] = new SelectList(_context.Documents, "DocumentId", "DocumentId", alhajjMaster.DocumentId);
            ViewData["ParameterId"] = new SelectList(_context.parameters, "ParameterId", "ParameterId", alhajjMaster.ParameterId);
            ViewData["UnitId"] = new SelectList(_context.units, "UnitId", "UnitId", alhajjMaster.UnitId);
            return View(alhajjMaster);
        }

        // GET: AlhajjMasters1/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alhajjMaster = await _context.AlhajjMasters.FindAsync(id);
            if (alhajjMaster == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.categories, "CategoryId", "CategoryId", alhajjMaster.CategoryId);
            ViewData["DocumentId"] = new SelectList(_context.Documents, "DocumentId", "DocumentId", alhajjMaster.DocumentId);
            ViewData["ParameterId"] = new SelectList(_context.parameters, "ParameterId", "ParameterId", alhajjMaster.ParameterId);
            ViewData["UnitId"] = new SelectList(_context.units, "UnitId", "UnitId", alhajjMaster.UnitId);
            return View(alhajjMaster);
        }

        // POST: AlhajjMasters1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PligrimageId,ServcieNumber,EmployeeStatus,NIC,NICExpire,Passport,PassportExpire,FullName,DateOfBirth,RankCode,RankDesc,RegistrationDate,Snapshote,HrmsUnitCode,HrmsUnitDesc,WorkLocation,Region,WilayaCode,WilayaDesc,VillageCode,VillageDesc,WorkPhone,GSM,ReletiveName1,ReletiveRelation1,RelativeGsm1,ReletiveName2,ReletiveRelation2,RelativeGsm2,SheikhName,SheikhGsm,FitFlag,FitResult,DoctorNote,CancelNote,Notes,AlhajYear,BloodGroup,DateOfEnlistment,PhotoPath,ConfirmCode,InjectionDate,CategoryId,UnitId,DocumentId,ParameterId,CreateOn,CreateBy,UpdatedOn,UpdatedBy")] AlhajjMaster alhajjMaster)
        {
            if (id != alhajjMaster.PligrimageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(alhajjMaster);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlhajjMasterExists(alhajjMaster.PligrimageId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.categories, "CategoryId", "CategoryId", alhajjMaster.CategoryId);
            ViewData["DocumentId"] = new SelectList(_context.Documents, "DocumentId", "DocumentId", alhajjMaster.DocumentId);
            ViewData["ParameterId"] = new SelectList(_context.parameters, "ParameterId", "ParameterId", alhajjMaster.ParameterId);
            ViewData["UnitId"] = new SelectList(_context.units, "UnitId", "UnitId", alhajjMaster.UnitId);
            return View(alhajjMaster);
        }

        // GET: AlhajjMasters1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var alhajjMaster = await _context.AlhajjMasters
                .Include(a => a.Category)
                .Include(a => a.Document)
                .Include(a => a.Parameter)
                .Include(a => a.Unit)
                .FirstOrDefaultAsync(m => m.PligrimageId == id);
            if (alhajjMaster == null)
            {
                return NotFound();
            }

            return View(alhajjMaster);
        }

        // POST: AlhajjMasters1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var alhajjMaster = await _context.AlhajjMasters.FindAsync(id);
            _context.AlhajjMasters.Remove(alhajjMaster);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlhajjMasterExists(int id)
        {
            return _context.AlhajjMasters.Any(e => e.PligrimageId == id);
        }
    }
}
