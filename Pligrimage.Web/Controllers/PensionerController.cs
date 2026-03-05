using HrmsHttpClient.HrmsClientApi;
using ITS.Core.Abstractions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Pligrimage.Entities;
using Pligrimage.Entities.Enum;
using Pligrimage.Services.Interface;
using Pligrimage.Web.Extensions;
using Pligrimage.Web.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pligrimage.Web.Controllers
{
    public class PensionerController : BaseController
    {
        public readonly IAlHajjMasterServcie _alhajjService;
        public readonly IUnitOfWork _unitOfWork;
        public readonly IUnitServcie _unitRepository;
        public readonly ICategoryService _categoryRepository;
        private readonly IDocumentServcie _documentRepository;
        private readonly IParameterService _parameterRepository;
        private readonly IEmployeeClient _employeeClient;
        private IHostingEnvironment _hostingEnvironment;  ///usig to upload file 

        public PensionerController(IAlHajjMasterServcie alhajService,
                                        IUnitOfWork unitOfWork,
                                        IUnitServcie unitRepository,
                                        ICategoryService categoryRepository,
                                        IDocumentServcie documentRepository,
                                        IParameterService parameterRepository,
                                        IEmployeeClient employeeClient,
                                        IHostingEnvironment hostingEnvironment)
        {
            _alhajjService = alhajService;
            _unitOfWork = unitOfWork;
            _unitRepository = unitRepository;
            _categoryRepository = categoryRepository;
            _documentRepository = documentRepository;
            _parameterRepository = parameterRepository;
            _employeeClient = employeeClient;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Create()
        {

            return View();
        }
        public async Task<IActionResult> GetAlhajjDataFormWebService(string ServiceNumber)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            //response = await _employeeClient.GetEmployeeByServiceNo(ServiceNumber.ToUpper());

            try
            {
                response = await _employeeClient.GetEmployeeByServiceNo(ServiceNumber.ToUpper());

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

            EmployeeDetailsDto WEemployeeInfo = JsonConvert.DeserializeObject<EmployeeDetailsDto>(json);

            AlhajjMaster newalhajjMaster = _alhajjService.AlhajjMasterMap(WEemployeeInfo);

            if (newalhajjMaster == null)
            {

                return BadRequest(100);
            }
            var parmList = new List<int> { 1, 2 };

            var Pensioner = _parameterRepository.Queryable().FirstOrDefault(c => parmList.Contains(c.ParameterId));

            if (newalhajjMaster.EmployeeStatus==0)
            {
                return BadRequest(string.Format("لأضافة المتقاعدين فقط", System.Environment.NewLine));

            }


            //var classTypeParameter = _parameterRepository.Queryable().FirstOrDefault(c => parmList.Contains(c.ParameterId));

            //if (newalhajjMaster.Age <= classTypeParameter.MinValue)
            //{
            //    return BadRequest(string.Format("غير مستوفي شرط العمر", System.Environment.NewLine));

            //}



            //var RankCodeListCode = new List<int> { 61, 62, 63 };

            //var classTypeParameter = _parameterRepository.Queryable().FirstOrDefault(c => RankCodeListCode.Contains(c.ParameterId));


            //if (newalhajjMaster.RankCode != classTypeParameter.Value)
            //{
            //    return BadRequest(string.Format("غير مستوفي شرط الرتبة", System.Environment.NewLine));
            //}

            var IsRankCodeExist = _parameterRepository.GetRankCodeListAsync().Where(x => x.Value == newalhajjMaster.RankCode).Any();

            if (IsRankCodeExist == false)
            {
                return BadRequest(string.Format("غير مستوفي شرط الرتبة", System.Environment.NewLine));
            }


            ViewData["ClassTypeList"] = _parameterRepository.GetClassTypeListAsync().Where(c => c.ParameterId != 3 && c.ParameterId != 4)
           .Select(c => new
           {
               c.ParameterId,
               c.DescArabic
           }).ToList();



            return PartialView("_Create", newalhajjMaster);

        }


        public ActionResult CreateAlhajjFromHrms(AlhajjMaster alhajjMaster)
        {

            var Static = _parameterRepository.Queryable().Where(c =>c.ParameterId==19 && c.Code == "EmpStatus").FirstOrDefault();

            //var ConsumedAllowedNumberService = _alhajjService.Queryable().Where(c =>c.ParameterId == 1 && c.EmployeeStatus !=0).Count();

            //var ConsumedStandByNumberService = _alhajjService.Queryable().Where(c => c.Unit.UnitCode == 7000 && c.ParameterId == 2).Count();

            var ConsumedAllowedNumberService = _alhajjService.Queryable().Where(c => c.ParameterId == 1 && c.EmployeeStatus == EmployeeStatus.Pension).Count();
            var ConsumedStandByNumberService = _alhajjService.Queryable().Where(c => c.ParameterId == 2 && c.EmployeeStatus == EmployeeStatus.Pension).Count();



            if (alhajjMaster.ParameterId == 1)
            {
                if (Static.MaxValue == ConsumedAllowedNumberService)
                {
                    return BadRequest(string.Format("تجازوت العدد المسموح به", System.Environment.NewLine));

                }
            }


            if (alhajjMaster.ParameterId == 2)
            {
                if (Static.MinValue == ConsumedStandByNumberService)
                {
                    return BadRequest(string.Format(" تجازوت العدد المسموح به من عدد الاحتياط", System.Environment.NewLine));

                }
            }

       

            if (alhajjMaster.NICExpire is null)
            {
                return BadRequest(string.Format("الرجاء ادخل رقم البطاقة الشخصية", System.Environment.NewLine));

            }

            if (alhajjMaster.NICExpire < DateTime.Now)
            {
                return BadRequest(string.Format("يرجى تجديد البطاقة الشخصية", System.Environment.NewLine));

            }

            else
            {
                var daysdifferent = (alhajjMaster.NICExpire - DateTime.Now).Value.TotalDays;
                var NicExpireDaysCondition = _parameterRepository.Queryable().Where(c => c.Code == "NiceExpire").SingleOrDefault().Value;
                if (daysdifferent < NicExpireDaysCondition)
                {
                    return BadRequest(string.Format("يرجى تجديد البطاقة الشخصية سو تنتهي خلال {0} يوم", daysdifferent, System.Environment.NewLine));

                }
            }

            if (alhajjMaster.PassportExpire is null)
            {
                return BadRequest(string.Format("الرجاء ادخل رقم جواز السفر", System.Environment.NewLine));

            }
            if (alhajjMaster.PassportExpire < DateTime.Now)
            {
                return BadRequest(string.Format("يرجى تجديد  جواز السفر", System.Environment.NewLine));

            }

            else
            {
                var dayCount = (alhajjMaster.PassportExpire - DateTime.Now).Value.TotalDays;
                var PassportCondation = _parameterRepository.Queryable().Where(c => c.Code == "PassportExpire").SingleOrDefault().Value;

                if (dayCount < PassportCondation)
                {
                    return BadRequest(string.Format("يرجى تجديد جواز السفر سوف ينتهي خلال {0} يوم", dayCount, System.Environment.NewLine));

                }
            }




            if (alhajjMaster != null && ModelState.IsValid)
            {
                try
                {
                    var isExist = _alhajjService.Queryable().Any(c => c.NIC == alhajjMaster.NIC && (c.ParameterId == 1 || c.ParameterId == 2));
                    if (isExist)
                    {
                        return BadRequest(string.Format("البيانات موجدوة مسبقا", System.Environment.NewLine));

                    }

                     alhajjMaster.CreateBy = LoggedUserName();
                     alhajjMaster.AlhajYear = DateTime.Now.Year;
                     alhajjMaster.RegistrationDate = DateTime.Now;
                     alhajjMaster.FitResult = 7;
                     //alhajjMaster.ConfirmCode = 51;
                    _alhajjService.Insert(alhajjMaster);
                    _unitOfWork.SaveChangesAsync();
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

        [PligrimageFiltter]
        public IActionResult Index()
        {
            ViewData["ClassTypeList"] = _parameterRepository.GetClassTypeListAsync().Where(c => c.ParameterId == 1)
          .Select(c => new
          {
              c.ParameterId,
              c.DescArabic
          }).ToList();


            return View();

        }

        public IActionResult PensionerRead()
        {

            var result = _alhajjService.Query().Include(c => c.Parameter).Include(c => c.Unit).Where(c => c.EmployeeStatus !=0).SelectAsync();


            return View(result);


        }


        public IActionResult StaticService()
        {

            var Static = _parameterRepository.Queryable().Where(c => c.ParameterId == 19 && c.Code == "EmpStatus").FirstOrDefault();
            var ConsumedAllowedNumberService = _alhajjService.Queryable().Where(c =>c.ParameterId == 1 && c.EmployeeStatus == EmployeeStatus.Pension).Count();
            var ConsumedStandByNumberService = _alhajjService.Queryable().Where(c => c.ParameterId == 2 && c.EmployeeStatus == EmployeeStatus.Pension).Count();


            var statcisStatusList = Enumerable.Empty<object>().Select(r => new { UnitName = 0, allowNumber = 0, standBy = 0, total = 0, remining = 0 }).ToList();


        

            var items = new[]{
                new { Type = "AllowNumber", Consumed=ConsumedAllowedNumberService,Total=Static.MaxValue },
                new { Type = "SatndByNumber", Consumed=ConsumedStandByNumberService,Total=Static.MinValue },

            };







            return Json(items);

        }

    }
}


