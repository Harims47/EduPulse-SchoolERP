-- =========================================================================
-- EduPulse AI - Sprint 02 Student Foundation Database Script
-- Target Engine: Microsoft SQL Server 2022
-- Includes: Students, Guardians, StudentGuardians tables with foreign keys and indexes
-- =========================================================================

IF OBJECT_ID('dbo.StudentGuardians', 'U') IS NOT NULL DROP TABLE dbo.StudentGuardians;
IF OBJECT_ID('dbo.Guardians', 'U') IS NOT NULL DROP TABLE dbo.Guardians;
IF OBJECT_ID('dbo.Students', 'U') IS NOT NULL DROP TABLE dbo.Students;
GO

-- 1. Students Table
CREATE TABLE dbo.Students (
    StudentId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    AdmissionNo VARCHAR(50) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    DateOfBirth DATE NOT NULL,
    Gender VARCHAR(10) NOT NULL,
    BloodGroup VARCHAR(5) NULL,
    GovernmentIdType VARCHAR(30) NULL,
    GovernmentIdNumber VARCHAR(100) NULL,
    SocialCategory VARCHAR(10) NULL,
    PhotoPath VARCHAR(500) NULL,
    AddressLine1 NVARCHAR(150) NULL,
    AddressLine2 NVARCHAR(150) NULL,
    City NVARCHAR(50) NULL,
    State NVARCHAR(50) NULL,
    Pincode VARCHAR(10) NULL,
    AdmissionDate DATE NOT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'Applied',
    ClassId UNIQUEIDENTIFIER NOT NULL,
    SectionId UNIQUEIDENTIFIER NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    ModifiedOn DATETIME2 NULL,
    ModifiedByUserId UNIQUEIDENTIFIER NULL,
    CONSTRAINT PK_Students PRIMARY KEY CLUSTERED (StudentId),
    CONSTRAINT FK_Students_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_Students_Classes FOREIGN KEY (ClassId) REFERENCES dbo.Classes (ClassId) ON DELETE NO ACTION,
    CONSTRAINT FK_Students_Sections FOREIGN KEY (SectionId) REFERENCES dbo.Sections (SectionId) ON DELETE NO ACTION,
    CONSTRAINT FK_Students_Users_CreatedBy FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION,
    CONSTRAINT FK_Students_Users_ModifiedBy FOREIGN KEY (ModifiedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Students_Tenant_AdmissionNo UNIQUE (TenantId, AdmissionNo),
    CONSTRAINT CK_Students_Status CHECK (Status IN ('Applied', 'Admitted', 'Active', 'Promoted', 'Repeating', 'Transferred', 'Graduated', 'Dropped', 'Archived'))
);
GO

-- 2. Guardians Table
CREATE TABLE dbo.Guardians (
    GuardianId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Phone VARCHAR(15) NOT NULL,
    Email VARCHAR(100) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    ModifiedOn DATETIME2 NULL,
    ModifiedByUserId UNIQUEIDENTIFIER NULL,
    CONSTRAINT PK_Guardians PRIMARY KEY CLUSTERED (GuardianId),
    CONSTRAINT FK_Guardians_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_Guardians_Users FOREIGN KEY (UserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION,
    CONSTRAINT FK_Guardians_Users_CreatedBy FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION,
    CONSTRAINT FK_Guardians_Users_ModifiedBy FOREIGN KEY (ModifiedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Guardians_Tenant_Phone UNIQUE (TenantId, Phone)
);
GO

-- 3. StudentGuardians Junction Table
CREATE TABLE dbo.StudentGuardians (
    StudentId UNIQUEIDENTIFIER NOT NULL,
    GuardianId UNIQUEIDENTIFIER NOT NULL,
    TenantId UNIQUEIDENTIFIER NOT NULL,
    RelationshipType VARCHAR(20) NOT NULL, -- 'Father', 'Mother', 'Guardian'
    IsPrimaryContact BIT NOT NULL DEFAULT 0,
    IsBillingResponsible BIT NOT NULL DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    ModifiedOn DATETIME2 NULL,
    ModifiedByUserId UNIQUEIDENTIFIER NULL,
    CONSTRAINT PK_StudentGuardians PRIMARY KEY CLUSTERED (StudentId, GuardianId),
    CONSTRAINT FK_StudentGuardians_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students (StudentId) ON DELETE NO ACTION,
    CONSTRAINT FK_StudentGuardians_Guardians FOREIGN KEY (GuardianId) REFERENCES dbo.Guardians (GuardianId) ON DELETE NO ACTION,
    CONSTRAINT FK_StudentGuardians_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_StudentGuardians_Users_CreatedBy FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION,
    CONSTRAINT FK_StudentGuardians_Users_ModifiedBy FOREIGN KEY (ModifiedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION,
    CONSTRAINT CK_StudentGuardians_RelationshipType CHECK (RelationshipType IN ('Father', 'Mother', 'Guardian', 'Other'))
);
GO

-- Indexes for performance
CREATE NONCLUSTERED INDEX IX_Students_Tenant_Search
ON dbo.Students (TenantId, ClassId, SectionId, IsDeleted)
INCLUDE (AdmissionNo, FirstName, LastName, Status);
GO

CREATE NONCLUSTERED INDEX IX_Guardians_Tenant_Lookup
ON dbo.Guardians (TenantId, Phone, IsDeleted)
INCLUDE (FirstName, LastName, Email);
GO

CREATE NONCLUSTERED INDEX IX_StudentGuardians_Tenant_Lookup
ON dbo.StudentGuardians (TenantId, StudentId, IsDeleted)
INCLUDE (GuardianId, RelationshipType, IsPrimaryContact);
GO
