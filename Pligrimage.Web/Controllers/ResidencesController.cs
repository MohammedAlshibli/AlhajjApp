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
    public class ResidencesController : Controller
    {
        public readonly IResidenceService _residenceRepository;
        public readonly IUnitOfWork _unitOfWork;

        public ResidencesController(IResidenceService residenceRepository,IUnitOfWork unitOfWork)
        {
            _residenceRepository = residenceRepository;
            _unitOfWork = unitOfWork;
        }



        [PligrimageFiltter]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ResidenceRead()
        {
            return View(_residenceRepository.Queryable());
        }


        [HttpPost]
        public IActionResult CreateResidence(Residences residences)
        {
            if (residences != null && ModelState.IsValid) { }
            {
                _residenceRepository.Insert(residences);
                _unitOfWork.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Residences");

        }

        [HttpPost]
        public ActionResult UpdateResidence(Residences residences)
        {
            if (residences != null && ModelState.IsValid)
            {
                _residenceRepository.Update(residences);
                _unitOfWork.SaveChangesAsync();
            }
            return View(residences);
        }


        [HttpPost]
        public ActionResult DeleteResidence(Residences residences)
        {
            if (residences != null && ModelState.IsValid)
            {
                _residenceRepository.Delete(residences);
                _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Residences");

        }

    }
}
