-- =========================================================================
-- EduPulse AI - Sprint 01 Database Initialization Script
-- Target Engine: Microsoft SQL Server 2022
-- Includes: Tenants, TenantSettings, Users, Roles, UserRoles, AcademicYears, Classes, Sections, ClassSections, Staff
-- =========================================================================

-- Ensure tables are created cleanly in order of dependencies
IF OBJECT_ID('dbo.TenantSettings', 'U') IS NOT NULL DROP TABLE dbo.TenantSettings;
IF OBJECT_ID('dbo.ClassSections', 'U') IS NOT NULL DROP TABLE dbo.ClassSections;
IF OBJECT_ID('dbo.Staff', 'U') IS NOT NULL DROP TABLE dbo.Staff;
IF OBJECT_ID('dbo.Sections', 'U') IS NOT NULL DROP TABLE dbo.Sections;
IF OBJECT_ID('dbo.Classes', 'U') IS NOT NULL DROP TABLE dbo.Classes;
IF OBJECT_ID('dbo.AcademicYears', 'U') IS NOT NULL DROP TABLE dbo.AcademicYears;
IF OBJECT_ID('dbo.UserRoles', 'U') IS NOT NULL DROP TABLE dbo.UserRoles;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL DROP TABLE dbo.Roles;
IF OBJECT_ID('dbo.Tenants', 'U') IS NOT NULL DROP TABLE dbo.Tenants;
GO

-- =========================================================================
-- STEP 1: CREATE TABLES, PRIMARY KEYS & CONSTRAINTS
-- =========================================================================

-- 1. Tenants Table (Global Root Tenant register)
CREATE TABLE dbo.Tenants (
    TenantId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    Name NVARCHAR(150) NOT NULL,
    RegistrationNo VARCHAR(50) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_Tenants PRIMARY KEY CLUSTERED (TenantId),
    CONSTRAINT UQ_Tenants_RegistrationNo UNIQUE (RegistrationNo)
);
GO

-- 2. Roles Table (Global Metadata Roles lookup)
CREATE TABLE dbo.Roles (
    RoleId VARCHAR(30) NOT NULL,
    Description NVARCHAR(150) NOT NULL,
    CONSTRAINT PK_Roles PRIMARY KEY CLUSTERED (RoleId)
);
GO

-- 3. Users Table (Tenant isolated system users)
CREATE TABLE dbo.Users (
    UserId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    Email VARCHAR(100) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (UserId),
    CONSTRAINT FK_Users_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Users_Tenant_Email UNIQUE (TenantId, Email)
);
GO

-- 4. UserRoles Junction Table (Maps RBAC membership)
CREATE TABLE dbo.UserRoles (
    UserId UNIQUEIDENTIFIER NOT NULL,
    RoleId VARCHAR(30) NOT NULL,
    TenantId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT PK_UserRoles PRIMARY KEY CLUSTERED (UserId, RoleId),
    CONSTRAINT FK_UserRoles_Users FOREIGN KEY (UserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION,
    CONSTRAINT FK_UserRoles_Roles FOREIGN KEY (RoleId) REFERENCES dbo.Roles (RoleId) ON DELETE NO ACTION,
    CONSTRAINT FK_UserRoles_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION
);
GO

-- 5. AcademicYears Table (Defines financial year limits)
CREATE TABLE dbo.AcademicYears (
    AcademicYearId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    Name VARCHAR(20) NOT NULL, -- e.g. "2026-2027"
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    ModifiedOn DATETIME2 NULL,
    ModifiedByUserId UNIQUEIDENTIFIER NULL,
    CONSTRAINT PK_AcademicYears PRIMARY KEY CLUSTERED (AcademicYearId),
    CONSTRAINT FK_AcademicYears_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_AcademicYears_Users_CreatedBy FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION,
    CONSTRAINT FK_AcademicYears_Users_ModifiedBy FOREIGN KEY (ModifiedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION,
    CONSTRAINT UQ_AcademicYears_Tenant_Name UNIQUE (TenantId, Name),
    CONSTRAINT CK_AcademicYears_Dates CHECK (EndDate > StartDate)
);
GO

-- 6. Classes Table (Academic grade standards)
CREATE TABLE dbo.Classes (
    ClassId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(30) NOT NULL,
    SortOrder INT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Classes PRIMARY KEY CLUSTERED (ClassId),
    CONSTRAINT FK_Classes_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Classes_Tenant_Name UNIQUE (TenantId, Name)
);
GO

-- 7. Sections Table (Classroom section segments)
CREATE TABLE dbo.Sections (
    SectionId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(10) NOT NULL,
    CONSTRAINT PK_Sections PRIMARY KEY CLUSTERED (SectionId),
    CONSTRAINT FK_Sections_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Sections_Tenant_Name UNIQUE (TenantId, Name)
);
GO

-- 8. Staff Table (Simple CRUD - Teacher and Accountant profile records)
CREATE TABLE dbo.Staff (
    StaffId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER NULL, -- Nullable for staff who do not have system access
    EmployeeCode VARCHAR(50) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Phone VARCHAR(15) NOT NULL,
    Designation VARCHAR(50) NULL,
    PhotoPath VARCHAR(500) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    ModifiedOn DATETIME2 NULL,
    ModifiedByUserId UNIQUEIDENTIFIER NULL,
    CONSTRAINT PK_Staff PRIMARY KEY CLUSTERED (StaffId),
    CONSTRAINT FK_Staff_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_Staff_Users_UserId FOREIGN KEY (UserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION,
    CONSTRAINT FK_Staff_Users_CreatedBy FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION,
    CONSTRAINT FK_Staff_Users_ModifiedBy FOREIGN KEY (ModifiedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Staff_Tenant_Code UNIQUE (TenantId, EmployeeCode)
);
GO

-- 9. ClassSections Table (With ClassTeacherId homeroom assignments)
CREATE TABLE dbo.ClassSections (
    ClassSectionId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    ClassId UNIQUEIDENTIFIER NOT NULL,
    SectionId UNIQUEIDENTIFIER NOT NULL,
    AcademicYearId UNIQUEIDENTIFIER NOT NULL,
    ClassTeacherId UNIQUEIDENTIFIER NULL, -- References Staff(StaffId)
    Capacity INT NOT NULL DEFAULT 40,
    CONSTRAINT PK_ClassSections PRIMARY KEY CLUSTERED (ClassSectionId),
    CONSTRAINT FK_ClassSections_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_ClassSections_Classes FOREIGN KEY (ClassId) REFERENCES dbo.Classes (ClassId) ON DELETE NO ACTION,
    CONSTRAINT FK_ClassSections_Sections FOREIGN KEY (SectionId) REFERENCES dbo.Sections (SectionId) ON DELETE NO ACTION,
    CONSTRAINT FK_ClassSections_AcademicYears FOREIGN KEY (AcademicYearId) REFERENCES dbo.AcademicYears (AcademicYearId) ON DELETE NO ACTION,
    CONSTRAINT FK_ClassSections_Staff FOREIGN KEY (ClassTeacherId) REFERENCES dbo.Staff (StaffId) ON DELETE NO ACTION,
    CONSTRAINT UQ_ClassSections_Combo UNIQUE (TenantId, ClassId, SectionId, AcademicYearId),
    CONSTRAINT CK_ClassSections_Capacity CHECK (Capacity > 0)
);
GO

-- 10. TenantSettings Table (Global school configuration parameters)
CREATE TABLE dbo.TenantSettings (
    TenantId UNIQUEIDENTIFIER NOT NULL,
    ActiveAcademicYearId UNIQUEIDENTIFIER NULL, -- References AcademicYears
    SchoolCode VARCHAR(20) NOT NULL,
    SchoolName NVARCHAR(150) NOT NULL,
    LogoPath VARCHAR(500) NULL,
    AttendanceCutoffTime TIME NOT NULL DEFAULT '09:30:00',
    ReceiptPrefix VARCHAR(10) NOT NULL DEFAULT 'REC-',
    TCPrefix VARCHAR(10) NOT NULL DEFAULT 'TC-',
    TimeZone VARCHAR(50) NOT NULL DEFAULT 'India Standard Time',
    DateFormat VARCHAR(20) NOT NULL DEFAULT 'dd/MM/yyyy',
    CreatedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ModifiedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_TenantSettings PRIMARY KEY CLUSTERED (TenantId),
    CONSTRAINT FK_TenantSettings_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_TenantSettings_AcademicYears FOREIGN KEY (ActiveAcademicYearId) REFERENCES dbo.AcademicYears (AcademicYearId) ON DELETE NO ACTION,
    CONSTRAINT UQ_TenantSettings_Code UNIQUE (SchoolCode)
);
GO

-- =========================================================================
-- STEP 2: CREATE INDEXES FOR FOREIGN KEYS & SEARCH PATHWAYS
-- =========================================================================

-- Security User Lookup
CREATE NONCLUSTERED INDEX IX_Users_TenantId_Email 
ON dbo.Users (TenantId, Email) 
INCLUDE (UserId, PasswordHash, IsActive, IsDeleted);
GO

-- Staff Search Directory index
CREATE NONCLUSTERED INDEX IX_Staff_Lookup
ON dbo.Staff (TenantId, IsActive, IsDeleted)
INCLUDE (EmployeeCode, FirstName, LastName, Designation);
GO

-- ClassSections mapping index
CREATE NONCLUSTERED INDEX IX_ClassSections_Composite
ON dbo.ClassSections (TenantId, AcademicYearId)
INCLUDE (ClassId, SectionId, ClassTeacherId);
GO

-- =========================================================================
-- STEP 3: SEED METADATA AND PILOT LOOKUP DATA
-- =========================================================================

-- 1. Seed global roles
INSERT INTO dbo.Roles (RoleId, Description) VALUES
('SchoolAdmin', 'Administrative tenant manager with full rights.'),
('Accountant', 'Financial cashier with billing rights.');
GO

-- Declaring IDs to bind dependencies in seed operations
DECLARE @SeedTenantId UNIQUEIDENTIFIER = NEWID();
DECLARE @AdminUserId UNIQUEIDENTIFIER = NEWID();
DECLARE @AccountantUserId UNIQUEIDENTIFIER = NEWID();
DECLARE @AcadYearId UNIQUEIDENTIFIER = NEWID();
DECLARE @ClassId_LKG UNIQUEIDENTIFIER = NEWID();
DECLARE @ClassId_UKG UNIQUEIDENTIFIER = NEWID();
DECLARE @ClassId_C1 UNIQUEIDENTIFIER = NEWID();
DECLARE @ClassId_C2 UNIQUEIDENTIFIER = NEWID();
DECLARE @ClassId_C3 UNIQUEIDENTIFIER = NEWID();
DECLARE @SectionId_A UNIQUEIDENTIFIER = NEWID();
DECLARE @SectionId_B UNIQUEIDENTIFIER = NEWID();

-- 2. Insert Pilot Tenant row
INSERT INTO dbo.Tenants (TenantId, Name, RegistrationNo, IsActive)
VALUES (@SeedTenantId, N'Little Buds English Medium School', 'REG/2026/0014', 1);

-- 3. Insert Users (BCrypt Password hash for demo 'SeedPassword123')
INSERT INTO dbo.Users (UserId, TenantId, Email, PasswordHash, IsActive, IsDeleted)
VALUES 
(@AdminUserId, @SeedTenantId, 'admin@littlebuds.com', '$2a$11$Z5nkyR0/Z2sY1m.C2ZtYqecqN2Nl3a9g3k3n3n3n3n3n3n3n3n3n', 1, 0),
(@AccountantUserId, @SeedTenantId, 'cashier@littlebuds.com', '$2a$11$Z5nkyR0/Z2sY1m.C2ZtYqecqN2Nl3a9g3k3n3n3n3n3n3n3n3n3n', 1, 0);

-- 4. Map User Roles
INSERT INTO dbo.UserRoles (UserId, RoleId, TenantId)
VALUES 
(@AdminUserId, 'SchoolAdmin', @SeedTenantId),
(@AccountantUserId, 'Accountant', @SeedTenantId);

-- 5. Set TenantSettings Config
INSERT INTO dbo.TenantSettings (TenantId, ActiveAcademicYearId, SchoolCode, SchoolName, ReceiptPrefix, TimeZone)
VALUES (@SeedTenantId, NULL, 'LBS01', N'Little Buds School', 'REC-LBS-', 'India Standard Time');

-- 6. Set up Academic Year (April 1, 2026 to March 31, 2027)
INSERT INTO dbo.AcademicYears (AcademicYearId, TenantId, Name, StartDate, EndDate, CreatedByUserId)
VALUES (@AcadYearId, @SeedTenantId, '2026-2027', '2026-04-01', '2027-03-31', @AdminUserId);

-- Update TenantSettings Active Year reference
UPDATE dbo.TenantSettings 
SET ActiveAcademicYearId = @AcadYearId 
WHERE TenantId = @SeedTenantId;

-- 7. Seed Standard Classes lookup
INSERT INTO dbo.Classes (ClassId, TenantId, Name, SortOrder) VALUES
(@ClassId_LKG, @SeedTenantId, N'LKG', 1),
(@ClassId_UKG, @SeedTenantId, N'UKG', 2),
(@ClassId_C1, @SeedTenantId, N'Class 1', 3),
(@ClassId_C2, @SeedTenantId, N'Class 2', 4),
(@ClassId_C3, @SeedTenantId, N'Class 3', 5);

-- 8. Seed Standard Sections lookup
INSERT INTO dbo.Sections (SectionId, TenantId, Name) VALUES
(@SectionId_A, @SeedTenantId, N'A'),
(@SectionId_B, @SeedTenantId, N'B');

-- 9. Map ClassSections for Active Academic Year
INSERT INTO dbo.ClassSections (ClassSectionId, TenantId, ClassId, SectionId, AcademicYearId, ClassTeacherId, Capacity) VALUES
(NEWID(), @SeedTenantId, @ClassId_LKG, @SectionId_A, @AcadYearId, NULL, 30),
(NEWID(), @SeedTenantId, @ClassId_UKG, @SectionId_A, @AcadYearId, NULL, 30),
(NEWID(), @SeedTenantId, @ClassId_C1, @SectionId_A, @AcadYearId, NULL, 40),
(NEWID(), @SeedTenantId, @ClassId_C1, @SectionId_B, @AcadYearId, NULL, 40),
(NEWID(), @SeedTenantId, @ClassId_C2, @SectionId_A, @AcadYearId, NULL, 40),
(NEWID(), @SeedTenantId, @ClassId_C3, @SectionId_A, @AcadYearId, NULL, 40);
GO
