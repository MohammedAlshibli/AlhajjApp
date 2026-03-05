using System;
using System.Linq;
using ITS.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Pligrimage.Entities;
using Pligrimage.Services.Interface;
using Pligrimage.Web.Extensions;

namespace Pligrimage.Web.Controllers
{
    public class CategoriesController : BaseController
    {

        private readonly ICategoryService _categoryServcie;
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(ICategoryService categoryService, IUnitOfWork unitOfWork)
        {
            _categoryServcie = categoryService;
            _unitOfWork = unitOfWork;

        }

        // GET: Categories
        [PligrimageFiltter]
        public async Task<IActionResult> Index()
        {
            return View();
        }


        public async Task<IActionResult> CategoryRead()
        {

            return View(_categoryServcie.Queryable().ToList());
        }



        [HttpPost]
        public async Task<IActionResult> CreateCategory( Category category )
        {
            if (category != null && ModelState.IsValid)
            {
                category.CreateBy = LoggedUserName();
                category.CreateOn = DateTime.Now;
                _categoryServcie.Insert(category);
                await _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Categories");

        }


        [HttpPost]
        public async Task<IActionResult> UpdateCategory(Category category)
        {
            if (category != null && ModelState.IsValid)
            {
                category.UpdatedBy = LoggedUserName();
                category.UpdatedOn = DateTime.Now;
                _categoryServcie.Update(category);
                await _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Categories");
        }


        [HttpPost]
        public async Task<IActionResult> DeleteParameter(Category category)
        {
            if (category != null && ModelState.IsValid)
            {
                _categoryServcie.Delete(category);
                await _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Categories");
        }




















        //// GET: Categories/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var category = await _context.categories
        //        .FirstOrDefaultAsync(m => m.CategoryId == id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(category);
        //}

        //// GET: Categories/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Categories/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("CategoryId,DescArabic,DescEnglish,AlhajYear,QTY")] Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(category);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(category);
        //}

        //// GET: Categories/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var category = await _context.categories.FindAsync(id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(category);
        //}

        //// POST: Categories/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("CategoryId,DescArabic,DescEnglish,AlhajYear,QTY")] Category category)
        //{
        //    if (id != category.CategoryId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(category);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!CategoryExists(category.CategoryId))
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
        //    return View(category);
        //}

        //// GET: Categories/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var category = await _context.categories
        //        .FirstOrDefaultAsync(m => m.CategoryId == id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(category);
        //}

        //// POST: Categories/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var category = await _context.categories.FindAsync(id);
        //    _context.categories.Remove(category);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool CategoryExists(int id)
        //{
        //    return _context.categories.Any(e => e.CategoryId == id);
        //}
    }
}
