-- =========================================================================
-- SQL Migration Script: Add Audit and Soft Delete to AcademicYears Table
-- Target Engine: Microsoft SQL Server 2022
-- =========================================================================

BEGIN TRANSACTION;
BEGIN TRY

    -- 1. Add IsDeleted column if it does not exist
    IF COL_LENGTH('dbo.AcademicYears', 'IsDeleted') IS NULL
    BEGIN
        ALTER TABLE dbo.AcademicYears 
        ADD IsDeleted BIT NOT NULL CONSTRAINT DF_AcademicYears_IsDeleted DEFAULT 0;
    END

    -- 2. Add CreatedOn column if it does not exist
    IF COL_LENGTH('dbo.AcademicYears', 'CreatedOn') IS NULL
    BEGIN
        ALTER TABLE dbo.AcademicYears 
        ADD CreatedOn DATETIME2 NOT NULL CONSTRAINT DF_AcademicYears_CreatedOn DEFAULT SYSUTCDATETIME();
    END

    -- 3. Add CreatedByUserId column as NULLable first if it does not exist
    IF COL_LENGTH('dbo.AcademicYears', 'CreatedByUserId') IS NULL
    BEGIN
        ALTER TABLE dbo.AcademicYears 
        ADD CreatedByUserId UNIQUEIDENTIFIER NULL;
    END

    -- 4. Add ModifiedOn column if it does not exist
    IF COL_LENGTH('dbo.AcademicYears', 'ModifiedOn') IS NULL
    BEGIN
        ALTER TABLE dbo.AcademicYears 
        ADD ModifiedOn DATETIME2 NULL;
    END

    -- 5. Add ModifiedByUserId column if it does not exist
    IF COL_LENGTH('dbo.AcademicYears', 'ModifiedByUserId') IS NULL
    BEGIN
        ALTER TABLE dbo.AcademicYears 
        ADD ModifiedByUserId UNIQUEIDENTIFIER NULL;
    END

    -- 6. Populate CreatedByUserId for existing rows
    -- Associate existing records with the first user in the same tenant, or system fallback
    UPDATE ay
    SET ay.CreatedByUserId = COALESCE(
        (SELECT TOP 1 u.UserId FROM dbo.Users u WHERE u.TenantId = ay.TenantId ORDER BY u.CreatedOn ASC),
        (SELECT TOP 1 u.UserId FROM dbo.Users u ORDER BY u.CreatedOn ASC),
        '00000000-0000-0000-0000-000000000000'
    )
    FROM dbo.AcademicYears ay
    WHERE ay.CreatedByUserId IS NULL;

    -- 7. Enforce NOT NULL constraint on CreatedByUserId
    IF EXISTS (
        SELECT 1 
        FROM sys.columns 
        WHERE object_id = OBJECT_ID('dbo.AcademicYears') 
          AND name = 'CreatedByUserId' 
          AND is_nullable = 1
    )
    BEGIN
        ALTER TABLE dbo.AcademicYears 
        ALTER COLUMN CreatedByUserId UNIQUEIDENTIFIER NOT NULL;
    END

    -- 8. Add foreign key for CreatedByUserId if it does not exist
    IF NOT EXISTS (
        SELECT 1 
        FROM sys.foreign_keys 
        WHERE name = 'FK_AcademicYears_Users_CreatedBy' 
          AND parent_object_id = OBJECT_ID('dbo.AcademicYears')
    )
    BEGIN
        ALTER TABLE dbo.AcademicYears 
        ADD CONSTRAINT FK_AcademicYears_Users_CreatedBy 
        FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION;
    END

    -- 9. Add foreign key for ModifiedByUserId if it does not exist
    IF NOT EXISTS (
        SELECT 1 
        FROM sys.foreign_keys 
        WHERE name = 'FK_AcademicYears_Users_ModifiedBy' 
          AND parent_object_id = OBJECT_ID('dbo.AcademicYears')
    )
    BEGIN
        ALTER TABLE dbo.AcademicYears 
        ADD CONSTRAINT FK_AcademicYears_Users_ModifiedBy 
        FOREIGN KEY (ModifiedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION;
    END

    COMMIT TRANSACTION;
    PRINT 'Migration completed successfully.';

END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
    BEGIN
        ROLLBACK TRANSACTION;
    END
    
    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
    DECLARE @ErrorState INT = ERROR_STATE();
    
    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH
GO
