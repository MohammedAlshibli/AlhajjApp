using Hajj.Entities;
using Hajj.Services;
using Microsoft.AspNetCore.Mvc;

namespace ITS.Core.Api.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Create()
        {

            return View();
        }
    }
}