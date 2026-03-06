-- ============================================================
-- Seed: Flight Distribution Test Data
-- Matches the mock data shown in the FlightDistribution screen
-- Run against PligrimageDB after all migrations have been applied
-- ============================================================

BEGIN TRANSACTION;

-- ── 1. PARAMETERS ────────────────────────────────────────────────────────
-- Pilgrim types (ClassType): used as ParameterId on AlhajjMasters
-- Flight directions:          used as ParameterId on Flights
-- Fit codes:                  used as FitResult values (stored directly as int, not FK)

SET IDENTITY_INSERT [parameters] ON;

MERGE [parameters] AS target
USING (VALUES
  -- Pilgrim types (ClassType)
  (1,  'ClassType', N'أصلي',                     'Regular',              0, 0, 1),
  (2,  'ClassType', N'احتياط',                   'StandBy',              0, 0, 2),
  (3,  'ClassType', N'إداري',                    'Admin',                0, 0, 3),
  -- Confirmation codes (informational)
  (10, 'ConfirmCode', N'في الانتظار',             'Pending',              0, 0, 0),
  (11, 'ConfirmCode', N'مؤكد من السلاح',          'Confirmed',            0, 0, 51),
  (12, 'ConfirmCode', N'موافقة الإدارة العليا',   'HQ Approved',          0, 0, 77),
  (13, 'ConfirmCode', N'مُعاد للمراجعة',          'Cancelled',            0, 0, 99),
  -- Fit result codes
  (5,  'FitCode',  N'لائق',                      'Fit',                  0, 0, 5),
  (6,  'FitCode',  N'غير لائق',                  'NotFit',               0, 0, 6),
  (7,  'FitCode',  N'في الانتظار',               'Pending',              0, 0, 7),
  (8,  'FitCode',  N'لائق بشروط',                'ConditionallyFit',     0, 0, 8),
  (9,  'FitCode',  N'موافقة طبية نهائية',         'DoctorApproved',       0, 0, 9),
  -- Flight directions
  (34, 'FlightDirection', N'ذهاب',               'Departure',            0, 0, 34),
  (35, 'FlightDirection', N'عودة',               'Return',               0, 0, 35)
) AS source (ParameterId, Code, DescArabic, DescEnglish, MaxValue, MinValue, Value)
ON target.ParameterId = source.ParameterId
WHEN MATCHED THEN UPDATE SET
  Code=source.Code, DescArabic=source.DescArabic, DescEnglish=source.DescEnglish,
  MaxValue=source.MaxValue, MinValue=source.MinValue, Value=source.Value
WHEN NOT MATCHED THEN INSERT
  (ParameterId, Code, DescArabic, DescEnglish, MaxValue, MinValue, Value)
  VALUES (source.ParameterId, source.Code, source.DescArabic, source.DescEnglish,
          source.MaxValue, source.MinValue, source.Value);

SET IDENTITY_INSERT [parameters] OFF;

-- ── 2. UNITS (Branches / Aslaha) ─────────────────────────────────────────
SET IDENTITY_INSERT [units] ON;

MERGE [units] AS target
USING (VALUES
  (1, N'سلاح المدرعات', 'Armoured Corps',   0, '2025-01-01', 30, 5, 1, 1),
  (2, N'سلاح المشاة',   'Infantry Corps',   0, '2025-01-01', 30, 5, 2, 1),
  (3, N'سلاح الهندسة',  'Engineering Corps',0, '2025-01-01', 25, 5, 3, 1),
  (4, N'سلاح المدفعية', 'Artillery Corps',  0, '2025-01-01', 25, 5, 4, 1)
) AS source (UnitId, UnitNameAr, UnitNameEn, ModFlag, AlhajYear, AllowNumber, StandBy, UnitOrder, UnitCode)
ON target.UnitId = source.UnitId
WHEN MATCHED THEN UPDATE SET
  UnitNameAr=source.UnitNameAr, UnitNameEn=source.UnitNameEn,
  AllowNumber=source.AllowNumber, StandBy=source.StandBy,
  UnitOrder=source.UnitOrder
WHEN NOT MATCHED THEN INSERT
  (UnitId, UnitNameAr, UnitNameEn, ModFlag, AlhajYear, AllowNumber, StandBy, UnitOrder, UnitCode)
  VALUES (source.UnitId, source.UnitNameAr, source.UnitNameEn, source.ModFlag,
          source.AlhajYear, source.AllowNumber, source.StandBy, source.UnitOrder, source.UnitCode);

SET IDENTITY_INSERT [units] OFF;

-- ── 3. CATEGORIES (required FK) ───────────────────────────────────────────
SET IDENTITY_INSERT [categories] ON;

IF NOT EXISTS (SELECT 1 FROM [categories] WHERE CategoryId = 1)
  INSERT INTO [categories] (CategoryId, DescArabic, DescEnglish, AlhajYear, QTY)
  VALUES (1, N'افتراضي', 'Default', '2025-01-01', 999);

SET IDENTITY_INSERT [categories] OFF;

-- ── 4. DOCUMENTS (required FK) ────────────────────────────────────────────
SET IDENTITY_INSERT [Documents] ON;

IF NOT EXISTS (SELECT 1 FROM [Documents] WHERE DocumentId = 1)
  INSERT INTO [Documents] (DocumentId, FileName, ContentType, Path, DocumnetType, Year)
  VALUES (1, 'placeholder', 'none', '', 'default', 2025);

SET IDENTITY_INSERT [Documents] OFF;

-- ── 5. RESIDENCES (required FK) ───────────────────────────────────────────
SET IDENTITY_INSERT [Residences] ON;

IF NOT EXISTS (SELECT 1 FROM [Residences] WHERE ResidencesId = 1)
  INSERT INTO [Residences] (ResidencesId, Building, Room, RoomCapacity, Floor, Year)
  VALUES (1, N'مبنى مكة', 101, 4, 1, '2025-01-01');

SET IDENTITY_INSERT [Residences] OFF;

-- ── 6. FLIGHTS  (2 departure + 2 return) ─────────────────────────────────
-- Rule: D1→R2 (dep 1 → ret 2)  |  D2→R1 (dep 2 → ret 1)
SET IDENTITY_INSERT [Flights] ON;

MERGE [Flights] AS target
USING (VALUES
  (1, 'WY 501', '2025-05-25', '2025-05-25', 2025, 120, 'Departure', 34),
  (2, 'WY 503', '2025-05-27', '2025-05-27', 2025, 120, 'Departure', 34),
  (3, 'WY 502', '2025-06-20', '2025-06-20', 2025, 120, 'Return',    35),
  (4, 'WY 504', '2025-06-22', '2025-06-22', 2025, 120, 'Return',    35)
) AS source (FlightId, FlightNo, FlightDate, ArriveDate, FlightYear, FlightCapacity, Direction, ParameterId)
ON target.FlightId = source.FlightId
WHEN MATCHED THEN UPDATE SET
  FlightNo=source.FlightNo, FlightDate=source.FlightDate,
  ArriveDate=source.ArriveDate, FlightCapacity=source.FlightCapacity,
  Direction=source.Direction, ParameterId=source.ParameterId
WHEN NOT MATCHED THEN INSERT
  (FlightId, FlightNo, FlightDate, ArriveDate, FlightYear, FlightCapacity, Direction, ParameterId)
  VALUES (source.FlightId, source.FlightNo, source.FlightDate, source.ArriveDate,
          source.FlightYear, source.FlightCapacity, source.Direction, source.ParameterId);

SET IDENTITY_INSERT [Flights] OFF;

-- ── 7. BUSES (1 per flight, required FK on Passengers) ────────────────────
SET IDENTITY_INSERT [Buses] ON;

MERGE [Buses] AS target
USING (VALUES
  (1, 'Bus-A', 40, 2025, '2025-05-25', 1),
  (2, 'Bus-B', 40, 2025, '2025-05-27', 2),
  (3, 'Bus-C', 40, 2025, '2025-06-20', 3),
  (4, 'Bus-D', 40, 2025, '2025-06-22', 4)
) AS source (BusId, BusNo, BusCapacity, Year, Date, FlightId)
ON target.BusId = source.BusId
WHEN MATCHED THEN UPDATE SET BusNo=source.BusNo
WHEN NOT MATCHED THEN INSERT (BusId, BusNo, BusCapacity, Year, Date, FlightId)
  VALUES (source.BusId, source.BusNo, source.BusCapacity, source.Year, source.Date, source.FlightId);

SET IDENTITY_INSERT [Buses] OFF;

-- ── 8. PILGRIMS (AlhajjMasters) ───────────────────────────────────────────
-- 20 pilgrims matching the prototype:
--   IDs 1-3  → Admin (ParameterId=3)    → must be in Dep-F1
--   IDs 4-20 → Regular/StandBy          → distributed across F1/F2
-- All have: ConfirmCode=77 (HQ Approved), FitResult=9 (Doctor Approved)
-- TenantId = UnitId (multi-tenant rule)

SET IDENTITY_INSERT [AlhajjMasters] ON;

MERGE [AlhajjMasters] AS target
USING (VALUES
--  Id   ServcieNumber NIC        NICExpire      Passport  PassExpire     FullName                         DOB             RankCode RankDesc      RegDate        Snapshote      UnitCode UnitDesc           WorkLoc    Region    WCode WDesc  VCode VDesc  WorkPhone  GSM        Rel1  RC1 RG1  Rel2  RC2 RG2  FitFlag DoctorNote Notes  AlhajYear Blood   DateEnlist CategoryId UnitId DocId ParamId FitResult CancelNote ConfirmCode InjectionDate TenantId
  (1,  'M10000', 85234917, '2030-01-01', 7823401, '2028-03-22', N'نواف بن سالم البلوشي',   '1985-03-12', 6, N'رقيب أول',  '2025-01-15', '2025-01-15', 101, N'سلاح المدرعات', N'مسقط', N'مسقط', 1, N'مسقط', 1, N'مسقط', '99999001', '99888001', N'محمد', 1, 0, N'فاطمة', 2, 0, 0, N'سليم',  N'', 2025, 'A+',  '2000-01-01', 1, 1, 1, 3, 9, NULL, 77, '2025-02-10', 1),
  (2,  'M10001', 78901234, '2029-06-15', 9012345, '2026-06-01', N'سعيد بن خميس الحارثي',   '1980-07-22', 8, N'ملازم أول', '2025-01-16', '2025-01-16', 101, N'سلاح المدرعات', N'مسقط', N'مسقط', 1, N'مسقط', 1, N'مسقط', '99999002', '99888002', N'أحمد',  1, 0, N'مريم',  2, 0, 0, N'بصحة جيدة', N'', 2025, 'O+',  '1998-03-10', 1, 1, 1, 3, 9, NULL, 77, '2025-02-11', 1),
  (3,  'M10002', 91234567, '2028-09-30', 1234567, '2025-12-15', N'خالد بن محمد المعمري',   '1988-11-05', 4, N'عريف',      '2025-01-17', '2025-01-17', 102, N'سلاح المشاة',   N'مسقط', N'الداخلية', 2, N'الداخلية', 2, N'نزوى', '99999003', '99888003', N'علي',   1, 0, N'هند',   2, 0, 0, N'سليم',  N'', 2025, 'B-',  '2005-06-20', 1, 2, 1, 3, 9, NULL, 77, '2025-02-12', 2),
  (4,  'M10003', 82345678, '2031-03-20', 5556677, '2028-09-10', N'يوسف بن علي الريامي',    '1982-04-18', 9, N'رائد',      '2025-01-18', '2025-01-18', 102, N'سلاح المشاة',   N'مسقط', N'مسقط', 1, N'مسقط', 1, N'مسقط', '99999004', '99888004', N'حسن',  1, 0, N'سلمى',  2, 0, 0, N'سليم',  N'', 2025, 'AB+', '2001-09-05', 1, 2, 1, 1, 9, NULL, 77, '2025-02-13', 2),
  (5,  'M10004', 79012345, '2029-12-01', 8901234, '2029-01-10', N'حمد بن ناصر البريكي',    '1979-08-30', 10,N'عقيد',      '2025-01-19', '2025-01-19', 103, N'سلاح الهندسة',  N'مسقط', N'الباطنة', 3, N'الباطنة', 3, N'صحار', '99999005', '99888005', N'عمر',  1, 0, N'نورة',  2, 0, 0, N'سليم',  N'', 2025, 'A-',  '1997-04-15', 1, 3, 1, 1, 9, NULL, 77, '2025-02-14', 3),
  (6,  'M10005', 83456789, '2030-07-14', 3344556, '2027-05-20', N'سلطان بن عيسى المقبالي', '1987-02-14', 3, N'جندي أول',  '2025-01-20', '2025-01-20', 103, N'سلاح الهندسة',  N'مسقط', N'الباطنة', 3, N'الباطنة', 3, N'صحار', '99999006', '99888006', N'يوسف', 1, 0, N'ليلى',  2, 0, 0, N'سليم',  N'', 2025, 'O-',  '2007-11-23', 1, 3, 1, 2, 9, NULL, 77, '2025-02-15', 3),
  (7,  'M10006', 88901234, '2032-05-05', 2233445, '2027-08-30', N'مروان بن حمدان النوفلي',  '1983-09-11', 7, N'نقيب',      '2025-01-21', '2025-01-21', 104, N'سلاح المدفعية', N'مسقط', N'الشرقية', 4, N'الشرقية', 4, N'صور',  '99999007', '99888007', N'ناصر', 1, 0, N'رهام',  2, 0, 0, N'سليم',  N'', 2025, 'B+',  '1999-07-08', 1, 4, 1, 1, 9, NULL, 77, '2025-02-16', 4),
  (8,  'M10007', 76543210, '2028-11-20', 1122334, '2028-11-15', N'عبدالله بن سيف الشكيلي', '1986-06-25', 6, N'رقيب',      '2025-01-22', '2025-01-22', 104, N'سلاح المدفعية', N'مسقط', N'الشرقية', 4, N'الشرقية', 4, N'صور',  '99999008', '99888008', N'خالد', 1, 0, N'بدرية', 2, 0, 0, N'سليم',  N'', 2025, 'A+',  '2003-02-17', 1, 4, 1, 1, 9, NULL, 77, '2025-02-17', 4),
  (9,  'M10008', 84567891, '2030-04-12', 4455667, '2028-06-30', N'أحمد بن سالم الرواحي',   '1981-12-03', 5, N'رقيب أول',  '2025-01-23', '2025-01-23', 101, N'سلاح المدرعات', N'مسقط', N'مسقط', 1, N'مسقط', 1, N'مسقط', '99999009', '99888009', N'سالم', 1, 0, N'أسماء', 2, 0, 0, N'سليم',  N'', 2025, 'O+',  '2000-08-14', 1, 1, 1, 1, 9, NULL, 77, '2025-02-18', 1),
  (10, 'M10009', 77891234, '2029-08-18', 6677889, '2029-02-20', N'محمد بن حمد الهنائي',    '1984-05-17', 8, N'ملازم أول', '2025-01-24', '2025-01-24', 101, N'سلاح المدرعات', N'مسقط', N'مسقط', 1, N'مسقط', 1, N'مسقط', '99999010', '99888010', N'حمد',  1, 0, N'منى',   2, 0, 0, N'سليم',  N'', 2025, 'B+',  '2002-11-29', 1, 1, 1, 2, 9, NULL, 77, '2025-02-19', 1),
  (11, 'M10010', 89012345, '2031-01-28', 7788990, '2027-11-05', N'علي بن محمد الكندي',     '1979-03-22', 4, N'عريف',      '2025-01-25', '2025-01-25', 102, N'سلاح المشاة',   N'مسقط', N'الداخلية', 2, N'الداخلية', 2, N'نزوى', '99999011', '99888011', N'محمد', 1, 0, N'زينب',  2, 0, 0, N'سليم',  N'', 2025, 'A-',  '1997-06-11', 1, 2, 1, 1, 9, NULL, 77, '2025-02-20', 2),
  (12, 'M10011', 80123456, '2030-09-15', 8899001, '2028-04-14', N'ناصر بن أحمد الجابري',   '1986-10-08', 3, N'جندي',      '2025-01-26', '2025-01-26', 102, N'سلاح المشاة',   N'مسقط', N'مسقط', 1, N'مسقط', 1, N'مسقط', '99999012', '99888012', N'أحمد', 1, 0, N'خديجة',2, 0, 0, N'سليم',  N'', 2025, 'AB-', '2006-03-19', 1, 2, 1, 2, 9, NULL, 77, '2025-02-21', 2),
  (13, 'M10012', 75432109, '2028-07-22', 9900112, '2029-07-18', N'سالم بن ناصر العلوي',    '1980-01-14', 9, N'رائد',      '2025-01-27', '2025-01-27', 103, N'سلاح الهندسة',  N'مسقط', N'الظاهرة', 5, N'الظاهرة', 5, N'عبري', '99999013', '99888013', N'يحيى', 1, 0, N'مازن',  2, 0, 0, N'سليم',  N'', 2025, 'O-',  '1998-09-30', 1, 3, 1, 1, 9, NULL, 77, '2025-02-22', 3),
  (14, 'M10013', 86789012, '2030-12-10', 1011223, '2027-01-25', N'حسن بن عيسى الوهيبي',    '1988-04-26', 6, N'رقيب',      '2025-01-28', '2025-01-28', 103, N'سلاح الهندسة',  N'مسقط', N'الباطنة', 3, N'الباطنة', 3, N'صحار', '99999014', '99888014', N'عبدالله',1,0, N'حصة',  2, 0, 0, N'سليم',  N'', 2025, 'B-',  '2005-12-07', 1, 3, 1, 1, 9, NULL, 77, '2025-02-23', 3),
  (15, 'M10014', 92345678, '2032-02-17', 2233446, '2028-10-03', N'طارق بن سالم البادي',    '1985-07-19', 7, N'نقيب',      '2025-01-29', '2025-01-29', 104, N'سلاح المدفعية', N'مسقط', N'جنوب الباطنة', 6, N'جنوب الباطنة', 6, N'بركاء', '99999015','99888015',N'فيصل',1,0,N'سارة',2,0,0, N'سليم',  N'', 2025, 'A+',  '2001-05-14', 1, 4, 1, 2, 9, NULL, 77, '2025-02-24', 4),
  (16, 'M10015', 73456789, '2029-10-05', 3344557, '2029-03-12', N'عمر بن خليل الزدجالي',   '1982-11-23', 5, N'رقيب أول',  '2025-01-30', '2025-01-30', 104, N'سلاح المدفعية', N'مسقط', N'الشرقية', 4, N'الشرقية', 4, N'صور',  '99999016', '99888016', N'جمال', 1, 0, N'وفاء',  2, 0, 0, N'سليم',  N'', 2025, 'O+',  '1999-08-27', 1, 4, 1, 1, 9, NULL, 77, '2025-02-25', 4),
  (17, 'M10016', 87654321, '2031-06-30', 4455668, '2028-01-20', N'يحيى بن علي المحروقي',   '1978-09-15', 10,N'عقيد',      '2025-01-31', '2025-01-31', 101, N'سلاح المدرعات', N'مسقط', N'مسقط', 1, N'مسقط', 1, N'مسقط', '99999017', '99888017', N'كريم', 1, 0, N'ريم',   2, 0, 0, N'سليم',  N'', 2025, 'B+',  '1996-03-22', 1, 1, 1, 1, 9, NULL, 77, '2025-02-26', 1),
  (18, 'M10017', 81234567, '2030-03-08', 5566779, '2027-09-08', N'إبراهيم بن ناصر الصقري', '1984-02-10', 8, N'ملازم أول', '2025-02-01', '2025-02-01', 102, N'سلاح المشاة',   N'مسقط', N'الداخلية', 2, N'الداخلية', 2, N'نزوى', '99999018', '99888018', N'طارق', 1, 0, N'فريدة', 2, 0, 0, N'سليم',  N'', 2025, 'A-',  '2002-07-16', 1, 2, 1, 2, 9, NULL, 77, '2025-02-27', 2),
  (19, 'M10018', 78901235, '2029-05-25', 6677890, '2028-07-15', N'جاسم بن محمد العامري',   '1987-06-30', 4, N'عريف',      '2025-02-02', '2025-02-02', 103, N'سلاح الهندسة',  N'مسقط', N'الظاهرة', 5, N'الظاهرة', 5, N'عبري', '99999019', '99888019', N'وليد', 1, 0, N'إيمان', 2, 0, 0, N'سليم',  N'', 2025, 'AB+', '2006-10-04', 1, 3, 1, 1, 9, NULL, 77, '2025-02-28', 3),
  (20, 'M10019', 83456790, '2031-11-12', 7788901, '2029-05-30', N'فيصل بن أحمد الشريقي',   '1981-08-07', 6, N'رقيب',      '2025-02-03', '2025-02-03', 104, N'سلاح المدفعية', N'مسقط', N'الشرقية', 4, N'الشرقية', 4, N'صور',  '99999020', '99888020', N'رشيد', 1, 0, N'أماني', 2, 0, 0, N'سليم',  N'', 2025, 'O-',  '2000-02-18', 1, 4, 1, 1, 9, NULL, 77, '2025-02-28', 4)
) AS source (
  PligrimageId, ServcieNumber, NIC, NICExpire, Passport, PassportExpire, FullName, DateOfBirth,
  RankCode, RankDesc, RegistrationDate, Snapshote, UnitCode, UnitDesc, WorkLocation, Region,
  WilayaCode, WilayaDesc, VillageCode, VillageDesc, WorkPhone, GSM,
  ReletiveName1, ReletiveCode1, RelativeGsm1, ReletiveName2, ReletiveCode2, RelativeGsm2,
  FitFlag, DoctorNote, Notes, AlhajYear, BloodGroup, DateOfEnlistment,
  CategoryId, UnitId, DocumentId, ParameterId, FitResult, CancelNote, ConfirmCode,
  InjectionDate, TenantId
)
ON target.PligrimageId = source.PligrimageId
WHEN MATCHED THEN UPDATE SET
  FullName=source.FullName, RankDesc=source.RankDesc, UnitId=source.UnitId,
  ParameterId=source.ParameterId, FitResult=source.FitResult, ConfirmCode=source.ConfirmCode,
  BloodGroup=source.BloodGroup, InjectionDate=source.InjectionDate, TenantId=source.TenantId
WHEN NOT MATCHED THEN INSERT (
  PligrimageId, ServcieNumber, NIC, NICExpire, Passport, PassportExpire, FullName, DateOfBirth,
  RankCode, RankDesc, RegistrationDate, Snapshote, UnitCode, UnitDesc, WorkLocation, Region,
  WilayaCode, WilayaDesc, VillageCode, VillageDesc, WorkPhone, GSM,
  ReletiveName1, ReletiveCode1, RelativeGsm1, ReletiveName2, ReletiveCode2, RelativeGsm2,
  FitFlag, DoctorNote, Notes, AlhajYear, BloodGroup, DateOfEnlistment,
  CategoryId, UnitId, DocumentId, ParameterId, FitResult, CancelNote, ConfirmCode,
  InjectionDate, TenantId
) VALUES (
  source.PligrimageId, source.ServcieNumber, source.NIC, source.NICExpire,
  source.Passport, source.PassportExpire, source.FullName, source.DateOfBirth,
  source.RankCode, source.RankDesc, source.RegistrationDate, source.Snapshote,
  source.UnitCode, source.UnitDesc, source.WorkLocation, source.Region,
  source.WilayaCode, source.WilayaDesc, source.VillageCode, source.VillageDesc,
  source.WorkPhone, source.GSM, source.ReletiveName1, source.ReletiveCode1,
  source.RelativeGsm1, source.ReletiveName2, source.ReletiveCode2, source.RelativeGsm2,
  source.FitFlag, source.DoctorNote, source.Notes, source.AlhajYear,
  source.BloodGroup, source.DateOfEnlistment, source.CategoryId, source.UnitId,
  source.DocumentId, source.ParameterId, source.FitResult, source.CancelNote,
  source.ConfirmCode, source.InjectionDate, source.TenantId
);

SET IDENTITY_INSERT [AlhajjMasters] OFF;

-- ── 9. PASSENGERS (flight distribution) ──────────────────────────────────
-- Departure link:  D1(FlightId=1)→R2(FlightId=4)  |  D2(FlightId=2)→R1(FlightId=3)
-- Admins (IDs 1,2,3) → always D1
-- Auto-distribute rest: IDs 4-13 → D1 (fill to 50%)  |  IDs 14-20 → D2
-- Each pilgrim gets 2 rows: 1 departure + 1 return

DELETE FROM [Passengers] WHERE AlhajYear = 2025 AND PligrimageId BETWEEN 1 AND 20;

-- Admin 1 — D1 + R2
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,1,1,1,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,1,4,4,1);
-- Admin 2 — D1 + R2
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,2,1,1,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,2,4,4,1);
-- Admin 3 — D1 + R2
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,3,1,1,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,3,4,4,1);
-- IDs 4-13 → D1 + R2  (fills F1 to 13 passengers)
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,4,1,1,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,4,4,4,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,5,1,1,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,5,4,4,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,6,1,1,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,6,4,4,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,7,1,1,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,7,4,4,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,8,1,1,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,8,4,4,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,9,1,1,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,9,4,4,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,10,1,1,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,10,4,4,1);
-- IDs 14-20 → D2 + R1
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,14,2,2,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,14,3,3,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,15,2,2,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,15,3,3,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,16,2,2,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,16,3,3,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,17,2,2,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,17,3,3,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,18,2,2,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,18,3,3,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,19,2,2,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,19,3,3,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,20,2,2,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,20,3,3,1);
-- IDs 11,12,13 → D1 + R2 (completing F1)
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,11,1,1,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,11,4,4,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,12,1,1,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,12,4,4,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,13,1,1,1);
INSERT INTO [Passengers] (AlhajYear,PligrimageId,FlightId,BusId,ResidencesId) VALUES (2025,13,4,4,1);

-- ── 10. VERIFICATION ─────────────────────────────────────────────────────
SELECT 'parameters'    AS TableName, COUNT(*) AS Rows FROM [parameters]     WHERE Code IN ('ClassType','FlightDirection','FitCode')
UNION ALL SELECT 'units',       COUNT(*) FROM [units]          WHERE UnitId BETWEEN 1 AND 4
UNION ALL SELECT 'Flights',     COUNT(*) FROM [Flights]        WHERE FlightYear = 2025
UNION ALL SELECT 'Buses',       COUNT(*) FROM [Buses]          WHERE Year = 2025
UNION ALL SELECT 'AlhajjMasters',COUNT(*) FROM [AlhajjMasters] WHERE AlhajYear = 2025 AND ConfirmCode = 77
UNION ALL SELECT 'Passengers',  COUNT(*) FROM [Passengers]     WHERE AlhajYear = 2025;

-- ── FLIGHT DISTRIBUTION SUMMARY ───────────────────────────────────────────
SELECT
  f.FlightNo,
  f.Direction,
  COUNT(p.PassengerId) AS Passengers,
  f.FlightCapacity     AS Capacity
FROM [Flights] f
LEFT JOIN [Passengers] p ON p.FlightId = f.FlightId AND p.AlhajYear = 2025
WHERE f.FlightYear = 2025
GROUP BY f.FlightId, f.FlightNo, f.Direction, f.FlightCapacity
ORDER BY f.FlightId;

COMMIT TRANSACTION;

PRINT '✅ Seed completed successfully — AlhajjApp Flight Distribution test data loaded';
