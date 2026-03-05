using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITS.Core.Abstractions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Pligrimage.Data;
using Pligrimage.Entities;
using Pligrimage.Services.Interface;
using Pligrimage.Web.Extensions;
using Pligrimage.Web.Models;


namespace Pligrimage.Web.Controllers
{
    public class PassengersController : BaseController
    {

        private readonly IPassengerService _passengerRepository;
        private readonly IFlightServcie _flightRepository;
        private readonly IBusServcie _busRepository;
        private readonly IAlHajjMasterServcie _alHajjRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IParameterService _parameterRepository;
        private readonly IResidenceService _residenceRepository;



        public PassengersController(IPassengerService passengerRepository, IParameterService parameterRepository, IAlHajjMasterServcie alHajjMasterRepository, IFlightServcie flightRepository, IBusServcie busRepository, IUnitOfWork unitOfWork, IResidenceService residenceRepository)
        {
            _passengerRepository = passengerRepository;
            _alHajjRepository = alHajjMasterRepository;
            _busRepository = busRepository;
            _flightRepository = flightRepository;
            _unitOfWork = unitOfWork;
            _parameterRepository = parameterRepository;
            _residenceRepository = residenceRepository;

        }

        [PligrimageFiltter]
        public IActionResult Index()
        {
            

            ViewData["FlightType"] = _flightRepository.Queryable()
                 .Select(c => new
                 {
                     c.FlightId,
                     c.FlightNo
                 }).ToList();

            ViewData["BusList"] = _busRepository.Queryable()
               .Select(c => new
               {
                   c.BusId,
                   c.BusNo
               }).ToList();


            return View();
        }

     

        [PligrimageFiltter]
        public IActionResult SwapFlight()
        {
          
            return View();

        }

        public IActionResult UpdatePassenger(Passenger passenger)
        {
            if (passenger != null && ModelState.IsValid)
            {
                _passengerRepository.Update(passenger);
                _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Passengers");

        }



        [HttpPost]
        public JsonResult PassengerUpdateSwap(string swapListParam)
        {
            IList<SwapVM> swapVMsList = JsonConvert.DeserializeObject<List<SwapVM>>(swapListParam);
            //IList<Passenger> passengersList = new List <Passenger>() ;

            foreach (var item in swapVMsList)
            {
                var passanger = new Passenger()
                {

                    PassengerId = item.PassengerId,
                    PligrimageId = item.PligrimageId,
                    FlightId = item.FlightId,
                      
                };
                
                try
                {

                    _passengerRepository.Update(passanger);
                    var result = _unitOfWork.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                    throw;
                }
                
            };

            return null;

        }

        public IActionResult SwapFlightBus()
        {
            var flightList = _passengerRepository.Queryable().ToList();

            var type = flightList.Select(c => new
            {
                passangerId = c.PassengerId,
                flightNo = c.Flight.FlightNo,

            });

            return Json(type.OrderBy(x => x.flightNo).ToList());
        }
        public IActionResult FlightList()
        {
            var flightList = _passengerRepository.Queryable().Include(c=>c.Flight).Where(c=>c.Flight.ParameterId==34).Select(c=>new {
                c.Flight.FlightNo,
                c.Flight.FlightId

            }).Distinct().ToList();


            return Json(flightList);
        }
        public IActionResult CascadingFlightList(int flightID)
        {
            var flightList = _passengerRepository.Queryable().Include(c => c.Flight).Where(c => c.Flight.ParameterId == 34 && c.FlightId !=flightID).Select(c => new {
                c.Flight.FlightNo,
                c.Flight.FlightId

            }).Distinct().ToList();


            return Json(flightList);
        }


        public async Task<ActionResult> AlhajjFlightDepart()
        {

            int[] alhajjList = new int[] { 1, 3 }; //list from alhajj type 
            int[] fitresultList = new int[] { 9 }; // Doctor Approve from alhajj master
            int[] flightList = new int[] { 12, 13, 14 }; // list from  flight Diparture 
            var alhajjDipartureList = _alHajjRepository.Queryable().Where(c => alhajjList.Contains(c.ParameterId) && fitresultList.Contains(c.FitResult));
            var flightListDiparture = _flightRepository.Queryable().Include(c=>c.buses).Where(c => flightList.Contains(c.FlightId)&& c.ParameterId==34);


            var random = new Random();

            foreach (var Flight in flightListDiparture)
            {
                var  PassengerList = _passengerRepository.Queryable().Include(c=>c.Flight).Where(c=>c.Flight.ParameterId==34).Select(c =>c.PligrimageId);
                var list = alhajjDipartureList.Where(c=>!PassengerList.Contains(c.PligrimageId)).OrderBy(c => random.Next()).Take(Flight.FlightCapacity).ToList();
                var BusList = Flight.buses.OrderBy(c=>c.BusNo).ToList();
                var Index = 0;

                foreach (var item in list)
                {
                    var currentBus = BusList[Index];
                    var BusCapacity = _passengerRepository.Queryable().Include(c => c.Flight).Where(c => c.Flight.ParameterId == 34 && c.BusId==currentBus.BusId).Count();
                    if (BusCapacity>=currentBus.BusCapacity)
                    {
                        if (Index==BusList.Count())
                        {
                            return BadRequest("All Busses are full");
                        }
                        Index++;
                        currentBus= BusList[Index];


                    }
                    var passanger = new Passenger()
                    {
                        CreateBy = LoggedUserName(),
                        CreateOn = DateTime.Now,
                        AlhajjMaster = item,
                        PligrimageId = item.PligrimageId,
                        BusId = currentBus.BusId,
                        Buses= currentBus, 
                        FlightId = Flight.FlightId,
                        ResidencesId = 1,

                    };
                    try
                    {
                        _passengerRepository.Insert(passanger);
                        var result = await _unitOfWork.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }
             
                    
                }
            }


            return Ok();
        }

        public async Task<int> AlhajjFlightReturn()
        {

            int[] alhajjList = new int[] { 1, 3 };
            int[] fitresultList = new int[] { 9 };
            int[] flightList = new int[] { 15, 16, 17 };
            var alhajjDipartureList = _alHajjRepository.Queryable().Where(c => alhajjList.Contains(c.ParameterId) && fitresultList.Contains(c.FitResult));
            var flightListReturn = _flightRepository.Queryable().Include(c => c.buses).Where(c => flightList.Contains(c.FlightId)&& c.ParameterId==35);


            var random = new Random();

            foreach (var Flight in flightListReturn)
            {
                var PassengerList = _passengerRepository.Queryable().Include(c => c.Flight).Where(c => c.Flight.Parameter.ParameterId == 35).Select(c => c.PligrimageId);
                var list = alhajjDipartureList.Where(c => !PassengerList.Contains(c.PligrimageId)).OrderBy(c => random.Next()).Take(Flight.FlightCapacity).ToList();

                foreach (var item in list)
                {
                    var passanger = new Passenger()
                    {
                        CreateBy = LoggedUserName(),
                        CreateOn=DateTime.Now,
                    
                        AlhajjMaster = item,

                        PligrimageId = item.PligrimageId,

                        BusId =44 /*Flight.buses.FirstOrDefault().BusId*/,
                        Buses = Flight.buses.FirstOrDefault(),
                        FlightId = Flight.FlightId,
                        ResidencesId = 1,

                    };
                    try
                    {
                        _passengerRepository.Insert(passanger);
                        var result = await _unitOfWork.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }


                }
            }


            return 0;
        }



        [HttpPost]
        public ActionResult PDFExportSave(string contentType, string base64, string fileName)
        {
            var fileContent = Convert.FromBase64String(base64);
            return File(fileContent, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContent = Convert.FromBase64String(base64);
            return File(fileContent, contentType, fileName);
        }
        
  



        public IActionResult PassengerRead()
        {
            var result = _passengerRepository.Queryable().Include(c =>c.AlhajjMaster).Include(c=>c.Flight.Parameter).Include(c =>c.Buses); //inclue.bus is not working 
            //var result = _passengerRepository.Query().Include(c =>c.AlhajjMaster).Include(c=>c.Flight.Parameter).Include(c=>c.Buses).SelectAsync().Result;
            return Ok(result);

        }

        public IActionResult PassengerReadSwap(int flightID)
        {
            var result = _passengerRepository.Queryable().Include(c => c.AlhajjMaster).Include(c => c.Flight.Parameter).Include(c => c.Buses).Where(c=>c.FlightId==flightID).Select(c => new SwapVM
            {
                PassengerId = c.PassengerId,
                PligrimageId = c.PligrimageId,
                ServcieNumber = c.AlhajjMaster.ServcieNumber,
                FullName = c.AlhajjMaster.FullName,
                FlightId = c.FlightId,
                FlightNo = c.Flight.FlightNo



            }).ToList();
            

            return Json(result);

        }

        #region SwapData

        public IActionResult Swap1()
        {
            return View();
        }

        public IActionResult FlightCategory()
        {
            var FlightType = _flightRepository.Queryable().ToList();

            var type = FlightType.Select(c => new
            {
                flightId = c.FlightId,
                FlightType = c.FlightNo.TrimStart(),

            });

            return Json(type.OrderBy(x => x.FlightType).ToList());
        }

        public async Task <IActionResult> SwapTransfer(int passId)
        {
            var _details = await _passengerRepository.Queryable().Where(c => c.PassengerId == passId).Select
             (c => new SwapVM()
             {
                 PassengerId=passId,
                 FullName = c.AlhajjMaster.FullName,
                 ServcieNumber = c.AlhajjMaster.ServcieNumber,
                 NIC = c.AlhajjMaster.NIC,
                 FlightNo = c.Flight.FlightNo,
                 BusNo=c.Buses.BusNo,
                 FlightId=c.Flight.FlightId,
                 BusId=c.Buses.BusId

             }).SingleOrDefaultAsync(); 

            return View(_details);
        }



        public async Task<IActionResult> SwapTransfer2(int passId)
        {
            var _details = await _passengerRepository.Queryable().Where(c => c.PassengerId == passId).Select
             (c => new SwapVM()
             {
                 PassengerId=passId,
                 FullName = c.AlhajjMaster.FullName,
                 ServcieNumber = c.AlhajjMaster.ServcieNumber,
                 NIC = c.AlhajjMaster.NIC,
                 FlightNo = c.Flight.FlightNo,
                 BusNo = c.Buses.BusNo,
                 FlightId=c.Flight.FlightId,
                 BusId=c.Buses.BusId

             }).SingleOrDefaultAsync();

            return Json(_details);
        }


        [HttpPost]
        public async Task <ActionResult> UpdateSwap(SwapVM p1, SwapVM p2)           
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var person1 = _passengerRepository.Queryable().Where(c => c.PassengerId == p1.PassengerId).FirstOrDefault();

                    var person2 = _passengerRepository.Queryable().Where(c => c.PassengerId == p2.PassengerId).FirstOrDefault();


                    var person1BusId = person1.BusId;
                    var person1FlightId = person1.FlightId;
                 

                    var person2BusId = person2.BusId;
                    var person2FlightId = person2.FlightId;

                    person1.FlightId = person2.FlightId;
                    person1.BusId = person2.BusId;

                    person1.UpdatedBy = LoggedUserName();
                    person1.UpdatedOn = DateTime.Now;
                    _passengerRepository.Update(person1);
                    await _unitOfWork.SaveChangesAsync();


                    person2.BusId = person1BusId;
                    person2.FlightId = person1FlightId;

                    person2.UpdatedBy = LoggedUserName();
                    person2.UpdatedOn = DateTime.Now;
                    _passengerRepository.Update(person2);



                    var BusNo2 = p2.BusNo;
                    var FlightNo2 = p2.FlightNo;

                

                    var BusNo1 = p1.BusNo;
                    var FlightNo1 = p1.FlightNo;

                    p1.BusNo = BusNo2;
                    p1.FlightNo = FlightNo2;
                    p2.BusNo = BusNo1;
                    p2.FlightNo = FlightNo1;

                    await _unitOfWork.SaveChangesAsync();

                    return Ok(new {p1,p2});

                    //return View();

                }
            }
            catch (Exception)
            {

                throw;
            }

            return RedirectToAction("Swap1");
        }

        #endregion




        //p1flight=p1.FlightId  ;
        //        p1bus=p1.BusId  ;
        //        p2flight=p2.FlightId ;
        //        p2bus=p2.BusId;

        //        p1.FlightId = p2flight;
        //        p1.BusId = p2bus;
        //        p2.FlightId = p1flight;
        //        p2.BusId = p1bus;

        //        passenger.BusId=










        //----------------------Not Used------------------------
        public IActionResult IndexWithList()
        {
            //AlhajjList();
            //BusList();
            //RoomList();
            //FlightList();

            //  AlhajjFlightAsync();

            return View();
        }



        [HttpPost]
        public ActionResult PassengerPost(PassengerViewModel passengerViewModel)
        {
            var alhajjMasterList = JsonConvert.DeserializeObject<List<AlhajjMaster>>(Request.Form["AlhajjsList"]);

            passengerViewModel.AlhajjsList = alhajjMasterList;

            try
            {
                foreach (var item in passengerViewModel.AlhajjsList)
                {

                    Passenger passenger = new Passenger()
                    {
                        AlhajjMaster = item,

                        PligrimageId = item.PligrimageId,

                        BusId = passengerViewModel.BusId,
                        FlightId = passengerViewModel.FlightId,
                        ResidencesId = passengerViewModel.ResidencesId,
                        //PassengerSuppId = item.PligrimageId,

                    };


                    _passengerRepository.Insert(passenger);

                }
                var result = _unitOfWork.SaveChangesAsync();
            }
            catch (Exception es)
            {

                throw;
            }


            return Ok();

            //return this.Content(sb.ToString());
            //return Json(alhajjMasterList);
        }


        [HttpPost]
        public ActionResult PassengerProcess()
        {
            var Alhajjlist = _alHajjRepository.Queryable().Where(x => x.FitResult != 9).ToList();
            var FlightList = _flightRepository.Queryable().Where(x => x.FlightYear == DateTime.Now.Year).ToList();
            var BusList = _busRepository.Queryable().Where(x => x.Year == DateTime.Now.Year).ToList();


            int countFlight = 0;
            int countBus = 0;

            var FlightCapacity = Enumerable.Empty<object>().Select(r => new { flightId = 0, flightCapacity = 0 }).ToList();

            foreach (var item in FlightList)
            {
                FlightCapacity.Add(new
                {
                    flightId = item.FlightId,
                    flightCapacity = item.FlightCapacity,
                });

            }



            List<Passenger> passenger = new List<Passenger>();

            Passenger pass = new Passenger();



            foreach (var hajj in Alhajjlist.Where(x => x.WilayaCode == 5))
            {
                var s = FlightList.Where(c => c.Direction == "Makah").OrderByDescending(x => x.FlightId).SingleOrDefault().FlightCapacity;

                pass.PassengerId = hajj.PligrimageId;

                pass.FlightId = FlightList.Where(c => c.Direction == "Makah").OrderByDescending(x => x.FlightId).SingleOrDefault().FlightId;

                pass.BusId = BusList.Where(x => x.FlightId == pass.FlightId).SingleOrDefault().BusId;

                passenger.Add(pass);

            }






            return Ok();
        }

        public void RoomList()
        {

            var RoomList = _residenceRepository.Queryable()
              .Select(c => new
              {
                  c.ResidencesId,
                  c.Room,
                  c.RoomCapacity,
              }).ToList();
            ViewData["RoomList"] = RoomList;
        }

        //public void CategoryList()
        //{
        //    ViewData["ClassTypeList"] = _parameterRepository.GetClassTypeListAsync()
        //      .Select(c => new
        //      {
        //          c.ParameterId,
        //          c.DescArabic
        //      }).ToList();
        //}

        //public void FlightList()
        //{
        //    var FlightList = _flightRepository.Queryable()
        //        .Select(c => new
        //        {
        //            c.FlightId,
        //            FlightNo = string.Format("{0} رقم الرحلة", c.FlightNo)
        //        }).ToList();
        //    ViewBag.Flights = FlightList;
        //}

        public JsonResult BusList(int FlightId)
        {
            var BusList = _busRepository.Queryable().Where(c => c.FlightId == FlightId)
                .Select(c => new
                {
                    c.BusId,
                    BusNo = string.Format("{0} رقم الباص", c.BusNo)
                }).ToList();


            return Json(BusList);
        }

        public void AlhajjList()
        {
            var HajjList = _alHajjRepository.Queryable().Where(c => c.ParameterId == 3)
                .Select(c => new
                {
                    c.PligrimageId,
                    c.FullName,
                    //FullName=string.Format("ServiceNo:{0},Fullname:{1}", c.ServcieNumber,c.FullName)

                }).ToList();
            ViewData["AlhajjList"] = HajjList;
        }


    }
}
