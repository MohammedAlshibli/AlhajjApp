using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HrmsHttpClient.HrmsClientApi;
using ITS.Core.Abstractions.Services;
using ITS.Core.Abstractions.Trackable;
using ITS.Core.Services;
using Microsoft.EntityFrameworkCore;
using Pligrimage.Entities;
using Pligrimage.Entities.Enum;
using Pligrimage.Services.Interface;
namespace Pligrimage.Services.Implementation
{
   public  class AlHajjMasterServcie : Service<AlhajjMaster>, IAlHajjMasterServcie
    {

        public readonly IUnitServcie _unitRepository;

        public AlHajjMasterServcie(ITrackableRepository<AlhajjMaster> alhajjRepository,IUnitServcie unitRepository) : base(alhajjRepository)
        {
            _unitRepository = unitRepository;
        }

        public AlhajjMaster AlhajjMasterMap(EmployeeDetailsDto wEemployeeInfo)
        {
            var unit= _unitRepository.GetUnitByUnitCode( wEemployeeInfo.SERVICE, int.Parse(wEemployeeInfo.EMP_STAT));
            AlhajjMaster newalhajjMaster = new AlhajjMaster();
            newalhajjMaster.FullName = wEemployeeInfo.NAME_ARABIC;
            newalhajjMaster.ServcieNumber = wEemployeeInfo.SERVICE_NUMBER;
            newalhajjMaster.EmployeeStatus = int.Parse(wEemployeeInfo.EMP_STAT) ==0? EmployeeStatus.Employee : EmployeeStatus.Pension;
            newalhajjMaster.NIC = wEemployeeInfo.NIC_NO.All(char.IsDigit) ? wEemployeeInfo.NIC_NO: "لايوجد";
            newalhajjMaster.NICExpire = wEemployeeInfo.NIC_EXP_DATE;
            newalhajjMaster.Passport =wEemployeeInfo.PP_NO == "-" ? "لايوجد":wEemployeeInfo.PP_NO;
            newalhajjMaster.PassportExpire = wEemployeeInfo.PP_EXP_DATE;
            newalhajjMaster.DateOfBirth = wEemployeeInfo.DOB_T;
            newalhajjMaster.Age = GetAge(newalhajjMaster.DateOfBirth.GetValueOrDefault());
            newalhajjMaster.RankCode = int.Parse(wEemployeeInfo.RANK_CODE);
            newalhajjMaster.RankDesc = wEemployeeInfo.RANK_ARABIC;
            newalhajjMaster.HrmsUnitCode = int.Parse(wEemployeeInfo.UNIT);
            newalhajjMaster.HrmsUnitDesc = wEemployeeInfo.uniT_ARABIC;
            newalhajjMaster.Unit = unit;
            newalhajjMaster.UnitId = unit.UnitId;
            newalhajjMaster.Region = wEemployeeInfo.REGION_A;
            newalhajjMaster.WilayaCode  = wEemployeeInfo.WIL_CODE =="-"? 0: int.Parse(wEemployeeInfo.WIL_CODE);
            newalhajjMaster.WilayaDesc = wEemployeeInfo.WIL_ARABIC;
            newalhajjMaster.VillageCode = wEemployeeInfo.VIL_CODE == "-" ? 0 : int.Parse(wEemployeeInfo.VIL_CODE); 
            newalhajjMaster.VillageDesc = wEemployeeInfo.VIL_ARABIC;
            newalhajjMaster.GSM = wEemployeeInfo.GSM;
            newalhajjMaster.BloodGroup = wEemployeeInfo.Blood_A;
            newalhajjMaster.DateOfEnlistment = wEemployeeInfo.DOE_T;
     
            return newalhajjMaster;
        }
        

        //Action to Get the Age
        public int GetAge(DateTime bIRTHDAT)
        {

            if (bIRTHDAT < DateTime.Now)
            {
                return new DateTime(DateTime.Now.Subtract(bIRTHDAT).Ticks).Year - 1;

            }

            return 0;
        }


        public async Task<IEnumerable<AlhajjMaster>> ByServcieNumberAsync(string ServcieNumber)
        {
            return await Repository.Queryable().Where(c => c.ServcieNumber == ServcieNumber).ToListAsync();
        }

        public IEnumerable<AlhajjMaster> GetParameterId()
        {
            return Repository.Queryable().Where(c => c.ParameterId == 1 && c.ParameterId == 3);
        }



       
    }
}
