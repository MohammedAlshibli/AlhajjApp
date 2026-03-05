using ITS.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pligrimage.Entities;
using Pligrimage.Services.Interface;
using Pligrimage.Web.Extensions;
using Pligrimage.Web.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pligrimage.Web.Controllers
{
    public class ConfirmAlhajjMasterController : BaseController
    {
        private readonly IAlHajjMasterServcie _alhajjService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IParameterService _parameterServcie;
        private readonly IAdminService _adminService;
        private readonly HajjSettings _settings;

        public ConfirmAlhajjMasterController(
            IAlHajjMasterServcie alhajService,
            IUnitOfWork unitOfWork,
            IParameterService parameterService,
            IAdminService adminService,
            IOptions<HajjSettings> settings)
        {
            _alhajjService   = alhajService;
            _unitOfWork      = unitOfWork;
            _parameterServcie = parameterService;
            _adminService    = adminService;
            _settings        = settings.Value;
        }

        public IActionResult Index()
        {
            var confirmCodes = _parameterServcie.GetConfirmCodeParameter()
                .Select(c => new { c.ParameterId, c.DescArabic }).ToList();

            ViewData["ConfirmCode"] = confirmCodes;
            return View();
        }

        // BUG-FIX #6: now shows pilgrims with ConfirmCode == Pending (waiting for confirmation)
        // BUG-FIX #11: year-scoped
        // BUG-FIX #6: unit-scoped (was commented out)
        public IActionResult ConfirmAlhajjRead()
        {
            int activeYear = _settings.ActiveHajjYear;
            List<int> userServiceList = _adminService.GetUserServiceListByUnitCode(LoggedUserName()).ToList();

            var list = _alhajjService.Queryable()
                .Include(c => c.Unit)
                .Where(c =>
                    c.ConfirmCode == HajjConstants.ConfirmCode.Pending && // show unconfirmed
                    c.AlhajYear == activeYear &&
                    !c.IsDeleted &&
                    userServiceList.Contains(c.Unit.UnitCode)) // unit-scoped
                .Select(c => new {
                    c.PligrimageId,
                    c.FullName,
                    c.ServcieNumber,
                    c.NIC,
                    c.FitResult,
                    c.ConfirmCode,
                    UnitNameAr = c.Unit != null ? c.Unit.UnitNameAr : ""
                })
                .ToList();

            return Json(list);
        }

        // Confirm a pilgrim (set ConfirmCode = Confirmed)
        [HttpPost]
        public async Task<ActionResult> Confirm(int pligrimageId)
        {
            var pilgrim = _alhajjService.Queryable()
                .FirstOrDefault(c => c.PligrimageId == pligrimageId);

            if (pilgrim == null) return NotFound();

            // BUG-FIX #6: ConfirmCode actually set now
            pilgrim.ConfirmCode = HajjConstants.ConfirmCode.Confirmed;
            pilgrim.UpdatedBy   = LoggedUserName();
            pilgrim.UpdatedOn   = DateTime.Now;

            _alhajjService.Update(pilgrim);
            await _unitOfWork.SaveChangesAsync(); // BUG-FIX #1

            return Ok("تم التأكيد بنجاح");
        }

        // Cancel a pilgrim (soft-delete approach with cancel code)
        [HttpPost]
        public async Task<ActionResult> Cancel(int pligrimageId, string cancelNote)
        {
            var pilgrim = _alhajjService.Queryable()
                .FirstOrDefault(c => c.PligrimageId == pligrimageId);

            if (pilgrim == null) return NotFound();

            pilgrim.ConfirmCode = HajjConstants.ConfirmCode.Cancelled;
            pilgrim.CancelNote  = cancelNote;
            pilgrim.UpdatedBy   = LoggedUserName();
            pilgrim.UpdatedOn   = DateTime.Now;

            _alhajjService.Update(pilgrim);
            await _unitOfWork.SaveChangesAsync(); // BUG-FIX #1

            return Ok("تم الإلغاء");
        }

        [HttpPost]
        public async Task<ActionResult> Update(AlhajjMaster alhajjMaster)
        {
            if (alhajjMaster == null || !ModelState.IsValid)
                return BadRequest();

            alhajjMaster.UpdatedBy = LoggedUserName();
            alhajjMaster.UpdatedOn = DateTime.Now;
            _alhajjService.Update(alhajjMaster);
            await _unitOfWork.SaveChangesAsync(); // BUG-FIX #1

            return RedirectToAction("Index");
        }
    }
}
