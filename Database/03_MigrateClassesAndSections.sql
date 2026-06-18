-- =========================================================================
-- SQL Migration Script: Add Audit and Soft Delete to Classes and Sections
-- Target Engine: Microsoft SQL Server 2022
-- =========================================================================

BEGIN TRANSACTION;
BEGIN TRY

    -- ==========================================
    -- MIGRATION FOR CLASSES TABLE
    -- ==========================================

    -- 1. Add IsDeleted column to Classes
    IF COL_LENGTH('dbo.Classes', 'IsDeleted') IS NULL
    BEGIN
        ALTER TABLE dbo.Classes 
        ADD IsDeleted BIT NOT NULL CONSTRAINT DF_Classes_IsDeleted DEFAULT 0;
    END

    -- 2. Add CreatedOn column to Classes
    IF COL_LENGTH('dbo.Classes', 'CreatedOn') IS NULL
    BEGIN
        ALTER TABLE dbo.Classes 
        ADD CreatedOn DATETIME2 NOT NULL CONSTRAINT DF_Classes_CreatedOn DEFAULT SYSUTCDATETIME();
    END

    -- 3. Add CreatedByUserId column to Classes (nullable first)
    IF COL_LENGTH('dbo.Classes', 'CreatedByUserId') IS NULL
    BEGIN
        ALTER TABLE dbo.Classes 
        ADD CreatedByUserId UNIQUEIDENTIFIER NULL;
    END

    -- 4. Add ModifiedOn column to Classes
    IF COL_LENGTH('dbo.Classes', 'ModifiedOn') IS NULL
    BEGIN
        ALTER TABLE dbo.Classes 
        ADD ModifiedOn DATETIME2 NULL;
    END

    -- 5. Add ModifiedByUserId column to Classes
    IF COL_LENGTH('dbo.Classes', 'ModifiedByUserId') IS NULL
    BEGIN
        ALTER TABLE dbo.Classes 
        ADD ModifiedByUserId UNIQUEIDENTIFIER NULL;
    END

    -- 6. Populate CreatedByUserId for existing Classes using Dynamic SQL to avoid parse errors
    EXEC('
        UPDATE c
        SET c.CreatedByUserId = COALESCE(
            (SELECT TOP 1 u.UserId FROM dbo.Users u WHERE u.TenantId = c.TenantId ORDER BY u.CreatedOn ASC),
            (SELECT TOP 1 u.UserId FROM dbo.Users u ORDER BY u.CreatedOn ASC),
            ''00000000-0000-0000-0000-000000000000''
        )
        FROM dbo.Classes c
        WHERE c.CreatedByUserId IS NULL;
    ');

    -- 7. Make CreatedByUserId NOT NULL using Dynamic SQL
    IF EXISTS (
        SELECT 1 
        FROM sys.columns 
        WHERE object_id = OBJECT_ID('dbo.Classes') 
          AND name = 'CreatedByUserId' 
          AND is_nullable = 1
    )
    BEGIN
        EXEC('ALTER TABLE dbo.Classes ALTER COLUMN CreatedByUserId UNIQUEIDENTIFIER NOT NULL;');
    END

    -- 8. Add foreign keys for Classes
    IF NOT EXISTS (
        SELECT 1 
        FROM sys.foreign_keys 
        WHERE name = 'FK_Classes_Users_CreatedBy' 
          AND parent_object_id = OBJECT_ID('dbo.Classes')
    )
    BEGIN
        ALTER TABLE dbo.Classes 
        ADD CONSTRAINT FK_Classes_Users_CreatedBy 
        FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION;
    END

    IF NOT EXISTS (
        SELECT 1 
        FROM sys.foreign_keys 
        WHERE name = 'FK_Classes_Users_ModifiedBy' 
          AND parent_object_id = OBJECT_ID('dbo.Classes')
    )
    BEGIN
        ALTER TABLE dbo.Classes 
        ADD CONSTRAINT FK_Classes_Users_ModifiedBy 
        FOREIGN KEY (ModifiedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION;
    END


    -- ==========================================
    -- MIGRATION FOR SECTIONS TABLE
    -- ==========================================

    -- 1. Add IsDeleted column to Sections
    IF COL_LENGTH('dbo.Sections', 'IsDeleted') IS NULL
    BEGIN
        ALTER TABLE dbo.Sections 
        ADD IsDeleted BIT NOT NULL CONSTRAINT DF_Sections_IsDeleted DEFAULT 0;
    END

    -- 2. Add CreatedOn column to Sections
    IF COL_LENGTH('dbo.Sections', 'CreatedOn') IS NULL
    BEGIN
        ALTER TABLE dbo.Sections 
        ADD CreatedOn DATETIME2 NOT NULL CONSTRAINT DF_Sections_CreatedOn DEFAULT SYSUTCDATETIME();
    END

    -- 3. Add CreatedByUserId column to Sections (nullable first)
    IF COL_LENGTH('dbo.Sections', 'CreatedByUserId') IS NULL
    BEGIN
        ALTER TABLE dbo.Sections 
        ADD CreatedByUserId UNIQUEIDENTIFIER NULL;
    END

    -- 4. Add ModifiedOn column to Sections
    IF COL_LENGTH('dbo.Sections', 'ModifiedOn') IS NULL
    BEGIN
        ALTER TABLE dbo.Sections 
        ADD ModifiedOn DATETIME2 NULL;
    END

    -- 5. Add ModifiedByUserId column to Sections
    IF COL_LENGTH('dbo.Sections', 'ModifiedByUserId') IS NULL
    BEGIN
        ALTER TABLE dbo.Sections 
        ADD ModifiedByUserId UNIQUEIDENTIFIER NULL;
    END

    -- 6. Populate CreatedByUserId for existing Sections using Dynamic SQL
    EXEC('
        UPDATE s
        SET s.CreatedByUserId = COALESCE(
            (SELECT TOP 1 u.UserId FROM dbo.Users u WHERE u.TenantId = s.TenantId ORDER BY u.CreatedOn ASC),
            (SELECT TOP 1 u.UserId FROM dbo.Users u ORDER BY u.CreatedOn ASC),
            ''00000000-0000-0000-0000-000000000000''
        )
        FROM dbo.Sections s
        WHERE s.CreatedByUserId IS NULL;
    ');

    -- 7. Make CreatedByUserId NOT NULL using Dynamic SQL
    IF EXISTS (
        SELECT 1 
        FROM sys.columns 
        WHERE object_id = OBJECT_ID('dbo.Sections') 
          AND name = 'CreatedByUserId' 
          AND is_nullable = 1
    )
    BEGIN
        EXEC('ALTER TABLE dbo.Sections ALTER COLUMN CreatedByUserId UNIQUEIDENTIFIER NOT NULL;');
    END

    -- 8. Add foreign keys for Sections
    IF NOT EXISTS (
        SELECT 1 
        FROM sys.foreign_keys 
        WHERE name = 'FK_Sections_Users_CreatedBy' 
          AND parent_object_id = OBJECT_ID('dbo.Sections')
    )
    BEGIN
        ALTER TABLE dbo.Sections 
        ADD CONSTRAINT FK_Sections_Users_CreatedBy 
        FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION;
    END

    IF NOT EXISTS (
        SELECT 1 
        FROM sys.foreign_keys 
        WHERE name = 'FK_Sections_Users_ModifiedBy' 
          AND parent_object_id = OBJECT_ID('dbo.Sections')
    )
    BEGIN
        ALTER TABLE dbo.Sections 
        ADD CONSTRAINT FK_Sections_Users_ModifiedBy 
        FOREIGN KEY (ModifiedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION;
    END

    COMMIT TRANSACTION;
    PRINT 'Migration for Classes and Sections completed successfully.';

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
