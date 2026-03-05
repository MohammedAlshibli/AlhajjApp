using System;

namespace Pligrimage.Web.Dto
{
    public class JundEmployeeDto
    {
        public Guid PersonId { get; set; }
        public Guid? ServiceBranchId { get; set; }
        public string ServiceBranchNameAr { get; set; }
        public string ServiceBranchNameEn { get; set; }
        public int? HrmsServiceBranchCode { get; set; }
        public string ServiceNumber { get; set; }
        public string ServiceNumberHistory { get; set; }
        public Guid RankId { get; set; }
        public string RankEnlgish { get; set; }
        public string RankArabic { get; set; }
        public string ServiceNoPrefixAr { get; set; }
        public string ServiceNoPrefixEn { get; set; }
        public int ServiceNoPostfix { get; set; }
        public Guid? MainUnitId { get; set; }
        public string MainUnitAr { get; set; }
        public string MainUnitEn { get; set; }
        public int? HrmsMainUnitCode { get; set; }
        public Guid? CurrentUnitId { get; set; }
        public string CurrentUnitEn { get; set; }
        public string CurrentUnitAr { get; set; }
        public int? HrmsCurrentUnitCode { get; set; }
        public string EmpNameEn { get; set; }
        public string EmpNameAr { get; set; }
        public string FirstNameEn { get; set; }
        public string SecondNameEn { get; set; }
        public string ThirdNameEn { get; set; }
        public string LastNameEn { get; set; }
        public string FirstNameAr { get; set; }
        public string SecondNameAr { get; set; }
        public string ThirdNameAr { get; set; }
        public string LastNameAr { get; set; }
        public string Tel { get; set; }
        public string Mobile { get; set; }
        public Guid? PositionUnitId { get; set; }
        public string PositionAr { get; set; }
        public string PositionEn { get; set; }
        public int? HrmsPositionCode { get; set; }
        public Guid EmployeeStatusId { get; set; }
        public string EmployeeStatusEn { get; set; }
        public string EmployeeStatusAr { get; set; }
        public string Address { get; set; }
        public string UnitAttachedIn { get; set; }
        public int? HrmsUnitAttachedInCode { get; set; }
        public DateTime? AttachedDate { get; set; }
        public long CivilNumber { get; set; }
        public string PasspostNo { get; set; }
        public DateTime? PasspostExpiryDate { get; set; }
        public string NationalityAr { get; set; }
        public string NationalityEn { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Gender { get; set; }
        public string BloodGroup { get; set; }
        public DateTime? EmploymentStartDate { get; set; }
        public DateTime? EmploymentEndDate { get; set; }
        public int HrmsPfNo { get; set; }
        public string TradeNameAr { get; set; }
        public string TradeNameEn { get; set; }
        public bool? IsManager { get; set; }
        public Guid? UserId { get; set; }
        public Guid? TenantId { get; set; }
        public int RankOrder { get; set; }
        public DateTime? ContractFromDate { get; set; }
        public DateTime? ContractToDate { get; set; }


    }
}
