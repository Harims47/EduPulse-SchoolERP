# MVP Development & Implementation Plan: EduPulse AI

**Project Name:** EduPulse AI  
**Role Perspective:** Technical Project Manager, SaaS Architect, Senior .NET Lead, and React Lead  
**Target Delivery Window:** 3 Months (12 Weeks)  
**Developer Capacity:** Solo Full-Stack Developer (~40 hours/week, Total: 480 hours)  
**Core Stack:** React (Redux Toolkit + Bootstrap 5) | .NET 8 Web API + Dapper | SQL Server 2022 (Single DB Multi-Tenant with RLS)  

---

## 1. Recommended Development Sequence

To launch within the tight 90-day window, the build sequence must prioritize **structural architecture first**. A solo developer cannot afford to refactor foundational schema definitions mid-project. Therefore, the implementation follows a strict bottom-up, risk-mitigated model:

```
┌─────────────────────────────────────────────────────────────────┐
│ PHASE 1: Core Infra & Security (Weeks 1 - 2)                   │
│ - DB Schemas, RLS Setup, Auth APIs, React Shells & Layouts      │
└───────────────────────────────┬─────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│ PHASE 2: Registries & Onboarding (Weeks 3 - 6)                  │
│ - Academics configuration, Staff profiles, Admissions, SIS Roster│
└───────────────────────────────┬─────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│ PHASE 3: Billing & Finance Engine (Weeks 7 - 10)                │
│ - Fee groups, Invoicing, Cashier desk, FIFO allocations, Voids  │
└───────────────────────────────┬─────────────────────────────────┘
                                │
                                ▼
┌─────────────────────────────────────────────────────────────────┐
│ PHASE 4: Classroom Ops & Deploy (Weeks 11 - 12)                 │
│ - Attendance, Homework, Exams, Rollovers, QuestPDF, VPS Compose │
└─────────────────────────────────────────────────────────────────┘
```

1. **Infrastructure & Authentication (Sprint 1):** Focuses on secure tenant isolation (SQL Row-Level Security), Dapper context middleware, and the JWT token exchange.
2. **Academic & Staff Registries (Sprint 2):** Configures the physical schedule of classes, sections, and subjects before importing people.
3. **Student Directory & Admissions (Sprint 3):** Launches the admissions CRM pipeline and enables bulk imports via Excel to get data into the system.
4. **Billing & Invoices Engine (Sprint 4):** Standardizes fee groups, dynamic concessions, and builds the batch invoicing routine.
5. **Cashier Desks & Payment Auditing (Sprint 5):** Codes the desk ledger transactions, FIFO fee allocations, and voiding reversals.
6. **Classroom Operations, Rollovers & Deployments (Sprint 6):** Marks attendance, logs grades, compiles report card PDFs via QuestPDF, validates and executes year rollovers, and deploys the entire stack to the VM container.

---

## 2. Module Dependency Matrix

This matrix maps blockers. Modules in the **Blocked Module** column cannot be developed until their corresponding **Prerequisites** are completed and verified.

| Blocked Module | Key Prerequisites | Critical Dependencies / Blocks | Business Justification |
| :--- | :--- | :--- | :--- |
| **1. Multi-Tenant Admin** | None | Users, RBAC, Academic Years | Every DB query requires TenantId validation via RLS. Must be first. |
| **2. Users & RBAC** | Multi-Tenant Admin | Student SIS, Staff registries | Profiles must be bound to active security credentials. |
| **3. Class/Section Config**| Academic Years | Subjects, Staff mapping, Student SIS | Students must be mapped to class sections upon onboarding. |
| **4. Student SIS Roster** | Users, Class/Section Config | Attendance, Invoices, Exams | Cannot assign fees, marks, or attendance to non-existent students. |
| **5. Fees & Invoicing** | Student SIS Roster | Cashier Desk, Concessions | Ledger must have generated invoices before payment collection. |
| **6. Cashier Payments** | Fees & Invoicing | Ledger Reversals, Defaulter Reports | Receipts must allocate amounts to active invoice line items. |
| **7. Exams & Homework** | Student SIS, Staff | CBSE Report Card compile | Cannot record grades or homework without students and subjects. |
| **8. Rollover Manager** | Exams, SIS, Fees, Invoices | New Term operations | Year rollover requires all exams finalized and dues resolved. |

---

## 3. Database Implementation Order

Database migrations are managed using **DbUp** inside the .NET Web API and executed sequentially to prevent foreign key constraint violations. Table configurations incorporate all modifications from the Database Architect review:

```
┌────────────────────────────────────────────────────────┐
│              DATABASE TABLE GENERATION SEQUENCE        │
├────────────────────────────────────────────────────────┤
│  1. Tenants ──► 2. TenantSettings ──► 3. Users         │
│                                                        │
│  4. Roles ──► 5. UserRoles ──► 6. AcademicYears        │
│                                                        │
│  7. Classes ──► 8. Sections ──► 9. ClassSections       │
│                                                        │
│  10. Subjects ──► 11. ClassSectionSubjects             │
│                                                        │
│  12. Staff ──► 13. TeacherClassSubjectAssignment       │
│                                                        │
│  14. Students ──► 15. StudentClassHistory              │
│                                                        │
│  16. StudentDocuments ──► 17. Guardians                 │
│                                                        │
│  18. StudentGuardians ──► 19. DailyAttendance          │
│                                                        │
│  20. FeeGroups ──► 21. FeeLineItems                    │
│                                                        │
│  22. StudentInvoices ──► 23. StudentInvoiceDetails     │
│                                                        │
│  24. Concessions ──► 25. FeeReceipts                   │
│                                                        │
│  26. FeeReceiptAllocations ──► 27. Exams               │
│                                                        │
│  28. ExamMarks ──► 29. Homework                        │
│                                                        │
│  30. NotificationLogs ──► 31. StudentStatusHistory     │
└────────────────────────────────────────────────────────┘
```

1. **`Tenants`**: Root multi-tenant definition.
2. **`TenantSettings`**: Custom properties (school logo, receipt prefixes, TC prefixes, attendance cut-off).
3. **`Users`**, **`Roles`**, **`UserRoles`**: Identity database (stores hashed passwords, email indices, and RBAC flags).
4. **`AcademicYears`**: Term ranges (e.g., Start Date: April 1, End Date: March 31).
5. **`Classes`**: Grade levels (e.g., Nursery, Class 1, Class 2).
6. **`Sections`**: Physical section divisions (e.g., Section A, Section B).
7. **`ClassSections`**: Junction binding academic years, classes, and sections. *Includes `ClassTeacherId` (FK to Staff)*.
8. **`Subjects`**: Master course catalog (e.g., Mathematics, Science).
9. **`ClassSectionSubjects`**: Maps which subjects are taught in which specific class section.
10. **`Staff`**: Teacher and administrative profiles. *Includes designation and custom audit properties (`CreatedOn`, `CreatedByUserId`, `ModifiedOn`, `ModifiedByUserId`)*.
11. **`TeacherClassSubjectAssignment`**: Maps teachers to the subjects they instruct in specific class sections.
12. **`Students`**: Core pupil registry. *Includes encrypted `AadhaarNo`, `SocialCategory`, `MotherTongue`, `PhotoPath`, status, and standard audit fields*.
13. **`StudentClassHistory`**: Tracks historical promotions across academic years.
14. **`StudentDocuments`** *[NEW]*: Decouples files (TC scans, Aadhaar PDFs, birth records) from student profile rows.
15. **`Guardians`**: Mother, father, and local guardian profiles.
16. **`StudentGuardians`**: Junction mapping sibling relationships and parent-to-student links.
17. **`DailyAttendance`**: Registers presence status (`P`, `A`, `L`, `T`).
18. **`FeeGroups`**: Bins fee types (e.g., Tuition Fee, Transport Fee, Registration Fee).
19. **`FeeLineItems`**: Maps charges to specific academic years and classes.
20. **`StudentInvoices`**: Master header invoices. *Includes `InvoiceStatus` (Draft, Pending, PartiallyPaid, Paid, Overdue, Cancelled)*.
21. **`StudentInvoiceDetails`**: Line-item breakdowns of student invoices.
22. **`Concessions`**: Holds approved scholarships and fee discounts.
23. **`FeeReceipts`**: Header payment logs. *Includes cancellation metadata (`IsCancelled`, `CancelledOn`, `CancelledByUserId`, `CancellationReason`)*.
24. **`FeeReceiptAllocations`** *[NEW]*: Tracks partial payments by mapping receipts to specific invoice details line-items.
25. **`Exams`**: Master exam schedules (e.g., Term 1 Exam, Periodic Assessment 1).
26. **`ExamMarks`**: Student mark matrices mapped by exam, subject, and class.
27. **`Homework`**: Homework post log with direct volume file link attachments.
28. **`NotificationLogs`**: History of SMS and WhatsApp messages sent to parents.
29. **`StudentStatusHistory`**: Tracks student lifecycle transitions (`Applied` ──► `Admitted` ──► `Active` ──► `Transferred`).

---

## 4. API Implementation Order

Endpoints are built inside .NET controllers consuming raw SQL via Dapper, utilizing claims-based tenant injection via `sp_set_session_context` on connection open.

### Block 1: Security & Setup (Sprint 1, W1-2)
* `POST /api/auth/login` - Authenticates credentials, returns JWT containing `TenantId`, `UserId`, and `Roles`.
* `GET /api/settings` & `PUT /api/settings` - Fetches/updates school identity details and configuration thresholds.
* `GET /api/users` & `PUT /api/users/{id}/status` - Basic admin panel user access management.

### Block 2: Academic Structures (Sprint 2, W3-4)
* `GET /api/academic-years` & `POST /api/academic-years` - Year ranges for configuration dropdowns.
* `GET /api/classes` & `POST /api/classes` - Core grade level configurations.
* `GET /api/class-sections` & `POST /api/class-sections` - Configures sections and assigns `ClassTeacherId`.
* `GET /api/subjects` & `POST /api/subjects` & `PUT /api/subjects/{id}` & `DELETE /api/subjects/{id}` - Complete subject inventory management.
* `POST /api/class-section-subjects` - Maps subjects to class sections.

### Block 3: Onboarding & SIS (Sprint 3, W5-6)
* `POST /api/admissions` & `GET /api/admissions` - Intake pipeline registers prospective student data.
* `POST /api/admissions/{id}/approve` - SQL transaction: marks as `Admitted`, configures login, sets up student class history.
* `POST /api/admissions/{id}/reject` - Moves application to archived state with reasons.
* `GET /api/students/search` - Server-side paginated search filtering by name, admission code, class, and status.
* `GET /api/students/{id}` & `PUT /api/students/{id}` - Standard profile editor.
* `POST /api/students/import` - Bulk parser reads uploaded Excel files, executes bulk SQL inserts.
* `POST /api/students/{id}/documents` & `GET /api/students/{id}/documents` - Uploads/retrieves attachments (stored in Docker volume).
* `GET /api/staff` & `GET /api/staff/{id}` & `PUT /api/staff/{id}` & `POST /api/staff` - Staff database manager.
* `GET /api/guardians/{id}` - Guardian profile retrieval.

### Block 4: Fee Structure & Invoicing (Sprint 4, W7-8)
* `GET /api/fees/groups` & `POST /api/fees/groups` - Configures fee accounts (Tuition, Science Lab).
* `GET /api/fees/line-items` & `POST /api/fees/line-items` & `DELETE /api/fees/line-items/{id}` - Core pricing structures.
* `POST /api/invoices/generate-batch` - Invoicing engine: fetches section rosters, calculates net amounts, bulk-inserts details.
* `GET /api/invoices` & `GET /api/invoices/{id}` - Retrieves invoices and balances.

### Block 5: Cashier Ledger & Payments (Sprint 5, W9-10)
* `POST /api/receipts/collect` - FIFO Payment Engine: receives fee amount, inserts receipt, allocates funds across invoice lines in FIFO order, updates balances.
* `POST /api/receipts/{id}/cancel` - Payment cancellation: rolls back invoice allocations and marks receipt as cancelled.
* `POST /api/receipts/razorpay-webhook` - Online payment confirmation receiver.
* `POST /api/concessions` - Registers specific discounts for students.

### Block 6: Classroom Ops, Rollovers, Reports, & Dashboards (Sprint 6, W11-12)
* `GET /api/attendance/roster` & `POST /api/attendance/submit` - Roster fetch and attendance registration.
* `POST /api/attendance/unlock` - Overrides cutoff lock for a specific class section.
* `POST /api/homework` & `GET /api/homework/feed` - Teacher homework posts and parent updates feed.
* `POST /api/exams/{id}/marks/submit` & `PUT /api/exams/{id}/publish` - Registers exam score matrices and publishes them.
* `GET /api/reports/fee-defaulters` - Pulls parent contact info for outstanding fee balances.
* `GET /api/reports/daily-absentees` - Pulls list of absent students for the day.
* `POST /api/academic-years/rollover/validate` - Validates rollover prerequisites (unpaid dues, draft exams).
* `POST /api/academic-years/rollover/execute` - Promotes students and rolls outstanding dues to the next academic year.
* `GET /api/dashboard/admin` & `GET /api/dashboard/parent` - Performance indicator metrics and alerts feeds.
* `GET /api/notifications/logs` - Sent message records.

---

## 5. Frontend Screen Implementation Order

UI screens are developed in React, styled with Bootstrap 5, and state-managed via Redux Toolkit slices.

### Block 1: Security Setup (Weeks 1 - 2)
* `LoginScreen` - Basic authentication, redirects users to corresponding layout panels based on role claims.
* **Layout Templates (`AdminLayout`, `TeacherLayout`, `ParentLayout`)** - Core navigational headers and sidebars.

### Block 2: Infrastructure Panels (Weeks 3 - 4)
* `AcademicYearManager` - Controls current and future academic terms.
* `ClassSectionConfigurator` - Dynamic grid mapping sections, subjects, and homeroom teachers.

### Block 3: Onboarding & Directories (Weeks 5 - 6)
* `AdmissionsReviewBoard` - Pipeline interface to review, approve, or reject student applications.
* `StudentSearchDirectory` - Main listing page with pagination, filters, and bulk Excel import upload drawer.
* `StudentProfileForm` - Bio-data edit forms including parent mappings, Aadhaar configurations, and demographic parameters.
* `StudentDocumentsVault` - File attachment manager for student files.
* `Student360View` *[NEW]* - Tabbed view aggregating profile, attendance history, academic marks, and financial ledgers.
* `GuardianDirectory` & `StaffDirectory` - Management grids for parent and staff records.

### Block 4: Financial Core (Weeks 7 - 8)
* `FeeStructureConfigurator` - Assigns pricing rules to grade standards.
* `InvoiceDirectory` - Searchable invoice index showing payment status badges.

### Block 5: Payment Desks (Weeks 9 - 10)
* `CashierPaymentDesk` - Core workspace allowing partial or full payment entry with dynamic receipt preview.
* `ReceiptHistoryDesk` *[NEW]* - Logs receipts and contains the "Void Modal" to cancel payments.
* `ConcessionsReviewBoard` - Form to assign scholarships and dynamic fee deductions.

### Block 6: Classroom Logging, Reports, & Rollovers (Weeks 11 - 12)
* `DailyAttendanceSheet` - Desktop/mobile optimized grid with large `P`, `A`, `L`, `T` touch buttons (no complex swipes).
* `HomeworkPlanner` - Editor to post homework logs with file attachments.
* `GradeBookEntryGrid` - Inline spreadsheet grid for rapid marks entry.
* `ExamPublicationPanel` - Manages terms and publishes report cards.
* `SchoolReportsDesk` - PDF generation launcher for defaulters and rosters.
* `AcademicYearRolloverBoard` *[NEW]* - Preconditions validation panel and execution board.
* `TransferCertificateManager` *[NEW]* - Manages student exits, dues verification, and prints exit certificates.
* `TenantSettingsDashboard` - Manage school information and operational configurations.

---

## 6. Sprint Breakdown

```
┌───────────────────────────────────────────────────────────────────────────────────────┐
│ S1: Core Framework (W1-2)    ──► S2: Academic Config (W3-4) ──► S3: Onboarding (W5-6)  │
│                                                                                       │
│ S6: Ops & Deployment (W11-12) ◄── S5: Cashier Desks (W9-10)  ◄── S4: Billing (W7-8)    │
└───────────────────────────────────────────────────────────────────────────────────────┘
```

### Sprint 1: Core Framework & Security (Weeks 1 - 2)
* **Goal:** Scaffolding database, setting up Row-Level Security, security endpoints, JWT auth, and basic UI navigation structures.
* **Tasks:**
  * Write SQL scripts for tables 1-5 (`Tenants` through `UserRoles`).
  * Register Security RLS functions (`Security.fn_securitypredicate`) and bind security filter policies.
  * Initialize .NET 8 Web API project with Dapper, DbUp migrations, and custom connection factory mapping tenant variables to `SESSION_CONTEXT`.
  * Setup React SPA using Vite, configure Redux Toolkit store (auth slice), and Bootstrap 5 responsive wrappers.
  * Develop `LoginScreen`, `AdminLayout`, `TeacherLayout`, and `ParentLayout` structures.
* **Verification Checks:**
  * Attempt to query another tenant's data using a direct SQL query under tenant context; verify SQL Server RLS filters rows out.
  * Confirm JWT expiry triggers automatic client redirect to the Login screen.
* **Estimated Effort:** 70 Hours.

### Sprint 2: Academic Registry & Configuration (Weeks 3 - 4)
* **Goal:** Build structural settings for terms, classes, sections, subjects, and staff mappings.
* **Tasks:**
  * Deploy migrations for tables 6-11 (`AcademicYears` through `TeacherClassSubjectAssignment`).
  * Implement backend APIs for academic years, classes, sections, and subjects (with full CRUD).
  * Build React interfaces: `AcademicYearManager`, `ClassSectionConfigurator` (including `ClassTeacherId` mappings), and `StaffDirectory` with editing modals.
* **Verification Checks:**
  * Verify deleting a subject returns a block error if that subject is mapped to active class sections.
  * Confirm class teacher assignments display correctly in dropdown mappings.
* **Estimated Effort:** 70 Hours.

### Sprint 3: Onboarding & Student Directory (Weeks 5 - 6)
* **Goal:** Launch the admissions CRM pipeline, student search engine, profile details editor, and bulk imports.
* **Tasks:**
  * Deploy migrations for tables 12-16 (`Students` through `Guardians`).
  * Implement Admissions APIs (Submission, Listing, Approval, Rejection workflows).
  * Develop Student advanced search and profile edit APIs (storing encrypted Aadhaar numbers using AES-256-GCM at the API level).
  * Build React UI: `AdmissionsReviewBoard`, `StudentSearchDirectory` with CSV/Excel import dropzones, `StudentProfileForm`, and `StudentDocumentsVault`.
  * Build `Student360View` tabbed dashboard aggregating child information.
* **Verification Checks:**
  * Verify approving an admission transitions status to `Admitted` and populates `StudentClassHistory` within a single database transaction.
  * Confirm bulk Excel import correctly maps parents by phone number index to prevent duplicate guardian entries.
* **Estimated Effort:** 80 Hours.

### Sprint 4: Fee Structures & Billing Engine (Weeks 7 - 8)
* **Goal:** Set up billing pricing metrics and build the background invoicing engine.
* **Tasks:**
  * Deploy migrations for tables 17-21 (`FeeGroups` through `StudentInvoiceDetails`).
  * Implement CRUD APIs for fee groups and fee line items.
  * Develop .NET invoicing engine: `POST /api/invoices/generate-batch`. Inside a single database transaction, read student section rosters, deduct active concessions, and bulk insert invoices.
  * Build React UI: `FeeStructureConfigurator` and `InvoiceDirectory`.
* **Verification Checks:**
  * Confirm dynamic concessions subtract values correctly from invoice totals before line insertion.
  * Verify batch invoice generator handles a class of 100 students in less than 2 seconds.
* **Estimated Effort:** 75 Hours.

### Sprint 5: Cashier Ledger & Payments Desk (Weeks 9 - 10)
* **Goal:** Configure desk workstations to collect payments, allocate them via FIFO rules, print receipts, and void transactions.
* **Tasks:**
  * Deploy migrations for tables 22-24 (`Concessions` through `FeeReceiptAllocations`).
  * Implement `POST /api/receipts/collect`. Build a FIFO allocation loop: apply received amount to outstanding invoice details lines sorted by due date, insert receipt, and write allocations.
  * Build payment cancellation API (`POST /api/receipts/{id}/cancel`): rolls back invoice allocations and marks receipt as cancelled.
  * Build React UI: `CashierPaymentDesk` with browser-based thermal printer CSS layout, `ReceiptHistoryDesk` (with void modal), and `ConcessionsReviewBoard`.
* **Verification Checks:**
  * Test partial payment: Confirm a receipt for ₹5,000 against a ₹10,000 invoice (containing 3 different fee lines) allocates funds in FIFO order.
  * Verify voiding a receipt resets invoice status back to `PartiallyPaid` or `Pending` and recalculates outstanding balances.
* **Estimated Effort:** 85 Hours.

### Sprint 6: Operations, Rollovers, & VPS Deployment (Weeks 11 - 12)
* **Goal:** Create classroom log registers, execute academic rollovs, set up reports, and deploy services to target servers.
* **Tasks:**
  * Deploy migrations for tables 25-31 (`Exams` through `StudentStatusHistory`).
  * Implement APIs: Daily attendance logging, homework postings, grade entries, reports (defaulters list, daily absences), and rollover validations/executes.
  * Setup .NET Background Service with `System.Threading.Channels` for queueing notifications.
  * Integrate QuestPDF to compile student report cards.
  * Build React UI: `DailyAttendanceSheet` (P/A/L/T grid), `HomeworkPlanner`, `GradeBookEntryGrid`, `AcademicYearRolloverBoard` (Validate/Execute screen), and `TransferCertificateManager`.
  * Scaffold `docker-compose.yml` defining Caddy proxy, SQL Server, and .NET web api. Configure TLS via Let's Encrypt. Host the React build on Vercel.
* **Verification Checks:**
  * Verify daily attendance locks editing after 09:30 AM unless overridden by an admin unlock token.
  * Confirm year-end rollover blocks execution if there are pending unsubmitted grades.
* **Estimated Effort:** 100 Hours.

---

## 7. Project Milestones

| Milestone | Target Week | Core Deliverable | Definition of Done (DoD) | Verification Check |
| :--- | :--- | :--- | :--- | :--- |
| **M1: Core Infra** | End of Week 2 | RLS isolated tenant container | Auth token returned; tenant database isolation validated. | Run cross-tenant API requests; confirm SQL throws access errors. |
| **M2: Struct Setup** | End of Week 4 | Academic schedule ready | Years, classes, sections, and subjects mapped. | Check dropdown bindings populate data dynamically from APIs. |
| **M3: SIS Active** | End of Week 6 | Student profile database | Student registry active with document upload support. | Import 100-student test Excel file; confirm roster loads without errors. |
| **M4: Billing Up** | End of Week 8 | Invoices generated | Fee categories configured; batch invoice generation active. | Run batch generator; confirm invoices write to student ledgers. |
| **M5: Ledger Online**| End of Week 10 | Payment desk operational | Collect cash, print thermal receipts, allocate partial payments. | Make partial payment; check that invoice balance decreases by exact value. |
| **M6: GO-LIVE** | End of Week 12 | Deployed ERP system | Classroom modules operational, rollover engine live, Docker deployed. | Deploy system; verify parent dashboard displays attendance & fee alerts. |

---

## 8. Development Effort & Hour Estimates

Below is the resource allocation for the 3-month schedule.

| Task Category / Sprint Module | Database Migrations | Backend API (C#) | React Frontend (UI) | Testing & QA | Total Hours |
| :--- | :---: | :---: | :---: | :---: | :---: |
| **S1: Framework & Identity** | 8 hrs | 24 hrs | 20 hrs | 18 hrs | **70 Hours** |
| **S2: Academic Structure** | 6 hrs | 24 hrs | 22 hrs | 18 hrs | **70 Hours** |
| **S3: Onboarding & SIS** | 8 hrs | 24 hrs | 28 hrs | 20 hrs | **80 Hours** |
| **S4: Fee Billing System** | 6 hrs | 30 hrs | 20 hrs | 19 hrs | **75 Hours** |
| **S5: Payment desk & FIFO** | 8 hrs | 32 hrs | 25 hrs | 20 hrs | **85 Hours** |
| **S6: Operations & Rollovers**| 12 hrs | 34 hrs | 30 hrs | 24 hrs | **100 Hours** |
| **Deployment / DevOps Setup** | — | — | — | — | **30 Hours** |
| **Total Project Effort** | **48 hrs** | **168 hrs** | **145 hrs** | **119 hrs** | **480 Hours** |

---

## 9. MVP Timeline

The calendar below maps the week-by-week checkpoints and critical path tracks for the solo developer.

### Weekly Progression Calendar

```
 WEEK  │ ACTIVE WORKTRACKS & MILESTONES
───────┼─────────────────────────────────────────────────────────────
 W01   │ Scaffolding .NET API & React / DB Migrations 1-5
 W02   │ Configure JWT Authentication & RLS Policies  ──► [MILESTONE 1]
 W03   │ Academic Structures Database & CRUD APIs
 W04   │ Staff Directory & Section-Subject Mappings  ──► [MILESTONE 2]
 W05   │ Admissions CRM Pipeline / Student Directory DB
 W06   │ Excel Bulk Data Import / Aadhaar Encryption  ──► [MILESTONE 3]
 W07   │ Fees Catalog Config / Concessions Registry
 W08   │ Batch Billing Generation Algorithm Engine   ──► [MILESTONE 4]
 W09   │ Cashier Payment Desk / FIFO Allocation Logic
 W10   │ Receipt History Desk & Void Reversals Logic  ──► [MILESTONE 5]
 W11   │ Attendance Touch Grids / Marks Entry Grids / Rollover Validation
 W12   │ QuestPDF Reports / Docker VPS Deployments    ──► [MILESTONE 6]
```

### Critical Path Risks & Solo Developer Mitigations

A solo developer faces single-point-of-failure risks. Below are the top risks mapped to concrete mitigations:

1. **Risk: Multi-Tenant TenantId Leakage (Security Violation)**
   * *Detail:* Manual filtering via WHERE clauses invites developer oversight, risking cross-tenant data exposure.
   * *Mitigation:* Rely strictly on SQL Server Row-Level Security (RLS) policies. Verify that the Dapper Connection Factory automatically binds `TenantId` via `sp_set_session_context` on every connection. Do not write manual tenant filters in code queries.
2. **Risk: Payment Webhook Latency & Database Lockouts**
   * *Detail:* High-traffic collection events can trigger SQL lock contention on billing transaction ledgers.
   * *Mitigation:* Execute ledger modifications (invoices, receipt allocations) inside quick-running database transactions. Run billing metrics reports on isolated read queries utilizing `SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED` (NOLOCK) to prevent lockouts.
3. **Risk: Scope Creep on Notification Alerts**
   * *Detail:* Setting up SMS and WhatsApp integrations (Gupshup) can run into carrier registration delays (DLT approval in India).
   * *Mitigation:* Encapsulate alerts behind an interface. Build a simple email sender or local log writer mock for the first deployed version. When third-party integrations get approved, switch the implementation without rewriting core classroom models.
4. **Risk: Complex CBSE Report Card Customizations**
   * *Detail:* School administrative staff often demand highly customized report cards, which can waste weeks of developer time.
   * *Mitigation:* Freeze a single CBSE-compliant Report Card template layout for the MVP launch. Inform initial pilot schools that custom layouts are scheduled for Version 1.5. Use **QuestPDF** for rapid, code-based layout styling rather than configuring complex HTML-to-PDF templates.
5. **Risk: Excel Import Failures**
   * *Detail:* Schools often provide messy student lists, causing bulk migrations to fail because of syntax errors.
   * *Mitigation:* Provide schools with a strict, pre-configured Excel template download. Validate all records in-memory at the API layer first, showing a clear error list (e.g., *"Line 12: Invalid Date of Birth format"*), and block insertion until the file is corrected. This keeps the import logic simple.
