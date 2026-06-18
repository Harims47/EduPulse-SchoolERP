# Solution Architecture Document: AI-First School Management SaaS
**Project Name:** EduPulse AI  
**Role Perspective:** Startup CTO & Solution Architect  
**Core Technologies:** React | Redux Toolkit | Bootstrap | .NET 8 Web API | Dapper | SQL Server  
**Architecture Classification:** Lean Monolithic Web Architecture (Solo-Developer & 3-Month Launch Optimized)  

---

## Part 1: Startup CTO Architecture Audit & Simplifications

To launch a stable product and sign the first paying school within **90 days** as a solo developer, we must aggressively purge architectural noise. This review simplifies the previously designed system to support 1 to 5 schools (~1,500 to 10,000 total students) with near-zero hosting costs and minimal DevOps management overhead.

```
       ┌────────────────────────────────────────────────────────┐
       │              INFRASTRUCTURE AUDIT SUMMARY              │
       └────────────────────────────────────────────────────────┘
       [ Python AI Service ]  ──► REMOVE COMPLETELY (Defer all AI to v1.5+)
       [ Redis Queue ]        ──► REMOVE (Replace with .NET 8 In-Memory Channels)
       [ API Gateway ]        ──► REMOVE COMPLETELY (Direct Client-to-API calls)
       [ Presigned URL S3 ]   ──► SIMPLIFY (Direct Multipart API Form Uploads)
       [ Multi-Server ECS ]   ──► SIMPLIFY (Single VM Docker Compose / Azure App)
```

### 1.1. Specific Component Evaluations

| Component | Audit Decision | PM & CTO Rationale | Lean Alternative for MVP |
| :--- | :---: | :--- | :--- |
| **Python AI Microservice** | **Defer to v1.5+** | Running, hosting, and deploying a separate Python machine learning stack is a massive time sink. AI features are not core blockers for a school to purchase the MVP. | Defer OCR scanning and timetable solvers. For marketing elements (like AI lesson planners), call the OpenAI C# API directly from the .NET 8 API. |
| **Redis Cache & Message Broker** | **Remove for MVP** | Setting up, securing, and maintaining a Redis cluster is over-engineered. The load from 5 schools does not warrant distributed caching or queue brokers. | Replace with .NET 8 `System.Threading.Channels` (in-memory queues) and standard ASP.NET memory cache. |
| **API Gateway** | **Remove Completely** | Introduces extra network hops, certificate management overhead, and configuration friction. | Direct HTTPS communication between the React SPA and the .NET Web API. Manage CORS directly in .NET middleware. |
| **Complex Presigned S3/Blob Uploads** | **Simplify** | Generating presigned upload tokens and matching client-side S3 libraries requires complex error handling and storage configurations. | Direct multipart form uploads from React to a .NET API endpoint, saving files directly to local VM disk storage (Docker Volume) or using a direct .NET SDK write. |
| **AWS ECS / Azure App Service Scale** | **Simplify** | Multi-container ECS Fargate tasks, Application Load Balancers, and private subnets require high DevOps maintenance and cost up to $150+/month. | Deploy React to **Vercel/Netlify** (Free/Pro Tier) and host .NET 8 API + SQL Server on a single **$12/month AWS Lightsail or Azure VM** using simple Docker Compose. |

---

## Part 2: Revised MVP Architecture

The simplified architecture consolidates components into a single stateless API container and a relational SQL database. The frontend is fully decoupled and served directly via CDN.

```
┌────────────────────────────────┐
│      Vercel CDN Host           │
│  - React Single Page App (SPA)  │
│  - Redux State / Bootstrap UI  │
└───────────────┬────────────────┘
                │
                │ HTTPS (TLS 1.3 Direct REST Call)
                ▼
┌──────────────────────────────────────────────────────────────┐
│                  AWS Lightsail / Azure VM                    │
│                                                              │
│  ┌────────────────────────────────────────────────────────┐  │
│  │               .NET 8 API Docker Container              │  │
│  │                                                        │  │
│  │  [ Direct Dapper Query Handlers ]                      │  │
│  │  [ In-Memory Channel Background Worker ]                │  │
│  │  [ File Controller (Writes to Mounted Volume) ]        │  │
│  │                                                        │  │
│  └────────────────────────────┬───────────────────────────┘  │
│                               │                              │
│                               ▼ Localhost TCP                │
│  ┌────────────────────────────────────────────────────────┐  │
│  │           SQL Server Express Docker Container          │  │
│  │  - Single DB Shared Schema                             │  │
│  │  - Row-Level Security Enabled                          │  │
│  └────────────────────────────────────────────────────────┘  │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

### 2.1. Multi-Tenant Strategy (Kept for MVP)
We retain the **Shared Database, Shared Schema** Row-Level Security (RLS) model because it is not over-engineered. Setting it up takes less than 2 hours and prevents major data leak security failures.

* **SQL Server Security Predicate:** Enforces tenant boundaries globally.
* **Dapper Wrapper:** The database connection interceptor populates the `SESSION_CONTEXT('TenantId')` key on every connection open. This keeps all raw SQL queries safe from data leaks:
  ```csharp
  public class TenantConnectionFactory : IDbConnectionFactory
  {
      private readonly string _connectionString;
      private readonly ITenantProvider _tenantProvider;

      public TenantConnectionFactory(string connectionString, ITenantProvider tenantProvider)
      {
          _connectionString = connectionString;
          _tenantProvider = tenantProvider;
      }

      public async Task<IDbConnection> CreateConnectionAsync()
      {
          var connection = new SqlConnection(_connectionString);
          await connection.OpenAsync();
          
          var tenantId = _tenantProvider.GetTenantId();
          if (!string.IsNullOrEmpty(tenantId))
          {
              // Inject TenantId securely into the connection session context
              using (var command = connection.CreateCommand())
              {
                  command.CommandText = "EXEC sp_set_session_context @key=N'TenantId', @value=@tenantId, @read_only=1";
                  command.Parameters.Add(new SqlParameter("@tenantId", tenantId));
                  await command.ExecuteNonQueryAsync();
              }
          }
          return connection;
      }
  }
  ```

---

## 3. Core Subsystem Architectures

### 3.1. Database Strategy
* **Engine:** SQL Server Express (Docker Container) on the VM. It is free and easily supports up to 10GB of data, which fits 5 schools for several years.
* **ORM:** Dapper micro-ORM for fast, simple data querying.
* **Migrations:** DbUp library compiled inside the .NET 8 Web API project. Migrations run automatically on application start.

### 3.2. Authentication & Authorization
* **Format:** Encrypted JWT containing User ID, Role, and Tenant ID.
* **Validation:** ASP.NET Core built-in JWT bearer authorization middleware.
* **Role Check:** Custom `.NET Action Filters` intercept incoming API requests to check permissions.

### 3.3. File Storage Strategy
* **Upload Pipeline:** React submits files via standard multipart form data (`POST /api/documents/upload`).
* **Storage Path:** The Web API writes the stream directly to a designated persistent folder on the host machine using a unique GUID filename.
* **Docker Mounting:** The storage directory is mounted as a Docker host volume to ensure files survive container restarts.
* **Scale Option:** The folder path can be swapped with a single-class S3 client wrapper in Version 1.5 without changing any business workflows.

### 3.4. Notification Strategy (In-Memory Processing)
To replace Redis while retaining non-blocking API behaviors, the system uses a native **.NET 8 Background Service** with a thread-safe `System.Threading.Channels` queue.

```csharp
// Thread-Safe In-Memory Queue Service
public interface INotificationQueue
{
    ValueTask QueueNotificationAsync(NotificationJob job);
    ValueTask<NotificationJob> DequeueAsync(CancellationToken cancellationToken);
}

public class NotificationQueue : INotificationQueue
{
    private readonly Channel<NotificationJob> _queue;

    public NotificationQueue()
    {
        // Unbounded channel for low-volume MVP needs
        _queue = Channel.CreateUnbounded<NotificationJob>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });
    }

    public async ValueTask QueueNotificationAsync(NotificationJob job)
    {
        await _queue.Writer.WriteAsync(job);
    }

    public async ValueTask<NotificationJob> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}
```

* **The Background Worker:** A C# background worker implements `IHostedService`, continuously monitoring the channel queue in a non-blocking loop and executing HTTP calls to the SMS/WhatsApp gateways.

---

## 4. Audit Logging Strategy

For the MVP, audit logs are saved directly to a SQL Server table (`AuditLogs`).
* **Trigger:** Intercepted by database utility controllers when executing key update queries.
* **Payload:** Structured JSON fields log before-and-after snapshots of modifications to prevent complex audit setup tools.

---

## 5. Security & DPDP Compliance
* **TLS Layer:** Handled by a free Let's Encrypt certificate configured on the VM using a lightweight reverse proxy (Caddy or Nginx) running in Docker.
* **Encryption at Rest:** Sensitive parameters (Aadhaar, Parent Contacts) are hashed/encrypted inside the database using standard cryptographic utilities in the C# application code.

---

## 6. Infrastructure & Deployment Architecture

### Solo Developer Docker Compose Configuration
The entire server stack is deployed on a single VM using a lightweight configuration file:

```yaml
version: '3.8'

services:
  webapi:
    image: edupulse-api:latest
    build:
      context: .
      dockerfile: src/EduPulse.WebAPI/Dockerfile
    ports:
      - "5000:8080"
    environment:
      - ConnectionStrings__DefaultConnection=Server=db;Database=EduPulseDb;User Id=sa;Password=YourSecurePassword123!;TrustServerCertificate=True;
    volumes:
      - app-uploads:/app/uploads
    depends_on:
      - db

  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - MSSQL_SA_PASSWORD=YourSecurePassword123!
    ports:
      - "1433:1433"
    volumes:
      - sql-data:/var/opt/mssql

volumes:
  app-uploads:
  sql-data:
```

### Deployment Pipeline
1. **Frontend:** React builds are committed to GitHub and automatically deployed to Vercel upon git pushes.
2. **Backend VM:** Run `docker compose down && docker compose up -d --build` on the target VM to pull, compile, and execute the latest updates.

---

## 7. Recommended Folder Structure (Unchanged)
The folder structures outlined in the previous architecture document are retained. They scale well and do not add operational friction for a solo developer.
* **Backend:** Domain, Infrastructure, WebAPI assembly separations are preserved.
* **Frontend:** Feature/Layout structures in React are kept clean and organized.
