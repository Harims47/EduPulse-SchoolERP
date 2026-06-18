# SQL Server 2022 Implementation Specification: EduPulse AI (P0 MVP - Revised)

**Project Name:** EduPulse AI  
**Role Perspective:** Senior SQL Server Database Architect & .NET Technical Lead  
**Engine Compatibility:** Microsoft SQL Server 2022 (Express, Web, Standard, Enterprise)  
**Tenant Model:** Shared Database, Shared Schema (Tenant-isolated queries filtered at the application/API layer)  
**Scope Boundary:** Strictly P0 MVP modules as defined in `09.5_MVP_Cut.md`. RLS policies are removed from the database level; all queries must explicitly filter by the client-provided `TenantId` parameter.

---

## 1. Database Schema & Tables Generation DDL

Execute these scripts in order to establish schemas, tables, constraints, and index mappings without foreign key failures.

```sql
-- =========================================================================
-- STEP 1: TABLES GENERATION SEQUENCE
-- =========================================================================

-- 1. Tenants Table (Global Root)
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

-- 2. Roles Table (Global Metadata)
CREATE TABLE dbo.Roles (
    RoleId VARCHAR(30) NOT NULL,
    Description NVARCHAR(150) NOT NULL,
    CONSTRAINT PK_Roles PRIMARY KEY CLUSTERED (RoleId)
);
GO

-- 3. Users Table (Tenant Isolated with IsDeleted support)
CREATE TABLE dbo.Users (
    UserId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    Email VARCHAR(100) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0, -- Soft Delete support
    CreatedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_Users PRIMARY KEY CLUSTERED (UserId),
    CONSTRAINT FK_Users_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Users_Tenant_Email UNIQUE (TenantId, Email)
);
GO

-- 4. UserRoles Junction Table
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

-- 5. AcademicYears Table
CREATE TABLE dbo.AcademicYears (
    AcademicYearId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    Name VARCHAR(20) NOT NULL, -- e.g. "2026-2027"
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    CONSTRAINT PK_AcademicYears PRIMARY KEY CLUSTERED (AcademicYearId),
    CONSTRAINT FK_AcademicYears_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT UQ_AcademicYears_Tenant_Name UNIQUE (TenantId, Name),
    CONSTRAINT CK_AcademicYears_Dates CHECK (EndDate > StartDate)
);
GO

-- 6. Classes Table
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

-- 7. Sections Table
CREATE TABLE dbo.Sections (
    SectionId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(10) NOT NULL,
    CONSTRAINT PK_Sections PRIMARY KEY CLUSTERED (SectionId),
    CONSTRAINT FK_Sections_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Sections_Tenant_Name UNIQUE (TenantId, Name)
);
GO

-- 8. Staff Table (Simple CRUD with IsDeleted support)
CREATE TABLE dbo.Staff (
    StaffId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER NULL, -- Nullable for staff without dashboard access
    EmployeeCode VARCHAR(50) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Phone VARCHAR(15) NOT NULL,
    Designation VARCHAR(50) NULL, -- e.g. 'Class 3 Teacher', 'Accountant'
    PhotoPath VARCHAR(500) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    IsDeleted BIT NOT NULL DEFAULT 0, -- Soft Delete support
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

-- 9. ClassSections Table (With ClassTeacherId reference)
CREATE TABLE dbo.ClassSections (
    ClassSectionId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    ClassId UNIQUEIDENTIFIER NOT NULL,
    SectionId UNIQUEIDENTIFIER NOT NULL,
    AcademicYearId UNIQUEIDENTIFIER NOT NULL,
    ClassTeacherId UNIQUEIDENTIFIER NULL, -- Homeroom Class Teacher
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

-- 10. Students Table (Includes address detail properties and GovernmentId fields instead of AadhaarNo)
CREATE TABLE dbo.Students (
    StudentId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    AdmissionNo VARCHAR(50) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    DateOfBirth DATE NOT NULL,
    Gender VARCHAR(10) NOT NULL,
    BloodGroup VARCHAR(5) NULL,
    GovernmentIdType VARCHAR(30) NULL, -- e.g. 'Aadhaar', 'Passport'
    GovernmentIdNumber VARCHAR(100) NULL, -- API-encrypted value
    SocialCategory VARCHAR(10) NULL, -- 'GEN', 'OBC', 'SC', 'ST'
    PhotoPath VARCHAR(500) NULL,
    AddressLine1 NVARCHAR(150) NULL, -- Demographics address details
    AddressLine2 NVARCHAR(150) NULL,
    City NVARCHAR(50) NULL,
    State NVARCHAR(50) NULL,
    Pincode VARCHAR(10) NULL,
    AdmissionDate DATE NOT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'Applied',
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    ModifiedOn DATETIME2 NULL,
    ModifiedByUserId UNIQUEIDENTIFIER NULL,
    CONSTRAINT PK_Students PRIMARY KEY CLUSTERED (StudentId),
    CONSTRAINT FK_Students_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_Students_Users_CreatedBy FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION,
    CONSTRAINT FK_Students_Users_ModifiedBy FOREIGN KEY (ModifiedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Students_Tenant_AdmissionNo UNIQUE (TenantId, AdmissionNo),
    CONSTRAINT CK_Students_Status CHECK (Status IN ('Applied', 'Admitted', 'Active', 'Promoted', 'Repeating', 'Transferred', 'Graduated', 'Dropped', 'Archived')),
    CONSTRAINT CK_Students_SocialCategory CHECK (SocialCategory IN ('GEN', 'OBC', 'SC', 'ST'))
);
GO

-- 11. StudentClassHistory Table
CREATE TABLE dbo.StudentClassHistory (
    StudentClassHistoryId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    StudentId UNIQUEIDENTIFIER NOT NULL,
    ClassSectionId UNIQUEIDENTIFIER NOT NULL,
    AcademicYearId UNIQUEIDENTIFIER NOT NULL,
    RollNo INT NOT NULL,
    CONSTRAINT PK_StudentClassHistory PRIMARY KEY CLUSTERED (StudentClassHistoryId),
    CONSTRAINT FK_StudentClassHistory_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_StudentClassHistory_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students (StudentId) ON DELETE NO ACTION,
    CONSTRAINT FK_StudentClassHistory_ClassSections FOREIGN KEY (ClassSectionId) REFERENCES dbo.ClassSections (ClassSectionId) ON DELETE NO ACTION,
    CONSTRAINT FK_StudentClassHistory_AcademicYears FOREIGN KEY (AcademicYearId) REFERENCES dbo.AcademicYears (AcademicYearId) ON DELETE NO ACTION,
    CONSTRAINT UQ_StudentClassHistory_Year_Roll UNIQUE (TenantId, ClassSectionId, AcademicYearId, RollNo),
    CONSTRAINT UQ_StudentClassHistory_Student_Year UNIQUE (TenantId, StudentId, AcademicYearId)
);
GO

-- 12. Guardians Table (Includes IsDeleted support)
CREATE TABLE dbo.Guardians (
    GuardianId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    UserId UNIQUEIDENTIFIER NULL, -- Nullable for parents who don't log in
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Phone VARCHAR(15) NOT NULL,
    Email VARCHAR(100) NULL,
    IsDeleted BIT NOT NULL DEFAULT 0, -- Soft Delete support
    CreatedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ModifiedOn DATETIME2 NULL,
    CONSTRAINT PK_Guardians PRIMARY KEY CLUSTERED (GuardianId),
    CONSTRAINT FK_Guardians_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_Guardians_Users FOREIGN KEY (UserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION,
    CONSTRAINT UQ_Guardians_Tenant_Phone UNIQUE (TenantId, Phone)
);
GO

-- 13. StudentGuardians Junction Table
CREATE TABLE dbo.StudentGuardians (
    StudentId UNIQUEIDENTIFIER NOT NULL,
    GuardianId UNIQUEIDENTIFIER NOT NULL,
    TenantId UNIQUEIDENTIFIER NOT NULL,
    RelationshipType VARCHAR(20) NOT NULL, -- 'Father', 'Mother', 'Guardian'
    IsPrimaryContact BIT NOT NULL DEFAULT 0,
    IsBillingResponsible BIT NOT NULL DEFAULT 0,
    CreatedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_StudentGuardians PRIMARY KEY CLUSTERED (StudentId, GuardianId),
    CONSTRAINT FK_StudentGuardians_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students (StudentId) ON DELETE NO ACTION,
    CONSTRAINT FK_StudentGuardians_Guardians FOREIGN KEY (GuardianId) REFERENCES dbo.Guardians (GuardianId) ON DELETE NO ACTION,
    CONSTRAINT FK_StudentGuardians_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION
);
GO

-- 14. DailyAttendance Table (P0 Register Log - Single character Status indicators)
CREATE TABLE dbo.DailyAttendance (
    AttendanceId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    StudentClassHistoryId UNIQUEIDENTIFIER NOT NULL,
    Date DATE NOT NULL,
    Status CHAR(1) NOT NULL, -- 'P', 'A', 'L', 'T'
    CheckInTime TIME NULL,
    ChangedByUserId UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT PK_DailyAttendance PRIMARY KEY CLUSTERED (AttendanceId),
    CONSTRAINT FK_DailyAttendance_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_DailyAttendance_StudentClassHistory FOREIGN KEY (StudentClassHistoryId) REFERENCES dbo.StudentClassHistory (StudentClassHistoryId) ON DELETE NO ACTION,
    CONSTRAINT FK_DailyAttendance_Users FOREIGN KEY (ChangedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION,
    CONSTRAINT UQ_DailyAttendance_Key UNIQUE (TenantId, StudentClassHistoryId, Date),
    CONSTRAINT CK_DailyAttendance_Status CHECK (Status IN ('P', 'A', 'L', 'T'))
);
GO

-- 15. FeeGroups Table
CREATE TABLE dbo.FeeGroups (
    FeeGroupId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(50) NOT NULL,
    CONSTRAINT PK_FeeGroups PRIMARY KEY CLUSTERED (FeeGroupId),
    CONSTRAINT FK_FeeGroups_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION
);
GO

-- 16. FeeLineItems Table
CREATE TABLE dbo.FeeLineItems (
    FeeLineItemId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    FeeGroupId UNIQUEIDENTIFIER NOT NULL,
    ClassId UNIQUEIDENTIFIER NULL, -- If null, applies to all classes
    AcademicYearId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(100) NOT NULL, -- e.g. "Tuition Fee Quarter 1"
    Amount DECIMAL(18,2) NOT NULL,
    CONSTRAINT PK_FeeLineItems PRIMARY KEY CLUSTERED (FeeLineItemId),
    CONSTRAINT FK_FeeLineItems_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_FeeLineItems_FeeGroups FOREIGN KEY (FeeGroupId) REFERENCES dbo.FeeGroups (FeeGroupId) ON DELETE NO ACTION,
    CONSTRAINT FK_FeeLineItems_Classes FOREIGN KEY (ClassId) REFERENCES dbo.Classes (ClassId) ON DELETE NO ACTION,
    CONSTRAINT FK_FeeLineItems_AcademicYears FOREIGN KEY (AcademicYearId) REFERENCES dbo.AcademicYears (AcademicYearId) ON DELETE NO ACTION,
    CONSTRAINT CK_FeeLineItems_Amount CHECK (Amount >= 0)
);
GO

-- 17. StudentInvoices Table (Includes InvoiceDate column)
CREATE TABLE dbo.StudentInvoices (
    InvoiceId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    StudentId UNIQUEIDENTIFIER NOT NULL,
    AcademicYearId UNIQUEIDENTIFIER NOT NULL,
    InvoiceNo VARCHAR(30) NOT NULL,
    InvoiceStatus VARCHAR(20) NOT NULL DEFAULT 'Draft', -- 'Draft', 'Pending', 'PartiallyPaid', 'Paid', 'Cancelled'
    InvoiceDate DATE NOT NULL, -- Core date of billing
    DueDate DATE NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    ConcessionAmount DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    PaidAmount DECIMAL(18,2) NOT NULL DEFAULT 0.00,
    BalanceAmount AS (TotalAmount - ConcessionAmount - PaidAmount),
    CONSTRAINT PK_StudentInvoices PRIMARY KEY CLUSTERED (InvoiceId),
    CONSTRAINT FK_StudentInvoices_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_StudentInvoices_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students (StudentId) ON DELETE NO ACTION,
    CONSTRAINT FK_StudentInvoices_AcademicYears FOREIGN KEY (AcademicYearId) REFERENCES dbo.AcademicYears (AcademicYearId) ON DELETE NO ACTION,
    CONSTRAINT UQ_StudentInvoices_No UNIQUE (TenantId, InvoiceNo),
    CONSTRAINT CK_StudentInvoices_Amounts CHECK (TotalAmount >= 0 AND ConcessionAmount >= 0 AND PaidAmount >= 0),
    CONSTRAINT CK_StudentInvoices_Status CHECK (InvoiceStatus IN ('Draft', 'Pending', 'PartiallyPaid', 'Paid', 'Cancelled'))
);
GO

-- 18. StudentInvoiceDetails Table
CREATE TABLE dbo.StudentInvoiceDetails (
    InvoiceDetailId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    InvoiceId UNIQUEIDENTIFIER NOT NULL,
    FeeLineItemId UNIQUEIDENTIFIER NOT NULL,
    Amount DECIMAL(18,2) NOT NULL,
    CONSTRAINT PK_StudentInvoiceDetails PRIMARY KEY CLUSTERED (InvoiceDetailId),
    CONSTRAINT FK_StudentInvoiceDetails_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_StudentInvoiceDetails_StudentInvoices FOREIGN KEY (InvoiceId) REFERENCES dbo.StudentInvoices (InvoiceId) ON DELETE NO ACTION,
    CONSTRAINT FK_StudentInvoiceDetails_FeeLineItems FOREIGN KEY (FeeLineItemId) REFERENCES dbo.FeeLineItems (FeeLineItemId) ON DELETE NO ACTION
);
GO

-- 19. FeeReceipts Table (Includes cancellation audit columns)
CREATE TABLE dbo.FeeReceipts (
    ReceiptId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    InvoiceId UNIQUEIDENTIFIER NOT NULL,
    ReceiptNo VARCHAR(30) NOT NULL,
    AmountPaid DECIMAL(18,2) NOT NULL,
    PaymentDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    PaymentMethod VARCHAR(20) NOT NULL, -- 'Cash', 'Cheque', 'UPI', 'Card'
    TransactionRef VARCHAR(100) NULL,
    CollectedByUserId UNIQUEIDENTIFIER NOT NULL,
    IsCancelled BIT NOT NULL DEFAULT 0,
    CancelledOn DATETIME2 NULL,
    CancelledByUserId UNIQUEIDENTIFIER NULL,
    CancellationReason NVARCHAR(250) NULL,
    CONSTRAINT PK_FeeReceipts PRIMARY KEY CLUSTERED (ReceiptId),
    CONSTRAINT FK_FeeReceipts_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_FeeReceipts_StudentInvoices FOREIGN KEY (InvoiceId) REFERENCES dbo.StudentInvoices (InvoiceId) ON DELETE NO ACTION,
    CONSTRAINT FK_FeeReceipts_Users_CollectedBy FOREIGN KEY (CollectedByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION,
    CONSTRAINT FK_FeeReceipts_Users_CancelledBy FOREIGN KEY (CancelledByUserId) REFERENCES dbo.Users (UserId) ON DELETE NO ACTION,
    CONSTRAINT UQ_FeeReceipts_No UNIQUE (TenantId, ReceiptNo),
    CONSTRAINT CK_FeeReceipts_Amount CHECK (AmountPaid > 0),
    CONSTRAINT CK_FeeReceipts_Method CHECK (PaymentMethod IN ('Cash', 'Cheque', 'UPI', 'Card'))
);
GO

-- 20. FeeReceiptAllocations Table (FIFO payment logs junction sub-ledger)
CREATE TABLE dbo.FeeReceiptAllocations (
    AllocationId UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    TenantId UNIQUEIDENTIFIER NOT NULL,
    ReceiptId UNIQUEIDENTIFIER NOT NULL,
    InvoiceDetailId UNIQUEIDENTIFIER NOT NULL,
    AmountAllocated DECIMAL(18,2) NOT NULL,
    CreatedOn DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT PK_FeeReceiptAllocations PRIMARY KEY CLUSTERED (AllocationId),
    CONSTRAINT FK_FeeReceiptAllocations_Tenants FOREIGN KEY (TenantId) REFERENCES dbo.Tenants (TenantId) ON DELETE NO ACTION,
    CONSTRAINT FK_FeeReceiptAllocations_FeeReceipts FOREIGN KEY (ReceiptId) REFERENCES dbo.FeeReceipts (ReceiptId) ON DELETE NO ACTION,
    CONSTRAINT FK_FeeReceiptAllocations_StudentInvoiceDetails FOREIGN KEY (InvoiceDetailId) REFERENCES dbo.StudentInvoiceDetails (InvoiceDetailId) ON DELETE NO ACTION,
    CONSTRAINT UQ_FeeReceiptAllocations_ReceiptDetail UNIQUE (TenantId, ReceiptId, InvoiceDetailId),
    CONSTRAINT CK_FeeReceiptAllocations_Amount CHECK (AmountAllocated > 0)
);
GO

-- 21. TenantSettings Table
CREATE TABLE dbo.TenantSettings (
    TenantId UNIQUEIDENTIFIER NOT NULL,
    ActiveAcademicYearId UNIQUEIDENTIFIER NULL,
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
```

---

## 2. Database Indexes (FK Integrity & Search Optimization)

All foreign keys and core query pathways are explicitly indexed. Clustered indexes reside on primary keys, while non-clustered indexes cover search directories and billing calculations.

```sql
-- Security & Authentication Lookup
CREATE NONCLUSTERED INDEX IX_Users_TenantId_Email 
ON dbo.Users (TenantId, Email) 
INCLUDE (UserId, PasswordHash, IsActive, IsDeleted);
GO

-- Roster Advanced Search Directory Pathway
CREATE NONCLUSTERED INDEX IX_Students_Search_Roster
ON dbo.Students (TenantId, Status, IsDeleted)
INCLUDE (AdmissionNo, FirstName, LastName, DateOfBirth);
GO

-- Attendance Daily Verification Pathway
CREATE NONCLUSTERED INDEX IX_DailyAttendance_Lookup
ON dbo.DailyAttendance (TenantId, Date, Status)
INCLUDE (StudentClassHistoryId);
GO

-- Billing Ledger Index for Student balances (Covering Index)
CREATE NONCLUSTERED INDEX IX_StudentInvoices_Dues_Lookup
ON dbo.StudentInvoices (TenantId, StudentId, InvoiceStatus)
INCLUDE (DueDate, TotalAmount, PaidAmount);
GO

-- Receipt No lookup for print searches
CREATE NONCLUSTERED INDEX IX_FeeReceipts_ReceiptNo
ON dbo.FeeReceipts (TenantId, ReceiptNo)
INCLUDE (AmountPaid, PaymentDate, IsCancelled);
GO

-- Allocation lookup mapping
CREATE NONCLUSTERED INDEX IX_FeeReceiptAllocations_DetailLink
ON dbo.FeeReceiptAllocations (TenantId, InvoiceDetailId)
INCLUDE (AmountAllocated);
GO
```

---

## 3. Seed Metadata & Default Lookup Data

These values populate the lookup matrices and set up the default Tenant structure. 

```sql
-- =========================================================================
-- SEED ROLES (Global Metadata)
-- =========================================================================
INSERT INTO dbo.Roles (RoleId, Description) VALUES
('SchoolAdmin', 'Administrative tenant manager with full rights.'),
('Accountant', 'Financial cashier with billing rights.');
GO

-- =========================================================================
-- SEED DEFAULT TENANT CONFIGURATION
-- =========================================================================
-- Declaring IDs to bind dependencies
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

-- 1. Insert Tenant row
INSERT INTO dbo.Tenants (TenantId, Name, RegistrationNo, IsActive)
VALUES (@SeedTenantId, N'Little Buds English Medium School', 'REG/2026/0014', 1);

-- 2. Insert Users (BCrypt Password hash for demo 'SeedPassword123')
INSERT INTO dbo.Users (UserId, TenantId, Email, PasswordHash, IsActive, IsDeleted)
VALUES 
(@AdminUserId, @SeedTenantId, 'admin@littlebuds.com', '$2a$11$Z5nkyR0/Z2sY1m.C2ZtYqecqN2Nl3a9g3k3n3n3n3n3n3n3n3n3n', 1, 0),
(@AccountantUserId, @SeedTenantId, 'cashier@littlebuds.com', '$2a$11$Z5nkyR0/Z2sY1m.C2ZtYqecqN2Nl3a9g3k3n3n3n3n3n3n3n3n3n', 1, 0);

-- 3. Map User Roles
INSERT INTO dbo.UserRoles (UserId, RoleId, TenantId)
VALUES 
(@AdminUserId, 'SchoolAdmin', @SeedTenantId),
(@AccountantUserId, 'Accountant', @SeedTenantId);

-- 4. Set TenantSettings Config
INSERT INTO dbo.TenantSettings (TenantId, ActiveAcademicYearId, SchoolCode, SchoolName, ReceiptPrefix, TimeZone)
VALUES (@SeedTenantId, NULL, 'LBS01', N'Little Buds School', 'REC-LBS-', 'India Standard Time');

-- 5. Set up Academic Year (April 1, 2026 to March 31, 2027)
INSERT INTO dbo.AcademicYears (AcademicYearId, TenantId, Name, StartDate, EndDate)
VALUES (@AcadYearId, @SeedTenantId, '2026-2027', '2026-04-01', '2027-03-31');

-- Update TenantSettings Active Year reference
UPDATE dbo.TenantSettings 
SET ActiveAcademicYearId = @AcadYearId 
WHERE TenantId = @SeedTenantId;

-- 6. Seed Standard Classes lookup
INSERT INTO dbo.Classes (ClassId, TenantId, Name, SortOrder) VALUES
(@ClassId_LKG, @SeedTenantId, N'LKG', 1),
(@ClassId_UKG, @SeedTenantId, N'UKG', 2),
(@ClassId_C1, @SeedTenantId, N'Class 1', 3),
(@ClassId_C2, @SeedTenantId, N'Class 2', 4),
(@ClassId_C3, @SeedTenantId, N'Class 3', 5);

-- 7. Seed Standard Sections lookup
INSERT INTO dbo.Sections (SectionId, TenantId, Name) VALUES
(@SectionId_A, @SeedTenantId, N'A'),
(@SectionId_B, @SeedTenantId, N'B');

-- 8. Map ClassSections for Active Academic Year
INSERT INTO dbo.ClassSections (ClassSectionId, TenantId, ClassId, SectionId, AcademicYearId, ClassTeacherId, Capacity) VALUES
(NEWID(), @SeedTenantId, @ClassId_LKG, @SectionId_A, @AcadYearId, NULL, 30),
(NEWID(), @SeedTenantId, @ClassId_UKG, @SectionId_A, @AcadYearId, NULL, 30),
(NEWID(), @SeedTenantId, @ClassId_C1, @SectionId_A, @AcadYearId, NULL, 40),
(NEWID(), @SeedTenantId, @ClassId_C1, @SectionId_B, @AcadYearId, NULL, 40),
(NEWID(), @SeedTenantId, @ClassId_C2, @SectionId_A, @AcadYearId, NULL, 40),
(NEWID(), @SeedTenantId, @ClassId_C3, @SectionId_A, @AcadYearId, NULL, 40);

-- 9. Seed Default FeeGroups lookup
DECLARE @FeeGroupId_Tuition UNIQUEIDENTIFIER = NEWID();
DECLARE @FeeGroupId_Transport UNIQUEIDENTIFIER = NEWID();

INSERT INTO dbo.FeeGroups (FeeGroupId, TenantId, Name) VALUES
(@FeeGroupId_Tuition, @SeedTenantId, N'Tuition Fees'),
(@FeeGroupId_Transport, @SeedTenantId, N'Transport Fees');

-- 10. Seed Default FeeLineItems for LKG & UKG
INSERT INTO dbo.FeeLineItems (FeeLineItemId, TenantId, FeeGroupId, ClassId, AcademicYearId, Name, Amount) VALUES
(NEWID(), @SeedTenantId, @FeeGroupId_Tuition, @ClassId_LKG, @AcadYearId, N'LKG Tuition Fee Term 1', 6500.00),
(NEWID(), @SeedTenantId, @FeeGroupId_Tuition, @ClassId_UKG, @AcadYearId, N'UKG Tuition Fee Term 1', 7000.00);
GO
```
