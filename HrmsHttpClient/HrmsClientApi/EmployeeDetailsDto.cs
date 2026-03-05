using System;
 

namespace HrmsHttpClient.HrmsClientApi
{
    public class EmployeeDetailsDto
    {

        public string SERVICE_NUMBER { get; set; }

        public string OLD_SERVICE_NUMBER { get; set; }
        public string NAME_ARABIC { get; set; }
        public string NIC_NO { get; set; }
        public DateTime? NIC_EXP_DATE { get; set; }
        public DateTime? PP_EXP_DATE { get; set; }
        public string PP_NO { get; set; }
        public DateTime? DOB_T { get; set; }
        public string RANK_CODE { get; set; }
        public string RANK_ARABIC { get; set; }

        public DateTime? L_PROM_DT_T { get; set; }

        public string OFF_NON_CODE { get; set; }

        public int SERVICE { get; set; }
        public string uniT_ARABIC { get; set; }
        public string UNIT { get; set; }
        public string REGION_A { get; set; }

        public string WIL_CODE { get; set; }
        public string WIL_ARABIC { get; set; }

        public string VIL_CODE { get; set; }

        public string VIL_ARABIC { get; set; }

        public string TEL { get; set; }

        public string GSM { get; set; }

        public string BLD_GRP { get; set; }
        public string Blood_A { get; set; }

        public DateTime? DOE_T { get; set; }

        public string SEX_CODE { get; set; }

        public string SEX_A { get; set; }

        public string RELIGION { get; set; }

        public string EMP_STAT { get; set; }

        public string EMP_STATUS_A { get; set; }


    }

}