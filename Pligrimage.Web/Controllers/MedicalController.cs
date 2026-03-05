using Pligrimage.Entities;
﻿using System;
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
using Pligrimage.Web.Models;

// Magic number constants
using HC = Pligrimage.Entities.HajjConstants;

namespace Pligrimage.Web.Controllers
{
    public class MedicalController : BaseController
    {
        public readonly IAlHajjMasterServcie _alHajjRepostory;
        public readonly IParameterService _parameterRepostory;
        public readonly IAdminService _adminService;
        public readonly IUnitOfWork _unitOfWork;

        public MedicalController(IAlHajjMasterServcie alHajjRepostory, IParameterService parameterRepostory, IUnitOfWork unitOfWork,IAdminService adminService)
        {
            _alHajjRepostory = alHajjRepostory;
            _unitOfWork = unitOfWork;
            _parameterRepostory = parameterRepostory;
            _adminService = adminService;
        }

        [PligrimageFiltter]
        public IActionResult Index()
        {
            //var FitResultParameter = _parameterRepostory.Queryable().Where(c => c.Code == "FitCode")
            var FitResultParameter = _parameterRepostory.GetFitCodeTypeList()

            .Select(c => new
            {
                c.ParameterId,
                c.DescArabic
            }).ToList();

            ViewData["FitResult"] = FitResultParameter;
            return View();
        }

        public async Task<IActionResult> ReadMedical()
        {
            int medYear = DateTime.Now.Year; // TODO: inject HajjSettings
            var list = _alHajjRepostory.Queryable().Include(c => c.Unit)
                .Select(c => new
                {
                    c.PligrimageId,
                    c.FullName,
                    c.ServcieNumber,
                    c.NIC,
                    UnitNameAr = c.Unit != null ? c.Unit.UnitNameAr : "",
                    c.BloodGroup,
                    c.FitResult,
                    InjectionDate = c.InjectionDate.ToString("dd/MM/yyyy"),
                    c.DoctorNote
                }).ToList();
            return Json(list);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateMedical( AlhajjMaster alhajjMaster)
        {
            if (alhajjMaster != null && ModelState.IsValid)
            {
                alhajjMaster.UpdatedBy = LoggedUserName();
                alhajjMaster.UpdatedOn= DateTime.Now;
                _alHajjRepostory.Update(alhajjMaster);
                await _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Medical");

        }



        public IActionResult DoctorNote()
        {
            var FitResultParameter = _parameterRepostory.GetFitCodeTypeList()

                .Select(c => new
                {
                    c.ParameterId,
                    c.DescArabic
                }).ToList();

            ViewData["FitResult"] = FitResultParameter;
            return View();

        }

        public IActionResult DoctorRead()
        {
           
            var alhajjAdminlist = _alHajjRepostory.Queryable().Where(c => c.FitResult == HajjConstants.FitResult.Pending ).Select(c => new DoctorNoteModel()
            {
                Name = c.FullName,
                ServcieNumber = c.ServcieNumber,
                NationalID = c.NIC,
                DoctorNote = c.DoctorNote,
                UnitNameAr = c.Unit.UnitNameAr,
                BloodGroup=c.BloodGroup,
                //parameter=c.FitResult,
                //Parameter=new ParameterViewModel()
                //{
                //    ParameterID=Parameter
                //}
                

            }).ToList();
            return View(alhajjAdminlist);


        }


        [HttpPost]
        public async Task<IActionResult> UpdateDoctorNOte( DoctorNoteModel doctorNoteModel )
        {
            try
            {
                var alhajjDetails = _alHajjRepostory.Queryable().Where(c => c.ServcieNumber == doctorNoteModel.ServcieNumber).FirstOrDefault();
                alhajjDetails.ServcieNumber = doctorNoteModel.ServcieNumber;
                alhajjDetails.DoctorNote = doctorNoteModel.DoctorNote;
               


                if (alhajjDetails != null && ModelState.IsValid)
                {

                     alhajjDetails.UpdatedBy = LoggedUserName();
                     alhajjDetails.UpdatedOn = DateTime.Now;
                    _alHajjRepostory.Update(alhajjDetails);
                    await _unitOfWork.SaveChangesAsync();
                }
                return View(alhajjDetails);
            }
            catch (Exception)
            {

                throw;
            }

    
        }

        public async Task<IActionResult> IndexMedical()
        {
            var x = _alHajjRepostory.Queryable().Include(c => c.Parameter).ToList();
            return View(x);
        }

        public async Task<IActionResult> UpdateDoctorNote(int PligrimageId)
        {
            if(PligrimageId == null)
            {
                return NotFound();
            }
            var alhalist = _alHajjRepostory.FindAsync(PligrimageId);
            if (alhalist==null)
            {
                return NotFound();
            }
            return View(alhalist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDoctorNote(AlhajjMaster alhajjMaster)
        {
            if (ModelState.IsValid)
            {
                _alHajjRepostory.Update(alhajjMaster);
                await _unitOfWork.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(alhajjMaster);
        }

    }
}
