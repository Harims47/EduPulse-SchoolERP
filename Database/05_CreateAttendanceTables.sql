-- =========================================================================
-- EduPulse AI - Sprint 03 Attendance Module Database Script
-- Target Engine: Microsoft SQL Server 2022
-- Includes: AttendanceSessions, AttendanceEntries tables with foreign keys and indexes
-- =========================================================================

IF OBJECT_ID('dbo.AttendanceEntries', 'U') IS NOT NULL DROP TABLE dbo.AttendanceEntries;
IF OBJECT_ID('dbo.AttendanceSessions', 'U') IS NOT NULL DROP TABLE dbo.AttendanceSessions;
GO

-- 1. AttendanceSessions Table
CREATE TABLE dbo.AttendanceSessions (
    AttendanceSessionId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    ClassId UNIQUEIDENTIFIER NOT NULL,
    SectionId UNIQUEIDENTIFIER NOT NULL,
    Date DATE NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    ModifiedOn DATETIME2 NULL,
    ModifiedByUserId UNIQUEIDENTIFIER NULL,
    CONSTRAINT PK_AttendanceSessions PRIMARY KEY CLUSTERED (AttendanceSessionId),
    CONSTRAINT FK_AttendanceSessions_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_AttendanceSessions_Classes FOREIGN KEY (ClassId) REFERENCES dbo.Classes (ClassId) ON DELETE NO ACTION,
    CONSTRAINT FK_AttendanceSessions_Sections FOREIGN KEY (SectionId) REFERENCES dbo.Sections (SectionId) ON DELETE NO ACTION,
    CONSTRAINT FK_AttendanceSessions_Users_CreatedBy FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION,
    CONSTRAINT FK_AttendanceSessions_Users_ModifiedBy FOREIGN KEY (ModifiedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION,
    CONSTRAINT UQ_AttendanceSessions_Tenant_Class_Section_Date UNIQUE (TenantId, ClassId, SectionId, Date)
);
GO

-- 2. AttendanceEntries Table
CREATE TABLE dbo.AttendanceEntries (
    AttendanceEntryId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    AttendanceSessionId UNIQUEIDENTIFIER NOT NULL,
    StudentId UNIQUEIDENTIFIER NOT NULL,
    TenantId UNIQUEIDENTIFIER NOT NULL,
    Status VARCHAR(10) NOT NULL, -- 'P', 'A', 'L', 'T'
    Remarks NVARCHAR(200) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    ModifiedOn DATETIME2 NULL,
    ModifiedByUserId UNIQUEIDENTIFIER NULL,
    CONSTRAINT PK_AttendanceEntries PRIMARY KEY CLUSTERED (AttendanceEntryId),
    CONSTRAINT FK_AttendanceEntries_Sessions FOREIGN KEY (AttendanceSessionId) REFERENCES dbo.AttendanceSessions (AttendanceSessionId) ON DELETE NO ACTION,
    CONSTRAINT FK_AttendanceEntries_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students (StudentId) ON DELETE NO ACTION,
    CONSTRAINT FK_AttendanceEntries_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_AttendanceEntries_Users_CreatedBy FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION,
    CONSTRAINT FK_AttendanceEntries_Users_ModifiedBy FOREIGN KEY (ModifiedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION,
    CONSTRAINT UQ_AttendanceEntries_Session_Student UNIQUE (AttendanceSessionId, StudentId),
    CONSTRAINT CK_AttendanceEntries_Status CHECK (Status IN ('P', 'A', 'L', 'T'))
);
GO

-- Optimized indexes
CREATE NONCLUSTERED INDEX IX_AttendanceSessions_Lookup
ON dbo.AttendanceSessions (TenantId, ClassId, SectionId, Date, IsDeleted);
GO

CREATE NONCLUSTERED INDEX IX_AttendanceEntries_Session
ON dbo.AttendanceEntries (TenantId, AttendanceSessionId, StudentId, IsDeleted)
INCLUDE (Status, Remarks);
GO

CREATE NONCLUSTERED INDEX IX_AttendanceEntries_Student
ON dbo.AttendanceEntries (TenantId, StudentId, IsDeleted)
INCLUDE (AttendanceSessionId, Status);
GO
