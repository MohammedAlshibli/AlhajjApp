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
    public class DocumentsController : Controller
    {

        private readonly IDocumentServcie _docuemntServcie;
        private readonly IUnitOfWork _unitOfWork;

        public DocumentsController(IDocumentServcie documentServcie, IUnitOfWork unitOfWork)
        {
            _docuemntServcie = documentServcie;
            _unitOfWork = unitOfWork;
        }


        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> DocumentRead()
        {

            return View(_docuemntServcie.Queryable().ToList());
        }

        public async Task<IActionResult> Create()
        {
         
            return View();
        }
        public string FormatVisitDate(DateTime? date)
        {
            if (!date.HasValue) return "";
            return date.Value.ToString("dd/MM/yyyy hh:mm tt")
                .Replace("AM", "Morning")
                .Replace("PM", "Evening");
        }
        [HttpPost]
        public async Task<IActionResult> CreateDocument(Document document)
        {
            if (document != null && ModelState.IsValid)
            {
                _docuemntServcie.Insert(document);
                await _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Documents");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDocument(Document document)
        {
            if (document != null && ModelState.IsValid)
            {
                _docuemntServcie.Update(document);
                await _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Documents");
        }


        [HttpPost]
        public async Task<IActionResult> DeleteDocument(Document document)
        {
            if (document != null && ModelState.IsValid)
            {
                _docuemntServcie.Delete(document);
                await _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Documents");
        }










        //private readonly AppDbContext _context;

        //public DocumentsController(AppDbContext context)
        //{
        //    _context = context;
        //}

        //// GET: Documents
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Documents.ToListAsync());
        //}

        //// GET: Documents/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var document = await _context.Documents
        //        .FirstOrDefaultAsync(m => m.DocumentId == id);
        //    if (document == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(document);
        //}

        //// GET: Documents/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Documents/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("DocumentId,FileName,ContentType,Path,DocumnetType,Year")] Document document)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(document);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(document);
        //}

        //// GET: Documents/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var document = await _context.Documents.FindAsync(id);
        //    if (document == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(document);
        //}

        //// POST: Documents/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("DocumentId,FileName,ContentType,Path,DocumnetType,Year")] Document document)
        //{
        //    if (id != document.DocumentId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(document);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!DocumentExists(document.DocumentId))
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
        //    return View(document);
        //}

        //// GET: Documents/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var document = await _context.Documents
        //        .FirstOrDefaultAsync(m => m.DocumentId == id);
        //    if (document == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(document);
        //}

        //// POST: Documents/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var document = await _context.Documents.FindAsync(id);
        //    _context.Documents.Remove(document);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool DocumentExists(int id)
        //{
        //    return _context.Documents.Any(e => e.DocumentId == id);
        //}
    }
}
