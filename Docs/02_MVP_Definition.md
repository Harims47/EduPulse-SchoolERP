# MVP Definition & Feature Prioritization Matrix
**Project:** EduPulse AI  
**Role Perspective:** Startup CTO & SaaS Product Manager  
**Target Constraint:** Solo Full-Stack Developer | 3-Month Launch Timeline | First Paying School Target  
**Tech Stack:** React (Mobile-Responsive Web) | .NET 8 Web API | SQL Server  

---

## Executive CTO & PM Analysis

### The 3-Month Solo Developer Reality Check
As a solo developer, your most scarce resource is **time**. In a 3-month (12-week) timeline, building a full-scale enterprise School ERP from scratch is a fast track to failure. 

To sign your **first paying school within 90 days**, we must bypass all non-essential features, avoid deep technical debt, and build *only* what the School Owner will pay for immediately. 
* **The Buyer is the Owner/Principal, not the Teacher or Student.** The features that sell the product are **Fee Security (Zero Leakage)**, **Parent Communication (WhatsApp)**, and **CBSE Compliance (Report Cards)**.
* **The Tech Stack is highly advantageous:** 
  * **.NET 8 Web API** provides enterprise-grade security, fast execution, and strict type safety (reducing runtime bugs).
  * **SQL Server** is transactional, ideal for double-entry accounting ledger entries.
  * **React** allows us to build a single responsive web application that runs on Admin Desktops and scales down to Parent/Teacher Mobile Browsers, eliminating the need to compile, test, and submit native Android/iOS apps to stores.

### The "AI-First" Positioning Trap
While we market the platform as "AI-First" to stand out, **AI is not the core MVP blocker**. A school will not buy an AI exam grader if the system cannot print a valid fee receipt or handle class attendance. 
* **MVP Rule:** Standardize and secure the operational database first.
* **AI Strategy:** Keep AI dependencies to **Zero** for the core launch, but design the SQL schema and API routes to easily plug in AI services in Version 1.5.

---

## 1. Feature Prioritization Matrix

The priority score is calculated using the following formula:
$$\text{Priority Score} = \text{Business Value (1-3)} + \text{Revenue Impact (1-3)} - \text{Development Complexity (1-3)} + 5$$
*Scale: 1 to 10 (Higher score = higher priority).*

### 1.1. Core Multi-Tenancy & Security
| Feature | Business Value | Dev Complexity | Revenue Impact | AI Dependency | Priority Score | Target Release |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: |
| **Row-Level Security (RLS) Tenant Isolation** | High (3) | Medium (2) | High (3) | None | **9/10** | **Must Have (MVP)** |
| **Super Admin Tenant Provisioning Dashboard** | Low (1) | Low (1) | Low (1) | None | **6/10** | Should Have (v1.5) |
| **RBAC (Role-Based Access Control) Engine** | High (3) | Low (1) | Medium (2) | None | **9/10** | **Must Have (MVP)** |

### 1.2. Student & Staff Information System (SIS)
| Feature | Business Value | Dev Complexity | Revenue Impact | AI Dependency | Priority Score | Target Release |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: |
| **Basic Profile Management & Doc Upload** | High (3) | Low (1) | Low (1) | None | **8/10** | **Must Have (MVP)** |
| **Bulk Excel Import (Student Roll)** | High (3) | Low (1) | Medium (2) | None | **9/10** | **Must Have (MVP)** |
| **Academic Term Promotion Engine** | Medium (2) | Medium (2) | Low (1) | None | **6/10** | Should Have (v1.5) |

### 1.3. Fee Management & Collections (The core revenue driver)
| Feature | Business Value | Dev Complexity | Revenue Impact | AI Dependency | Priority Score | Target Release |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: |
| **Flexible Fee Structure Builder** | High (3) | Medium (2) | High (3) | None | **9/10** | **Must Have (MVP)** |
| **Online Gateway Integration (Razorpay/UPI)**| High (3) | Medium (2) | High (3) | None | **9/10** | **Must Have (MVP)** |
| **Manual Receipting (Cash/Cheque Desk)** | High (3) | Medium (2) | High (3) | None | **9/10** | **Must Have (MVP)** |
| **WhatsApp/SMS Fee Alerts & Reminders** | High (3) | Low (1) | High (3) | None | **10/10** | **Must Have (MVP)** |
| **Tally ERP Ledger Export (.XML)** | Medium (2) | Medium (2) | Medium (2) | None | **7/10** | Should Have (v1.5) |
| **Predictive Collection & Attrition Dashboard**| Medium (2) | High (3) | Medium (2) | High | **6/10** | Future Roadmap |

### 1.4. Attendance & Parent Alerts
| Feature | Business Value | Dev Complexity | Revenue Impact | AI Dependency | Priority Score | Target Release |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: |
| **Teacher Web-Mobile Attendance Sheet** | High (3) | Low (1) | Medium (2) | None | **9/10** | **Must Have (MVP)** |
| **Absentee Push Notifications & WhatsApp** | High (3) | Low (1) | High (3) | None | **10/10** | **Must Have (MVP)** |
| **Biometric / RFID HW Integration** | Low (1) | High (3) | Low (1) | None | **4/10** | Future Roadmap |

### 1.5. Examination & Grading Engine
| Feature | Business Value | Dev Complexity | Revenue Impact | AI Dependency | Priority Score | Target Release |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: |
| **Marks Entry Portal for Teachers** | High (3) | Low (1) | Medium (2) | None | **9/10** | **Must Have (MVP)** |
| **CBSE Grading & PDF Report Generator** | High (3) | Medium (2) | High (3) | None | **9/10** | **Must Have (MVP)** |
| **AI OCR Handwritten Answer Sheet Scanner** | Medium (2) | High (3) | Medium (2) | High | **6/10** | Future Roadmap |

### 1.6. Parent-School Communication Hub
| Feature | Business Value | Dev Complexity | Revenue Impact | AI Dependency | Priority Score | Target Release |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: |
| **Mobile-Responsive Homework Diary** | High (3) | Low (1) | Medium (2) | None | **9/10** | **Must Have (MVP)** |
| **Broadcast Notices / Circulars** | Medium (2) | Low (1) | Medium (2) | None | **8/10** | **Must Have (MVP)** |
| **Two-way Parent-Teacher Chat** | Low (1) | High (3) | Low (1) | None | **4/10** | Nice to Have (v2) |
| **AI Conversational WhatsApp Bot** | Medium (2) | Medium (2) | Medium (2) | Medium | **7/10** | Should Have (v1.5) |

### 1.7. Academic Scheduling
| Feature | Business Value | Dev Complexity | Revenue Impact | AI Dependency | Priority Score | Target Release |
| :--- | :---: | :---: | :---: | :---: | :---: | :---: |
| **Basic Interactive Timetable View** | Medium (2) | Low (1) | Low (1) | None | **7/10** | **Must Have (MVP)** |
| **Dynamic substitution manager** | Medium (2) | Medium (2) | Low (1) | None | **6/10** | Should Have (v1.5) |
| **AI Constraint-Solver Timetable Generator**| Low (1) | High (3) | Low (1) | High | **4/10** | Future Roadmap |
| **AI Lesson Plan Drafter (GPT-4 API)** | High (3) | Low (1) | High (3) | Low | **10/10** | Should Have (v1.5) |

---

## 2. Categorized Product Release Phases

### Phase 1: Must Have (MVP) — Launch Target (Month 3)
*Focus: operational safety, compliance, cash collection, and mobile-friendly communication.*
* **SIS Core:** Basic profiles and bulk upload via Excel (essential for migrating from the first school's spreadsheets).
* **Ledger Billing & Payments:** Configurable fee allocations, manual cash/cheque entries with receipt generation, and a Razorpay API payment link.
* **Basic Communications:** Homework tracking and general circular alerts.
* **Core Academics:** Web-based teacher mark entries, automatic board grade calculations (CGPA/marks), and print-ready PDF report card downloads.
* **Attendance Engine:** Single-tap attendance logs for class teachers with automated trigger alerts via SMS/WhatsApp APIs.

### Phase 2: Should Have (Version 1.5) — Target (Month 5)
*Focus: AI-assisted automation hooks and self-service dashboards.*
* **AI Lesson Plan Drafter:** Integrate an OpenAI prompt template matching CBSE standards so teachers can draft a week's syllabus plan in seconds. Great marketing point for scaling.
* **AI WhatsApp Parent Bot:** Basic Dialogflow/OpenAI WhatsApp Bot querying database variables (e.g. fees due, holiday calendar) to save admin response times.
* **Dynamic Substitution Finder:** Algorithmic replacement suggestions when a teacher files for a leave of absence.
* **Tally ERP Synchronization:** XML invoice schema downloader for school accountants.

### Phase 3: Nice to Have (Version 2) — Target (Month 8)
*Focus: Collaboration pipelines and academic optimization.*
* **Dynamic Timetable Engine:** A web UI drag-and-drop tool that alerts for teacher clashes.
* **Two-way Live Chats:** Controlled messaging portal enabling parents to schedule virtual or in-person meetings with teachers.
* **Student Progression Engine:** Automatic class roll generation for transition cycles.

### Phase 4: Future Roadmap (Year 2+)
*Focus: Deep native hardware bindings and specialized ML model integration.*
* **OCR Written Document Scanner:** Custom computer vision pipeline to extract handwriting scores off physical sheets.
* **RFID Geofenced Transport Controls:** Native integrations with tracking hardware and dynamic re-routing calculations.
* **Financial Risk Forecast System:** Predictive models scoring probability of student dropout or defaults based on behavioral history.

---

## 3. The MVP Launch Scope Checklist

To hit your 3-month launch goal, compile your codebase with this exact boundary. Do not write a single line of code outside this list.

```
                  ┌──────────────────────────────────────────────┐
                  │              MVP SCOPE BOUNDARY              │
                  └──────────────────────┬───────────────────────┘
                                         │
       ┌─────────────────────────────────┼─────────────────────────────────┐
       ▼                                 ▼                                 ▼
┌──────────────┐                  ┌──────────────┐                  ┌──────────────┐
│  SIS CORE    │                  │ FINANCE PORTAL│                  │  ACADEMICS   │
├──────────────┤                  ├──────────────┤                  ├──────────────┤
│ - Bulk Import│                  │ - Gateway Pay│                  │ - Marks Entry│
│ - Base DB RLS│                  │ - LEDGER bills│                  │ - CBSE Grades│
│ - RBAC Access│                  │ - WA Reminders│                  │ - PDF Cards  │
└──────────────┘                  └──────────────┘                  └──────────────┘
```

### 3.1. Infrastructure & Authentication
* [ ] **Multi-Tenancy:** Implement Single Database with tenant filters in Entity Framework Core (`EF Core Query Filters` linked to the JWT `tenant_id` claim). This avoids database-per-tenant complexity for a solo dev.
* [ ] **Auth Layer:** ASP.NET Core Identity issuing JWT tokens with basic roles (`SuperAdmin`, `SchoolAdmin`, `Teacher`, `Parent`).

### 3.2. Administrative Dashboard (React Web)
* [ ] **Excel Bulk Upload:** A React page that parses an Excel sheet (Student name, Roll number, Phone, Address, Father's Name) and saves it to SQL Server in one bulk write.
* [ ] **Class Configurator:** Admin settings page defining Classes (e.g., LKG, Class 1A, Class 2B) and associated subjects.

### 3.3. Financial System
* [ ] **Billing Engine:** Set up fee profiles (e.g., Class 1 Tuition Fee: ₹2500/month, Transport Fee Slab A: ₹800/month).
* [ ] **Offline Payment Entry:** Cashier screen where the accountant enters custom cash amounts and clicks "Pay", printing a HTML/CSS template to the thermal/desktop printer.
* [ ] **Online Payments (Razorpay):** API webhook integration. Send an SMS/WhatsApp reminder with a dynamic payment URL (Razorpay Payment Link). When paid, Razorpay webhook updates SQL Server tables automatically.
* [ ] **Late Fee Alerts:** Cron job in .NET (using `Quartz.NET` or a simple Background Service) that runs on the 10th of every month, scanning unpaid balances and sending SMS reminders via a transactional SMS gateway (e.g., Twilio/Gupshup).

### 3.4. Academic & Attendance Modules
* [ ] **Mobile Attendance Screen:** A simple React grid of student names with checkboxes. Checked = Present. Unchecked = Absent.
* [ ] **SMS Absence Alert:** If marked absent, send a standardized API request: *"Dear Parent, [StudentName] was marked absent today, [Date]. - [SchoolName]"*.
* [ ] **Grade Entries:** Standard spreadsheet-like React table where teachers enter marks out of 100 for exams.
* [ ] **Report Card PDF Compiler:** Render report card views using a standard library (like `.NET QuestPDF` or HTML-to-PDF converters) to spit out clean, standard PDF cards.

---

## 4. Startup CTO Architectural Blueprint (Solo Dev Focus)

### Multi-Tenant Isolation Strategy (SQL Server + EF Core)
To bypass the administration complexity of managing 20 different SQL databases, use a **Shared Database, Shared Schema** approach.

```csharp
// EF Core DbContext Implementation for Tenant Isolation
public class ApplicationDbContext : DbContext
{
    private readonly string _tenantId;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ITenantService tenantService)
        : base(options)
    {
        _tenantId = tenantService.GetTenantId();
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Invoice> Invoices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply global filter for Multi-Tenancy isolation
        modelBuilder.Entity<Student>().HasQueryFilter(s => s.TenantId == _tenantId);
        modelBuilder.Entity<Invoice>().HasQueryFilter(i => i.TenantId == _tenantId);
        
        // Add index on TenantId for performance optimization
        modelBuilder.Entity<Student>().HasIndex(s => s.TenantId);
        modelBuilder.Entity<Invoice>().HasIndex(i => i.TenantId);
    }
}
```

> [!IMPORTANT]
> **Avoid Single-Point-of-Failure in Global Queries**
> Every DB insert operation must intercept or explicitly set the `TenantId`. Implement an override on `SaveChanges()` to automatically populate `TenantId` for all entities implementing a base `ITenantEntity` interface before writing to SQL Server.

### deployment Strategy
* **Backend:** Deploy .NET 8 Web API on a lightweight Linux VM on AWS (EC2) or Azure (App Service) inside a Docker container.
* **Database:** SQL Server hosted on AWS RDS or Azure SQL. Keep resources modest to start (e.g., 2 vCPUs, 4GB RAM), which easily handles 500-3000 concurrent users for a single school.
* **Frontend:** Build the React application as static HTML/JS/CSS assets and host on cloud CDNs (AWS CloudFront / Vercel / Netlify) for near-zero cost and lightning-fast load speed.
