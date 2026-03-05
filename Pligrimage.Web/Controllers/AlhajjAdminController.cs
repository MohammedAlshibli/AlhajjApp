using Microsoft.EntityFrameworkCore;
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using HrmsHttpClient.HrmsClientApi;
using ITS.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Pligrimage.Entities;
using Pligrimage.Services.Implementation;
using Pligrimage.Services.Interface;
using Pligrimage.Web.Common.ViewModel;
using Pligrimage.Web.Dto;
using Pligrimage.Web.Extensions;
using Pligrimage.Web.Models;

namespace Pligrimage.Web.Controllers
{

    public class AlhajjAdminController : BaseController
    {
        public readonly IAlHajjMasterServcie _alhajjRepository;
        public readonly IUnitOfWork _unitOfWork;
        public readonly IUnitServcie _unitRepository;
        public readonly ICategoryService _categoryRepository;
        private readonly IDocumentServcie _documentRepository;
        private readonly IParameterService _parameterRepository;
        private readonly IEmployeeClient _employeeClient;
        private readonly IAdminService _adminService;
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;



        public AlhajjAdminController(IAlHajjMasterServcie alhajjRepository,
                                        IUnitOfWork unitOfWork,
                                        IUnitServcie unitRepository,
                                        ICategoryService categoryRepository,
                                        IDocumentServcie documentRepository,
                                        IParameterService parameterRepository,
                                       IEmployeeClient employeeClient,
                                        IAdminService adminService,
                                            IEmployeeService employeeService,
    IMapper mapper)
        {
            _alhajjRepository = alhajjRepository;
            _unitOfWork = unitOfWork;
            _unitRepository = unitRepository;
            _categoryRepository = categoryRepository;
            _documentRepository = documentRepository;
            _parameterRepository = parameterRepository;
            _employeeClient = employeeClient;
            _adminService = adminService;
            _employeeService=employeeService;
            _mapper = mapper;   

        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string bese64, string Filename)
        {
            var fileContents = Convert.FromBase64String(bese64);
            return File(fileContents, contentType, Filename);
        }

        public IActionResult Create()
        {

            return View();
        }


        public async Task<IActionResult> GetAlhajjDataFormWebService(string ServiceNumber)
        {

            var userService = _adminService.GetUserServiceList(LoggedUserName());

            HttpResponseMessage response = new HttpResponseMessage();


            try
            {
                response = await _employeeService.GetEmployeeBymilitaryServiceId(ServiceNumber);

            }
            catch (Exception ex)
            {

                return BadRequest(string.Format("There is an error in web service api connection!!{0}, contact with Admin", System.Environment.NewLine));
            }

            if (!response.IsSuccessStatusCode)
            {
                var responError = (int)response.StatusCode;
                return BadRequest(string.Format("There is Error with No {0},{1} ", responError, response.StatusCode));
            };
            string json = await response.Content.ReadAsStringAsync();
            JundEmployeeDto _fromHrms = JsonConvert.DeserializeObject<JundEmployeeDto>(json);

            var test = JsonConvert.DeserializeObject(json);

            if (test == null)
            {

                return BadRequest(100);
            }
            var newuser = _mapper.Map<AlhajjMaster>(_fromHrms);


            //AlhajjMaster newalhajjMaster = _alhajjRepository.AlhajjMasterMap(_fromHrms);



            //if (!userService.Contains(newuser.Unit.UnitId)) return PartialView("_error");




            if (newuser == null)
            {

                return BadRequest(100);
            }

            //  if (newuser.EmployeeStatus != 0)
            //  {
            //      return BadRequest(string.Format("الرجاء استخدام شاشة المتقاعدين ", System.Environment.NewLine));
            //  }


            //  var classTypeParameter = _parameterRepository.Queryable().FirstOrDefault(c => c.ParameterId ==3);

            //  if (newuser.Age <= classTypeParameter.Value)
            //  {
            //      return BadRequest(string.Format("غير مستوفي شرط العمر", System.Environment.NewLine));

            //  }

            ViewData["ClassTypeList"] = _parameterRepository.GetClassTypeListAsync()
           .Select(c => new
           {
               c.ParameterId,
               c.DescArabic
           }).ToList();

            ViewData["CategoryList"] = _categoryRepository.Queryable()
          .Select(c => new
          {
              c.CategoryId,
              c.DescArabic,
          }).ToList();



            ViewData["RTNCodeList"] = _parameterRepository.Queryable()
               .Select(c => new
               {
                   c.ParameterId,
                   c.DescArabic
               }).ToList();


            return PartialView("_Create", newuser);

        }



        public IActionResult StaticService()
        {
            // Important Note
            // parameter table descption (value for (years)) (maxValue for (allowNumber)) (minValue for Standby))

            var StaticService = _parameterRepository.Queryable().Where(c => c.ParameterId == 1).SingleOrDefault();
            var ConsumedAllowedNumberService = _alhajjRepository.Queryable().Count();
            var ConsumedStandByNumberService = _alhajjRepository.Queryable().Count();


            var statcisStatusList = Enumerable.Empty<object>().Select(r => new { allowNumber = 0, standBy = 0, total = 0, remining = 0 }).ToList();

            var items = new[]{
                new { Type = "AllowNumber", Consumed=ConsumedAllowedNumberService,Total=StaticService.MaxValue },
                new { Type = "SatndByNumber", Consumed=ConsumedStandByNumberService,Total=StaticService.MinValue },

        };
            return Json(items);

        }


        //public IActionResult StaticService()
        //{

        //    List<int> userServiceList = _adminService.GetUserServiceListByUnitCode(LoggedUserName()).ToList();

        //    var StaticService = _unitRepository.Queryable().Where(x => userServiceList.Contains(x.UnitCode)).ToList();


        //    //var ConsumedAllowedNumberService = _alhajjRepository.Queryable().Where(c => c.Unit.UnitCode == 1 && c.ParameterId == 1).Count();
        //    //var ConsumedStandByNumberService = _alhajjService.Queryable().Where(c => c.Unit.UnitCode == 7000 && c.ParameterId == 2).Count();

        //    //var ConsumedAllowedNumberService = _alhajjService.Queryable().Where(c => StaticService.Contains(c.Unit.UnitCode) && c.ParameterId == 1 && c.EmployeeStatus == EmployeeStatus.Employeed).Count();
        //    //var ConsumedStandByNumberService = _alhajjService.Queryable().Where(c => userServiceList.Contains(c.Unit.UnitId) && c.ParameterId == 2).Count();


        //    //int busy = _unitRepository.Queryable().Where(x => x.UnitCode == 7000).Count();

        //    List<ServiceStatcisVM> serviceStatcis = new List<ServiceStatcisVM>();


        //    foreach (var item in StaticService)
        //    {
        //        var statcisVM = new ServiceStatcisVM();

        //        statcisVM.ServiceName = item.UnitNameAr.ToString();
        //        statcisVM.AllowNumber = _alhajjRepository.Queryable().Where(c => c.Unit.UnitCode == item.UnitCode && c.ParameterId == 1 && c.EmployeeStatus == EmployeeStatus.Employeed && c.FitResult != 8).Count();
        //        statcisVM.FromAllowNumber = item.AllowNumber;
        //        statcisVM.SatndByNumber = _alhajjRepository.Queryable().Where(c => c.Unit.UnitCode == item.UnitCode && c.ParameterId == 2).Count();
        //        statcisVM.FromSatndByNumber = item.StandBy;
        //        serviceStatcis.Add(statcisVM);

        //    }

        //    return PartialView("_staticService", serviceStatcis);


        //}





        [PligrimageFiltter]
        public IActionResult Index()
        {

            ViewData["ClassTypeList"] = _parameterRepository.GetClassTypeListAsync().Where(c => c.ParameterId == 3)
            .Select(c => new
            {
                c.ParameterId,
                c.DescArabic
            }).ToList();

            ViewData["CategoryList"] = _categoryRepository.Queryable()
            .Select(c => new
            {
                c.CategoryId,
                c.DescArabic,
            }).ToList();

            //ViewData["RTNCodeList"] = _parameterRepository.Queryable().Where(c => c.Code == "RTNCode")
            //     .Select(c => new
            //     {
            //         c.ParameterId,
            //         c.DescArabic
            //     }).ToList();



            return View();
        }

        public IActionResult AlhajjRead()
        {
            var result = _alhajjRepository.Query().Include(c => c.Parameter).Include(c => c.Unit).Where(c => c.ParameterId == 3).SelectAsync();
            return Ok(result);
        }

        public IActionResult MasterDetails(int pligrimageId)
        {
            var result =_alhajjRepository.Queryable().Where(c => c.PligrimageId == pligrimageId);
            return Ok(result);
        }

         
        
        [HttpPost]
        public async Task<ActionResult> CreateAlhajjAdminFromHrms(AlhajjMaster alhajjMaster)
        {

             

            var StaticService = _parameterRepository.Queryable().Where(c => c.ParameterId == 1).SingleOrDefault();
            var ConsumedAllowedNumberService = _alhajjRepository.Queryable().Where(c => c.ParameterId == 3).Count();
            var ConsumedStandByNumberService = _alhajjRepository.Queryable().Where(c => c.ParameterId == 2).Count();

            // Important Note
            // parameter table descption (value for (years)) (maxValue for (allowNumber)) (minValue for Standby))
            
            if (alhajjMaster.ParameterId == 3)
            {
                if (StaticService.MaxValue == ConsumedAllowedNumberService)
                {
                    return BadRequest(string.Format("تجازوت الحدد المسموح به", System.Environment.NewLine));

                }
            }

            if (alhajjMaster.ParameterId == 2)
            {
                if (StaticService.MinValue == ConsumedStandByNumberService)
                {
                    return BadRequest(string.Format("تجازوت الحدد المسموح به", System.Environment.NewLine));

                }
            }

            if (alhajjMaster.NICExpire < DateTime.Now)
            {
                return BadRequest(string.Format("يرجى تجديد البطاقة الشخصية", System.Environment.NewLine));

            }
            else
            {
                //var daysdifferent = (alhajjMaster.NICExpire - DateTime.Now).Value.TotalDays;
                //var NicExpireDaysCondition = _parameterRepository.Queryable().Where(c => c.Code == "NiceExpire").SingleOrDefault().Value;
                //if (daysdifferent < NicExpireDaysCondition)
                //{
                //    return BadRequest(string.Format("يرجى تجديد البطاقة الشخصية سو تنتهي خلال {0} يوم", daysdifferent, System.Environment.NewLine));

                //}
            }
            //Short only for test

            //if (alhajjMaster.PassportExpire < DateTime.Now)
            //{
            //    return BadRequest(string.Format("يرجى تجديد  جواز السفر", System.Environment.NewLine));

            //}

            //else
            //{
            //    var dayCount = (alhajjMaster.PassportExpire - DateTime.Now).Value.TotalDays;
            //    var PassportCondation = _parameterRepository.Queryable().Where(c => c.Code == "PassportExpire").SingleOrDefault().Value;

            //    if (dayCount < PassportCondation)
            //    {
            //        return BadRequest(string.Format("يرجى تجديد جواز السفر سوف ينتهي خلال {0} يوم", dayCount, System.Environment.NewLine));

            //    }
            //}
            alhajjMaster.ParameterId = 1;
            alhajjMaster.Passport = "1541222";
            alhajjMaster.PassportExpire = DateTime.Now;
            alhajjMaster.NICExpire = DateTime.Now;

            if (alhajjMaster != null && ModelState.IsValid)
            {
                try
                {

                    var isExist = _alhajjRepository.Queryable().Any(c => c.NIC == alhajjMaster.NIC && c.ParameterId == 3);
                    if (isExist)
                    {
                        return BadRequest(string.Format("البيانات موجدوة مسبقا", System.Environment.NewLine));

                    }

                    alhajjMaster.CreateBy = LoggedUserName();
                    alhajjMaster.CreateOn = DateTime.Now;
                    alhajjMaster.AlhajYear = DateTime.Now.Year;
                    alhajjMaster.FitResult = 1;
                    alhajjMaster.RegistrationDate = DateTime.Now;
                    _alhajjRepository.Insert(alhajjMaster);
                   await _unitOfWork.SaveChangesAsync();
                    return Ok("success");

                }
                catch (Exception ex)
                {

                    return BadRequest(string.Format("لم يتم الحفظ يوجد نقص فالبيانات", System.Environment.NewLine));

                }



            }

            else
            {
                return Ok();
            }
        }




        [HttpPost]
        public ActionResult UpdateAlhajj( AlhajjMaster alhajjMaster)
        {
            if (alhajjMaster != null && ModelState.IsValid)
            {
                _alhajjRepository.Update(alhajjMaster);
                _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "AlhajjAdmin");
        }



        [HttpPost]
        public ActionResult DeleteAlhajj( AlhajjMaster alhajjMaster)
        {
            if (alhajjMaster != null && ModelState.IsValid)
            {
                _alhajjRepository.Delete(alhajjMaster);
                _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "AlhajjAdmin");
    
        }











    }
}
