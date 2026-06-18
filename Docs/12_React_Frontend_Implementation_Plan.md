# React Frontend Implementation Plan: EduPulse AI (Updated Stack)

**Project Name:** EduPulse AI  
**Role Perspective:** Senior React Architect & SaaS Frontend Lead  
**Target Delivery Window:** 4 to 6 Weeks  
**Architectural Goal:** Solo-developer speed, responsive Bootstrap 5 styling, clean print-friendly layouts, high-performance form state management, and optimized server-state caching.

---

## 1. Frontend Technology Stack

The project relies strictly on the following vetted frontend stack to maximize performance, maintainability, and compilation speeds while avoiding code overhead.

### Included Packages:
* **Core:** React 19 (Latest rendering features) & Vite (Rapid bundling and HMR)
* **Styling:** Bootstrap 5 (Responsive UI grid system, native classes)
* **Global Client State:** Redux Toolkit (Auth token, user parameters, layout context)
* **Server State Caching:** React Query / TanStack Query (Query caching, mutations, loading states, cache invalidation)
* **HTTP Client:** Axios (API communication, headers injection, interceptors)
* **Routing:** React Router v6 (Declarative client routing)
* **Form Management:** React Hook Form (Uncontrolled inputs, high-performance renders)
* **Validation Schema:** Yup & `@hookform/resolvers/yup` (Declarative form validation schemas)

### Excluded Packages (Prohibited):
* ❌ Material UI / Ant Design (Avoid heavy design system lock-in and layout bloat; use Vanilla Bootstrap 5 utility classes)
* ❌ Zustand (Rely on Redux Toolkit for global client state and React Query for server states)
* ❌ Redux Saga (Rely on React Query for async operations and standard Redux reducers for simple actions)
* ❌ Formik (Rely on React Hook Form for superior performance and reduced re-renders)

---

## 2. Project Scaffolding & Folder Structure

```
src/
├── assets/                  # CSS styles, brand logo SVG files
│   └── index.css            # Global CSS overrides, utility prints styling
├── components/              # Shared reusable components
│   ├── UI/                  # Buttons, Spinner loaders, Card wrappers, Alerts
│   └── Form/                # Custom React Hook Form input wrappers (TextBox, Select)
├── layouts/                 # Core wrapper structures
│   ├── LoginLayout.jsx      # Minimal, centered layout wrapper
│   ├── AdminLayout.jsx      # Navigation sidebar + active viewport outlet
│   └── PrintLayout.jsx      # Stripped-down viewport for thermal receipts / reports
├── routes/                  # Navigation configurations
│   ├── AppRoutes.jsx        # Navigation router paths mapping
│   └── ProtectedRoute.jsx   # Auth guard checking JWT roles claims
├── store/                   # Redux Toolkit (Client-side Global State)
│   ├── index.js             # Store bootstrapping config
│   └── authSlice.js         # Handles active JWT credentials & settings context
├── query/                   # React Query Configuration
│   ├── queryClient.js       # Global QueryClient instance
│   └── hooks/               # Custom query/mutation hooks (useStudents.js, useAttendance.js)
├── services/                # Network service API layers
│   ├── apiClient.js         # Base Axios client configured with interceptors
│   └── endpoints/           # Axios request promises
├── utils/                   # Shared helper functions
│   ├── formatters.js        # Date parse, Indian Rupee currency formats
│   └── schemas.js           # Yup validation schemas (studentSchema, loginSchema)
└── views/                   # Screen views mapped directly to route endpoints
```

---

## 3. State Management Strategy (Redux vs. React Query)

To keep the application highly responsive and avoid state sync bugs, we separate **Client State** from **Server State**.

```
                   FRONTEND STATE SEPARATION
 ┌───────────────────────────────────────────┐
 │                  Client State             │ ──► Managed by: REDUX TOOLKIT
 │  (Auth token, settings, sidebar toggles)  │
 └───────────────────────────────────────────┘
 ┌───────────────────────────────────────────┐
 │                  Server State             │ ──► Managed by: REACT QUERY
 │  (Student rosters, attendance grids,      │
 │   invoices list, outstanding reports)     │
 └───────────────────────────────────────────┘
```

### 3.1. Client State (Redux Toolkit)
* **Scope:** Authentication details, Active User claims, Active Academic Year selector context, Global UI Toast triggers.
* **Slice (`authSlice.js`):**
  * `token`: The active JWT bearer.
  * `user`: Profile summary parsed from claims (UserId, Email, Roles, TenantId).
  * `schoolName`, `schoolCode`, `receiptPrefix`: Basic settings context.

### 3.2. Server State (React Query)
* **Scope:** All data originating from the SQL Server database.
* **Queries (Read):** Student directory listing, daily attendance registers, invoice listings, receipt logs, defaulter rosters.
* **Mutations (Write):** Submitting daily attendance, uploading student Excel templates, generating invoice runs, collecting payments, voiding receipts.
* **Cache Lifecycle:**
  * Dropdown selections (classes, sections) are cached with `staleTime: Infinity` to prevent redundant API queries.
  * Collection entries invalidates `invoices` and `defaulters` queries (`queryClient.invalidateQueries(['invoices'])`), forcing React Query to pull updated ledger balances automatically.

---

## 4. Forms & Schema Validation (React Hook Form + Yup)

Forms (such as `StudentProfileForm`, `StaffDirectory` creation, `CashierPaymentDesk`) are built using **React Hook Form** for performance, mapping input registers directly to DOM fields.

1. **Schema Definitions:** Validation rules (e.g., student name lengths, standard Indian phone patterns, valid email patterns, and mandatory date inputs) are defined as declarative schemas in `utils/schemas.js` using **Yup**.
2. **Hook Integration:** Forms instantiate `useForm` with the Yup schema resolver:
   ```javascript
   const { register, handleSubmit, formState: { errors } } = useForm({
       resolver: yupResolver(studentSchema)
   });
   ```
3. **Bootstrap 5 UI Bindings:** Input components check for validation feedback. If `errors.fieldName` exists, they append the `.is-invalid` class to the input and display the custom validation error message inside a `.invalid-feedback` block.

---

## 5. API Layer Structure (Axios Client)

* **Configuration:** Scaffolds a single Axios client instance inside `services/apiClient.js` targeting `VITE_API_URL`.
* **Request Interceptor:** Reads JWT token from Redux store/localStorage and injects `Authorization: Bearer <token>` and `X-Tenant-Id: <TenantId>` headers.
* **Response Interceptor:**
  * Checks for `401 Unauthorized` errors. If expired, clears token state and redirects to `/login`.
  * Checks for connection drops or `500 Server Error` and fires global Redux-managed toast notifications.

---

## 6. Route Structure & Layout Mappings

Client-side routes are managed by **React Router (v6)** using nested components inside layout wrappers.

* `/login` ──► View: `LoginScreen` (uses `LoginLayout`)
* **Authenticated Views (Protected by `ProtectedRoute` guard, uses `AdminLayout`):**
  * `/dashboard` ──► View: `AdminDashboard` (KPI stats: collection metrics, active roster counters)
  * `/academics/years` ──► View: `AcademicYearManager`
  * `/academics/sections` ──► View: `ClassSectionConfigurator`
  * `/staff` ──► View: `StaffDirectory` (CRUD logs list)
  * `/students` ──► View: `StudentSearchDirectory`
  * `/students/new` ──► View: `StudentProfileForm` (React Hook Form wrapper)
  * `/students/:id` ──► View: `Student360View` (Tabbed query panel: profile, invoices, attendance log calendar)
  * `/attendance` ──► View: `DailyAttendanceSheet` (P/A/L/T checkboxes matrix)
  * `/billing/fees` ──► View: `FeeStructureConfigurator`
  * `/billing/invoices` ──► View: `InvoiceDirectory` (Invoice listing and batch billing launcher)
  * `/billing/payments` ──► View: `CashierPaymentDesk` (Dues summary sheet, cash entry form, local printer CSS styling)
  * `/billing/receipts` ──► View: `ReceiptHistoryDesk` (Collection log list and voiding modal)
  * `/reports/defaulters` ──► View: `SchoolReportsDesk` (Print-friendly collection logs with WhatsApp Web link launchers)
  * `/settings` ──► View: `TenantSettingsDashboard` (Profile config details)
* **Print View (uses `PrintLayout`):**
  * `/print/receipt/:id` ──► View: `CashierReceiptPrintSlip` (Prints thermal receipt)

---

## 7. Screen Implementation Order

Screens are built to align with the backend API sprints:

* **Week 1: Foundations & Shells**
  * `LoginScreen` (Yup validation resolver, auth Slice integration)
  * `AdminLayout` and `PrintLayout` structure configuration
  * `AdminDashboard` (Metrics cards)
* **Week 2: Configurations & Registries**
  * `AcademicYearManager` & `ClassSectionConfigurator` (Dropdown caching queries)
  * `StaffDirectory` (CRUD form modals using React Hook Form)
* **Week 3: Student Onboarding**
  * `StudentSearchDirectory` (Paginated queries directory and bulk import upload drawer)
  * `StudentProfileForm` (Address, parent contact details, and validation checks)
  * `DailyAttendanceSheet` (Roster grid with `P`, `A`, `L`, `T` touch boxes, and mutator logs)
* **Week 4: Fee Setup & Invoicing**
  * `FeeStructureConfigurator`
  * `InvoiceDirectory` (Invoice listings and batch billing trigger)
* **Week 5: Cashier workstation & Voids**
  * `CashierPaymentDesk` (Collect screen showing dues, transaction type select, and local receipt preview)
  * `ReceiptHistoryDesk` (Void payments trigger list)
* **Week 6: Reporting & GTM Hacks**
  * `SchoolReportsDesk` (Print-friendly HTML defaulters tables with native browser printing hook)
  * Integration of the WhatsApp Web `wa.me` quick launch buttons on the defaulters list.

---

## 8. Error Handling & Form Validation Strategy

1. **React Error Boundary:** Wraps the primary layouts outlet. If a rendering exception occurs, catches the crash and displays a custom Bootstrap alert: *"An unexpected rendering error occurred. Please refresh the page."*
2. **React Query Error Actions:** Custom Query Client configuration defines default behavior for queries:
   * Displays toast messages on query fetch failures.
   * Auto-retries failed query fetches up to 3 times with exponential backoff before showing error flags (does not retry on 401/403 errors).
3. **Form Errors:** React Hook Form reads validation failures mapped by Yup. Bootstrap's `.is-invalid` classes display feedback messages inline, blocking form submissions until resolved.

---

## 9. GTM Deployment & Routing Strategy

* **Build Tool:** Compiled using Vite production build commands: `npm run build`. Builds static HTML, JS, and CSS files in `/dist`.
* **Hosting:** Hosted directly on **Vercel** or Netlify.
* **Routing Fallback:** To prevent client-side routing crashes on page refreshes, a `vercel.json` file is added to the root directory, rewriting all unmatched paths back to `/index.html`:
  ```json
  {
    "rewrites": [
      { "source": "/(.*)", "destination": "/index.html" }
    ]
  }
  ```
