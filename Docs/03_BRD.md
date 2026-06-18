# Business Requirements Document (BRD): AI-First School Management SaaS
**Project Name:** EduPulse AI (MVP Scope)  
**Role Perspective:** Senior Business Analyst  
**Technologies Enforced:** React | .NET 8 Web API | SQL Server  
**Target Audience:** Client Stakeholders, Engineering Team  

---

## Document Control & Version History

| Version | Date | Author | Description of Changes | Approved By |
| :--- | :--- | :--- | :--- | :--- |
| 1.0.0 | 2026-06-18 | Senior Business Analyst | Initial MVP Business Requirements Document | *Pending Client Review* |

---

## Executive Overview
This document defines the functional and non-functional requirements for the Minimum Viable Product (MVP) of **EduPulse AI**—a multi-tenant, cloud-native school management platform designed for K-12 private schools in India. 

The primary business objective is to deliver a highly stable, operational core system within **3 months** that replaces legacy school administration overhead, minimizes cash-fee leakages, standardizes grade books to board compliance (CBSE/State Board), and establishes a clean mobile-web parent communication channel.

### System Scope Boundaries:
* **Included:** Core Administrative Setup, Multi-Tenant Security, Student registries (Excel migration), Daily Attendance alerts, double-entry financial ledger logic, offline fee collection counters, online UPI/Gateway integrations, CBSE grade computations, and dynamic mobile-responsive diaries.
* **Excluded (Future Phase):** Automated AI substitutions, OCR paper scanners, native app compilation (iOS/Android store releases), dynamic geofenced transport coordinates, and live biometric hardware sync.

---

## Module 1: Multi-Tenant Administration

### 1.1. Business Objective
Enable a single cloud application instance to serve multiple distinct school clients securely. Each school (tenant) must have isolated data parameters and local administrative settings (e.g., logo, active academic years, classes, and subjects) without performance interference or cross-tenant exposure.

### 1.2. Functional Requirements
* **FR-1.1: Tenant Profile Setup:** The system must capture profile metadata for each school: official School Name, Registration Number, Affiliation Board (CBSE/State Board), School Address, Primary Contact Details, and Brand Assets (Logo, Header Images).
* **FR-1.2: Academic Year Configuration:** The system must support the configuration of multiple academic calendar cycles (e.g., "April 2026 – March 2027"). Admins must define a single "Active Year" which governs all default invoice cycles, grading, and attendances.
* **FR-1.3: Class & Section Configurator:** School admins must be able to create Class levels (e.g., LKG, UKG, Grade 1 to 12) and map Sections/Divisions (e.g., Section A, Section B) containing student enrollment caps.
* **FR-1.4: Subject-to-Class Mapping:** The system must allow mapping of curricular subjects (e.g., Mathematics, English Literature, Physics) to specific classes, defining whether the subject contains theory, practical, or both.

### 1.3. User Roles
* **Super Admin:** Full administrative privileges across all tenants (onboarding new school accounts).
* **School Admin:** Read-write access to their own school's tenant profile settings, classes, and subject configurations.
* **Teacher / Parent / Student:** Read-only access to configured classes, sections, and subjects mapped to their user credentials.

### 1.4. Business Rules
* **BR-1.1: Data Segregation:** No user from Tenant A can view, query, or modify any database record associated with Tenant B.
* **BR-1.2: Single Active Calendar:** Only one academic year can be set as "Active" for a tenant at any given time.
* **BR-1.3: Immutable Class Roster:** A class configuration cannot be deleted if active student enrollments are currently mapped to it.

### 1.5. Workflows
1. **Tenant Onboarding Flow:** Super Admin registers the school -> Sets up administrative username -> School Admin logs in -> Configures the active Academic Year -> Creates Classes and Sections -> Maps Subjects to Classes.

### 1.6. Reports
* **School Settings Audit Report:** Logs current active academic year, count of registered classes, total sections, and board affiliation configurations.

### 1.7. Permissions
* **Super Admin:** Create/Read/Update/Delete Tenant Profiles.
* **School Admin:** Create/Read/Update Academic Years, Classes, Sections, and Subject Configurations.
* **Other Roles:** Read-only access to academic structure.

### 1.8. Assumptions
* Initial tenant database creation will be performed manually or via backend scripts by the development team during the pilot launch phase.

### 1.9. Dependencies
* Relational database tables must be mapped to query filters tied to tenant authentication claims at the API layer.

---

## Module 2: User Management & RBAC (Role-Based Access Control)

### 2.1. Business Objective
Secure all application interactions by validating user identities and applying a strict permission model based on user roles (Super Admin, School Admin, Teacher, and Parent).

### 2.2. Functional Requirements
* **FR-2.1: Secure Login:** Authenticate users using unique usernames/emails and encrypted passwords.
* **FR-2.2: Password Reset:** Allow School Admins to trigger a temporary password reset for teachers or parents who lose access.
* **FR-2.3: Session Token Generation:** Issue secure JWT (JSON Web Token) sessions for authenticated React web-client requests.
* **FR-2.4: Profile View:** Allow users to view and update personal contact info (email, secondary phone number).

### 2.3. User Roles
* **School Admin:** Create and manage user credentials for Teachers, Parents, and Accountants.
* **All Roles:** Update own profile contact details and change password.

### 2.4. Business Rules
* **BR-2.1: Strong Passwords:** Passwords must meet basic length and complexity criteria (minimum 8 characters, 1 uppercase, 1 numeric, 1 special character).
* **BR-2.2: Account Suspension:** Suspended accounts must fail login immediately and invalidate all outstanding JWT session tokens.
* **BR-2.3: Parent-Student Association:** Every parent account must be linked to at least one active student record in the SIS registry.

### 2.5. Workflows
1. **User Authentication Flow:** User enters credentials -> API validates -> Returns JWT token mapped to User Role and Tenant ID -> React UI shows the matching Dashboard based on user profile claims.

### 2.6. Reports
* **User Authentication Logs:** Tracks login timestamps, IP addresses, and failed authentication attempts.

### 2.7. Permissions
* **School Admin:** Create/Read/Update/Suspend user accounts.
* **Teacher / Parent / Accountant:** Read own user profile, Update password.

### 2.8. Assumptions
* Multi-factor authentication (MFA) is out of scope for the MVP. Standard password authentication is sufficient.

### 2.9. Dependencies
* Module 1 (Multi-Tenant Administration) must validate the user is logged into the correct tenant domain.

---

## Module 3: Student Information System (SIS)

### 3.1. Business Objective
Maintain a secure, centralized digital registry of student enrollments, family associations, and regulatory documentation, enabling admins to migrate quickly from physical paperwork or Excel spreadsheets.

### 3.2. Functional Requirements
* **FR-3.1: Profile Creator:** Capture comprehensive student details: Admission Number (UID), Roll Number, Full Name, Gender, DOB, Blood Group, Joining Date, Class, Section, and Parent Details (Name, Phone, Email).
* **FR-3.2: Bulk Excel Import:** Provide an administrative page to download a standard `.xlsx` template. Admins fill student rosters offline and upload the spreadsheet to register hundreds of profiles in a single operation.
* **FR-3.3: Document Attachment Vault:** Upload scans of critical student documentation: Aadhaar Card, Birth Certificate, and Transfer Certificate (TC) in PDF/JPEG formats.

### 3.3. User Roles
* **School Admin:** Perform profile creation, edits, bulk uploads, and document attachments.
* **Teacher:** Read student records assigned to their classes (cannot edit).
* **Parent:** Read profile details matching their child.

### 3.4. Business Rules
* **BR-3.1: Unique Identifiers:** The Admission Number must be unique across the entire school tenant database.
* **BR-3.2: Roll Number Consistency:** A roll number must be unique within a specific Class-Section combination.
* **BR-3.3: Bulk Upload Rollback:** If any single record inside a bulk Excel import spreadsheet fails validation, the system must abort the transaction completely and report the exact rows/errors to prevent partial, dirty data writes.

### 3.5. Workflows
1. **Bulk Student Onboarding:** Admin downloads Excel template -> Fills out 500 student records -> Uploads file -> System runs validation checks -> If clean, database populates, auto-creates linked parent profiles, and sends a system confirmation log.

### 3.6. Reports
* **Class Roster List:** PDF/Excel export showing names, roll numbers, blood groups, and emergency primary contacts.
* **Missing Documents Checklist:** Identifies students missing required Aadhaar or birth certificate files.

### 3.7. Permissions
* **School Admin:** Full Create/Read/Update/Delete (CRUD) on Student Profiles and Attachments.
* **Teacher:** Read-only access.
* **Parent:** Read own child's profile only.

### 3.8. Assumptions
* Parents do not have permission to edit student profiles directly to prevent discrepancies in official academic records.

### 3.9. Dependencies
* Module 1 (Tenant configuration) must be completed before students can be mapped to active classes/sections.

---

## Module 4: Attendance Management

### 4.1. Business Objective
Track student daily attendance through a quick web interface and instantly notify parents of absences to reduce school liability and ensure prompt parental awareness.

### 4.2. Functional Requirements
* **FR-4.1: Mobile-Web Grid Interface:** Provide a mobile-responsive grid layout where teachers check/uncheck checkboxes next to student names. Checked = Present, Unchecked = Absent.
* **FR-4.2: Status Auditing:** Support tracking values: Present, Absent, Late.
* **FR-4.3: Automated Absence WhatsApp/SMS Alerts:** On finalization of attendance by the teacher, queue SMS/WhatsApp messages via external gateways (e.g. Twilio/Gupshup) to primary parents of absent students.
* **FR-4.4: Lockout Override:** Prevent teachers from modifying historical attendance records once a daily calendar limit passes, requiring admin authorization for corrections.

### 4.3. User Roles
* **Teacher:** Mark and submit daily attendance sheets for their assigned classes.
* **School Admin:** Review, edit historical attendance logs, and bypass daily lockout limits.
* **Parent:** Read historical daily attendance history for their child.

### 4.4. Business Rules
* **BR-4.1: Single Daily Log:** Attendance can only be logged once per day per class section.
* **BR-4.2: Finalization Threshold:** Teachers must submit attendance by 09:30 AM daily, after which modifications are locked.
* **BR-4.3: Absence Alert Trigger:** Alerts are only sent to primary contacts of students marked as "Absent".

### 4.5. Workflows
1. **Daily Attendance Flow:** Teacher logs into mobile browser -> Opens homeroom attendance roster -> Marks absent students -> Clicks "Finalize" -> System saves to SQL database -> background queues dispatch WhatsApp alerts to parents -> Parents receive alert: *"Dear Parent, [Name] was marked absent today [Date] - [SchoolName]"*.

### 4.6. Reports
* **Monthly Attendance Percentage Report:** Lists all students with their total days present/absent and overall attendance percentage (flagging levels below 75%).
* **Daily Absentee Register:** A quick log of all absentees on the active date.

### 4.7. Permissions
* **School Admin:** Read/Update all records.
* **Teacher:** Read/Write records for assigned classes on current date only.
* **Parent:** Read child's log.

### 4.8. Assumptions
* Period-wise attendance tracking is out of scope for the MVP. The system tracks homeroom attendance once per day.

### 4.9. Dependencies
* Module 3 (SIS) for current class lists and primary parent contact records.

---

## Module 5: Fee Management & Collections

### 5.1. Business Objective
Prevent revenue leakages, streamline administrative cash collections, and provide parents with easy digital payment reconciliation.

### 5.2. Functional Requirements
* **FR-5.1: Fee Structure Configurator:** Define fee line items (e.g., Tuition Fee, Computer Lab Fee, Transport Slabs) and group them into term bills mapped to classes.
* **FR-5.2: Student Ledger:** Maintain a ledger showing billed amounts, payment history, concessions, and balance dues.
* **FR-5.3: Cashier Payment Desk:** Process cash, card, or physical check collections manually at the front office. Generate simple, formatted print slips.
* **FR-5.4: Online Payment Gateway (Razorpay):** Generate Razorpay UPI/Card checkout URLs for bills.
* **FR-5.5: Auto-Reconciliation Webhook:** Intercept Razorpay transaction callbacks to automatically update SQL ledgers and send digital receipt validations without manual intervention.
* **FR-5.6: WhatsApp Bill Pushes:** Disburse automated payment notifications containing direct UPI checkout links.

### 5.3. User Roles
* **Accountant:** Setup fee configurations, record cash/cheque payments, print manual receipts.
* **School Admin:** Full review, approve fee waivers or manual ledger write-offs.
* **Parent:** View ledgers, execute online checkout.

### 5.4. Business Rules
* **BR-5.1: Immutable Financials:** No receipt record can be edited or deleted. Discrepancies must be corrected by issuing formal Credit Notes.
* **BR-5.2: FIFO Allocation:** Partial payments must allocate to the oldest unpaid fees first.
* **BR-5.3: Concession Audit Trace:** Concessions must record the approving Admin's ID and require a text rationale log.

### 5.5. Workflows
1. **Online Payment Workflow:** System generates monthly bill -> Queues WhatsApp reminder with Razorpay checkout URL -> Parent taps link -> Executes UPI payment -> Razorpay triggers payment success webhook -> .NET API records transaction, settles ledger balance -> Parent receives confirmation notification.

### 5.6. Reports
* **Daily Fee Collection Report:** Breakdown of total funds collected divided by payment methods (Cash, UPI, Cards, Cheques).
* **Defaulter List (Arrears Report):** Lists students with unpaid dues, total balance, and days past due.

### 5.7. Permissions
* **School Admin / Accountant:** Create/Read/Update Fee Structures and Ledgers.
* **Parent:** Read-only access to child's ledger, execute payment transactions.
* **Teacher:** No access.

### 5.8. Assumptions
* Merchant accounts (Razorpay) will be managed at the school tenant level; transaction fees are borne by the tenant or parent based on school policies.

### 5.9. Dependencies
* Module 3 (SIS) to calculate student mappings and Module 8 for alert delivery.

---

## Module 6: Examination & Report Cards

### 6.1. Business Objective
Reduce administrative effort for teachers during testing periods and standardize report card publication in accordance with board regulations.

### 6.2. Functional Requirements
* **FR-6.1: Exam Configuration:** Define academic terms, exam profiles (Mid-Term, Final), and allocate maximum marks/weightings.
* **FR-6.2: Grade Book Grid:** Provide teachers with a grid to input student numerical scores for their assigned classes.
* **FR-6.3: Board Scale Calculations:** Automatically convert numerical marks to standard board grades (e.g., CBSE 9-Point Scale: A1, A2, B1, etc.).
* **FR-6.4: QuestPDF Report Card Compiler:** Generate a print-ready, professional PDF report card combining test grades, class attendance percentages, and descriptive teacher feedback.

### 6.3. User Roles
* **Teacher:** Access the grade book to enter numerical scores for their mapped classes and subjects.
* **School Admin:** Configure exams, edit grades after submission locks, and publish report cards.
* **Parent:** Download compiled PDF report cards (visible only after admin publication).

### 6.4. Business Rules
* **BR-6.1: Assignment Matching:** Teachers can only access grade entries for subjects they are assigned to teach.
* **BR-6.2: Grade Modification Lockout:** Once a teacher clicks "Submit to Coordinator", the marks sheet freezes. Modifications require Admin unlock.
* **BR-6.3: Publication Visibility:** Parents cannot view scores or download report cards until the school admin sets the exam status to "Published".

### 6.5. Workflows
1. **Report Card Compilation:** Exams end -> Teachers enter scores into grade book grid -> System validates ranges, auto-calculates letter grades -> Teacher submits -> Admin reviews overall class lists -> Admin clicks "Publish Report Cards" -> Parents download PDFs from dashboard.

### 6.6. Reports
* **Subject Merit List:** Ranks students within a class section based on scores.
* **Pass/Fail Breakdown:** Statistical summary of students falling below passing marks.

### 6.7. Permissions
* **School Admin:** Create/Read/Update/Publish all exams.
* **Teacher:** Read/Write scores for mapped classes before lock.
* **Parent:** Read/Download PDF report cards post-publication.

### 6.8. Assumptions
* Standard CBSE evaluation guidelines are configured as default templates.

### 6.9. Dependencies
* Module 3 (SIS) for student rosters and Module 4 (Attendance) for attendance percentages on report cards.

---

## Module 7: Homework Management

### 7.1. Business Objective
Provide a unified digital assignment diary for students and parents to view daily learning tasks, replacing unstructured communication in WhatsApp groups.

### 7.2. Functional Requirements
* **FR-7.1: Assignment Builder:** Allow teachers to construct daily homework cards: Title, Instructions text, Subject mapping, Submission deadline, and PDF/Image document attachments.
* **FR-7.2: Digital Diary Feed:** Display a chronological list of assigned homework tasks for parents and students to view on mobile browsers.
* **FR-7.3: Homework Index:** Provide a quick lookup of previous assignments filtered by subject and date.

### 7.3. User Roles
* **Teacher:** Create, update, and manage homework cards for their assigned classes.
* **Parent / Student:** Read homework instructions and download file attachments.

### 7.4. Business Rules
* **BR-7.1: Class Context:** Homework assignments must map to a valid Academic Class, Section, and Subject.
* **BR-7.2: Attachment Constraint:** Upload attachments must be limited to a maximum of 5MB per file to control cloud storage limits.

### 7.5. Workflows
1. **Homework Posting Flow:** Teacher logs into dashboard -> Selects Class 4A - Physics -> Writes assignment instructions -> Attaches a worksheet PDF -> Submits -> System stores file in S3/Azure Blob -> Parents view the card on their daily dashboard feed.

### 7.6. Reports
* **Teacher Assignment Audits:** Logs count of homework items posted per subject to verify curriculum tracking.

### 7.7. Permissions
* **Teacher:** Create/Read/Update/Delete homework for assigned classes.
* **Parent / Student:** Read-only access, file download access.
* **School Admin:** Read-only oversight.

### 7.8. Assumptions
* Students will submit homework physically in class. The MVP does not support digital document uploads for student submissions.

### 7.9. Dependencies
* Module 3 (SIS) student class registries.

---

## Module 8: Circulars & Notifications

### 8.1. Business Objective
Ensure critical, official school announcements (such as emergency closures or event updates) are dispatched immediately and visible to all parent stakeholders.

### 8.2. Functional Requirements
* **FR-8.1: Notice Board Editor:** Create text notices with a Title, Description, and Date. Target audiences can be set to "All School" or filtered by specific classes.
* **FR-8.2: Emergency Alert Dispatcher:** Send instant SMS/WhatsApp alerts for high-priority announcements (e.g., severe weather holidays).
* **FR-8.3: Bulletin Feed:** Display active announcements on the Parent React UI homepage.

### 8.3. User Roles
* **School Admin:** Create, edit, delete, and broadcast circular notices.
* **Teacher / Parent / Student:** Read active notices.

### 8.4. Business Rules
* **BR-8.1: Circular Immutability:** Published circulars are read-only for teachers and parents.
* **BR-8.2: Text Message Limits:** SMS alerts must enforce a character limit matching telecom configurations to prevent billing escalations.

### 8.5. Workflows
1. **Emergency Announcement Flow:** School Admin logs in -> Creates notice: *"School Closed Tomorrow due to heavy rain"* -> Targets "All Classes" -> Marks as "High Priority Broadcast" -> Submits -> System publishes to React dashboard feed and dispatches WhatsApp alerts to all parents.

### 8.6. Reports
* **Message Delivery Log:** Reports count of successfully dispatched SMS/WhatsApp messages vs. errors.

### 8.7. Permissions
* **School Admin:** Full Create/Read/Update/Delete and Send permissions.
* **Other Roles:** Read-only access.

### 8.8. Assumptions
* Gateway costs (Twilio/Gupshup) are managed at the tenant configuration level.

### 8.9. Dependencies
* Module 2 (User details) and Module 3 (SIS) to resolve phone contacts.
