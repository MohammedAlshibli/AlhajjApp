using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITS.Core.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pligrimage.Entities;
using Pligrimage.Services.Interface;
using Pligrimage.Web.Extensions;

namespace Pligrimage.Web.Controllers
{
    public class UploadController : BaseController
    {

        IHostingEnvironment _hostingEnvironment;
        public readonly IUnitServcie _unitRepository;
        public readonly IAlHajjMasterServcie _alHajjRepostory;
        public readonly IUnitOfWork _unitOfWork;
        public readonly IParameterService _parameterRepository;
        public readonly ICategoryService _categoryRepository;
        public readonly IDocumentServcie _documentRepository;



        public UploadController(IHostingEnvironment env, IUnitServcie unitRepository,IAlHajjMasterServcie alHajjRepostory,IUnitOfWork unitOfWork,
            IParameterService parameterRepository, ICategoryService categoryRepository, IDocumentServcie documentRepository)

        {
            _hostingEnvironment = env;
            _unitRepository = unitRepository;
            _alHajjRepostory = alHajjRepostory;
            _unitOfWork = unitOfWork;
            _parameterRepository = parameterRepository;
            _categoryRepository = categoryRepository;
            _documentRepository = documentRepository;


        }

        //[PligrimageFiltter]
        //public IActionResult Index()
        //{
        //    return View();
        //}

        [PligrimageFiltter]

        public IActionResult ImportExport()
        {
            var list = new List<int> { 1, 2 };
            ViewBag.NonModService = _unitRepository.Queryable()
                //.Where(c => c.ModFlag == false)
                .Select(C=>new
                {
                    C.UnitNameAr,
                    C.UnitId
                });

            ViewBag.AlhajjType = _parameterRepository.Queryable()
              .Where(c => list.Contains(c.ParameterId))
              .Select(C => new
              {
                  C.DescArabic,
                  C.ParameterId
              });

            return View();


     

        }

  

        //[HttpPost]
        //public ActionResult OnPostImport()
        //{
        //    List<AlhajjMaster> alhajjMasterList = new List<AlhajjMaster>();

        //    //var classTypeList = _classType.ParaByCode("ClassType");

        //    int NonModServiceId = int.Parse(Request.Form["NonModServiceId"]);
        //    string NonModServieName = Request.Form["NonModServiceName"];

        //    int AlhajjTypeId = int.Parse(Request.Form["AlhajjTypeId"]);
        //    string AlhajjTypeDesc = Request.Form["AlhajjTypeDesc"];


        //    IFormFile file = Request.Form.Files[0];
        //    string folderName = "Upload";
        //    string webRootPath = _hostingEnvironment.WebRootPath;
        //    string newPath = Path.Combine(webRootPath, folderName);
        //    StringBuilder sb = new StringBuilder();
        //    if (!Directory.Exists(newPath))
        //    {
        //        Directory.CreateDirectory(newPath);
        //    }
        //    if (file.Length > 0)
        //    {
        //        string sFileExtension = Path.GetExtension(file.FileName).ToLower();
        //        ISheet sheet;
        //        string fullPath = Path.Combine(newPath, file.FileName);
        //        using (var stream = new FileStream(fullPath, FileMode.Create))
        //        {
        //            file.CopyTo(stream);
        //            stream.Position = 0;
        //            if (sFileExtension == ".xls")
        //            {
        //                HSSFWorkbook hssfwb = new HSSFWorkbook(stream); //This will read the Excel 97-2000 formats  
        //                sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook  
        //            }
        //            else
        //            {
        //                XSSFWorkbook hssfwb = new XSSFWorkbook(stream); //This will read 2007 Excel format  
        //                sheet = hssfwb.GetSheetAt(0); //get first sheet from workbook   
        //            }
               
                
        //            for (int i = ( sheet.FirstRowNum +2); i <= sheet.LastRowNum; i++) //Read Excel File
        //                /*for (int i =21; i <= sheet.LastRowNum; i++)*/ //Read Excel File

        //                {
        //                    IRow row = sheet.GetRow(i);
        //                if (row == null) continue;
        //                if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;

        //                AlhajjMaster item = new AlhajjMaster()
        //                {

        //                    ServcieNumber = row.GetCell(0).ToString(),
        //                    RankDesc = row.GetCell(1).ToString(),
        //                    FullName = row.GetCell(2).ToString(),
        //                    NIC = row.GetCell(3).ToString(),
        //                    NICExpire = DateTime.Parse(row.GetCell(4).ToString()),
        //                    //NICExpire = DateTime.ParseExact((row.GetCell(4).ToString()),"dd/MM/yyyy", CultureInfo.InvariantCulture ),

        //                    Passport = row.GetCell(5).ToString(),
        //                    PassportExpire = DateTime.Parse(row.GetCell(6).ToString()),
        //                    //PassportExpire = DateTime.ParseExact((row.GetCell(6).ToString()), "dd/MM/yyyy", CultureInfo.InvariantCulture),
        //                    Region = row.GetCell(7).ToString(),
        //                    WilayaDesc = row.GetCell(8).ToString(),
        //                    VillageDesc = row.GetCell(9).ToString(),

        //                    BloodGroup = row.GetCell(10,MissingCellPolicy.RETURN_NULL_AND_BLANK).ToString(),
        //                    GSM = row.GetCell(11).ToString(),
        //                    DateOfBirth = DateTime.Parse(row.GetCell(12).ToString()),
        //                    //DateOfBirth = DateTime.ParseExact((row.GetCell(12).ToString()), "dd/MM/yyyy", CultureInfo.InvariantCulture),

        //                    ReletiveName1 = row.GetCell(13).ToString(),
        //                    RelativeGsm1 = int.Parse(row.GetCell(14).ToString()),
        //                    ReletiveRelation1 = row.GetCell(15).ToString(),

        //                    ReletiveName2 = row.GetCell(16).ToString(),
        //                    RelativeGsm2 = int.Parse(row.GetCell(17, MissingCellPolicy.RETURN_NULL_AND_BLANK).ToString()),
        //                    ReletiveRelation2 = row.GetCell(18).ToString(),
        //                    Unit = _unitRepository.Queryable().FirstOrDefault(c => c.UnitId == NonModServiceId),
        //                    Parameter = _parameterRepository.Queryable().FirstOrDefault(c => c.ParameterId == AlhajjTypeId),

        //                    //Parameter = _parameterRepository.Queryable().FirstOrDefault(c => c.ParameterId ==1),
        //                    Document = _documentRepository.Queryable().FirstOrDefault(c => c.DocumentId ==1),
        //                    FitResult = 7,
        //                    AlhajYear = DateTime.Now.Year,

        //                };

        //                alhajjMasterList.Add(item);
                        

               
        //            }
        //        }
        //    }
        //    return Json(alhajjMasterList);
        //}
    }
}