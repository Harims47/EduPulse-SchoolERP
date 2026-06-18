# Sprint 01 Task Breakdown: Core Foundations (1 Week)

**Project Name:** EduPulse AI  
**Role Perspective:** Senior .NET 8 Technical Lead, React Technical Lead, SQL Server Architect, SaaS Architect, and Agile Project Manager  
**Sprint Goal:** Scaffold the database schema, configure JWT authentication, establish the tenant context middleware, set up the academic registries (Years, Classes, Sections, Staff simple CRUD), and build the administrative dashboard shell.  
**Developer Capacity:** Solo Developer | 28 Productive Hours (Target Window: 5 Days).

---

## 1. Sprint Objective

Build and verify the end-to-end multi-tenant skeleton. By the end of this sprint, an administrator must be able to log in, see their specific school dashboard metrics (mocked collections), view the staff roster, and configure the academic year, classes, and sections. Every API request must be secure and isolated within the requesting user's `TenantId` boundary.

---

## 2. Sprint Deliverables

* **Database Schema:** 10 core tables created (`Tenants`, `TenantSettings`, `Users`, `Roles`, `UserRoles`, `AcademicYears`, `Classes`, `Sections`, `ClassSections`, `Staff`) with default lookup seeds.
* **Authentication:** JWT secure exchange mapping roles (`SchoolAdmin`, `Accountant`) and the authenticated `TenantId` context.
* **Backend API:** Scaffolding complete with `DbUp` migration runner, `TenantResolverMiddleware` context injection, and REST endpoints for Auth, Academic Years, Classes, Sections, Staff, and Admin Dashboard KPI calculations.
* **React SPA:** Vite container with Redux Toolkit auth slice, React Query configuration, responsive Sidebar menu layouts (`AdminLayout`), and view panels for Login, Dashboard, Academic Year Management, Classes, Sections, and Staff registries.

---

## 3. Database Tasks (Est: 3 Hours)

Tasks are grouped into 2-hour implementation blocks.

### Task DB-1: Database Creation, Schemas & Table Initializations
* **Description:** Execute DDL migrations for P0 tables (`Tenants`, `TenantSettings`, `Users`, `Roles`, `UserRoles`, `AcademicYears`, `Classes`, `Sections`, `ClassSections`, `Staff`) in order of dependencies.
* **Estimated Hours:** 2 Hours
* **Dependencies:** None

### Task DB-2: Constraints, Indexes & Seed Data
* **Description:** Create foreign keys, non-clustered indexes for email and search queries, and seed roles (`SchoolAdmin`, `Accountant`) along with a pilot tenant and default admin logins.
* **Estimated Hours:** 1 Hour
* **Dependencies:** Task DB-1

---

## 4. Backend Tasks (Est: 10 Hours)

### Task BE-1: Project Scaffolding & DbUp Migration Setup
* **Description:** Scaffold the `.sln` and two projects (`EduPulse.Api` and `EduPulse.Core`). Embed DB scripts as resources, configure DbUp inside `Program.cs` to execute DDL migrations on application startup.
* **Estimated Hours:** 2 Hours
* **Dependencies:** Task DB-2

### Task BE-2: JWT Authentication & User Login Service
* **Description:** Build `/api/auth/login` endpoint. It queries `dbo.Users` using Dapper, verifies hashed credentials via BCrypt, and returns a signed JWT token containing claims for `TenantId`, `UserId`, and `Roles`.
* **Estimated Hours:** 2 Hours
* **Dependencies:** Task BE-1

### Task BE-3: TenantContext Resolver Middleware
* **Description:** Implement `ITenantContext` (scoped) and `TenantResolverMiddleware`. The middleware parses the JWT token, extracts `TenantId`/`UserId` claims, and injects them into the context, allowing repositories to query data using explicit parameter filters.
* **Estimated Hours:** 1.5 Hours
* **Dependencies:** Task BE-2

### Task BE-4: Academic Registries APIs (Years, Classes, Sections)
* **Description:** Develop REST endpoints (GET/POST) for academic years, classes, and sections. Every query must explicitly filter: `WHERE TenantId = @TenantId AND IsDeleted = 0`.
* **Estimated Hours:** 2 Hours
* **Dependencies:** Task BE-3

### Task BE-5: Staff CRUD APIs
* **Description:** Implement basic CRUD routes for the Staff registry. Integrates soft-deleting (`IsDeleted = 1`) and standard audit fields mapping (`CreatedByUserId`, `ModifiedByUserId`).
* **Estimated Hours:** 1.5 Hours
* **Dependencies:** Task BE-3

### Task BE-6: Admin Dashboard API (Mocked KPIs)
* **Description:** Implement `GET /api/dashboard/admin` to return summary counters (Student count, collection totals, outstanding dues). For Sprint 1, collections and dues values are returned as mock data parameters.
* **Estimated Hours:** 1 Hour
* **Dependencies:** Task BE-3

---

## 5. Frontend Tasks (Est: 11 Hours)

### Task FE-1: Vite Setup, Router & Redux Configuration
* **Description:** Scaffold Vite container. Install Bootstrap 5, Axios, React Router v6, Redux Toolkit, React Hook Form, and Yup. Setup the global Redux store (`authSlice`) and configure React Query client hooks.
* **Estimated Hours:** 2 Hours
* **Dependencies:** None

### Task FE-2: Login Screen & Route Protection Guards
* **Description:** Build `LoginScreen` using React Hook Form + Yup. Create `ProtectedRoute` guard wrapper to validate token properties and roles before routing.
* **Estimated Hours:** 2 Hours
* **Dependencies:** Task FE-1

### Task FE-3: AdminLayout & Navigation Sidebars
* **Description:** Construct the responsive `AdminLayout` panel. Implement collapsing sidebar navigation links and header displaying active academic year and tenant brand logo.
* **Estimated Hours:** 1.5 Hours
* **Dependencies:** Task FE-2

### Task FE-4: Academic Years & Class Config Screens
* **Description:** Implement `AcademicYearManager` and `ClassSectionConfigurator` pages. Fetch details using custom React Query hooks (`useAcademicYears`, `useClasses`).
* **Estimated Hours:** 2.5 Hours
* **Dependencies:** Task FE-3

### Task FE-5: Staff Directory Roster
* **Description:** Create the `StaffDirectory` page. Implements simple list grid and editing modal using React Hook Form and Yup validation schema.
* **Estimated Hours:** 1.5 Hours
* **Dependencies:** Task FE-3

### Task FE-6: Admin Dashboard Screen View
* **Description:** Implement the `AdminDashboard` landing page, rendering collection and outstanding metrics widgets.
* **Estimated Hours:** 1.5 Hours
* **Dependencies:** Task FE-3

---

## 6. Testing Tasks (Est: 4 Hours)

* **Task QA-1: Database Isolation & DDL Validation (1 Hour):** Verify script executions. Manually check that table relationships match indexes, and run direct queries to confirm soft-delete and unique constraints block invalid inserts.
* **Task QA-2: Postman API Integration Check (1.5 Hours):** Create a Postman collection. Test login endpoints, verify JWT payload outputs, and confirm requests to registry APIs fail with HTTP 401 when missing headers or presenting expired tokens.
* **Task QA-3: End-to-End Login & UI View Check (1.5 Hours):** Run the complete flow. Log in via `LoginScreen`, check Redux and localStorage variables, confirm routing redirects correctly, configure an academic year, and verify the class grid updates dynamically.

---

## 7. Daily Execution Plan

```
┌─────────────────────────────────────────────────────────────────────────┐
│ DAY 1: DDL Scripts, Core Scaffolding, and Redux Setup (W1 Start)        │
├─────────────────────────────────────────────────────────────────────────┤
│ DAY 2: Token Issuance, Tenant Middlewares, and Login Views              │
├─────────────────────────────────────────────────────────────────────────┤
│ DAY 3: Academic Years, Classes, Sections APIs, and Sidebar Layout       │
├─────────────────────────────────────────────────────────────────────────┤
│ DAY 4: Staff CRUD APIs, Staff Directory, and Section Configuration      │
├─────────────────────────────────────────────────────────────────────────┤
│ DAY 5: Dashboard Analytics, End-to-End Testing, and Git Sync (W1 Close) │
└─────────────────────────────────────────────────────────────────────────┘
```

* **Day 1: Scaffold & Schema**
  * *DB:* Complete Task DB-1 (run DDL schema tables).
  * *BE:* Scaffold .NET Solution and setup DbUp (Task BE-1).
  * *FE:* Scaffold Vite React app and setup Redux RTK store (Task FE-1).
  * *Day 1 Goal:* Compilation verified on both projects; database migrations execute on application startup.
* **Day 2: Secure Identity**
  * *BE:* Complete JWT token generation and authentication endpoints (Task BE-2, Task BE-3).
  * *FE:* Build Login form UI and Protected Route boundaries (Task FE-2).
  * *Day 2 Goal:* Able to log in as seed administrator; token holds TenantId claims, and invalid paths redirect to Login.
* **Day 3: Registries & Navigation**
  * *BE:* Implement academic structures routes (Task BE-4).
  * *FE:* Build Admin navigation layouts and dropdown caches (Task FE-3, Task FE-4).
  * *Day 3 Goal:* Admin sidebar displays correctly; class standards can be listed and saved in the database.
* **Day 4: Staff & Class Section Mappings**
  * *BE:* Implement Staff CRUD endpoints (Task BE-5).
  * *FE:* Build Staff Directory list and Class-Section assignment selectors (Task FE-4, Task FE-5).
  * *Day 4 Goal:* Class sections mapped to academic years, sections, and staff teachers with validation rules.
* **Day 5: Dashboard & Validation**
  * *BE:* Build dashboard counters endpoint (Task BE-6).
  * *FE:* Complete Dashboard view cards and Run QA tests (Task FE-6, QA Tasks).
  * *Day 5 Goal:* Full integration test passes. Commit all code and prepare for pilot data onboarding.

---

## 8. Definition of Done (DoD)

### Database:
* All DDL migrations execute through DbUp without syntax errors.
* Core constraints (foreign keys, check limits) are enforced.
* Non-clustered indexes are validated for all foreign keys.
* Default lookup roles and seed tenant are populated.

### Backend:
* APIs compile under .NET 8 without warnings.
* All routes are protected by JWT auth filters, except `/api/auth/login`.
* Every database call explicitly uses parameterized query contexts to bind `@TenantId`.
* Global error handling middleware catches database dropouts and returns formatted JSON.

### Frontend:
* React 19 app compiles cleanly (no warnings or console errors).
* JWT is persisted across browser refreshes in `localStorage`.
* React Query handles cache invalidation after mutator submissions.
* React Hook Form + Yup blocks submit actions if validation errors exist.

### Testing:
* Postman validation passes with zero test script failures.
* Cross-tenant injection checks verify data boundaries (Tenant B cannot read Tenant A records).
* Responsive UI tested on desktop and mobile viewports.

---

## 9. Git Commit Plan

To ensure clean rollback paths for a solo developer, commits must represent single logical increments:

1. `feat: chore - scaffold net8 backend and vite react frontend container`
2. `feat: db - script migration tables setup and configure dbup runner`
3. `feat: auth - implement jwt token issuer api and auth slices in react`
4. `feat: middleware - build tenant resolver middleware and connection context`
5. `feat: views - create login screen and protected routing guards`
6. `feat: layout - build dashboard sidebar layouts and navigation outlets`
7. `feat: academics - implement academic years, classes, and sections crud apis`
8. `feat: staff - build staff simple crud endpoints and directory views`
9. `feat: dashboard - implement admin stats widgets and daily attendance sheets`
10. `test: qa - complete integration tests and postman collection checklist`

---

## 10. AI Prompt Plan

Use these copy-paste prompts inside Claude or Cursor for rapid code generation.

### 10.1. Authentication & Tenant Resolve Module
* **Claude Prompt:**
  ```
  Act as a Senior .NET Architect. Write a Program.cs setup, a JWT Token Generation service, and a custom TenantResolverMiddleware for a .NET 8 Web API. 
  The middleware must read the 'TenantId' and 'UserId' claims from the JWT Authorization header and populate a scoped ITenantContext object (holding Guid TenantId and Guid UserId). 
  Write the custom IDbConnectionFactory using Microsoft.Data.SqlClient that repositories can use to instantiate connection blocks. Do not write database logic, generate configuration code and middlewares only.
  ```
* **Antigravity Prompt:**
  ```
  Review the tables Users, Roles, and UserRoles in 10_SQL_Server_Implementation.md. Write a SQL migration script for DbUp that creates these tables, defines foreign keys with ON DELETE NO ACTION, sets up indexes on email lookups, and seeds the system roles ('SchoolAdmin', 'Accountant') along with a default pilot school tenant user.
  ```
* **Cursor Usage Strategy:**
  1. Open `Program.cs` and `ITenantContext.cs`.
  2. Use Cursor Composer (`Ctrl + I` or `Cmd + I`) to request: *"Implement the TenantResolverMiddleware and register ITenantContext and the SqlConnectionFactory in Program.cs following 11_Backend_Implementation_Plan.md."*
  3. Validate using the built-in terminal: Run `dotnet build` to confirm compilation.

### 10.2. Academic Year & Classes Module
* **Claude Prompt:**
  ```
  Act as a Senior .NET Lead and Dapper Expert. Write the repository and service classes for the AcademicYears and Classes modules. 
  Every Dapper query MUST explicitly bind the Current Tenant ID using parameters (e.g. WHERE TenantId = @TenantId AND IsDeleted = 0). 
  Write the HTTP GET and POST controller endpoints, using FluentValidation rules to check that academic years end-dates occur after start-dates, and class sort orders are non-negative.
  ```
* **Cursor Usage Strategy:**
  1. Open the folders `/Repositories` and `/Services` in `EduPulse.Core`.
  2. Highlight the `ITenantContext` definition and use `Cmd + K` inside the new repository files: *"Create AcademicYearRepository implementing IAcademicYearRepository. Access IDbConnectionFactory and bind @TenantId parameters explicitly on all queries."*

### 10.3. Daily Attendance Module
* **Claude Prompt:**
  ```
  Act as a React Frontend Architect. Build a DailyAttendanceSheet component using React 19, React Query, and Bootstrap 5. 
  The component must fetch a class section roster grid using a custom React Query hook. 
  It must render students in a table showing their roll number, name, and large touch-friendly button options for attendance status (P, A, L, T). 
  When a status button is tapped, it must trigger a mutation using React Query, updating the attendance logs on the backend. Integrate Bootstrap alert banners for warning validations. Do not use external gesture packages.
  ```
* **Cursor Usage Strategy:**
  1. Create a component `DailyAttendanceSheet.jsx` in the frontend `/views` directory.
  2. In Cursor, select the component and press `Cmd + K`: *"Implement a daily attendance matrix grid using Bootstrap table utilities, large click buttons for P/A/L/T status, and TanStack React Query mutations."*
  3. Verify rendering using Vite dev server console logs.
