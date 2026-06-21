# UI Screen Inventory: EduPulse School ERP SaaS (Sprint 3)

**Project Name:** EduPulse School ERP SaaS  
**UI Framework:** React (Mobile-Responsive Web Layout)  
**Modules Covered:** Completed Sprint 1, 2, and 3 Backend Core Modules  
**Last Updated:** June 2026  

---

## Global Layout & Navigation Context
All screens operate within a responsive shell (`AdminLayout`, `TeacherLayout`, `ParentLayout`) containing:
* **Tenant Indicator:** Mapped automatically from the authenticated user's profile and active JWT claims.
* **Academic Year Dropdown:** Displays/stores active academic context globally (retrieved on session launch).
* **Sidebar Navigation:** Collapsible left-side menu showing mapped pages based on user roles and permissions.
* **Top Header:** User Profile logout, notifications, active School Logo, and tenant name.

---

## 1. Authentication Module

### 1.1. Screen Name: `LoginScreen`
* **Purpose:** Allows users to enter credentials to log in, receive a secure JWT token, and authenticate their session.
* **User Role:** All Roles / Anonymous (School Admin, Accountant, Teacher, Parent, Student)
* **Navigation Location:** Root URL (`/login`)
* **Required Actions:** 
  * Authenticate user credentials.
  * Save authenticated token to persistent state (localStorage/sessionState).
  * Redirect authenticated users to their corresponding dashboard homepages.
* **Fields:**
  * `Email` (Text, required, email format validation)
  * `Password` (Password, required, toggle-visibility eye icon)
* **Grid Columns:** N/A
* **Filters:** N/A
* **Buttons:**
  * `Sign In` (Primary action: invokes `POST /api/auth/login`)
  * `Forgot Password?` (Secondary action: redirects to recovery page)

---

## 2. Academic Years Module

### 2.1. Screen Name: `AcademicYearManager`
* **Purpose:** Setup and maintain the academic calendar years (start/end dates) defining school operational periods.
* **User Role:** `SchoolAdmin`
* **Navigation Location:** Sidebar Menu: Settings ──► Academic Years (`/settings/academic-years`)
* **Required Actions:** 
  * View list of calendar terms.
  * Register a new Academic Year.
  * Update current Academic Year dates.
  * Delete/Soft-delete Academic Year records.
* **Fields:**
  * `Name` (Text, required)
  * `StartDate` (Date picker, required)
  * `EndDate` (Date picker, required)
* **Grid Columns:**
  * `Name` (Academic Year label)
  * `Start Date` (Formatted date string)
  * `End Date` (Formatted date string)
  * `Actions` (Edit, Delete row action buttons)
* **Filters:** N/A
* **Buttons:**
  * `Add Academic Year` (Primary button: opens Create modal/drawer)
  * `Save` (Form submit button: invokes `POST /api/academic-years` or `PUT /api/academic-years/{id}`)
  * `Cancel` (Form abort button: closes modal/drawer)
  * `Edit` (Row-level action: loads details for editing)
  * `Delete` (Row-level action: triggers confirmation and invokes `DELETE /api/academic-years/{id}`)

---

## 3. Classes Module

### 3.1. Screen Name: `ClassManager`
* **Purpose:** Define and organize class standards (grades/years) in the school (e.g., Grade 1, Class 10).
* **User Role:** `SchoolAdmin`
* **Navigation Location:** Sidebar Menu: Settings ──► Classes (`/settings/classes`)
* **Required Actions:**
  * View class standards grid.
  * Create a new class standard.
  * Update class properties (Name, Sort Order).
  * Soft-delete class standard records.
* **Fields:**
  * `Name` (Text, required)
  * `SortOrder` (Integer input, required)
* **Grid Columns:**
  * `Class Name` (String)
  * `Sort Order` (Numeric order)
  * `Actions` (Edit, Delete row action buttons)
* **Filters:** N/A
* **Buttons:**
  * `Add Class` (Primary button: opens Create modal)
  * `Save` (Form submit button: invokes `POST /api/classes` or `PUT /api/classes/{id}`)
  * `Cancel` (Form abort button: closes modal)
  * `Edit` (Row-level action: loads class details)
  * `Delete` (Row-level action: prompts confirmation and invokes `DELETE /api/classes/{id}`)

---

## 4. Sections Module

### 4.1. Screen Name: `SectionManager`
* **Purpose:** Define classrooms/sections (e.g., Section A, Section B) which map to classes.
* **User Role:** `SchoolAdmin`
* **Navigation Location:** Sidebar Menu: Settings ──► Sections (`/settings/sections`)
* **Required Actions:**
  * View active sections list.
  * Register a new section.
  * Update section names.
  * Soft-delete section records.
* **Fields:**
  * `Name` (Text, required)
* **Grid Columns:**
  * `Section Name` (String)
  * `Actions` (Edit, Delete row action buttons)
* **Filters:** N/A
* **Buttons:**
  * `Add Section` (Primary button: opens Create modal)
  * `Save` (Form submit button: invokes `POST /api/sections` or `PUT /api/sections/{id}`)
  * `Cancel` (Form abort button: closes modal)
  * `Edit` (Row-level action: loads section details)
  * `Delete` (Row-level action: prompts confirmation and invokes `DELETE /api/sections/{id}`)

---

## 5. Staff Module

### 5.1. Screen Name: `StaffDirectory`
* **Purpose:** Maintain school teacher and administration staff profiles, linking details to optional system user logins.
* **User Role:** `SchoolAdmin`
* **Navigation Location:** Sidebar Menu: Staff Directory (`/staff`)
* **Required Actions:**
  * Roster directory search.
  * Register new staff record.
  * Edit staff details.
  * Soft-delete staff member records.
* **Fields:**
  * `UserId` (Select dropdown linking to system login accounts, optional)
  * `EmployeeCode` (Text, required, alphanumeric format validation)
  * `FirstName` (Text, required)
  * `LastName` (Text, required)
  * `Phone` (Text, required, phone validation format)
  * `Designation` (Text, optional)
  * `PhotoPath` (Image file upload/string, optional)
  * `IsActive` (Toggle checkbox, default: true)
* **Grid Columns:**
  * `Employee Code` (String)
  * `Full Name` (Combined FirstName + LastName)
  * `Designation` (String)
  * `Phone` (String)
  * `Status` (Badge: Active/Inactive)
  * `Actions` (Edit, Delete row action buttons)
* **Filters:**
  * `Search` (Text box filtering by Code, Name, Designation)
  - `Status Filter` (Dropdown: All, Active, Inactive)
* **Buttons:**
  * `Add Staff Member` (Primary button: redirects to `/staff/create` or opens creation drawer)
  * `Save` (Form submit button: invokes `POST /api/staff` or `PUT /api/staff/{id}`)
  * `Cancel` (Form abort button: closes creation drawer)
  * `Edit` (Row-level action: opens edit details layout)
  * `Delete` (Row-level action: prompts confirmation and invokes `DELETE /api/staff/{id}`)

---

## 6. Students Module

### 6.1. Screen Name: `StudentDirectory`
* **Purpose:** Manage active student registration profiles, assign class/section mappings directly (simplified MVP architecture), and manage status.
* **User Role:** `SchoolAdmin`, `Teacher` (read-only directory queries)
* **Navigation Location:** Sidebar Menu: Students (`/students`)
* **Required Actions:**
  * Search student logs database.
  * View detailed student bio profile.
  * Create a new student registration record.
  * Edit student record information.
  - Soft-delete student records.
* **Fields:**
  * `AdmissionNo` (Text, required)
  * `FirstName` (Text, required)
  * `LastName` (Text, required)
  * `DateOfBirth` (Date picker, required)
  * `Gender` (Dropdown selector: Male, Female, Other, required)
  * `BloodGroup` (Dropdown selector: A+, O-, etc., optional)
  * `GovernmentIdType` (Dropdown selector: Aadhaar, Passport, National ID, optional)
  * `GovernmentIdNumber` (Text, optional)
  * `SocialCategory` (Dropdown selector, optional)
  * `PhotoPath` (Image upload/file input, optional)
  * `AddressLine1` (Text, optional)
  * `AddressLine2` (Text, optional)
  * `City` (Text, optional)
  * `State` (Text, optional)
  * `Pincode` (Text, optional)
  * `AdmissionDate` (Date picker, required)
  * `Status` (Dropdown selector: Active, Inactive, Transferred, Suspended, required)
  * `ClassId` (Dropdown selection listing Classes, required)
  * `SectionId` (Dropdown selection listing Sections, required)
* **Grid Columns:**
  * `Admission No` (String)
  * `Full Name` (Combined FirstName + LastName)
  * `Class & Section` (Name mapped dynamically)
  * `Status` (Badge indicator matching status state)
  * `Admission Date` (Formatted date string)
  * `Actions` (View, Edit, Delete, and Manage Guardians row buttons)
* **Filters:**
  * `Search` (Text query filtering by Admission Number, Name)
  * `Class Filter` (Dropdown listing classes)
  * `Section Filter` (Dropdown listing sections)
  * `Status Filter` (Dropdown listing student statuses)
* **Buttons:**
  * `Register Student` (Primary button: redirects to create form layout `/students/create`)
  * `View` (Row-level action: opens full read-only student profile summary drawer/page)
  * `Edit` (Row-level action: redirects to edit page `/students/edit/{id}`)
  * `Delete` (Row-level action: prompts confirmation and invokes `DELETE /api/students/{id}`)
  * `Manage Guardians` (Row-level action: redirects to the student's guardian linkage sub-view)
  * `Save` (Form submit button: invokes `POST /api/students` or `PUT /api/students/{id}`)
  * `Cancel` (Form abort button: redirects back to roster)

---

## 7. Guardians Module

### 7.1. Screen Name: `GuardianDirectory`
* **Purpose:** Manage global repository of student parents/guardians independently from student profiles.
* **User Role:** `SchoolAdmin`
* **Navigation Location:** Sidebar Menu: Guardians ──► Directory (`/guardians`)
* **Required Actions:**
  * Search guardian registry database.
  * Create new guardian contact.
  * Edit guardian contact information.
  * Soft-delete guardian records.
* **Fields:**
  * `UserId` (Select dropdown mapping to User login accounts, optional)
  * `FirstName` (Text, required)
  * `LastName` (Text, required)
  * `Phone` (Text, required, phone format validation)
  * `Email` (Text, optional, email validation)
* **Grid Columns:**
  * `Full Name` (String)
  * `Phone` (String)
  * `Email` (String)
  * `Actions` (Edit, Delete row action buttons)
* **Filters:**
  * `Search` (Text query filtering by Name, Phone, Email)
* **Buttons:**
  * `Add Guardian` (Primary button: opens Create modal)
  * `Save` (Form submit button: invokes `POST /api/guardians` or `PUT /api/guardians/{id}`)
  * `Cancel` (Form abort button: closes modal)
  * `Edit` (Row-level action: loads details for editing)
  * `Delete` (Row-level action: prompts confirmation and invokes `DELETE /api/guardians/{id}`)

### 7.2. Screen Name: `StudentGuardianLinker`
* **Purpose:** Associate multiple parents/guardians to a single student profile and assign relationship roles.
* **User Role:** `SchoolAdmin`
* **Navigation Location:** Student Directory ──► Click "Manage Guardians" on row (`/students/{studentId}/guardians`)
* **Required Actions:**
  * View current linked guardians mapped to the active student.
  * Link an existing guardian from directory database to this student.
  * Update relationship type, primary contact, and billing responsible parameters.
  * Unlink/remove guardian association from this student.
* **Fields:**
  * `GuardianId` (Autocomplete selection dropdown search query of guardian directory, required)
  * `RelationshipType` (Dropdown selector: Father, Mother, Guardian, Uncle, Aunt, Grandparent, Brother, Sister, required)
  * `IsPrimaryContact` (Checkbox, marks if this guardian is main contact)
  * `IsBillingResponsible` (Checkbox, marks if this guardian is billed invoices)
* **Grid Columns:**
  * `Full Name` (String)
  * `Phone` (String)
  * `Relationship` (String type)
  * `Primary Contact` (Badge: Yes/No)
  * `Billing Responsible` (Badge: Yes/No)
  * `Actions` (Edit Link, Unlink row action buttons)
* **Filters:** N/A (Scoped directly to the selected student)
* **Buttons:**
  * `Link Existing Guardian` (Primary button: opens linkage modal)
  * `Save Link` (Form submit button: invokes `POST /api/students/{studentId}/guardians` or `PUT /api/students/{studentId}/guardians/{guardianId}`)
  * `Cancel` (Form abort button: closes modal)
  * `Edit Link` (Row-level action: opens relationship settings edit popup)
  * `Unlink` (Row-level action: prompts confirmation and invokes `DELETE /api/students/{studentId}/guardians/{guardianId}`)

---

## 8. Attendance Module

### 8.1. Screen Name: `DailyAttendanceSheet`
* **Purpose:** Bulk register and update the daily attendance sheet for a selected Class, Section, and Date.
* **User Role:** `Teacher`, `SchoolAdmin`
* **Navigation Location:** Sidebar Menu: Attendance ──► Daily Log (`/attendance/mark`)
* **Required Actions:**
  * Select target Class, Section, and Date.
  * Load interactive roster of mapped students.
  * Mark attendance status for each student row.
  * Add custom comments/remarks for specific student attendance issues.
  * Save marked attendance bulk list (idempotent upsert transaction).
* **Fields:**
  * `Date` (Date picker, required, restricted to current date or past dates)
  * `ClassId` (Dropdown selector, required)
  * `SectionId` (Dropdown selector, required)
  * Individual Student Row Inputs:
    * `Status` (Radio button options: Present, Absent, Late, HalfDay, required)
    * `Remarks` (Text input, optional, max 200 chars)
* **Grid Columns:**
  * `Admission No` (String, read-only)
  * `Student Name` (FirstName + LastName, read-only)
  * `Attendance Status` (Radio selection: P / A / L / H)
  * `Remarks` (Inline text input)
* **Filters:**
  * `Class Selector` (Active options)
  * `Section Selector` (Active options)
  * `Date Selector` (Restricted calendar)
* **Buttons:**
  * `Load Roster` (Queries student list via `GET /api/attendance/daily`)
  * `Submit Attendance` (Saves complete roster status bulk request via `POST /api/attendance/mark`)
  - `Reset Sheet` (Clears filters and selections)

### 8.2. Screen Name: `StudentAttendanceHistory`
* **Purpose:** Review individual student historical daily logs and statistics.
* **User Role:** `SchoolAdmin`, `Teacher`, `Parent`
* **Navigation Location:** Student Directory ──► View Student Profile ──► Click Attendance History tab (`/students/{id}/attendance-history`)
* **Required Actions:**
  * View chronological lists of student attendance status.
* **Fields:** N/A (Read-only summary view)
* **Grid Columns:**
  * `Date` (Formatted date string)
  * `Class & Section` (String)
  * `Status` (Colored badge: Present/Absent/Late/HalfDay)
  * `Remarks` (String, display text or blank)
* **Filters:**
  * `Date Range` (Start Date & End Date calendars)
* **Buttons:**
  * `Print Log` (Triggers browser printer layout)

### 8.3. Screen Name: `ClassAttendanceDashboard`
* **Purpose:** Monitor attendance metrics and absentee timeline stats per class standard.
* **User Role:** `SchoolAdmin`, `Teacher`
* **Navigation Location:** Sidebar Menu: Attendance ──► Analytics (`/attendance/analytics`)
* **Required Actions:**
  * Query aggregate stats timeline summaries for classes.
* **Fields:**
  * `ClassId` (Select Class dropdown, required)
  * `StartDate` (Date picker, optional)
  * `EndDate` (Date picker, optional)
* **Grid Columns:**
  * `Date` (Formatted date string)
  * `Total Present` (Numeric)
  * `Total Absent` (Numeric)
  * `Total Late` (Numeric)
  * `Total HalfDay` (Numeric)
  * `Attendance Rate` (Percentage rate of Present / Total)
* **Filters:**
  * `Class Selector` (Dropdown selection)
  * `Date Range Filter` (Start Date, End Date)
* **Buttons:**
  * `Query Analytics` (Triggers backend data request via `GET /api/attendance/class/{classId}`)
  * `Export Summary` (Generates spreadsheet reports download)
