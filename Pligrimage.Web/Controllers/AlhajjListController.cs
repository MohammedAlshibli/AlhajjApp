using System;
using System.Linq;
using System.Threading.Tasks;
using ITS.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pligrimage.Entities;
using Pligrimage.Services.Interface;

namespace Pligrimage.Web.Controllers
{
    public class AlhajjListController : BaseController
    {
        public readonly IAlHajjMasterServcie _alhajjRepository;
        public readonly IUnitServcie _unitService;
        private readonly IParameterService _parameterRepository;
        public readonly IUnitOfWork _unitOfWork;



        public AlhajjListController(IAlHajjMasterServcie alhajjRepository,
                                        IUnitOfWork unitOfWork,
                                        IParameterService parameterRepository,
                                        IUnitServcie unitServcie)
        {
            _alhajjRepository = alhajjRepository;
            _unitOfWork = unitOfWork;
            _parameterRepository = parameterRepository;
            _unitService = unitServcie;
        }

       // [PligrimageFiltter]
        public IActionResult Index()
        {
            //ViewData["RegionCode"] = _parameterRepository.Queryable().Where(c => c.Code == "RegionCode")
            //  .Select(c => new
            //  {
            //      c.ParameterId,
            //      c.DescArabic
            //  }).ToList();

            ViewData["ClassTypeList"] = _parameterRepository.GetClassTypeListAsync()
           .Select(c => new
           {
               c.ParameterId,
               c.DescArabic
           }).ToList();


          

            var FitResultParameter = _parameterRepository.GetFitCodeTypeList()

            .Select(c => new
            {
                c.ParameterId,
                c.DescArabic
            }).ToList();

            ViewData["FitResult"] = FitResultParameter;



       


            return View();

        }

        public IActionResult AlhajjRead()
        {
            //var result = _alhajjRepository.Query().Include(c => c.Unit).SelectAsync().Result;
            var list = _alhajjRepository.Queryable().Include(c => c.Unit).ToList();

            return View(list);
        }

        public ActionResult GridDetails(string Id)
        {
            var model = _alhajjRepository.Queryable().Where(x => x.PligrimageId == int.Parse(Id)).FirstOrDefault();
            return PartialView("_GridDetails", model);
        }



        public IActionResult AlhajjType()
        {
            var alhajjType = _unitService.Queryable().ToList();

            var type = alhajjType.Select(c => new
            {
                parameterId = c.UnitId,
                alhajjType = c.UnitNameAr.TrimStart(),

            });

            return Json(type.OrderBy(x => x.alhajjType).ToList());
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string bese64, string Filename)
        {
            var fileContents = Convert.FromBase64String(bese64);
            return File(fileContents, contentType, Filename);
        }



        [HttpPost]
        public IActionResult UpdateAlhajjType( AlhajjMaster alhajjMaster)
        {
            if (alhajjMaster != null && ModelState.IsValid)
            {
                alhajjMaster.UpdatedBy = LoggedUserName();
                alhajjMaster.UpdatedOn = DateTime.Now;
                _alhajjRepository.Update(alhajjMaster);
                _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "AlhajjList");
        }

        [HttpPost]
        public ActionResult DeleteAlhajj(AlhajjMaster alhajjMaster)
        {
            if (alhajjMaster != null && ModelState.IsValid)
            {
                _alhajjRepository.Delete(alhajjMaster);
                _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "AlhajjList");

        }
    }
}