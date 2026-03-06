using Microsoft.EntityFrameworkCore;
using Pligrimage.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pligrimage.Data
{
    /// <summary>
    /// Seeds the database with test data that matches the flight distribution prototype.
    ///
    /// Data seeded:
    ///   14 Parameters  (ClassType / FlightDirection / FitCode / ConfirmCode)
    ///    4 Units        (Armoured, Infantry, Engineering, Artillery)
    ///    4 Flights      (WY501 dep, WY503 dep, WY502 ret, WY504 ret)
    ///    4 Buses
    ///   20 AlhajjMasters (3 admins + 17 regular/standby — all HQ-approved + doctor-approved)
    ///   40 Passengers  (each pilgrim: 1 departure row + 1 return row)
    ///
    /// Linkage rule: Dep F1 → Ret F2  |  Dep F2 → Ret F1
    /// Admins (ParameterId=3) always placed in Dep F1.
    /// </summary>
    public static class AppDbContextSeed
    {
        public static void Seed(AppDbContext ctx)
        {
            SeedParameters(ctx);
            SeedUnits(ctx);
            SeedFlights(ctx);
            SeedBuses(ctx);
            SeedResidences(ctx);
            SeedCategories(ctx);
            SeedDocuments(ctx);
            SeedPilgrims(ctx);
            SeedPassengers(ctx);
        }

        private static void SeedParameters(AppDbContext ctx)
        {
            var rows = new[]
            {
                new Parameter { ParameterId=1,  Code="ClassType",       DescArabic="أصلي",                   DescEnglish="Regular",         Value=1  },
                new Parameter { ParameterId=2,  Code="ClassType",       DescArabic="احتياط",                 DescEnglish="StandBy",         Value=2  },
                new Parameter { ParameterId=3,  Code="ClassType",       DescArabic="إداري",                  DescEnglish="Admin",           Value=3  },
                new Parameter { ParameterId=5,  Code="FitCode",         DescArabic="لائق",                   DescEnglish="Fit",             Value=5  },
                new Parameter { ParameterId=6,  Code="FitCode",         DescArabic="غير لائق",               DescEnglish="NotFit",          Value=6  },
                new Parameter { ParameterId=7,  Code="FitCode",         DescArabic="في الانتظار",            DescEnglish="Pending",         Value=7  },
                new Parameter { ParameterId=8,  Code="FitCode",         DescArabic="لائق بشروط",             DescEnglish="ConditionallyFit",Value=8  },
                new Parameter { ParameterId=9,  Code="FitCode",         DescArabic="موافقة طبية نهائية",     DescEnglish="DoctorApproved",  Value=9  },
                new Parameter { ParameterId=10, Code="ConfirmCode",     DescArabic="في الانتظار",            DescEnglish="Pending",         Value=0  },
                new Parameter { ParameterId=11, Code="ConfirmCode",     DescArabic="مؤكد من السلاح",         DescEnglish="Confirmed",       Value=51 },
                new Parameter { ParameterId=12, Code="ConfirmCode",     DescArabic="موافقة الإدارة العليا",  DescEnglish="HQApproved",      Value=77 },
                new Parameter { ParameterId=13, Code="ConfirmCode",     DescArabic="مُعاد للمراجعة",         DescEnglish="Returned",        Value=99 },
                new Parameter { ParameterId=34, Code="FlightDirection", DescArabic="ذهاب",                   DescEnglish="Departure",       Value=34 },
                new Parameter { ParameterId=35, Code="FlightDirection", DescArabic="عودة",                   DescEnglish="Return",          Value=35 },
            };
            foreach (var r in rows)
                if (!ctx.parameters.Any(x => x.ParameterId == r.ParameterId))
                    ctx.parameters.Add(r);
            ctx.SaveChanges();
        }

        private static void SeedUnits(AppDbContext ctx)
        {
            var units = new[]
            {
                new Unit { UnitId=1, UnitNameAr="سلاح المدرعات", UnitNameEn="Armoured Corps",    ModFlag=false, AlhajYear=new DateTime(2025,1,1), AllowNumber=30, StandBy=5, UnitOrder=1, UnitCode=101 },
                new Unit { UnitId=2, UnitNameAr="سلاح المشاة",   UnitNameEn="Infantry Corps",    ModFlag=false, AlhajYear=new DateTime(2025,1,1), AllowNumber=30, StandBy=5, UnitOrder=2, UnitCode=102 },
                new Unit { UnitId=3, UnitNameAr="سلاح الهندسة",  UnitNameEn="Engineering Corps", ModFlag=false, AlhajYear=new DateTime(2025,1,1), AllowNumber=25, StandBy=5, UnitOrder=3, UnitCode=103 },
                new Unit { UnitId=4, UnitNameAr="سلاح المدفعية", UnitNameEn="Artillery Corps",   ModFlag=false, AlhajYear=new DateTime(2025,1,1), AllowNumber=25, StandBy=5, UnitOrder=4, UnitCode=104 },
            };
            foreach (var u in units)
                if (!ctx.units.Any(x => x.UnitId == u.UnitId))
                    ctx.units.Add(u);
            ctx.SaveChanges();
        }

        private static void SeedFlights(AppDbContext ctx)
        {
            // D1→R2 | D2→R1 linkage
            var flights = new[]
            {
                new Flight { FlightId=1, FlightNo="WY 501", FlightDate=new DateTime(2025,5,25), ArriveDate=new DateTime(2025,5,25), FlightYear=2025, FlightCapacity=120, Direction="Departure", ParameterId=34 },
                new Flight { FlightId=2, FlightNo="WY 503", FlightDate=new DateTime(2025,5,27), ArriveDate=new DateTime(2025,5,27), FlightYear=2025, FlightCapacity=120, Direction="Departure", ParameterId=34 },
                new Flight { FlightId=3, FlightNo="WY 502", FlightDate=new DateTime(2025,6,20), ArriveDate=new DateTime(2025,6,20), FlightYear=2025, FlightCapacity=120, Direction="Return",    ParameterId=35 },
                new Flight { FlightId=4, FlightNo="WY 504", FlightDate=new DateTime(2025,6,22), ArriveDate=new DateTime(2025,6,22), FlightYear=2025, FlightCapacity=120, Direction="Return",    ParameterId=35 },
            };
            foreach (var f in flights)
                if (!ctx.Flights.Any(x => x.FlightId == f.FlightId))
                    ctx.Flights.Add(f);
            ctx.SaveChanges();
        }

        private static void SeedBuses(AppDbContext ctx)
        {
            var buses = new[]
            {
                new Buses { BusId=1, BusNo="Bus-A", BusCapacity=60, Year=2025, Date=new DateTime(2025,5,25), FlightId=1 },
                new Buses { BusId=2, BusNo="Bus-B", BusCapacity=60, Year=2025, Date=new DateTime(2025,5,27), FlightId=2 },
                new Buses { BusId=3, BusNo="Bus-C", BusCapacity=60, Year=2025, Date=new DateTime(2025,6,20), FlightId=3 },
                new Buses { BusId=4, BusNo="Bus-D", BusCapacity=60, Year=2025, Date=new DateTime(2025,6,22), FlightId=4 },
            };
            foreach (var b in buses)
                if (!ctx.Buses.Any(x => x.BusId == b.BusId))
                    ctx.Buses.Add(b);
            ctx.SaveChanges();
        }

        private static void SeedResidences(AppDbContext ctx)
        {
            if (!ctx.Residences.Any(r => r.ResidencesId == 1))
            {
                ctx.Residences.Add(new Residences { ResidencesId=1, Building="مبنى مكة", Room=101, RoomCapacity=4, Floor=1, Year=new DateTime(2025,1,1) });
                ctx.SaveChanges();
            }
        }

        private static void SeedCategories(AppDbContext ctx)
        {
            if (!ctx.categories.Any(c => c.CategoryId == 1))
            {
                ctx.categories.Add(new Category { CategoryId=1, DescArabic="افتراضي", DescEnglish="Default", AlhajYear=new DateTime(2025,1,1), QTY=999 });
                ctx.SaveChanges();
            }
        }

        private static void SeedDocuments(AppDbContext ctx)
        {
            if (!ctx.Documents.Any(d => d.DocumentId == 1))
            {
                ctx.Documents.Add(new Document { DocumentId=1, FileName="placeholder", ContentType="none", Path="", DocumnetType="default", Year=2025 });
                ctx.SaveChanges();
            }
        }

        private static void SeedPilgrims(AppDbContext ctx)
        {
            // type 3 = Admin (must stay in Dep F1)
            // type 1 = Regular
            // type 2 = StandBy
            var data = new[]
            {
                (1,  "M10000","نواف بن سالم البلوشي",  "رقيب أول",  1,1,"A+", 3),
                (2,  "M10001","سعيد بن خميس الحارثي",  "ملازم أول", 1,1,"O+", 3),
                (3,  "M10002","خالد بن محمد المعمري",  "عريف",       2,1,"B-", 3),
                (4,  "M10003","يوسف بن علي الريامي",   "رائد",       2,1,"AB+",1),
                (5,  "M10004","حمد بن ناصر البريكي",   "عقيد",       3,1,"A-", 1),
                (6,  "M10005","سلطان بن عيسى المقبالي","جندي أول",   3,1,"O-", 2),
                (7,  "M10006","مروان بن حمدان النوفلي", "نقيب",      4,1,"B+", 1),
                (8,  "M10007","عبدالله بن سيف الشكيلي","رقيب",       4,1,"A+", 1),
                (9,  "M10008","أحمد بن سالم الرواحي",  "رقيب أول",  1,1,"O+", 1),
                (10, "M10009","محمد بن حمد الهنائي",   "ملازم أول", 1,1,"B+", 2),
                (11, "M10010","علي بن محمد الكندي",    "عريف",       2,1,"A-", 1),
                (12, "M10011","ناصر بن أحمد الجابري",  "جندي",       2,1,"AB-",2),
                (13, "M10012","سالم بن ناصر العلوي",   "رائد",       3,1,"O-", 1),
                (14, "M10013","حسن بن عيسى الوهيبي",   "رقيب",       3,1,"B-", 1),
                (15, "M10014","طارق بن سالم البادي",   "نقيب",       4,1,"A+", 2),
                (16, "M10015","عمر بن خليل الزدجالي",  "رقيب أول",  4,1,"O+", 1),
                (17, "M10016","يحيى بن علي المحروقي",  "عقيد",       1,1,"B+", 1),
                (18, "M10017","إبراهيم بن ناصر الصقري","ملازم أول",  2,1,"A-", 2),
                (19, "M10018","جاسم بن محمد العامري",  "عريف",       3,1,"AB+",1),
                (20, "M10019","فيصل بن أحمد الشريقي",  "رقيب",       4,1,"O-", 1),
            };

            foreach (var (id, sn, name, rank, unitId, docId, blood, pType) in data)
            {
                if (ctx.AlhajjMasters.Any(x => x.PligrimageId == id)) continue;
                ctx.AlhajjMasters.Add(new AlhajjMaster
                {
                    PligrimageId     = id,
                    ServcieNumber    = sn,
                    FullName         = name,
                    RankDesc         = rank,
                    RankCode         = 6,
                    UnitId           = unitId,
                    UnitCode         = unitId * 100 + 1,
                    BloodGroup       = blood,
                    AlhajYear        = 2025,
                    ParameterId      = pType,
                    FitResult        = HajjConstants.FitResult.DoctorApproved,  // 9
                    ConfirmCode      = HajjConstants.ConfirmCode.HQApproved,     // 77
                    InjectionDate    = new DateTime(2025, 2, 10).AddDays(id),
                    RegistrationDate = new DateTime(2025, 1, 15).AddDays(id),
                    CategoryId       = 1,
                    DocumentId       = 1,
                    NIC              = 80000000 + id,
                    NICExpire        = new DateTime(2030, 1, 1),
                    Passport         = 1000000 + id,
                    PassportExpire   = new DateTime(2028, 6, 1),
                    TenantId         = unitId,
                    FitFlag          = true,
                    DoctorNote       = "سليم",
                    Notes            = "",
                    Region           = "مسقط",
                    WilayaCode       = 1,
                    VillageCode      = 1,
                    ReletiveCode1    = 1,
                    ReletiveCode2    = 2,
                });
            }
            ctx.SaveChanges();
        }

        private static void SeedPassengers(AppDbContext ctx)
        {
            // Clear existing year-2025 passengers to avoid duplicates on re-run
            var old = ctx.Passengers.Where(p => p.AlhajYear == 2025).ToList();
            if (old.Any()) { ctx.Passengers.RemoveRange(old); ctx.SaveChanges(); }

            var rows = new List<Passenger>();
            void Add(int pid, int flightId, int busId)
                => rows.Add(new Passenger { PligrimageId=pid, FlightId=flightId, BusId=busId, ResidencesId=1, AlhajYear=2025 });

            // IDs 1-13 → Dep F1 (FlightId=1) + Ret R2 (FlightId=4)
            foreach (var id in new[]{ 1,2,3,4,5,6,7,8,9,10,11,12,13 })
            {
                Add(id, 1, 1); // departure F1
                Add(id, 4, 4); // return    R2
            }
            // IDs 14-20 → Dep F2 (FlightId=2) + Ret R1 (FlightId=3)
            foreach (var id in new[]{ 14,15,16,17,18,19,20 })
            {
                Add(id, 2, 2); // departure F2
                Add(id, 3, 3); // return    R1
            }

            ctx.Passengers.AddRange(rows);
            ctx.SaveChanges();
        }
    }
}
