using AutoMapper;
using HrmsHttpClient.HrmsClientApi;
using ITS.Core.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pligrimage.Entities;
using Pligrimage.Entities.Enum;
using Pligrimage.Services.Implementation;
using Pligrimage.Services.Interface;
using Pligrimage.Web.Common.ViewModel;
using Pligrimage.Web.Dto;
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

    public class AlhajjMastersController : BaseController
    {

        public readonly IAlHajjMasterServcie _alhajjService;
        public readonly IUnitOfWork _unitOfWork;
        public readonly IUnitServcie _unitRepository;
        public readonly ICategoryService _categoryRepository;
        private readonly IDocumentServcie _documentRepository;
        private readonly IParameterService _parameterRepository;
        private readonly IEmployeeClient _employeeClient;
        public readonly IAdminService _adminService;
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        private readonly DbContext _dbContext;

        private IHostingEnvironment _hostingEnvironment;  ///usig to upload file 

        public AlhajjMastersController(IAlHajjMasterServcie alhajService,
                                        IUnitOfWork unitOfWork,
                                        IUnitServcie unitRepository,
                                        ICategoryService categoryRepository,
                                        IDocumentServcie documentRepository,
                                        IParameterService parameterRepository,
                                       IEmployeeClient employeeClient,
                                        IHostingEnvironment hostingEnvironment,
                                         IAdminService adminService,
                                         IEmployeeService employeeService,
                                         IMapper mapper,
                                          DbContext dbContext
                                        )
        {
            _alhajjService = alhajService;
            _unitOfWork = unitOfWork;
            _unitRepository = unitRepository;
            _categoryRepository = categoryRepository;
            _documentRepository = documentRepository;
            _parameterRepository = parameterRepository;
            _employeeClient = employeeClient;
            _hostingEnvironment = hostingEnvironment;
            _adminService = adminService;
            _employeeService = employeeService;
            _mapper = mapper;
            _dbContext = dbContext;
        }



        public async Task<IActionResult> GetAlhajjDataFormWebService(string ServiceNumber)
        {
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

            //if (_fromHrms.EmployeeStatus != 0)
            //{
            //    return BadRequest(string.Format("الرجاء استخدام شاشة المتقاعدين ", System.Environment.NewLine));
            //}

            var parmList = new List<int> { 1, 2 };

            var classTypeParameter = _parameterRepository.Queryable().ToList();

            //-- ملاحظة تم استخدام لحساب العمر في-- حاج ---حاج احتياط ---حاج اداري 
            // MinValue  

            //if (_fromHrms.Age <= classTypeParameter.MinValue)
            //{
            //    return BadRequest(string.Format("غير مستوفي شرط العمر", System.Environment.NewLine));

            //}

            //var IsRankCodeExist = _parameterRepository.GetRankCodeListAsync().Where(x=>x.Value == _fromHrms.RankCode).Any();

            //if (IsRankCodeExist == false)
            //{
            //    return BadRequest(string.Format("غير مستوفي شرط الرتبة", System.Environment.NewLine));
            //}



            ViewData["ClassTypeList"] = _parameterRepository.Queryable().ToList()
            .Select(c => new
            {
                c.ParameterId,
                c.DescArabic
            }).ToList();

            var newuser = _mapper.Map<AlhajjMaster>(_fromHrms);

            return PartialView("_Create", newuser);

        }


        public IActionResult AlhajjType()
        {
            var alhajjType = _parameterRepository.Queryable().Where(x => x.Code == "ClassType").ToList();

            var type = alhajjType.Select(c => new
            {
                parameterId = c.ParameterId,
                alhajjType = c.DescArabic.TrimStart(),

            });

            return Json(type.OrderBy(x => x.alhajjType).ToList());
        }

        public async Task<string> GetEmployeeDetails(string militaryServiceId)
        {

            JundToken jundtoken = new JundToken();
            var securityToken = jundtoken.GetsecurityToken();


            var apiClient = new HttpClient();
            apiClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", securityToken);
            var response = await apiClient.GetAsync($"https://jundbe/api/app/integration/person-by-military-service-id/" + militaryServiceId);

            var content = response.Content.ReadAsStringAsync().Result;

            HttpResponseMessage resp = new HttpResponseMessage()
            {
                Content = new StringContent(content, System.Text.Encoding.UTF8, "Application/json")
            };

            //var content = response.Content.ReadAsStringAsync().Result;


            return content;


        }

        public IActionResult StaticService()
        {

            List<int> userServiceList = _adminService.GetUserServiceListByUnitCode(LoggedUserName()).ToList();

            var StaticService = _unitRepository.Queryable().Where(x => userServiceList.Contains(x.UnitCode)).ToList();


            //var ConsumedAllowedNumberService = _alhajjService.Queryable().Where(c => c.Unit.UnitCode ==  c.ParameterId == 1).Count();
            //var ConsumedStandByNumberService = _alhajjService.Queryable().Where(c => c.Unit.UnitCode == 7000 && c.ParameterId == 2).Count();

            //var ConsumedAllowedNumberService = _alhajjService.Queryable().Where(c => StaticService.Contains(c.Unit.UnitCode) && c.ParameterId == 1 && c.EmployeeStatus == EmployeeStatus.Employeed).Count();
            //var ConsumedStandByNumberService = _alhajjService.Queryable().Where(c => userServiceList.Contains(c.Unit.UnitId) && c.ParameterId == 2).Count();


            //int busy = _unitRepository.Queryable().Where(x => x.UnitCode == 7000).Count();

            List<ServiceStatcisVM> serviceStatcis = new List<ServiceStatcisVM>();


            foreach (var item in StaticService)
            {
              var statcisVM = new ServiceStatcisVM();

                statcisVM.ServiceName = item.UnitNameAr.ToString();
                statcisVM.AllowNumber = _alhajjService.Queryable().Where(c => c.Unit.UnitCode == item.UnitCode && c.ParameterId == 1 && c.EmployeeStatus == EmployeeStatus.Employee && c.FitResult != 6).Count();
                statcisVM.FromAllowNumber =item.AllowNumber;
                statcisVM.SatndByNumber = _alhajjService.Queryable().Where(c => c.Unit.UnitCode == item.UnitCode && c.ParameterId == 2).Count();
                statcisVM.FromSatndByNumber = item.StandBy;
                serviceStatcis.Add(statcisVM);

            }

            return PartialView("_staticService", serviceStatcis);
           

        }





        //[PligrimageFiltter]
        public IActionResult Index()
        {
            //ViewData["RTNCodeList"] = _parameterRepository.Queryable().Where(c => c.Code == "RTNCode")
            //       .Select(c => new
            //       {
            //           c.ParameterId,
            //           c.DescArabic
            //       }).ToList();

            ViewData["ClassTypeList"] = _parameterRepository.GetClassTypeListAsync().Where(c => c.ParameterId != 3)
              .Select(c => new
              {
                  c.ParameterId,
                  c.DescArabic
              }).ToList();

            //ViewData["RTNCodeList"] = _parameterRepository.GetRTNCodeListAsync()
            //  .Select(c => new
            //  {
            //      c.ParameterId,
            //      c.DescArabic
            //  }).ToList();


            return View();
        }


        public IActionResult AlhajjRead()
        {
            List<int> userServiceList = _adminService.GetUserServiceListByUnitCode(LoggedUserName()).ToList();

            var result = _alhajjService.Query()
                .Include(c => c.Unit)/*.Include(c =>c.Parameter)*/
                //.Where(c =>c.ParameterId==1 || c.ParameterId==2 ||c.EmployeeStatus==0).SelectAsync().Result.ToDataSourceResult(request);
                .Where(c => c.EmployeeStatus == 0 && c.ParameterId !=3 && userServiceList.Contains(c.Unit.UnitCode)).SelectAsync();



            return Ok(result);


        }
        public IActionResult MasterDetails(int pligrimageId)
        {
            var result=_alhajjService.Queryable().Where(c => c.PligrimageId == pligrimageId);
            return RedirectToAction("Index", "AlhajjMaster");

        }

        public IActionResult AlhajjReadJson()
        {

            var result = _alhajjService.Query().Include(c => c.Parameter).Include(c => c.Unit).Where(c => c.ParameterId == 1 || c.ParameterId == 2).SelectAsync();


            return Ok(result);


        }


        [HttpPost]
        public ActionResult CreateAlhajjFromHrms(AlhajjMaster alhajjMaster)
        {

            var StaticService = _unitRepository.Queryable().Where(x => x.UnitId == alhajjMaster.UnitId).FirstOrDefault();
            var ConsumedAllowedNumberService = _alhajjService.Queryable().Where( c =>c.UnitId==alhajjMaster.UnitId && c.ParameterId == 1).Count();
            var ConsumedStandByNumberService = _alhajjService.Queryable().Where(c => c.UnitId == alhajjMaster.UnitId && c.ParameterId == 2).Count();


            //List<int> userServiceList = _adminService.GetUserServiceListByUnitCode(LoggedUserName()).ToList();

            //var StaticService1 = _unitRepository.Queryable().Where(x => userServiceList.Contains(x.UnitCode));


            //var ConsumedAllowedNumberService = _alhajjService.Queryable().Where(c => c.ParameterId == 1).Count();

            //var ConsumedStandByNumberService = _alhajjService.Queryable().Where(c => c.ParameterId == 2).Count();



            if (alhajjMaster.ParameterId == 1)
            {
                if (StaticService.AllowNumber== ConsumedAllowedNumberService)
                {
                    return BadRequest(string.Format("تجازوت العدد المسموح به", System.Environment.NewLine));

                }
            }

            if (alhajjMaster.ParameterId == 2)
            {
                if (StaticService.StandBy == ConsumedStandByNumberService)
                {
                    return BadRequest(string.Format("تجازوت العدد المسموح به", System.Environment.NewLine));

                }
            }

            //if (alhajjMaster.NICExpire < DateTime.Now)
            //{
            //    return BadRequest(string.Format("يرجى تجديد البطاقة الشخصية", System.Environment.NewLine));

            //}

            //else
            //{
            //    var daysdifferent = (alhajjMaster.NICExpire - DateTime.Now).Value.TotalDays;
            //    var NicExpireDaysCondition = _parameterRepository.Queryable().Where(c => c.Code == "NiceExpire").SingleOrDefault().Value;
            //    if (daysdifferent < NicExpireDaysCondition)
            //    {
            //        return BadRequest(string.Format("يرجى تجديد البطاقة الشخصية سو تنتهي خلال {0} يوم", daysdifferent, System.Environment.NewLine));

            //    }
            //}


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




            if (alhajjMaster != null && ModelState.IsValid)
            {
                try
                {
                    var isExist = _alhajjService.Queryable().Any(c => c.NIC == alhajjMaster.NIC && (c.ParameterId == 1 || c.ParameterId == 2));
                    if (isExist)
                    {
                        return BadRequest(string.Format("البيانات موجدوة مسبقا", System.Environment.NewLine));

                    }
                    alhajjMaster.ParameterId = 1;
                    alhajjMaster.Passport = "1541222";
                    alhajjMaster.PassportExpire = DateTime.Now;
                    alhajjMaster.NICExpire = DateTime.Now;
                    alhajjMaster.CreateBy = LoggedUserName();
                    alhajjMaster.CreateOn = DateTime.Now;
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



        [HttpPost]
        public ActionResult UpdateAlhajj( AlhajjMaster alhajjMaster)
        {
            if (alhajjMaster != null && ModelState.IsValid)
            {
                _alhajjService.Update(alhajjMaster);
                _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "AlhajjMasters");

        }



        [HttpPost]
        public ActionResult DeleteAlhajjMaster( AlhajjMaster alhajjMaster)
        {
            if (alhajjMaster != null && ModelState.IsValid)
            {
                _alhajjService.Delete(alhajjMaster);
                _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "AlhajjMasters");

        }



        // GET: AlhajjMasters/Create
        public IActionResult Create()
        {

            return View();
        }



        #region CreateAlhajjNonModFromExcelFile

        [HttpPost]
        public IActionResult CreateAlhajjNonMod([FromBody] IEnumerable<AlhajjMaster> AlhajjMasterList)
        {
            var alhajjListCount = AlhajjMasterList.Count(c =>c.Parameter.ParameterId==1);
            var alhajjStandByCount = AlhajjMasterList.Count(c => c.Parameter.ParameterId == 2);
            var selectedUnit = AlhajjMasterList.Select(x => x.Unit).FirstOrDefault();
     
            try

            {

                if (alhajjListCount > selectedUnit.AllowNumber)
                {
                    return BadRequest(string.Format("{0}{1}", "تجاوزت الحد المسموح به الرجاء مراجعة الملف المرفق : العدد", alhajjListCount, System.Environment.NewLine));
                }

                if (alhajjStandByCount > selectedUnit.StandBy)
                {
                    return BadRequest(string.Format("{0}{1}", "تجاوزت الحد المسموح به الرجاء مراجعة الملف المرفق : العدد", alhajjStandByCount, System.Environment.NewLine));
                }

                foreach (var item in AlhajjMasterList)
                {

                    var isExist = _alhajjService.Queryable().Where(c => c.NIC == item.NIC).Any();
                    if (isExist == true)
                    {
                        return BadRequest(string.Format("{0}{1}", " البيانات موجودة مسبقا:", item.ServcieNumber, System.Environment.NewLine));

                    }

                    else
                    {
                        item.CreateBy = LoggedUserName();
                        item.CreateOn = DateTime.Now;
                        item.RegistrationDate = DateTime.Now;
                        _alhajjService.Insert(item);
                    }
   
                }

                var result = _unitOfWork.SaveChangesAsync();
                 return Ok("تم حفظ البيانات بنجاح");
            }


            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }




        #endregion







    }
}
