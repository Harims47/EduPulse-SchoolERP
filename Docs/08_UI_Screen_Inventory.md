# UI Screen Inventory: EduPulse AI
**Project Name:** EduPulse AI  
**Role Perspective:** Senior Product Designer & UX Architect  
**UI Framework:** React with Bootstrap 5 (Mobile-Responsive Web layout)  
**Status:** Design Freeze - Ready for Wireframes  

---

## Global Navigation Framework
The application uses a responsive master page layout (`AdminLayout`, `TeacherLayout`, `ParentLayout`) with:
* **Sidebar Navigation:** Collapsible left-side menu showing mapped modules based on the active role.
* **Top Header Bar:** School Logo, active Academic Year dropdown indicator, Tenant School Name, Notification quick-alerts icon, and User Profile logout controls.
* **Content Canvas:** Main container displaying grid lists, configuration forms, and operational card dashboards.

---

## 1. Authentication Module

### 1.1. Screen Name: `LoginScreen`
* **Purpose:** Allows users to input credentials and authenticate to retrieve their role-specific dashboard.
* **User Roles:** All Roles (Anonymous access).
* **Navigation Flow:** Root URL (`/login`) ──► On success ──► Redirects to `/dashboard`.
* **Key Components:**
  * School Branding Header (Logo placeholder).
  * Email text input (with email validations).
  * Password text input (with toggle-visibility eyeball icon).
  * "Sign In" primary action button.
  * Forgot Password help links.
* **Grid Columns:** N/A.
* **Filters:** N/A.
* **Actions:** `SubmitLogin`.
* **API Dependencies:** `POST /api/auth/login`

---

## 2. Dashboard Module

### 2.1. Screen Name: `AdminDashboard`
* **Purpose:** High-level summary of school financial health and key operational indicators for School Owners and Admins.
* **User Roles:** `SchoolAdmin`
* **Navigation Flow:** Sidebar: Dashboard (`/dashboard`).
* **Key Components:**
  * KPI Metric Cards:
    * Total Fee Collection progress bar (Total Billed vs. Paid vs. Overdue).
    * Today's Attendance Rate percentage.
    * Active Student count.
  * Charts Panel (using simple Bootstrap chart libraries):
    * Monthly Fee Collections line chart.
    * Today's Absentee Count list by class.
  * Notification log widget (Recent alerts).
* **Grid Columns:** N/A.
* **Filters:** Active Academic Year context (Global).
* **Actions:** `QuickLinkToFeeReceipts`, `TriggerNotificationBroadcast`.
* **API Dependencies:** `GET /api/dashboard/admin`

### 2.2. Screen Name: `TeacherDashboard`
* **Purpose:** Focused cockpit showing mapped classrooms, current term schedules, and pending grading sheets.
* **User Roles:** `Teacher`
* **Navigation Flow:** Default homepage on login for Teachers.
* **Key Components:**
  * "My Mapped Sections" card grid.
  * Quick-Action buttons: "Mark Attendance Today", "Post Homework".
  * Alert Lists: "Pending Marks Submissions" for current exams.
* **Grid Columns:** N/A.
* **Filters:** Current Date context.
* **Actions:** `OpenAttendanceSheet`, `OpenGradebook`.
* **API Dependencies:** `GET /api/dashboard/teacher`

### 2.3. Screen Name: `ParentDashboard`
* **Purpose:** Mobile-friendly home screen for parents containing child summaries (designed to scale cleanly on mobile browsers).
* **User Roles:** `Parent`
* **Navigation Flow:** Default homepage on login for Parents.
* **Key Components:**
  * Sibling Selector dropdown (if parent has multiple children).
  * Profile Overview widget (Child name, roll number, section, homeroom teacher name).
  * Balance Due KPI Card with direct "Pay Outstanding Fees Online" button.
  * Quick-View tabs: "Today's Homework", "Attendance Percentage", "Notice Board".
* **Grid Columns:** N/A.
* **Filters:** Child Selector.
* **Actions:** `PayOnlineLink`, `DownloadReportCard`, `DownloadHomeworkAttachment`.
* **API Dependencies:** `GET /api/dashboard/parent`

---

## 3. Students Module

### 3.1. Screen Name: `StudentSearchDirectory`
* **Purpose:** Admin directory to query student rosters and update active profiles.
* **User Roles:** `SchoolAdmin`, `Accountant`, `Teacher`
* **Navigation Flow:** Sidebar: Students ──► Directory (`/students`).
* **Key Components:**
  * Grid list showing student details.
  * Upload dropzone to import rosters via Excel.
  * "Create Student" button linking to form layouts.
* **Grid Columns:** `AdmissionNo`, `FullName`, `ClassSectionName`, `GuardianPhone`, `Status`, `Actions`.
* **Filters:** Class & Section select, Status dropdown (`Active`, `Applied`, etc.), Text Search (Name/Admission No).
* **Actions:** `EditProfile`, `ManageDocuments`, `DownloadExcelRoster`, `InitiateTCRequest`.
* **API Dependencies:** `GET /api/students/search`, `POST /api/students/import`

### 3.2. Screen Name: `StudentProfileForm`
* **Purpose:** Tabbed data editor to create or modify detailed student bio-data.
* **User Roles:** `SchoolAdmin`
* **Navigation Flow:** Student Directory ──► Click "Create" or "Edit" (`/students/form` or `/students/edit/{id}`).
* **Key Components:**
  * Tab 1: Bio-Data Form (Admission Number, Name, DOB, Gender, Blood Group, Aadhaar Number, Category).
  * Tab 2: Guardian Mapping Form (Guardian name, phone, email, sibling mappings).
  * Tab 3: Student Profile Photo upload tool.
* **Grid Columns:** N/A.
* **Filters:** N/A.
* **Actions:** `SaveProfile`, `CancelChanges`.
* **API Dependencies:** `POST /api/students`, `PUT /api/students/{id}`, `GET /api/students/{id}`

### 3.3. Screen Name: `StudentDocumentsVault`
* **Purpose:** File manager to archive and download scanned registration paperwork.
* **User Roles:** `SchoolAdmin`
* **Navigation Flow:** Student Directory ──► Click "Documents" (`/students/{id}/documents`).
* **Key Components:**
  * Document Drag & Drop File Upload dropzone.
  * File Lists card grid.
* **Grid Columns:** `DocumentType`, `FileName`, `UploadedOn`, `UploadedBy`, `Actions`.
* **Filters:** N/A.
* **Actions:** `UploadDocument`, `DownloadFile`, `DeleteDocument`.
* **API Dependencies:** `GET /api/students/{id}/documents`, `POST /api/students/{id}/documents`

### 3.4. Screen Name: `AdmissionsReviewBoard`
* **Purpose:** Admissions pipeline to review online student registrations, perform document audits, and approve enrollment.
* **User Roles:** `SchoolAdmin`
* **Navigation Flow:** Sidebar: Students ──► Admissions (`/students/admissions`).
* **Key Components:**
  * Registration pipeline grid.
  * Approve Application popup modal (asks for Admission No, ClassSection, Roll No allocation).
  * Reject Application popup modal (requires text rejection comments).
* **Grid Columns:** `ApplicationNo`, `StudentName`, `ParentPhone`, `TargetClass`, `SubmissionDate`, `Actions`.
* **Filters:** Status dropdown (`Applied`, `Archived`), Target Class filter.
* **Actions:** `ApproveApplication`, `RejectApplication`.
* **API Dependencies:** `GET /api/admissions`, `POST /api/admissions/{id}/approve`, `POST /api/admissions/{id}/reject`

---

## 4. Guardians Module

### 4.1. Screen Name: `GuardianDirectory`
* **Purpose:** Directory of parents and guardians linked to student profiles.
* **User Roles:** `SchoolAdmin`, `Accountant`
* **Navigation Flow:** Sidebar: Guardians (`/guardians`).
* **Key Components:**
  * Text search table.
  * Detail Modal listing linked siblings.
* **Grid Columns:** `GuardianName`, `Phone`, `Email`, `LinkedStudentsCount`, `Actions`.
* **Filters:** SearchTerm text input.
* **Actions:** `EditGuardianContact`, `ViewLinkedSiblings`.
* **API Dependencies:** `GET /api/guardians`, `PUT /api/guardians/{id}`, `GET /api/guardians/{id}`

---

## 5. Staff Module

### 5.1. Screen Name: `StaffDirectory`
* **Purpose:** Admin tool to manage employee profiles and designations.
* **User Roles:** `SchoolAdmin`
* **Navigation Flow:** Sidebar: Staff (`/staff`).
* **Key Components:**
  * Staff list showing Designation status.
  * "Register Employee" button.
* **Grid Columns:** `EmployeeCode`, `FullName`, `Designation`, `Phone`, `IsActive`, `Actions`.
* **Filters:** Designation filter, Search Term input, Status check.
* **Actions:** `EditStaffProfile`, `ToggleActiveStatus`.
* **API Dependencies:** `GET /api/staff`, `POST /api/staff`, `PUT /api/staff/{id}`, `GET /api/staff/{id}`

---

## 6. Academic Years Module

### 6.1. Screen Name: `AcademicYearManager`
* **Purpose:** Configure calendar years and control active term transitions.
* **User Roles:** `SchoolAdmin`
* **Navigation Flow:** Sidebar: Settings ──► Academic Years (`/settings/years`).
* **Key Components:**
  * Create Academic Year form layout (Dates, Calendar title inputs).
  * Grid list showing years.
* **Grid Columns:** `AcademicYearName`, `StartDate`, `EndDate`, `StatusLabel`, `Actions`.
* **Filters:** N/A.
* **Actions:** `CreateYear`, `ToggleActiveYear`, `InitiateRolloverChecks`.
* **API Dependencies:** `GET /api/academic-years`, `POST /api/academic-years`, `PUT /api/academic-years/{id}/activate`, `POST /api/academic-years/rollover/validate`, `POST /api/academic-years/rollover/execute`

---

## 7. Classes & Sections Module

### 7.1. Screen Name: `ClassSectionConfigurator`
* **Purpose:** Configure classes, assign sections, map subjects, and set capacity limits.
* **User Roles:** `SchoolAdmin`
* **Navigation Flow:** Sidebar: Settings ──► Classes & Sections (`/settings/classes`).
* **Key Components:**
  * Class grid layout. Expanding a class standard card shows sections, capacities, homeroom teachers, and mapped subjects.
  * Modal: "Create Section Map" (assigns section names, capacities, and Class Teachers).
  * Modal: "Map Subject to Section" (links subject names to class section rosters).
* **Grid Columns:** N/A.
* **Filters:** Global Academic Year toggle.
* **Actions:** `AddClass`, `DeleteClass`, `AddSection`, `MapSubject`, `AssignClassTeacher`.
* **API Dependencies:** `GET /api/classes`, `POST /api/classes`, `GET /api/class-sections`, `POST /api/class-sections`, `GET /api/subjects`, `POST /api/class-section-subjects`

---

## 8. Attendance Module

### 8.1. Screen Name: `DailyAttendanceSheet`
* **Purpose:** Mobile-responsive checklist grid for teachers to mark daily attendance.
* **User Roles:** `Teacher`, `SchoolAdmin`
* **Navigation Flow:** Sidebar: Attendance ──► Daily Log (`/attendance`).
* **Key Components:**
  * Class & Section selectors.
  * List grid displaying student roll numbers and names.
  * Responsive check/uncheck status buttons (`Present`/`Absent`/`Leave`/`Late`).
  * "Finalize Daily Log" submission button.
* **Grid Columns:** `RollNo`, `StudentName`, `StatusToggles`.
* **Filters:** ClassSection selection, Date Picker.
* **Actions:** `SubmitAttendance`, `RequestAdminUnlock`.
* **API Dependencies:** `GET /api/attendance/roster`, `POST /api/attendance/submit`, `POST /api/attendance/unlock`

---

## 9. Fees Module

### 9.1. Screen Name: `FeeStructureConfigurator`
* **Purpose:** Setup fee categories and define line item templates mapped to classes.
* **User Roles:** `SchoolAdmin`, `Accountant`
* **Navigation Flow:** Sidebar: Fees ──► Structure (`/fees/structure`).
* **Key Components:**
  * Fee Groups registration panel.
  * Fee Line Items grid builder.
* **Grid Columns:** `FeeGroupName`, `LineItemName`, `ClassTarget`, `BilledAmount`, `Actions`.
* **Filters:** Global Academic Year.
* **Actions:** `AddFeeGroup`, `AddLineItem`, `DeleteLineItem`.
* **API Dependencies:** `GET /api/fees/groups`, `POST /api/fees/groups`, `GET /api/fees/line-items`, `POST /api/fees/line-items`, `DELETE /api/fees/line-items/{id}`

---

## 10. Invoices Module

### 10.1. Screen Name: `InvoiceDirectory`
* **Purpose:** Audit generated student invoices, outstanding balances, and track payment schedules.
* **User Roles:** `SchoolAdmin`, `Accountant`
* **Navigation Flow:** Sidebar: Fees ──► Invoices (`/fees/invoices`).
* **Key Components:**
  * Student Invoice list table.
  * "Generate Invoices Batch" button.
  * Invoice detail modal (shows line details and concession deductions).
* **Grid Columns:** `InvoiceNo`, `StudentName`, `ClassName`, `TotalAmount`, `BalanceAmount`, `InvoiceStatus`, `DueDate`, `Actions`.
* **Filters:** ClassSection selection, Invoice Status check, Overdue filter, Search Term input.
* **Actions:** `ViewInvoiceDetails`, `TriggerManualBatchInvoice`, `PrintInvoiceSlip`.
* **API Dependencies:** `GET /api/invoices`, `GET /api/invoices/{id}`, `POST /api/invoices/generate-batch`

---

## 11. Receipts Module

### 11.1. Screen Name: `CashierPaymentDesk`
* **Purpose:** front office cashier workstation to collect cash/cheque payments, check outstanding balances, and print receipt slips.
* **User Roles:** `Accountant`, `SchoolAdmin`
* **Navigation Flow:** Sidebar: Fees ──► Payment Desk (`/fees/cashier`).
* **Key Components:**
  * Student lookup search input.
  * Student Profile Snapshot (Name, Class, Admission No).
  * Invoice selection checklist showing outstanding balances.
  * Payment configuration panel (amount input, payment type selection [Cash, Cheque, UPI, Card], transaction reference).
  * Print Receipt template wrapper (HTML/CSS layout optimized for 80mm thermal printers).
* **Grid Columns:** N/A.
* **Filters:** Student Search.
* **Actions:** `CollectPayment`, `CancelReceiptTransaction`.
* **API Dependencies:** `POST /api/receipts/collect`, `POST /api/receipts/{id}/cancel`, `GET /api/invoices`

---

## 12. Concessions Module

### 12.1. Screen Name: `ConcessionsReviewBoard`
* **Purpose:** Apply fee concessions, scholarships, and sibling discounts to student billing accounts.
* **User Roles:** `SchoolAdmin`
* **Navigation Flow:** Sidebar: Fees ──► Concessions (`/fees/concessions`).
* **Key Components:**
  * Student concession assignment form modal.
  * Active concessions table list.
* **Grid Columns:** `StudentName`, `BilledFeeLineItem`, `ConcessionType`, `Value`, `ApprovalCode`, `Actions`.
* **Filters:** Target Class, Concession Type.
* **Actions:** `ApproveConcession`, `RevokeConcession`.
* **API Dependencies:** `POST /api/concessions`, `GET /api/concessions/student/{id}`

---

## 13. Exams Module

### 13.1. Screen Name: `GradeBookEntryGrid`
* **Purpose:** Spreadsheet-like edit grid for teachers to input test scores.
* **User Roles:** `Teacher`, `SchoolAdmin`
* **Navigation Flow:** Sidebar: Exams ──► Grade Book (`/exams/grades`).
* **Key Components:**
  * Roster grid with arrow-key keyboard navigation.
  * "Submit to Coordinator" locked status finalization button.
* **Grid Columns:** `RollNo`, `StudentName`, `MarksObtainedInput`, `IsAbsentCheck`, `TeacherRemarksInput`.
* **Filters:** ClassSection selection, Subject dropdown, Exam standard selector.
* **Actions:** `SaveMarksDraft`, `SubmitGradesToCoordinator`.
* **API Dependencies:** `GET /api/exams/{id}/marks`, `POST /api/exams/{id}/marks/submit`

### 13.2. Screen Name: `ExamPublicationPanel`
* **Purpose:** Principal control center to configure exam milestones and publish report cards.
* **User Roles:** `SchoolAdmin`
* **Navigation Flow:** Sidebar: Exams ──► Manage Milestones (`/exams/milestones`).
* **Key Components:**
  * Exam creation card layouts.
  * Class publish progression indicator (shows status per class: "Maths: Submitted, English: Pending").
  * "Publish All Class Report Cards" button.
* **Grid Columns:** `ExamName`, `ClassTarget`, `MaxMarks`, `DateScheduled`, `PublishStatus`, `Actions`.
* **Filters:** Active Year.
* **Actions:** `AddExam`, `PublishReportCards`.
* **API Dependencies:** `POST /api/exams`, `GET /api/exams`, `PUT /api/exams/{id}/publish`

---

## 14. Homework Module

### 14.1. Screen Name: `HomeworkPlanner`
* **Purpose:** Form to assign daily homework tasks with document attachments.
* **User Roles:** `Teacher`
* **Navigation Flow:** Sidebar: Homework ──► Manage (`/homework/manage`).
* **Key Components:**
  * Homework composer form.
  * Attachment dropzone (Max 5MB file upload).
* **Grid Columns:** N/A.
* **Filters:** ClassSection, Date.
* **Actions:** `PublishHomework`.
* **API Dependencies:** `POST /api/homework`

### 14.2. Screen Name: `ParentDiaryFeedScreen`
* **Purpose:** Chronological homework board optimized for mobile parent views.
* **User Roles:** `Parent`, `Student`
* **Navigation Flow:** Parent Dashboard ──► Homework Diary (`/diary`).
* **Key Components:**
  * Timeline feed cards.
  * Clickable PDF/image attachment download links.
* **Grid Columns:** N/A.
* **Filters:** Sibling Selector, Date query.
* **Actions:** `DownloadFileAttachment`.
* **API Dependencies:** `GET /api/homework/feed`

---

## 15. Notifications Module

### 15.1. Screen Name: `NotificationRegistry`
* **Purpose:** Review notification broadcasts and carrier delivery statuses.
* **User Roles:** `SchoolAdmin`
* **Navigation Flow:** Sidebar: Communications ──► Delivery Logs (`/comms/logs`).
* **Key Components:**
  * Notification dispatch history table.
* **Grid Columns:** `RecipientPhone`, `MessagePayload`, `StatusTag`, `RetryAttempts`, `CarrierError`, `DispatchTimestamp`.
* **Filters:** Status dropdown (`Pending`, `Sent`, `Delivered`, `Failed`), Date range.
* **Actions:** `TriggerRetryJob`.
* **API Dependencies:** `GET /api/notifications/logs`

---

## 16. Reports Module

### 16.1. Screen Name: `SchoolReportsDesk`
* **Purpose:** Operational reports board for administrators.
* **User Roles:** `SchoolAdmin`, `Accountant`
* **Navigation Flow:** Sidebar: Reports (`/reports`).
* **Key Components:**
  * Report Cards grid.
  * PDF print-preview component with export capabilities.
* **Reports Mapped:**
  * **Fee Defaulter Report:** Lists outstanding invoices by student and class standard.
  * **Daily Absentee Register:** List of students absent today, including parent contact phones.
  * **Monthly Class Attendance Register:** Grid showing attendance percentages.
* **Filters:** Mapped per report (Date, ClassSection, Overdue Days thresholds).
* **Actions:** `ExportToExcel`, `PrintPDF`, `SendNotificationAlertsToDefaulters`.
* **API Dependencies:** `GET /api/reports/fee-defaulters`, `GET /api/reports/daily-absentees`, `GET /api/attendance/student/{id}`

---

## 17. Settings Module

### 17.1. Screen Name: `TenantSettingsDashboard`
* **Purpose:** Setup school profiles, branding rules, format prefixes, and administrative cutoff times.
* **User Roles:** `SchoolAdmin`
* **Navigation Flow:** Sidebar: Settings (`/settings`).
* **Key Components:**
  * School details form (School Name, Address, Code, Board registration details).
  * System rules config panel (Attendance Cutoff Time, Date Format select, Timezone).
  * Prefix format inputs (Fee receipt prefix, TC prefix).
  * School Logo file uploader widget.
* **Grid Columns:** N/A.
* **Filters:** N/A.
* **Actions:** `SaveConfigurationChanges`.
* **API Dependencies:** `GET /api/settings`, `PUT /api/settings`
