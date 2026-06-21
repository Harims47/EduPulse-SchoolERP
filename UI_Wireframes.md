# UI Low-Fidelity Wireframes: EduPulse School ERP SaaS (Sprint 3)

**Project Name:** EduPulse School ERP SaaS  
**Document Type:** UX/UI Design Specifications  
**Status:** Design Freeze - Ready for Interface Implementation  

This document presents low-fidelity ASCII wireframes and design rules for the core screens of the completed backend modules.

---

## 1. LoginScreen (Authentication)

### ASCII Wireframe
```text
+--------------------------------------------------------------------------+
|                                                                          |
|                           +------------------+                           |
|                           |  [EduPulse Logo] |                           |
|                           |    EDUPULSE AI   |                           |
|                           +------------------+                           |
|                                                                          |
|                        +------------------------+                        |
|                        | Sign In to Your Portal |                        |
|                        |                        |                        |
|                        | Email Address          |                        |
|                        | [ user@schoolerp.com ] |                        |
|                        |                        |                        |
|                        | Password               |                        |
|                        | [ ********         [o] |                        |
|                        |                        |                        |
|                        | [x] Remember Me        |                        |
|                        |                        |                        |
|                        |   [    SIGN IN    ]    |                        |
|                        |                        |                        |
|                        | Forgot Password?       |                        |
|                        +------------------------+                        |
|                                                                          |
+--------------------------------------------------------------------------+
```

### Layout Specifications
* **Header Layout:** Clean, centered brand section containing the logo icon placeholder and large bold title typography. No navigation headers exist on this public route.
* **Sidebar Layout:** N/A (Full viewport layout, no sidebar present).
* **Filters Section:** N/A.
* **Form Layout:** A single centered card container with a maximum width of `450px`. Fields are stacked vertically:
  * Email label and input (placeholder showing sample credentials format).
  * Password label, password input, and inline visibility toggle button `[o]` on the right.
* **Table Layout:** N/A.
* **Buttons:**
  * `[ SIGN IN ]`: Primary call-to-action button, full card width, accent color with hover status animations.
  * `Forgot Password?`: Text link underneath the form.
* **Mobile Responsive Behavior:** Card container scales down to `100%` width on screens `<576px`, with minimal gutters padding (`16px`) from device edges. Vertical gutters expand slightly for thumb-friendly input spacing.

---

## 2. System Dashboard

### ASCII Wireframe
```text
+-------------------------------------------------------------------------------------------------+
| [LOGO] EduPulse School ERP        Academic Year: [ 2026-2027 v ]              (Profile) (Logout)|
+-------------------------------------------------------------------------------------------------+
|  Navigation       |                                                                             |
|  [D] Dashboard (*)|  System Dashboard                                                           |
|  [A] Academics v  |                                                                             |
|    - Years        |  +--------------------+  +--------------------+  +--------------------+     |
|    - Classes      |  | Total Students     |  | Total Staff        |  | Today Attendance   |     |
|    - Sections     |  |                    |  |                    |  |                    |     |
|  [S] Staff        |  |  [ 1,420 Active ]  |  |   [ 85 Active ]    |  |  [ 95.6% Marked ]  |     |
|  [S] Students     |  +--------------------+  +--------------------+  +--------------------+     |
|  [G] Guardians    |                                                                             |
|  [A] Attendance   |  Quick Links                                                                |
|                   |  +------------------+  +------------------+  +------------------+           |
|                   |  | [ Mark Attendance] |  | [Register Student] |  | [ Add Staff ]    |           |
|                   |  +------------------+  +------------------+  +------------------+           |
|                   |                                                                             |
|                   |  Recent System Audits Log                                                   |
|                   |  +-----------------------------------------------------------------------+  |
|                   |  | Timestamp           | Action               | Performed By             |  |
|                   |  |---------------------|----------------------|--------------------------|  |
|                   |  | 2026-06-18 10:15 AM | Mark Attendance (10A)| admin@edupulse.com       |  |
|                   |  | 2026-06-18 09:30 AM | Added Student (S102) | admin@edupulse.com       |  |
|                   |  +-----------------------------------------------------------------------+  |
+-------------------+-----------------------------------------------------------------------------+
```

### Layout Specifications
* **Header Layout:** Fixed top navbar spanning `100%` viewport width. Contains brand logo left, academic year selector dropdown in the middle, and user settings/profile triggers on the right.
* **Sidebar Layout:** Left panel fixed position sidebar containing icon links to modules. Highlighted state `(*)` marks active route dashboard.
* **Filters Section:** N/A (Maintained via global academic year dropdown in top header).
* **Form Layout:** N/A.
* **Table Layout:** A standard summary table tracking transactional audits: Columns display Timestamp, Action title, and Performed By user.
* **Buttons:**
  * `[ Mark Attendance ]`, `[ Register Student ]`, `[ Add Staff ]`: Quick-action shortcut buttons formatted in a grid structure.
* **Mobile Responsive Behavior:** 
  * Sidebar hides on screen width `<992px`, replaced by a hamburger navigation toggle in top header.
  * Stat cards stack vertically into a single-column layout instead of three columns.
  * The Audit Table wraps with scroll support (`overflow-x: auto`) and hides secondary metadata columns like 'Performed By' on mobile.

---

## 3. StudentDirectory

### ASCII Wireframe
```text
+-------------------------------------------------------------------------------------------------+
| [LOGO] EduPulse School ERP        Academic Year: [ 2026-2027 v ]              (Profile) (Logout)|
+-------------------------------------------------------------------------------------------------+
|  Navigation       |                                                                             |
|  [D] Dashboard    |  Student Directory                          [ + REGISTER STUDENT ]          |
|  [A] Academics v  |                                                                             |
|    - Years        |  +-----------------------------------------------------------------------+  |
|    - Classes      |  | Filters:                                                              |  |
|    - Sections     |  | Search Name/ID: [ Doe, John        ]  Class: [ Class 10 v ]           |  |
|  [S] Staff        |  | Section: [ Section A v ]              Status: [ Active     v ]           |  |
|  [S] Students (*) |  +-----------------------------------------------------------------------+  |
|  [G] Guardians    |                                                                             |
|  [A] Attendance   |  Roster List                                                                |
|                   |  +-----------------------------------------------------------------------+  |
|                   |  | Adm. No  | Student Name    | Class/Sec  | Status   | Actions          |  |
|                   |  |----------|-----------------|------------|----------|------------------|  |
|                   |  | ADM-102  | John Doe        | Class 10-A | Active   | (V) (E) (D) (G)  |  |
|                   |  | ADM-103  | Jane Smith      | Class 10-A | Active   | (V) (E) (D) (G)  |  |
|                   |  +-----------------------------------------------------------------------+  |
|                   |  Showing 1-2 of 2 entries                          Page: [ < ] [ 1 ] [ > ]  |
+-------------------+-----------------------------------------------------------------------------+
```

### Layout Specifications
* **Header Layout:** Fixed top navbar spanning `100%` viewport width.
* **Sidebar Layout:** Standard sidebar shell layout.
* **Filters Section:** Filter bar containing a text search input for Name/ID query, alongside dropdown pickers for Class, Section, and Status fields.
* **Form Layout:** N/A.
* **Table Layout:** Tabular presentation of student rosters:
  * Columns: Admission No, Student Name, Class/Section label, Status badge, and Actions columns.
  * Inline Action Icons: `(V)` View Profile details, `(E)` Edit properties, `(D)` Delete record, and `(G)` Manage associated Guardians.
* **Buttons:**
  * `[ + REGISTER STUDENT ]`: Accent colored primary trigger button positioned top-right, redirecting to the Registration screen.
  * Page selection buttons `[ < ]`, `[ 1 ]`, `[ > ]` aligned right at the table footer.
* **Mobile Responsive Behavior:** Filter fields stack into a vertical grid list. Table drops less crucial columns like Admission Date, displaying only Admission No, Student Name, and the actions row. Action icons consolidate inside a collapsible menu list.

---

## 4. StudentCreateEdit (Form)

### ASCII Wireframe
```text
+-------------------------------------------------------------------------------------------------+
| [LOGO] EduPulse School ERP        Academic Year: [ 2026-2027 v ]              (Profile) (Logout)|
+-------------------------------------------------------------------------------------------------+
|  Navigation       |                                                                             |
|  [D] Dashboard    |  Register New Student / Edit Student                                        |
|  [A] Academics v  |                                                                             |
|    - Years        |  +-----------------------------------------------------------------------+  |
|    - Classes      |  | Profile Information                                                   |  |
|    - Sections     |  | Admission No:  [ ADM-102           ]   Adm. Date: [ 2026-06-18 v ]    |  |
|  [S] Staff        |  | First Name:    [ John              ]   Last Name: [ Doe            ]    |  |
|  [S] Students (*) |  | Date of Birth: [ 2012-05-14 v ]        Gender:    [ Male       v ]    |  |
|  [G] Guardians    |  | Class:         [ Class 10    v ]        Section:   [ Section A  v ]    |  |
|  [A] Attendance   |  | Status:        [ Active      v ]        Category:  [ General    v ]    |  |
|                   |  |                                                                       |  |
|                   |  | Contact Address                                                       |  |
|                   |  | Address L1:    [ 123 Main Street                                   ]  |  |
|                   |  | City:          [ Metropolis        ]   State:     [ New York   v ]    |  |
|                   |  | Pincode:       [ 10001             ]                                  |  |
|                   |  +-----------------------------------------------------------------------+  |
|                   |  [   SAVE STUDENT RECORD   ]                  [ CANCEL / RETURN ]        |
+-------------------+-----------------------------------------------------------------------------+
```

### Layout Specifications
* **Header & Sidebar Layout:** Standard master dashboard shell.
* **Filters Section:** N/A.
* **Form Layout:** A multi-column grouped layout with field cards:
  * *Profile Info Section:* Columns matching Admission Number, Admission Date, Names, Date of Birth, Gender, target Class and Section selects, Category lookup, and Status indicator.
  * *Contact Address Section:* Spanning full-width layout inputs for street address details, alongside town, state, and postal code grids.
* **Table Layout:** N/A.
* **Buttons:**
  * `[ SAVE STUDENT RECORD ]`: Primary action validation submit button, highlighted.
  * `[ CANCEL / RETURN ]`: Neutral secondary action button returning users to student lists.
* **Mobile Responsive Behavior:** All grid structures collapse into a single-column layout containing full-width form inputs. Buttons stack vertically at the page base.

---

## 5. GuardianDirectory (Directory & Link Manager)

### ASCII Wireframe
```text
+-------------------------------------------------------------------------------------------------+
| [LOGO] EduPulse School ERP        Academic Year: [ 2026-2027 v ]              (Profile) (Logout)|
+-------------------------------------------------------------------------------------------------+
|  Navigation       |                                                                             |
|  [D] Dashboard    |  Guardian Directory                                 [ + ADD GUARDIAN ]      |
|  [A] Academics v  |                                                                             |
|    - Years        |  +-----------------------------------------------------------------------+  |
|    - Classes      |  | Filters:                                                              |  |
|    - Sections     |  | Search Name/Phone/Email: [ 555-0199                         ]         |  |
|  [S] Staff        |  +-----------------------------------------------------------------------+  |
|  [S] Students     |                                                                             |
|  [G] Guardians (*)|  Active Guardian Roster                                                      |
|  [A] Attendance   |  +-----------------------------------------------------------------------+  |
|                   |  | Guardian Name      | Phone        | Email              | Actions      |  |
|                   |  |--------------------|--------------|--------------------|--------------|  |
|                   |  | Richard Doe        | 555-0199     | richard@example.com| (E) (D) (L)  |  |
|                   |  | Mary Doe           | 555-0200     | mary@example.com   | (E) (D) (L)  |  |
|                   |  +-----------------------------------------------------------------------+  |
|                   |                                                        Page: [ < ] 1 [ > ]  |
+-------------------+-----------------------------------------------------------------------------+
```

### Layout Specifications
* **Header & Sidebar Layout:** Standard master dashboard shell.
* **Filters Section:** Single text field filter matching Name, Phone, or Email targets.
* **Form Layout:** N/A (Detail entry uses a modal popup containing fields for Names, Phone, and Email details).
* **Table Layout:** Clean lists tracking Guardian Name, Phone number, Email address, and inline actions. Actions contain Edit `(E)`, Delete `(D)`, and Link Relationship mapping triggers `(L)`.
* **Buttons:**
  * `[ + ADD GUARDIAN ]`: Top-right primary action to open creation layout modals.
* **Mobile Responsive Behavior:** Search input expands to `100%` width. Roster table hides the email column, showing only Name, Phone, and action items.

---

## 6. StaffDirectory

### ASCII Wireframe
```text
+-------------------------------------------------------------------------------------------------+
| [LOGO] EduPulse School ERP        Academic Year: [ 2026-2027 v ]              (Profile) (Logout)|
+-------------------------------------------------------------------------------------------------+
|  Navigation       |                                                                             |
|  [D] Dashboard    |  Staff Directory                                    [ + ADD STAFF MEMBER ]  |
|  [A] Academics v  |                                                                             |
|    - Years        |  +-----------------------------------------------------------------------+  |
|    - Classes      |  | Filters:                                                              |  |
|    - Sections     |  | Search Staff: [ Smith         ]  Status: [ Active v ]                 |  |
|  [S] Staff (*)    |  +-----------------------------------------------------------------------+  |
|  [S] Students     |                                                                             |
|  [G] Guardians    |  Active Staff Roster                                                         |
|  [A] Attendance   |  +-----------------------------------------------------------------------+  |
|                   |  | Code    | Full Name         | Designation   | Phone      | Status     |  |
|                   |  |---------|-------------------|---------------|------------|------------|  |
|                   |  | STF-012 | Robert Smith      | Principal     | 555-0300   | Active (E) |  |
|                   |  | STF-015 | Jane Alcott       | Math Teacher  | 555-0305   | Active (E) |  |
|                   |  +-----------------------------------------------------------------------+  |
|                   |                                                        Page: [ < ] 1 [ > ]  |
+-------------------+-----------------------------------------------------------------------------+
```

### Layout Specifications
* **Header & Sidebar Layout:** Standard master dashboard shell.
* **Filters Section:** Grid filters tracking Code/Name searches alongside Active status toggles.
* **Form Layout:** Detail create form includes: EmployeeCode, FirstName, LastName, Phone, Designation, and user account assignments.
* **Table Layout:** Displays Code, Full Name, Designation, Phone, Status badge, and Edit actions.
* **Buttons:**
  * `[ + ADD STAFF MEMBER ]`: Triggers staff registration page.
* **Mobile Responsive Behavior:** Table scales by wrapping values or hiding phone numbers, keeping Code, Name, and Status options clear on viewport sizes `<768px`.

---

## 7. DailyAttendanceSheet

### ASCII Wireframe
```text
+-------------------------------------------------------------------------------------------------+
| [LOGO] EduPulse School ERP        Academic Year: [ 2026-2027 v ]              (Profile) (Logout)|
+-------------------------------------------------------------------------------------------------+
|  Navigation       |                                                                             |
|  [D] Dashboard    |  Daily Attendance Sheet                                                     |
|  [A] Academics v  |                                                                             |
|    - Years        |  +-----------------------------------------------------------------------+  |
|    - Classes      |  | Date:  [ 2026-06-18 v ]  Class:  [ Class 10 v ]  Section: [ Sec A v ]  |  |
|    - Sections     |  |                                                       [ LOAD ROSTER ] |  |
|  [S] Staff        |  +-----------------------------------------------------------------------+  |
|  [S] Students     |                                                                             |
|  [G] Guardians    |  Class Roster (Date: June 18, 2026)                                         |
|  [A] Attendance(*)|  +-----------------------------------------------------------------------+  |
|                   |  | Adm. No  | Student Name    | Attendance Status        | Remarks       |  |
|                   |  |----------|-----------------|--------------------------|---------------|  |
|                   |  | ADM-102  | John Doe        | (P) [A] [L] [H]          | [           ] |  |
|                   |  | ADM-103  | Jane Smith      | [P] (A) [L] [H]          | [ Sick leave] |  |
|                   |  +-----------------------------------------------------------------------+  |
|                   |  [   SUBMIT ATTENDANCE SHEET   ]                      [ RESET SHEET ]        |
+-------------------+-----------------------------------------------------------------------------+
```

### Layout Specifications
* **Header & Sidebar Layout:** Standard master dashboard shell.
* **Filters Section:** Input date selector, Class standard choice, Section division, and a prominent `[ LOAD ROSTER ]` action button.
* **Form Layout:** Inlined in the table rows for mass data updates.
* **Table Layout:**
  * Columns: Admission No, Student Name, Attendance Status, and Remarks.
  * Status options are rendered as a custom radio button group (P: Present, A: Absent, L: Late, H: HalfDay) to ensure fast keyboard/mouse navigation.
  * Remarks is an inline text input field.
* **Buttons:**
  * `[ LOAD ROSTER ]`: Query roster for chosen criteria.
  * `[ SUBMIT ATTENDANCE SHEET ]`: Primary action triggering the bulk `POST /api/attendance/mark` payload.
  * `[ RESET SHEET ]`: Clears input forms.
* **Mobile Responsive Behavior:** 
  * Select elements stack vertically.
  * Student rows convert to cards on mobile layouts.
  * Each student card displays the Student Name, a horizontal button selector group `[P] [A] [L] [H]`, and an inline comment textbox wrapper underneath.
