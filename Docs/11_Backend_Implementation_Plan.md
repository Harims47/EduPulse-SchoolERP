# .NET 8 & Dapper Backend Implementation Plan: EduPulse AI

**Project Name:** EduPulse AI  
**Role Perspective:** Senior .NET 8 Solution Architect & Dapper Expert  
**Target Delivery Window:** 4 to 6 Weeks  
**Architectural Goal:** Solo-developer speed, clean separation of concerns, zero over-engineering, type safety, and explicit tenant boundary security checks.

---

## 1. Solution Structure

To maximize compilation speed and ease of testing, the backend follows a simple **2-Project Solution** model. This avoids Clean Architecture multi-layer overhead while preserving the separation between presentation (API) and business/data access layers (Core).

```
EduPulseSaaS/
│
├── EduPulseSaaS.sln                 # Solution root
│
├── src/
│   ├── EduPulse.Core/               # Domain, DTOs, Business, Migrations, Data Access (Dapper)
│   │   └── EduPulse.Core.csproj
│   │
│   └── EduPulse.Api/                # Web API: Controllers, Middlewares, DI configuration, Program.cs
│       └── EduPulse.Api.csproj
│
└── tests/
    └── EduPulse.Tests/              # Unit and integration validation tests
        └── EduPulse.Tests.csproj
```

---

## 2. Folder Structure

### 2.1. `EduPulse.Core` (Data & Logic)
```
EduPulse.Core/
├── Common/
│   ├── Interfaces/                  # Shared services (ITenantContext, IDateTime)
│   └── Security/                    # Encryption helpers (AES-256 for Government IDs)
├── Database/
│   ├── Connection/                  # SqlConnectionFactory interfaces & implementations
│   ├── Migrations/                  # SQL DDL script files run by DbUp (Embedded Resources)
│   └── Seed/                        # Seeding SQL script files
├── Domain/
│   └── Entities/                    # Database-mapped POCO models (Student, Invoice, User)
├── Dtos/
│   ├── Auth/                        # Request/Response models for identity
│   ├── Common/                      # Paginated request/response shapes
│   ├── SIS/                         # Student & Parent data exchange contracts
│   └── Finance/                     # Invoice & receipt collection records
├── Repositories/
│   ├── Interfaces/                  # Repository contracts (IStudentRepository)
│   └── Implementations/             # Dapper implementations using explicit SQL strings
├── Services/
│   ├── Interfaces/                  # Business services contracts (IInvoiceService)
│   └── Implementations/             # Business operations, validation execution, and transactions
└── Validation/                      # FluentValidation classes for incoming DTO validation
```

### 2.2. `EduPulse.Api` (Presentation & Infrastructure)
```
EduPulse.Api/
├── Controllers/
│   ├── AuthController.cs            # Login and token issuance
│   ├── AcademicController.cs        # Years, classes, sections, and staff setup
│   ├── StudentController.cs         # SIS, search, and bulk Excel imports
│   ├── AttendanceController.cs      # Roster fetch and attendance registers
│   ├── FinanceController.cs         # Billing setup, cashier collect, and voids
│   └── ReportController.cs          # Defaulter list and absentee directories
├── Infrastructure/
│   ├── Authentication/              # JWT validation extensions & claims constants
│   ├── Middlewares/                 # GlobalExceptionHandler, TenantResolverMiddleware
│   └── ModelBinders/                # Decimal formatting settings
├── Uploads/                         # Mounted Docker volume path (student photos/documents)
├── Program.cs                       # App bootstrapping, middleware registration, DI setup
└── appsettings.json                 # DB strings, JWT secrets, environment variables
```

---

## 3. Project Configuration & Dependencies

The project uses minimal third-party libraries, selecting only high-performance, industry-standard packages to avoid external dependency bottlenecks.

### `EduPulse.Core.csproj` Packages:
* `Microsoft.Data.SqlClient` (Core database driver)
* `Dapper` (Micro-ORM for direct SQL execution)
* `dbup-sqlserver` (Embedded SQL database migration framework)
* `FluentValidation.DependencyInjectionExtensions` (Clean request validation)
* `BCrypt.Net-Next` (Secure hashing for user credentials)

### `EduPulse.Api.csproj` Packages:
* `Microsoft.AspNetCore.Authentication.JwtBearer` (Token-based authentication validation)
* `ExcelDataReader` (Fast, zero-dependency Excel stream reader for bulk roster imports)
* `Serilog.AspNetCore` (Structured logger writing to Console)

---

## 4. Authentication Flow

Authentication uses JWT bearer tokens. Roles are stored in database security claims and validated on every API call.

```
┌──────────┐              Credentials             ┌──────────┐
│  Client  ├─────────────────────────────────────►│  Server  │
│          │◄─────────────────────────────────────┤ (API)    │
│          │        JWT Token containing:         └──────────┘
│          │        - TenantId Claim
│          │        - Role Claims (Admin/Accountant)
│          │        - UserId Claim
└──────────┘
```

1. **Exchange:** Client posts credentials to `/api/auth/login`.
2. **Evaluation:** Backend queries `dbo.Users` (filtering by Email), retrieves the record, and validates the password using `BCrypt.Verify()`.
3. **Generation:** API generates a JWT token containing claims for:
   * `TenantId` (UUID)
   * `UserId` (UUID)
   * `Role` (User roles mapping, e.g. `SchoolAdmin` or `Accountant`)
4. **Validation:** Client attaches the token to subsequent HTTP request headers: `Authorization: Bearer <Token>`.
5. **Authorization:** API endpoints use standard ASP.NET attributes to enforce access limits:
   * `[Authorize(Roles = "SchoolAdmin")]`
   * `[Authorize(Roles = "SchoolAdmin, Accountant")]`

---

## 5. Tenant Resolution Strategy

Since Row-Level Security (RLS) is removed from the database engine, **tenant isolation is enforced at the application level**. All queries must explicitly bind a `TenantId` parameter.

### 5.1. The `ITenantContext` Scoped Contract
Create a scoped interface `ITenantContext` to hold tenant parameters retrieved from HTTP requests.
* **Properties:** `Guid TenantId { get; }`, `Guid UserId { get; }`.
* **Registration:** Registered as `builder.Services.AddScoped<ITenantContext, TenantContext>()`.

### 5.2. `TenantResolverMiddleware` Execution
A custom HTTP middleware runs after authentication:
1. Validates if the authenticated HTTP User principal contains a `TenantId` claim.
2. If the user is authenticated, it parses this claim and injects it into the scoped `ITenantContext` instance.
3. If the request targets a public page (such as prospective parent admissions registrations), it checks for the `X-Tenant-Id` header to resolve the context.
4. If no Tenant ID can be resolved for secure routes, it blocks the request, returning an HTTP `401 Unauthorized` status.

### 5.3. Connection Management & Factory
To prevent connection leaks and simplify Dapper instantiation:
* Define a scoped `IDbConnectionFactory` interface.
* The factory exposes a method: `IDbConnection CreateConnection()`.
* It retrieves the connection string from configuration and returns an active `SqlConnection`.
* The repository consumes this factory within `using` statements, ensuring connections are closed immediately after query execution.

---

## 6. Repository Design (Enforcing Explicit Tenant Filters)

Every database query must explicitly filter by the client's `TenantId` using parameters. This is verified using the following design guidelines:

### 6.1. Explicit Parameter Mapping
Repositories inject `IDbConnectionFactory` and `ITenantContext`. Every repository query passes `@TenantId` from the context:
```csharp
// Example conceptual pattern within Repositories:
var sql = "SELECT * FROM dbo.Students WHERE TenantId = @TenantId AND IsDeleted = 0;";
using var connection = _connectionFactory.CreateConnection();
return await connection.QueryAsync<Student>(sql, new { TenantId = _tenantContext.TenantId });
```

### 6.2. Soft-Delete Exclusion
Every repository reading data must explicitly check the soft-delete flag: `WHERE TenantId = @TenantId AND IsDeleted = 0` to filter out soft-deleted records.

### 6.3. Audit Column Injection
For write operations (`INSERT`, `UPDATE`), the repository or service layer automatically injects:
* `CreatedOn` = `DateTime.UtcNow`
* `CreatedByUserId` = `_tenantContext.UserId`
* `ModifiedOn` = `DateTime.UtcNow`
* `ModifiedByUserId` = `_tenantContext.UserId`

---

## 7. Service Layer & Transaction Design

The service layer implements business rules, executes validations, and defines database transaction boundaries.

### 7.1. FluentValidation Checks
API controllers do not validate models. Instead, controllers pass DTOs directly to service methods. The service executes validation before hitting repositories, throwing standard validation exceptions that are caught by the global exception handler middleware.

### 7.2. Transaction Scope Management
For operations modifying multiple tables (such as bulk student Excel imports, generating invoice line items, or collecting fee receipts), services wrap database calls in transaction scopes.
* **Implementation:** The service opens a connection, starts an `IDbTransaction`, and passes both the connection and transaction references to repository methods.
* **Error Handling:** If any operation fails, the transaction is rolled back, preventing orphaned records.
```csharp
// Transaction pattern in services:
using var connection = _connectionFactory.CreateConnection();
connection.Open();
using var transaction = connection.BeginTransaction();
try
{
    // Pass connection & transaction references to repository methods
    await _invoiceRepository.InsertAsync(invoice, connection, transaction);
    await _invoiceRepository.InsertDetailsAsync(details, connection, transaction);
    
    transaction.Commit();
}
catch
{
    transaction.Rollback();
    throw;
}
```

---

## 8. DTO Design

Request and response contracts are decoupled from database entities to prevent API contract breakage during database schema changes.
* **C# Record Types:** Use immutable C# `record` types for all DTO definitions. This ensures clean model serialization.
* **Type Constraints:** Government IDs are mapped using separate DTO parameters (`GovernmentIdType`, `GovernmentIdNumber`) and encrypted via AES-256 in the service layer before database insertion.
* **Manual Mapping:** Avoid slow reflection mappers. Map DTOs to entities manually inside service extensions to keep compilation times fast and queries transparent.

---

## 9. API Implementation Order (FDP Alignment)

Backend routes are developed sequentially to align with the frontend release plan:

* **Sprint 1 (Weeks 1-2): Identity & Infrastructure**
  * `POST /api/auth/login`
  * `GET /api/settings` & `PUT /api/settings`
  * `GET /api/staff` & `POST /api/staff` & `PUT /api/staff/{id}` (Simple employee records setup)
* **Sprint 2 (Weeks 3-4): Academics & Roster**
  * `GET /api/academic-years` & `POST /api/academic-years`
  * `GET /api/classes` & `POST /api/classes`
  * `GET /api/class-sections` & `POST /api/class-sections`
  * `POST /api/students/import` (Fast Excel parser using `ExcelDataReader` to bulk insert rosters)
  * `GET /api/students/search` & `GET /api/students/{id}` & `PUT /api/students/{id}`
  * `GET /api/attendance/roster` & `POST /api/attendance/submit` & `POST /api/attendance/unlock` (Daily log registers)
* **Sprint 3 (Weeks 5-6): Finance, Collections, & Reports**
  * `GET /api/fees/line-items` & `POST /api/fees/line-items`
  * `POST /api/invoices/generate-batch` (Invoicing engine)
  * `GET /api/invoices` & `GET /api/invoices/{id}`
  * `POST /api/receipts/collect` (FIFO algorithm)
  * `POST /api/receipts/{id}/cancel` (Voiding)
  * `GET /api/reports/fee-defaulters` (Outstanding dues listing)

---

## 10. Database Migration Strategy (DbUp)

To enable zero-touch cloud deployments, the database migration scripts are managed within the API project and executed automatically on startup.

### 10.1. Embedded Scripts
SQL scripts containing the DDL tables definitions, indexes, and seeds are stored as `.sql` files in the `EduPulse.Core` project. Set their Build Action to **Embedded Resource**.

### 10.2. DbUp Runner Configuration
On application start (configured inside `Program.cs`), DbUp checks the target SQL database:
1. If the database does not exist, DbUp creates it.
2. It checks for a metadata table `dbo.SchemaVersions` (automatically created by DbUp) to identify which scripts have run.
3. It executes new migration scripts in alphabetical/numerical sequence inside database transaction blocks.
4. If a script fails, execution blocks, the transaction rolls back, and application startup aborts.

### 10.3. Execution Safety
* **Idempotency:** Write DDL scripts to run only if the target object does not exist (e.g., using `IF NOT EXISTS` or standard table-existence checks).
* **Environment Guard:** Seeding lookup scripts (like test tenant accounts) are wrapped in conditions so they only run in non-production environments.
