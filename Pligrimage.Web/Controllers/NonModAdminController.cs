using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using HrmsHttpClient.HrmsClientApi;
using ITS.Core.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Pligrimage.Entities;
using Pligrimage.Services.Interface;
using Pligrimage.Web.Extensions;
using Pligrimage.Web.Models;

namespace Pligrimage.Web.Controllers
{
    public class NonModAdminController : BaseController
    {

        public readonly IAlHajjMasterServcie _alhajjRepository;
        public readonly IUnitOfWork _unitOfWork;
        public readonly IUnitServcie _unitRepository;
        public readonly ICategoryService _categoryRepository;
        private readonly IDocumentServcie _documentRepository;
        private readonly IParameterService _parameterRepository;
        private IHostingEnvironment _hostingEnvironment;  ///usig to upload file 




        public NonModAdminController(IAlHajjMasterServcie alhajjRepository,
                                           IUnitOfWork unitOfWork,
                                           IUnitServcie unitRepository,
                                           ICategoryService categoryRepository,
                                           IDocumentServcie documentRepository,
                                           IParameterService parameterRepository,
                                           IHostingEnvironment hostingEnvironment
                                          )
        {
            _alhajjRepository = alhajjRepository;
            _unitOfWork = unitOfWork;
            _unitRepository = unitRepository;
            _categoryRepository = categoryRepository;
            _documentRepository = documentRepository;
            _parameterRepository = parameterRepository;
            _hostingEnvironment = hostingEnvironment;
        }

        [PligrimageFiltter]
        public IActionResult Index()
        {
            var UnitList = _unitRepository.Queryable().Where(c => c.ModFlag == false)
            .Select(c => new 
            {
              c.UnitId,
              c.UnitNameAr,
            }).ToList();
            ViewData["Unit"] = UnitList;


            var CategoryList = _categoryRepository.Queryable()
              .Select(c => new
              {
                  c.CategoryId,
                  c.DescArabic
              }).ToList();
            ViewData["Categories"] = CategoryList;

            var DocumentList = _documentRepository.Queryable()
            .Select(c => new
            {
                c.DocumentId,
                c.DocumnetType
            }).ToList();
            ViewData["Document"] = DocumentList;


            //ViewData["RTNCodeList"] = _parameterRepository.Queryable().Where(c => c.Code == "RTNCode")
            //   .Select(c => new
            //   {
            //       c.ParameterId,
            //       c.DescArabic
            //   }).ToList();


            //ViewData["RegionCode"] = _parameterRepository.Queryable().Where(c => c.Code == "RegionCode")
            //   .Select(c => new
            //   {
            //       c.ParameterId,
            //       c.DescArabic
            //   }).ToList();

            return View();
        }

        private void UnitList()
        {
            var UnitList = _unitRepository.Queryable().Where(c => c.ModFlag == false)
              .Select(c => new
              {
                  c.UnitId,
                  c.UnitNameAr
              }).ToList();
            ViewData["Unit"] = UnitList;
            ViewBag.UnitId = UnitList;
        }


        public IActionResult ReadNonModAdmin()
        {
            //var result = _alhajjRepository.Queryable();
            var result = _alhajjRepository.Query().Include(c => c.Parameter).Include(c => c.Unit).Where(c => c.ParameterId == 3 && c.Unit.ModFlag==false).SelectAsync();


            return Json(result);
        }

        public IActionResult ReadNonModAdminDetails(int pligrimageId)
        {
            return View(_alhajjRepository.Queryable().Where(c => c.PligrimageId == pligrimageId));
        }


        public IActionResult NonModAdminDetails(int pligrimageId)
        {
            return View(_alhajjRepository.Queryable().Where(c => c.PligrimageId == pligrimageId));
        }




        //public  ActionResult GridDetails(string Id)
        //{
        //    var model = _alhajjRepository.Queryable().Where(x => x.PligrimageId == int.Parse(Id)).FirstOrDefault();
        //    return PartialView("_GridDetails", model);
        //}





        [HttpPost]
        public ActionResult CreateNonModAdmin(AlhajjMaster alhajjMaster)
        {
            // Important Note
            // parameter table descption (value for (years)) (maxValue for (allowNumber)) (minValue for Standby))


            var AdminCount = _alhajjRepository.Queryable().Where(c => c.ParameterId == 3).Count();
            var allowNumberCount = _parameterRepository.Queryable().Where(c =>c.ParameterId==3).SingleOrDefault();

            try
            {
                if (alhajjMaster.ServcieNumber== null)
                {
                    return BadRequest(string.Format("لا توجد بيانات ", System.Environment.NewLine));
                }

                if (AdminCount == allowNumberCount.MaxValue )
                {
                    return BadRequest(string.Format("تجاوزت العدد المسموح به ", System.Environment.NewLine));
                }

                if (alhajjMaster != null && ModelState.IsValid)
                {
                    alhajjMaster.CreateBy = LoggedUserName();
                    alhajjMaster.CreateOn = DateTime.Now;

                    alhajjMaster.ParameterId = 3;
                    alhajjMaster.AlhajYear = DateTime.Now.Year;
                    alhajjMaster.FitResult = 9;
                    //alhajjMaster.ConfirmCode = 51;
                    _alhajjRepository.Insert(alhajjMaster);
                    _unitOfWork.SaveChangesAsync();
                    return Ok("success");
                }
                return RedirectToAction("Index", "NonModAdmin");
            }
            catch (Exception)
            {
                //throw new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
                return BadRequest(string.Format("تجاوزت العدد المسموح به ", System.Environment.NewLine));
            }
        }


        [HttpPost]
        public ActionResult UpdateNonModAdmin(AlhajjMaster alhajjMaster)
        {
            if (alhajjMaster != null && ModelState.IsValid)
            {
                _alhajjRepository.Update(alhajjMaster);
                _unitOfWork.SaveChangesAsync();
            }
            //return Json(new[] { alhajjMaster }.ToDataSourceResult(request, ModelState));
            return View(alhajjMaster);
        }


        [HttpPost]
        public ActionResult DeleteNonModAdmin(AlhajjMaster alhajjMaster)
        {
            if ( alhajjMaster != null && ModelState.IsValid)
            {
                _alhajjRepository.Delete(alhajjMaster);
                _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "NonModAdmin");
        }

    }
}