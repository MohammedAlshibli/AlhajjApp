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
    public class BusesController : BaseController
    {
        private readonly IBusServcie _busServcie;
        private readonly IFlightServcie _flightServcie;
        private readonly IUnitOfWork _unitOfWork;
        


        public BusesController(IFlightServcie flightServcie,IUnitOfWork unitOfWork, IBusServcie busServcie)
        {
            _flightServcie = flightServcie;
            _unitOfWork = unitOfWork;
            _busServcie = busServcie;

        }

        [PligrimageFiltter]
        public IActionResult Index()
        {
            //var ff = LoggedUserName();
            FlightList();
            return View();
        }

        public void FlightList()
        {
            var FlightList = _flightServcie.Queryable()
                .Select(c => new
                    {
                        c.FlightId,
                        c.FlightNo
                    }).ToList();
            ViewData["Flights"] = FlightList;


        }
        public IActionResult BusRead()
        {

            return Json(_busServcie.Queryable().ToList());
            
        }

        [HttpPost]
        public ActionResult CreateBus(Buses buses)
        {

            buses.CreateOn = DateTime.Now;
            buses.CreateBy = LoggedUserName();

            if (buses != null && ModelState.IsValid)
            {
                _busServcie.Insert(buses);
                _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Buses");

        }


        [HttpPost]
        public ActionResult UpdateBus(Buses buses)
        {
            buses.UpdatedBy = LoggedUserName();
            buses.UpdatedOn = DateTime.Now;
            if (buses != null && ModelState.IsValid)
            {
                _busServcie.Update(buses);
                _unitOfWork.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Buses");
        }


        [HttpPost]
        public ActionResult DeleteBus(Buses buses)
        {
            if (buses != null && ModelState.IsValid)
            {
                _busServcie.Delete(buses);
                _unitOfWork.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Buses");
        }









        
    }
}
