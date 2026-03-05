namespace Pligrimage.Entities
{
    /// <summary>
    /// Central constants for all Hajj domain magic numbers.
    /// Every ParameterId, FitResult, ConfirmCode, etc. is defined here.
    /// No raw integers should appear in controller logic.
    /// </summary>
    public static class HajjConstants
    {
        // ── Pilgrim types (Parameter.Code = "ClassType") ───────────────────
        public static class PilgrimType
        {
            public const int Regular  = 1;   // حاج اصلي
            public const int StandBy  = 2;   // حاج احتياط
            public const int Admin    = 3;   // حاج اداري (Non-Mod)
        }

        // ── Medical fit result codes ────────────────────────────────────────
        public static class FitResult
        {
            public const int Pending          = 7;   // مسجل - awaiting exam
            public const int Fit              = 5;   // لائق
            public const int ConditionallyFit = 8;   // لائق بشروط
            public const int NotFit           = 6;   // غير لائق
            public const int DoctorApproved   = 9;   // موافقة طبية نهائية
        }

        // ── Confirmation codes ──────────────────────────────────────────────
        public static class ConfirmCode
        {
            public const int Pending   = 0;   // لم يتم التأكيد
            public const int Confirmed = 51;  // تم التأكيد
            public const int Cancelled = 99;  // ملغي
        }

        // ── Flight direction parameter IDs ──────────────────────────────────
        public static class FlightDirection
        {
            public const int Departure = 34;  // ذهاب
            public const int Return    = 35;  // عودة
        }

        // ── Employee status parameter code ──────────────────────────────────
        public static class ParamCode
        {
            public const string ClassType   = "ClassType";
            public const string NicExpire   = "NiceExpire";
            public const string PassExpire  = "PassportExpire";
            public const string EmpStatus   = "EmpStatus";
            public const string ConfirmCode = "ConfirmCode";
        }
    }
}
