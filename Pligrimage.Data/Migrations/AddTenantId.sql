-- ============================================================
-- Migration: AddTenantId
-- Adds TenantId to AlhajjMasters, Passengers, and Users tables
-- Run this script against PligrimageDB before deploying the app
-- ============================================================

BEGIN TRANSACTION;

-- ── 1. AlhajjMasters ──────────────────────────────────────────────────────
ALTER TABLE AlhajjMasters
ADD TenantId INT NOT NULL DEFAULT 0;

-- Backfill: set TenantId = UnitId for existing records
UPDATE AlhajjMasters
SET TenantId = ISNULL(UnitId, 0)
WHERE TenantId = 0;

-- Drop the old global NIC unique index
IF EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_AlhajjMasters_NIC' AND object_id = OBJECT_ID('AlhajjMasters')
)
    DROP INDEX IX_AlhajjMasters_NIC ON AlhajjMasters;

-- New unique index: NIC unique per unit per year (active records only)
CREATE UNIQUE NONCLUSTERED INDEX IX_AlhajjMasters_TenantId_NIC_AlhajYear
    ON AlhajjMasters (TenantId, NIC, AlhajYear)
    WHERE IsDeleted = 0;

-- Index for fast tenant queries
CREATE NONCLUSTERED INDEX IX_AlhajjMasters_TenantId
    ON AlhajjMasters (TenantId)
    INCLUDE (AlhajYear, ParameterId, FitResult, IsDeleted);

PRINT '✅ AlhajjMasters: TenantId added, unique index updated';

-- ── 2. Passengers ────────────────────────────────────────────────────────
ALTER TABLE Passengers
ADD TenantId INT NOT NULL DEFAULT 0;

UPDATE Passengers
SET p.TenantId = ISNULL(m.UnitId, 0)
FROM Passengers p
INNER JOIN AlhajjMasters m ON p.PligrimageId = m.PligrimageId
WHERE p.TenantId = 0;

CREATE NONCLUSTERED INDEX IX_Passengers_TenantId_AlhajYear
    ON Passengers (TenantId, AlhajYear);

PRINT '✅ Passengers: TenantId added';

-- ── 3. Users ──────────────────────────────────────────────────────────────
ALTER TABLE Users
ADD TenantId INT NOT NULL DEFAULT 0;

-- Backfill: TenantId = MainUnitId for existing users (0 for SysAdmins)
UPDATE Users
SET TenantId = CASE
    WHEN IsSysAdmin = 1 THEN 0
    ELSE ISNULL(MainUnitId, 0)
END
WHERE TenantId = 0;

CREATE NONCLUSTERED INDEX IX_Users_TenantId
    ON Users (TenantId);

PRINT '✅ Users: TenantId added';

-- ── 4. IsDeleted column (if not already present) ─────────────────────────
-- AlhajjMasters
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('AlhajjMasters') AND name = 'IsDeleted'
)
BEGIN
    ALTER TABLE AlhajjMasters ADD IsDeleted BIT NOT NULL DEFAULT 0;
    ALTER TABLE AlhajjMasters ADD DeletedOn DATETIME NULL;
    ALTER TABLE AlhajjMasters ADD DeletedBy NVARCHAR(100) NULL;
    PRINT '✅ AlhajjMasters: IsDeleted/DeletedOn/DeletedBy added';
END

-- Passengers
IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('Passengers') AND name = 'IsDeleted'
)
BEGIN
    ALTER TABLE Passengers ADD IsDeleted BIT NOT NULL DEFAULT 0;
    ALTER TABLE Passengers ADD DeletedOn DATETIME NULL;
    ALTER TABLE Passengers ADD DeletedBy NVARCHAR(100) NULL;
    PRINT '✅ Passengers: IsDeleted/DeletedOn/DeletedBy added';
END

COMMIT TRANSACTION;

PRINT '✅ Migration AddTenantId completed successfully';
