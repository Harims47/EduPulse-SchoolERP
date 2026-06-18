# Software Requirements Specification (SRS): AI-First School Management SaaS
**Project Name:** EduPulse AI (MVP Scope)  
**Role Perspective:** Senior Functional Analyst  
**Target Architecture:** React (Frontend SPA) | .NET 8 Web API | SQL Server (Single DB, Multi-Tenant)  
**Status:** Implementation-Ready  

---

## 1. Multi-Tenant Administration

### 1.1. User Stories
* **US-1.1:** As a Super Admin, I want to onboard a new school tenant by inputting their profile details so that they can access their isolated dashboard.
* **US-1.2:** As a School Admin, I want to configure the academic year, classes, and sections so that the system mirrors my physical school structure.
* **US-1.3:** As a School Admin, I want to map subjects to classes so that teachers can record attendance and grades.

### 1.2. Functional Requirements
* **FR-1.1 (Tenant Onboarding Form):** The Super Admin portal must provide fields to capture: School Name, Brand Logo, Board Affiliation Type (CBSE / State Board / ICSE), Tax ID/GSTIN, Contact Phone, and Primary Admin Email.
* **FR-1.2 (Academic Year Matrix):** The system must allow creating school years with Start Date (dd/mm/yyyy) and End Date (dd/mm/yyyy). The UI must provide a single-toggle control to set the "Active Year".
* **FR-1.3 (Class & Section Configurator):** Provide an interface to create classes (LKG to 12th) and assign sections (A, B, C). Sections must contain a numerical field defining the Student Capacity Limit.
* **FR-1.4 (Subject Mapping Roster):** Provide a drag-and-drop or checklist interface to bind subjects to a specific class and section, configuring each subject's evaluation mode (Theory-only, Practical-only, or Mixed).

### 1.3. Validation Rules
* **VR-1.1 (School Name):** Alphanumeric and standard spaces. Range: 5–150 characters. Required.
* **VR-1.2 (Logo Asset):** Image file format restricted to `.png` or `.jpeg`. Max size: 2MB. Aspect ratio: 1:1 recommended.
* **VR-1.3 (Academic Year Duration):** The end date must be exactly after the start date. Year format must follow `YYYY-YYYY` (e.g., `2026-2027`).
* **VR-1.4 (Capacity Limit):** Section student capacity must be an integer between 1 and 100.

### 1.4. Acceptance Criteria
* **AC-1.1:** Given a valid school profile input, when the Super Admin clicks "Register Tenant", then the system must create the tenant record, generate a unique `TenantId` (UUID), and initialize the schema boundaries for that tenant.
* **AC-1.2:** Given an active academic year toggle, when a new year is marked active, then the system must automatically deactivate the previous year and lock editing of past transactions.

### 1.5. Error Scenarios
* **ERR_DUPLICATE_SECTION:** If an admin attempts to create "Class 5A" when it already exists under the active tenant, the system must highlight the fields red and output: *"Section A already exists for Class 5."*
* **ERR_INVALID_FILE_TYPE:** If an admin uploads a `.pdf` to the Logo field, the UI must intercept the file upload, clear the input, and display a toast alert: *"Invalid file format. Upload only PNG or JPEG."*

### 1.6. Business Rules
* **BR-1.1 (Tenant Isolation):** Every database query must implicitly include the context tenant key: `WHERE tenant_id = @ActiveTenant`. Cross-tenant querying is strictly prevented.
* **BR-1.2 (Active Year Lock):** Only one year can be "Active". Financial records and report card marks entries can only be written to tables linked to the current "Active" academic year.

### 1.7. Dependencies
* None. This is the root configuration module.

### 1.8. UI Components Required
* **`TenantRegisterForm`:** Multi-field registration dashboard (text fields, file drag-drop zone).
* **`AcademicCalendarGrid`:** List of years showing status labels (`Active`, `Archived`) with toggle switches.
* **`ClassBuilderPanel`:** Accordion card list. Expanding a class shows sections, mapped subjects, and capacity inputs.

---

## 2. Role-Based Access Control (RBAC)

### 2.1. User Stories
* **US-2.1:** As a registered user (Admin, Teacher, Parent), I want to enter my username and password on a login screen so that I can safely enter the platform.
* **US-2.2:** As an Admin, I want to change user permissions or deactivate accounts so that internal staff access is controlled.

### 2.2. Functional Requirements
* **FR-2.1 (Login Panel):** A single login page capturing Email and Password. 
* **FR-2.2 (JWT Issuance Service):** The backend must authenticate credentials and issue a JSON Web Token (JWT) containing Claims: `UserId`, `TenantId`, `UserRole`, and `PermissionsList`.
* **FR-2.3 (Account Management Dashboard):** An admin view listing all staff profiles with dynamic switches to "Activate", "Deactivate", or edit "Assign Role" values.

### 2.3. Validation Rules
* **VR-2.1 (Email Format):** Must pass standard email regex format verification. Required.
* **VR-2.2 (Password Complexity):** Must contain a minimum of 8 characters, including 1 uppercase letter, 1 lowercase letter, 1 number, and 1 special symbol (e.g. `@`, `#`, `!`).

### 2.4. Acceptance Criteria
* **AC-2.1:** Given a deactivated user, when they attempt to log in with valid credentials, then the API must deny authorization and return an explicit suspension alert.
* **AC-2.2:** Given an active session, when the token time-to-live (TTL) of 12 hours expires, then the React client must purge the token and redirect to the Login Page.

### 2.5. Error Scenarios
* **ERR_AUTHENTICATION_FAILED:** Incorrect password inputs must trigger a generic error label: *"Invalid username or password"* (preventing username verification attacks).
* **ERR_ACCESS_DENIED (HTTP 403):** If a Teacher attempts to access the Fee Configuration API route directly, the server must return an HTTP 403 Forbidden payload.

### 2.6. Business Rules
* **BR-2.1 (Immutability of Super Admin):** The root Super Admin account cannot be deactivated or deleted.
* **BR-2.2 (Token Expiry):** All client-side tokens must expire and delete local state after exactly 12 hours of inactivity.

### 2.7. Dependencies
* Module 1 (Multi-Tenant Administration) to extract `TenantId` values during authorization matches.

### 2.8. UI Components Required
* **`LoginForm`:** Form layout featuring email inputs, password inputs (with hide/reveal eyeball icon), and a "Submit" action button.
* **`UserListGrid`:** Paginated search table showing Name, Email, assigned Role, and dynamic active/inactive status toggle.

---

## 3. Student Information System (SIS)

### 3.1. User Stories
* **US-3.1:** As an Admin, I want to enter a new student's record manually so that their basic biodata is recorded.
* **US-3.2:** As an Admin, I want to import a list of students from an Excel sheet so that I don't have to enter them manually one by one.
* **US-3.3:** As a Parent, I want to log in and see my child's current enrollment status, class assignment, and profile files.

### 3.2. Functional Requirements
* **FR-3.1 (Manual Student Registry Form):** Form inputs capturing: Name, Date of Birth, Roll Number, Class/Section, Guardian Name, Primary Phone, Email, Address, and Document Upload attachments.
* **FR-3.2 (Bulk Import Dropzone):** React page with a file dropzone. The page checks, parses, and inserts student rosters from a uploaded `.xlsx` template.
* **FR-3.3 (Data Parser & Validator):** An API service that runs validations on uploaded spreadsheets, parsing fields row-by-row before committing records.

### 3.3. Validation Rules
* **VR-3.1 (Date of Birth):** The student must be at least 3 years old (for LKG) and not older than 20 years old based on the current academic year start.
* **VR-3.2 (Phone Format):** Guardians' phone contacts must pass a 10-digit Indian mobile format match (regex: `^[6-9]\d{9}$`).
* **VR-3.3 (File Size Check):** Scanned PDF profiles (Aadhaar, Transfer Certificate) must be smaller than 5MB per upload.

### 3.4. Acceptance Criteria
* **AC-3.1:** Given a bulk Excel file with 50 valid student rows, when the user uploads the sheet, then the database transaction must create all 50 student records and map them to their class sections without issues.
* **AC-3.2:** Given an Excel upload containing 49 clean rows and 1 row with a missing parent phone number, when uploaded, then the system must abort all database insertions, rollback, and highlight: *"Row 32: Parent Phone is required."*

### 3.5. Error Scenarios
* **ERR_DUPLICATE_ADMISSION_NO:** Attempting to assign an Admission Number that is already allocated inside the active tenant must abort the record and display: *"Admission Number [XYZ] is already assigned."*
* **ERR_EXCEL_HEADER_MISMATCH:** If the column headers in the uploaded Excel do not match the standard template exactly, the system must abort parsing and display: *"Template structure mismatch. Download and use the standard Excel template."*

### 3.6. Business Rules
* **BR-3.1 (Admission Number Key):** Every student has a unique Admission Number across the active tenant domain.
* **BR-3.2 (Roll Number Bounds):** Roll numbers must be positive integers starting from 1. No two active students in the same Section can share the same Roll Number.

### 3.7. Dependencies
* Module 1 (Academic Year, Classes, and Sections must exist).
* Module 2 (User profiles are auto-generated for primary parents during student creation).

### 3.8. UI Components Required
* **`StudentProfileForm`:** Multi-step tabbed form (Step 1: Bio-data, Step 2: Parent mapping, Step 3: Scanned document upload).
* **`ExcelImportDropzone`:** File selection box with a "Download Standard Excel Template" button and a real-time parsing errors log widget.
* **`StudentRosterGrid`:** Filtering table showing rows by Class, Section, and Name with a search bar.

---

## 4. Attendance Management

### 4.1. User Stories
* **US-4.1:** As a Class Teacher, I want to open my class attendance sheet on my phone browser during morning registration so that I can quickly check-off absent students.
* **US-4.2:** As a Parent, I want to receive an instant WhatsApp alert if my child is marked absent so that I am aware of their safety.

### 4.2. Functional Requirements
* **FR-4.1 (Web-Mobile Roster Matrix):** A mobile-optimized React screen showing a list of students sorted by Roll Number with checkboxes. By default, all students are selected (Present). Unchecking a student sets status to "Absent".
* **FR-4.2 (Notification Worker Hook):** On final submit of attendance, the API queues a task to call external WhatsApp/SMS service endpoints for all absent students.
* **FR-4.3 (Attendance Lock State):** Once attendance is submitted for the day, the input roster switches to "Locked" status, displaying a read-only log.

### 4.3. Validation Rules
* **VR-4.1 (No Future Logging):** Attendance can only be logged for the current system date or past dates. Logging for future calendar dates must block.
* **VR-4.2 (Daily Timeline Lockout):** Roster writing is locked for Teachers at 09:30 AM. After 09:30 AM, adjustments require a School Admin account overrides.

### 4.4. Acceptance Criteria
* **AC-4.1:** Given a finalized attendance register, when the teacher submits, then the system must immediately mark the class attendance status as "Locked" and disallow further submissions by the teacher on that date.
* **AC-4.2:** Given the lock state, when a School Admin applies an unlock override, then the teacher can correct the attendance logs on that current date.

### 4.5. Error Scenarios
* **ERR_ATTENDANCE_LOCKED:** A teacher attempting to modify attendance after 09:30 AM must receive a warning popup: *"Attendance submission window closed. Contact the administrator for updates."*
* **ERR_SMS_GATEWAY_TIMEOUT:** If the SMS gateway times out, the backend system must cache the alert in a queue retry state for 3 attempts before setting the alert status to "Failed" in the logs.

### 4.6. Business Rules
* **BR-4.1 (Single Daily Log):** Only one attendance session can exist per Class-Section per day.
* **BR-4.2 (Primary Contact Alerts):** Attendance notifications are routed strictly to the "Primary Contact Phone" linked to the student profile.

### 4.7. Dependencies
* Module 3 (SIS rosters and parent numbers).

### 4.8. UI Components Required
* **`AttendanceRosterView`:** Simple, responsive checkbox list with single-click "Mark All Present" button.
* **`AttendanceLockoutBanner`:** Visual state bar showing Status (`Draft`, `Locked`, `Admin Overridden`) and a countdown timer to the 09:30 AM cutoff.

---

## 5. Fee Management & Collections

### 5.1. User Stories
* **US-5.1:** As an Accountant, I want to configure custom term fees mapped to classes so that student accounts are billed accurately.
* **US-5.2:** As an Accountant, I want to collect cash payments at the front desk and print a paper voucher receipt so that physical audits reconcile.
* **US-5.3:** As a Parent, I want to view my child's billed fees statement and pay via an integrated UPI checkout screen so that I do not need to visit the school counter.

### 5.2. Functional Requirements
* **FR-5.1 (Fee Structure Configuration):** Interface to define fee names (e.g. Admission, Tuition, Transport), monthly rates, and apply them across classes.
* **FR-5.2 (Cashier Payments Dashboard):** Accountant terminal page to search students, view outstanding balances, input payment amount, select type (Cash, Cheque, UPI), and print a matching invoice receipt.
* **FR-5.3 (Razorpay Checkout Hook):** A webhook handler in .NET 8 that captures successful Razorpay transactions, creates a payment voucher record in SQL Server, and updates the student ledger balance.

### 5.3. Validation Rules
* **VR-5.1 (Payment Decimal Checks):** Input payment amounts must be positive decimal numbers with a maximum of two decimal places.
* **VR-5.2 (Cheque/Transaction Reference):** If payment type is Cheque or Online Gateway, a Transaction/Cheque Reference Number is required. Range: 4–50 characters.

### 5.4. Acceptance Criteria
* **AC-5.1:** Given a payment input of ₹2,500 cash, when processed, then the system must create a payment receipt, update the outstanding balance in the ledger, and open the browser's native print modal displaying a clean thermal print template.
* **AC-5.2:** Given an online payment process, when a Razorpay callback is confirmed, the ledger balance must update in real-time, matching transaction tokens.

### 5.5. Error Scenarios
* **ERR_OVERPAYMENT_BLOCKED:** If a parent/cashier inputs an amount exceeding the student's overall outstanding balance, the system must block the transaction and output: *"Payment exceeds outstanding balance of [BalanceAmount]."*
* **ERR_TRANSACTION_RECONCILIATION_FAIL:** If Razorpay triggers a success webhook but the transaction reference is missing in the payload, the transaction must write to an administrative queue for manual approval.

### 5.6. Business Rules
* **BR-5.1 (Immutable Audit Trails):** Deletions or updates to generated invoices/receipts are blocked. Reversals must be processed via Credit Notes.
* **BR-5.2 (First-In, First-Out Collection):** Any payments made must automatically offset the oldest due fee cycles first.

### 5.7. Dependencies
* Module 3 (Student Registry).
* External Gateway (Razorpay Account Integration).

### 5.8. UI Components Required
* **`FeeStructureBuilder`:** Class-mapping inputs for custom billing models.
* **`CashierPaymentTerminal`:** Search bar, profile snapshot, dynamic billing outstanding ledger, input field, and a "Print Receipt" trigger button.
* **`ParentLedgerDashboard`:** Responsive ledger history table with a green "Pay Balance Online" button.
* **`ReceiptPrintPreview`:** Highly clean HTML receipt layout optimized for standard 80mm thermal receipt printers.

---

## 6. Examination & Report Cards

### 6.1. User Stories
* **US-6.1:** As a Teacher, I want to enter student exam scores into a simple online grid interface so that I don't have to manage manual sheets.
* **US-6.2:** As an Admin, I want to set the grading criteria according to CBSE rules and publish PDF report cards so that parents can download them securely.

### 6.2. Functional Requirements
* **FR-6.1 (Marks Entry Matrix Grid):** Spreadsheet-like interface in React allowing teachers to input numeric grades per student for a selected exam and subject.
* **FR-6.2 (CBSE Grade Converter Service):** A backend converter class matching standard CBSE 9-Point rules:
  * Top 1/8th of passed students: A1
  * Next 1/8th: A2
  * Next 1/8th: B1 (down to E - Fail).
* **FR-6.3 (QuestPDF Generation Service):** An server-side engine that formats student biodata, attendance metrics, exam grades, and teacher notes into a compiled PDF.

### 6.3. Validation Rules
* **VR-6.1 (Numerical Bounds):** Entered marks must be positive numbers between 0 and the configured Maximum Marks limit for the exam.
* **VR-6.2 (Teacher Remarks):** Custom comments are restricted to 250 characters.

### 6.4. Acceptance Criteria
* **AC-6.1:** Given a finalized class grade input, when the teacher clicks "Submit to Coordinator", then the input fields lock, preventing further teacher-level updates.
* **AC-6.2:** Given a published exam, when a parent logs in, then they can see the "Download Report Card" PDF link, which was invisible in draft status.

### 6.5. Error Scenarios
* **ERR_OUT_OF_RANGE_MARK:** If a teacher enters "85" on a test with a max score configuration of "80", the cell must highlight red, block form submission, and show a validation label: *"Score cannot exceed Max Marks (80)."*
* **ERR_PDF_COMPILATION_FAIL:** If missing student details cause compilation errors, the PDF rendering engine must log the error parameters and output a placeholder error display to the administrator instead of freezing the UI.

### 6.6. Business Rules
* **BR-6.1 (Grade Locking):** Report card grades freeze upon administrative publication and cannot be unlocked without a logged override.
* **BR-6.2 (Parent Visibility Restriction):** Grades are restricted from parent views until administrative publication is completed.

### 6.7. Dependencies
* Module 3 (SIS data).
* Module 4 (Attendance history for compilation).

### 6.8. UI Components Required
* **`GradeBookGrid`:** Inline edit table with tabular inputs (arrow-key navigation enabled).
* **`ReportCardPublishPanel`:** Class summary page showing progress indicators (e.g. "Maths: Submitted, Science: Pending") and a main "Publish All" action button.
* **`ParentPDFViewer`:** Clean modal with print controls.

---

## 7. Homework Management

### 7.1. User Stories
* **US-7.1:** As a Teacher, I want to post homework instructions and upload reference worksheets so that my students are clear on tasks.
* **US-7.2:** As a Parent, I want to view a daily homework diary feed on my phone web browser so that I can track my child's assignments.

### 7.2. Functional Requirements
* **FR-7.1 (Assignment Publisher):** Provide a form capturing: Title, Subject mapping, Submission Due Date, Instruction details, and File Attachment upload controls.
* **FR-7.2 (Chronological Homework Diary):** Mobile parent dashboard showing card layouts of homework items categorized by date and subject.
* **FR-7.3 (Static Document Uploader):** Component uploading file attachments to secure cloud storage (AWS S3/Azure Blob) and returning public reference links.

### 7.3. Validation Rules
* **VR-7.1 (Due Date Horizon):** Submission due date must be greater than or equal to the current system date. Past dates are rejected.
* **VR-7.2 (Attachment Limit):** File attachments must not exceed 5MB. Permitted file types: `.pdf`, `.jpeg`, `.jpg`, `.png`.

### 7.4. Acceptance Criteria
* **AC-7.1:** Given a completed homework entry form, when the teacher submits, then the system must upload the attachment to storage and list the item in the matching parent dashboard diaries.
* **AC-7.2:** Given an assignment card on the mobile diary feed, when the parent taps the attachment icon, then the browser must download the PDF resource.

### 7.5. Error Scenarios
* **ERR_FILE_TOO_LARGE:** Uploading an asset larger than 5MB must show a browser-level validation flag: *"Upload failed: File exceeds the maximum limit of 5MB."*

### 7.6. Business Rules
* **BR-7.1 (Syllabus Boundary):** Homework logs must tie strictly to active class/section models. Homework posts cannot cross different academic terms.

### 7.7. Dependencies
* Module 3 (SIS Class rosters).

### 7.8. UI Components Required
* **`HomeworkForm`:** Text editor, calendar picker, dropdown selectors for Class/Section/Subject, and file selector.
* **`ParentDiaryFeed`:** Infinite scroll card layouts sorted by Date (newest first). Each card displays Subject, Title, Assignment Text, Due Date, and Download Attachment button.

---

## 8. Circulars & Notifications

### 8.1. User Stories
* **US-8.1:** As a School Admin, I want to compose a critical notice and broadcast it immediately so that all parents receive SMS alerts on their mobile numbers.
* **US-8.2:** As a Parent, I want to view active school announcements on my dashboard notice board so that I do not miss official notices.

### 8.2. Functional Requirements
* **FR-8.1 (Circular Notice Builder):** Rich text panel where admins write notices, select priority (`Normal`, `Emergency`), and target audience (`All School`, `Specific Class`).
* **FR-8.2 (Broadcast SMS Dispatch Manager):** Background job dispatcher that breaks down circular target groups, formats message texts, and schedules pushes to Twilio/Gupshup APIs.
* **FR-8.3 (Bulletin Feed Widget):** Notice list widget pinned to the parent dashboard dashboard dashboard landing screen.

### 8.3. Validation Rules
* **VR-8.1 (Character Lengths):** Notice Titles: 5–100 characters. Notice Body: 10–1000 characters.
* **VR-8.2 (Broadcast SMS length limits):** Normal SMS broadcasts are restricted to 160 characters per SMS credit. If text exceeds 160 characters, the UI must show a character count warning.

### 8.4. Acceptance Criteria
* **AC-8.1:** Given an "Emergency" holiday alert, when published by the admin, then the system must display the notice in the parent feed immediately and dispatch SMS alerts to all targeted parents within 5 minutes.
* **AC-8.2:** Given a class-specific circular (e.g. Class 2 only), when published, then parents of Class 3 students must not see the announcement.

### 8.5. Error Scenarios
* **ERR_BROADCAST_GATEWAY_DOWN:** If the external notification service returns a gateway down error, the dashboard notice must still publish, but a warning must show on the admin's delivery report: *"Notice published. Broadcast alerts failed due to carrier error. Retrying in background..."*

### 8.6. Business Rules
* **BR-8.1 (Audit Visibility):** Published announcements cannot be edited by parents or teachers. Deletions must leave audit logs in system events.

### 8.7. Dependencies
* Module 2 & 3 (Contact indexes).

### 8.8. UI Components Required
* **`CircularComposer`:** Rich text workspace, target audience multi-select filters, priority radio buttons, and a "Publish and Broadcast" trigger button.
* **`NoticeBulletinBoard`:** Pinned list on the React homepage displaying active announcements, with color-coded tags (`Emergency` - Red, `General` - Blue).
* **`BroadcastDeliveryTracker`:** Logs indicating target counts, dispatch successes, and failure statistics.
