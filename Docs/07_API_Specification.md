# API Specification: EduPulse AI
**Project Name:** EduPulse AI  
**Role Perspective:** Principal Solution Architect & Senior .NET API Architect  
**Core Technologies:** React | Redux Toolkit | .NET 8 Web API | Dapper | SQL Server  
**Data Protocols:** RESTful JSON  
**Header Standards:** `X-Tenant-Id` (UUID) or parsed implicitly from `Authorization: Bearer <JWT>`  

---

## Global API Rules & Design Standards

### 1. Implicit Tenant Context (RLS Protection)
All authenticated endpoints extract the `TenantId` claim from the incoming JWT token. Developers must NOT pass `tenantId` in the JSON request body or URL path for authenticated requests. The backend API retrieves the `TenantId` from `HttpContext.User` claims and binds it to the database session context.

### 2. Standard JSON Response Envelope
For success responses (unless binary file stream like PDFs), the API returns HTTP 200/201 with the domain payload directly. For error scenarios, the API returns the following structured envelope:
```json
{
  "errorCode": "ERR_SPECIFIC_CODE",
  "message": "Human-readable explanation of error.",
  "errors": {
    "FieldName": ["Validation validation check failed details."]
  },
  "timestamp": "2026-06-18T13:48:00Z"
}
```

---

## 1. Authentication APIs

### 1.1. User Login (Auth Token Issuance)
* **Endpoint:** `POST /api/auth/login`
* **HTTP Method:** `POST`
* **Request Model:**
  ```json
  {
    "email": "admin@school.com",
    "password": "Password123!"
  }
  ```
* **Response Model (200 OK):**
  ```json
  {
    "token": "eyJhbGciOiJIUzI1NiIsIn...",
    "expiresAt": "2026-06-19T01:48:00Z",
    "user": {
      "userId": "d4c9b321-4e10-4820-bbf5-894c244243f2",
      "email": "admin@school.com",
      "role": "SchoolAdmin",
      "schoolName": "EduPulse Academy"
    }
  }
  ```
* **Validation Rules:**
  * `email`: String. Required. Valid email format. Max 100 characters.
  * `password`: String. Required. Minimum 8 characters.
* **Authorization Rules:** `AllowAnonymous`
* **Business Rules:**
  * API validates active status on the `Users` and matching `Tenants` tables. Deactivated users return HTTP 403 Forbidden.
  * Failed attempts return generic error `ERR_INVALID_CREDENTIALS` (HTTP 401).

---

## 2. User Management APIs

### 2.1. List Users (School Directory)
* **Endpoint:** `GET /api/users`
* **HTTP Method:** `GET`
* **Request Query Parameters:**
  * `pageIndex` (default 1)
  * `pageSize` (default 20)
  * `searchTerm` (optional name filter)
* **Response Model (200 OK):**
  ```json
  {
    "items": [
      {
        "userId": "98a9c321-4e10-4820-bbf5-894c244243a4",
        "email": "teacher@school.com",
        "role": "Teacher",
        "isActive": true
      }
    ],
    "totalCount": 42
  }
  ```
* **Authorization Rules:** `RequiresRole(SchoolAdmin)`
* **Business Rules:**
  * Queries are automatically restricted by the tenant context extracted from the JWT token.

### 2.2. Toggle User Status (Deactivate / Reactivate)
* **Endpoint:** `PUT /api/users/{userId}/status`
* **HTTP Method:** `PUT`
* **Request Model:**
  ```json
  {
    "isActive": false
  }
  ```
* **Response Model (204 No Content):** Empty payload.
* **Authorization Rules:** `RequiresRole(SchoolAdmin)`
* **Business Rules:**
  * Prevents self-deactivation. Attempting to deactivate the logged-in user returns `ERR_SELF_DEACTIVATION_BLOCKED` (HTTP 400).
  * Suspends active sessions immediately upon token expiration window clearance.

---

## 3. Student APIs

### 3.1. Create Student Profile
* **Endpoint:** `POST /api/students`
* **HTTP Method:** `POST`
* **Request Model:**
  ```json
  {
    "admissionNo": "ADM2026001",
    "firstName": "Rahul",
    "lastName": "Sharma",
    "dateOfBirth": "2018-05-15",
    "gender": "Male",
    "bloodGroup": "O+",
    "socialCategory": "GEN",
    "aadhaarNo": "123456789012",
    "admissionDate": "2026-04-01",
    "parentPhone": "9876543210",
    "parentEmail": "parent@email.com",
    "parentFirstName": "Alok",
    "parentLastName": "Sharma"
  }
  ```
* **Response Model (201 Created):**
  ```json
  {
    "studentId": "e1a9c321-4e10-4820-bbf5-894c244243b7",
    "admissionNo": "ADM2026001",
    "status": "Applied"
  }
  ```
* **Validation Rules:**
  * `admissionNo`: String. Max 50 characters. Required.
  * `dateOfBirth`: Date. Required. Under 20 years old and over 3 years old.
  * `parentPhone`: String. Required. Valid Indian mobile format (10 digits).
  * `socialCategory`: String. Check values: `GEN`, `OBC`, `SC`, `ST`.
* **Authorization Rules:** `RequiresRole(SchoolAdmin)`
* **Business Rules:**
  * Checks for duplicate `admissionNo` within the tenant context. If found, returns `ERR_DUPLICATE_ADMISSION` (HTTP 400).
  * Automatically creates a `User` profile mapping to the `Parent` role using the `parentEmail` if the account does not exist.

### 3.2. Bulk Student Excel Import
* **Endpoint:** `POST /api/students/import`
* **HTTP Method:** `POST`
* **Request Model:** Multipart Form Data (`file` key containing Excel sheet).
* **Response Model (200 OK):**
  ```json
  {
    "processedCount": 140,
    "success": true
  }
  ```
* **Authorization Rules:** `RequiresRole(SchoolAdmin)`
* **Business Rules:**
  * Validates column headers match the standard template exactly.
  * Transaction rollback occurs if validation errors are detected on any row, returning error details with row numbers (e.g. `ERR_ROW_VALIDATION_FAILED` at Row 14).

### 3.3. Upload Student Document File
* **Endpoint:** `POST /api/students/{studentId}/documents`
* **HTTP Method:** `POST`
* **Request Model:** Multipart Form Data (`file` containing document, `documentType` string parameter).
* **Response Model (200 OK):**
  ```json
  {
    "documentId": "a9f9c321-4e10-4820-bbf5-894c244243f9",
    "fileName": "aadhaar_ राहुल.pdf",
    "filePath": "/uploads/documents/aadhaar_Rahul.pdf"
  }
  ```
* **Validation Rules:**
  * File size: Max 5MB.
  * `documentType`: Required. Must be `Aadhaar`, `TC`, `BirthCertificate`, or `Other`.
* **Authorization Rules:** `RequiresRole(SchoolAdmin)`
* **Business Rules:**
  * The file stream writes directly to mounted local storage volumes. The database logs references using SQL transactions.

---

## 4. Guardian APIs

### 4.1. Get Guardian and Linked Siblings Profile
* **Endpoint:** `GET /api/guardians/{guardianId}`
* **HTTP Method:** `GET`
* **Response Model (200 OK):**
  ```json
  {
    "guardianId": "87b9c321-4e10-4820-bbf5-894c244243c4",
    "firstName": "Alok",
    "lastName": "Sharma",
    "phone": "9876543210",
    "email": "parent@email.com",
    "students": [
      {
        "studentId": "e1a9c321-4e10-4820-bbf5-894c244243b7",
        "firstName": "Rahul",
        "lastName": "Sharma",
        "class": "Class 3A",
        "rollNo": 12
      }
    ]
  }
  ```
* **Authorization Rules:** `RequiresRole(SchoolAdmin, Teacher, Parent)`
* **Business Rules:**
  * Parent accounts can only access records matching their verified `GuardianId`. Cross-parent queries return HTTP 403 Forbidden.

---

## 5. Staff APIs

### 5.1. Create Staff Profile
* **Endpoint:** `POST /api/staff`
* **HTTP Method:** `POST`
* **Request Model:**
  ```json
  {
    "employeeCode": "EMP014",
    "firstName": "Sita",
    "lastName": "Iyer",
    "phone": "9876543222",
    "email": "sita.iyer@school.com",
    "designation": "PGT Physics"
  }
  ```
* **Response Model (201 Created):**
  ```json
  {
    "staffId": "65b9c321-4e10-4820-bbf5-894c244243d5",
    "employeeCode": "EMP014"
  }
  ```
* **Authorization Rules:** `RequiresRole(SchoolAdmin)`
* **Business Rules:**
  * Creates a corresponding `User` authentication login with default role mapping corresponding to staff profiles.

---

## 6. Academic Year APIs

### 6.1. Create Academic Year
* **Endpoint:** `POST /api/academic-years`
* **HTTP Method:** `POST`
* **Request Model:**
  ```json
  {
    "name": "2026-2027",
    "startDate": "2026-04-01",
    "endDate": "2027-03-31"
  }
  ```
* **Response Model (201 Created):**
  ```json
  {
    "academicYearId": "d3b9c321-4e10-4820-bbf5-894c244243a1",
    "name": "2026-2027"
  }
  ```
* **Validation Rules:**
  * `name`: String. Required. Must match format `YYYY-YYYY`.
  * `startDate` and `endDate`: Date. Required. `endDate` must be later than `startDate`.

---

## 7. Class & Section APIs

### 7.1. Create Class Section Allocation
* **Endpoint:** `POST /api/class-sections`
* **HTTP Method:** `POST`
* **Request Model:**
  ```json
  {
    "classId": "1a9c3210-4e10-4820-bbf5-894c244243b1",
    "sectionId": "2b9c3210-4e10-4820-bbf5-894c244243b2",
    "academicYearId": "d3b9c321-4e10-4820-bbf5-894c244243a1",
    "classTeacherId": "65b9c321-4e10-4820-bbf5-894c244243d5",
    "capacity": 40
  }
  ```
* **Response Model (201 Created):**
  ```json
  {
    "classSectionId": "5c9c3210-4e10-4820-bbf5-894c244243b3"
  }
  ```
* **Validation Rules:**
  * `capacity`: Integer. Must be greater than 0 and less than 100.
* **Authorization Rules:** `RequiresRole(SchoolAdmin)`

---

## 8. Attendance APIs

### 8.1. Load Attendance Roster Grid
* **Endpoint:** `GET /api/attendance/roster`
* **HTTP Method:** `GET`
* **Request Query Parameters:**
  * `classSectionId` (Required UUID)
  * `date` (Required Date format YYYY-MM-DD)
* **Response Model (200 OK):**
  ```json
  {
    "classSectionId": "5c9c3210-4e10-4820-bbf5-894c244243b3",
    "date": "2026-06-18",
    "isLocked": false,
    "students": [
      {
        "studentClassHistoryId": "f7a9c321-4e10-4820-bbf5-894c244243d4",
        "rollNo": 1,
        "fullName": "Aarav Mehta",
        "status": "P"
      }
    ]
  }
  ```
* **Authorization Rules:** `RequiresRole(SchoolAdmin, Teacher)`

### 8.2. Submit Daily Attendance Register
* **Endpoint:** `POST /api/attendance/submit`
* **HTTP Method:** `POST`
* **Request Model:**
  ```json
  {
    "classSectionId": "5c9c3210-4e10-4820-bbf5-894c244243b3",
    "date": "2026-06-18",
    "records": [
      {
        "studentClassHistoryId": "f7a9c321-4e10-4820-bbf5-894c244243d4",
        "status": "A",
        "checkInTime": null
      }
    ]
  }
  ```
* **Response Model (200 OK):**
  ```json
  {
    "isSuccess": true,
    "alertsQueued": 1
  }
  ```
* **Business Rules:**
  * Locks submissions after 09:30 AM for teachers (returns `ERR_ATTENDANCE_LOCKED`). Overrides require a `SchoolAdmin` role.
  * Queues automated SMS/WhatsApp alerts for absent students.

---

## 9. Fee APIs

### 9.1. Create Fee Line Item
* **Endpoint:** `POST /api/fees/line-items`
* **HTTP Method:** `POST`
* **Request Model:**
  ```json
  {
    "feeGroupId": "8e9c3210-4e10-4820-bbf5-894c244243a4",
    "classId": "1a9c3210-4e10-4820-bbf5-894c244243b1",
    "academicYearId": "d3b9c321-4e10-4820-bbf5-894c244243a1",
    "name": "Term 1 Tuition Fee",
    "amount": 25000.00
  }
  ```
* **Response Model (201 Created):**
  ```json
  {
    "feeLineItemId": "7e9c3210-4e10-4820-bbf5-894c244243a5"
  }
  ```
* **Authorization Rules:** `RequiresRole(SchoolAdmin, Accountant)`

---

## 10. Invoice APIs

### 10.1. Get Invoice Details
* **Endpoint:** `GET /api/invoices/{invoiceId}`
* **HTTP Method:** `GET`
* **Response Model (200 OK):**
  ```json
  {
    "invoiceId": "9e9c3210-4e10-4820-bbf5-894c244243a6",
    "invoiceNo": "INV-2026-0004",
    "studentName": "Rahul Sharma",
    "invoiceStatus": "Pending",
    "dueDate": "2026-07-01",
    "totalAmount": 25000.00,
    "concessionAmount": 5000.00,
    "paidAmount": 0.00,
    "balanceAmount": 20000.00,
    "details": [
      {
        "invoiceDetailId": "0a9c3210-4e10-4820-bbf5-894c244243f1",
        "name": "Term 1 Tuition Fee",
        "amount": 25000.00
      }
    ]
  }
  ```
* **Authorization Rules:** `RequiresRole(SchoolAdmin, Accountant, Parent)`

### 10.2. Generate Batch Invoices for Class
* **Endpoint:** `POST /api/invoices/generate-batch`
* **HTTP Method:** `POST`
* **Request Model:**
  ```json
  {
    "classSectionId": "5c9c3210-4e10-4820-bbf5-894c244243b3",
    "academicYearId": "d3b9c321-4e10-4820-bbf5-894c244243a1",
    "dueDate": "2026-07-01",
    "feeLineItemIds": [
      "7e9c3210-4e10-4820-bbf5-894c244243a5"
    ]
  }
  ```
* **Response Model (200 OK):**
  ```json
  {
    "invoicesGenerated": 38,
    "totalValue": 950000.00
  }
  ```
* **Authorization Rules:** `RequiresRole(SchoolAdmin, Accountant)`

---

## 11. Receipt APIs

### 11.1. Collect Manual Cashier Payment
* **Endpoint:** `POST /api/receipts/collect`
* **HTTP Method:** `POST`
* **Request Model:**
  ```json
  {
    "invoiceId": "9e9c3210-4e10-4820-bbf5-894c244243a6",
    "amountPaid": 10000.00,
    "paymentMethod": "Cash",
    "transactionRef": null
  }
  ```
* **Response Model (201 Created):**
  ```json
  {
    "receiptId": "1b9c3210-4e10-4820-bbf5-894c244243f9",
    "receiptNo": "REC-2026-0014",
    "allocatedLineItems": [
      {
        "invoiceDetailId": "0a9c3210-4e10-4820-bbf5-894c244243f1",
        "allocatedAmount": 10000.00
      }
    ]
  }
  ```
* **Authorization Rules:** `RequiresRole(SchoolAdmin, Accountant)`
* **Business Rules:**
  * The API automatically applies FIFO allocation rules to distribute payments across billed invoice details, inserting records into `FeeReceiptAllocations`.
  * Triggers database transactions to update `StudentInvoices.BalanceAmount` and `PaidAmount`.
  * Computes the new `InvoiceStatus` state (e.g. updating status to `PartiallyPaid` or `Paid`).

### 11.2. Reverse/Cancel Payment Receipt
* **Endpoint:** `POST /api/receipts/{receiptId}/cancel`
* **HTTP Method:** `POST`
* **Request Model:**
  ```json
  {
    "cancellationReason": "Cheque bounce from parent bank."
  }
  ```
* **Response Model (204 No Content):** Empty.
* **Authorization Rules:** `RequiresRole(SchoolAdmin, Accountant)`
* **Business Rules:**
  * Reverses all allocations mapped to this receipt in the ledger, updates the invoice balance back to unpaid, and changes the invoice status accordingly.

### 11.3. Razorpay Gateway Webhook Callback
* **Endpoint:** `POST /api/receipts/razorpay-webhook`
* **HTTP Method:** `POST`
* **Request Model:** Standard Razorpay Webhook JSON payload containing signature matching keys and transaction indicators.
* **Response Model (200 OK):** Empty.
* **Authorization Rules:** `AllowAnonymous` (Signature verified in Web API request body using encryption keys).
* **Business Rules:**
  * Verifies signatures. If matching transaction records are found, updates database invoice balance columns automatically.

---

## 12. Concession APIs

### 12.1. Approve Student Concession
* **Endpoint:** `POST /api/concessions`
* **HTTP Method:** `POST`
* **Request Model:**
  ```json
  {
    "studentId": "e1a9c321-4e10-4820-bbf5-894c244243b7",
    "feeLineItemId": "7e9c3210-4e10-4820-bbf5-894c244243a5",
    "type": "Percentage",
    "value": 20.00,
    "approvalCode": "SCH-2026-004"
  }
  ```
* **Response Model (201 Created):**
  ```json
  {
    "concessionId": "4c9c3210-4e10-4820-bbf5-894c244243b8"
  }
  ```
* **Validation Rules:**
  * `type`: String. Required. Must be `Percentage` or `FixedAmount`.
  * `value`: Decimal. Value limits: if type is Percentage, limit to `[0, 100]`.
* **Authorization Rules:** `RequiresRole(SchoolAdmin)`

---

## 13. Exam APIs

### 13.1. Save Exam Marks Entries
* **Endpoint:** `POST /api/exams/{examId}/marks/submit`
* **HTTP Method:** `POST`
* **Request Model:**
  ```json
  {
    "records": [
      {
        "studentId": "e1a9c321-4e10-4820-bbf5-894c244243b7",
        "marksObtained": 85.50,
        "isAbsent": false,
        "teacherRemarks": "Excellent conceptual understanding."
      }
    ]
  }
  ```
* **Response Model (200 OK):**
  ```json
  {
    "submittedRecords": 1,
    "status": "Draft"
  }
  ```
* **Validation Rules:**
  * `marksObtained`: Decimal. Must be less than or equal to exam `MaxMarks`.
* **Authorization Rules:** `RequiresRole(SchoolAdmin, Teacher)`
* **Business Rules:**
  * Prevents updates if the coordinator has locked submissions or published exam results.

---

## 14. Homework APIs

### 14.1. Create Homework Notice
* **Endpoint:** `POST /api/homework`
* **HTTP Method:** `POST`
* **Request Model:** Multipart Form Data containing:
  * `classSectionSubjectId` (UUID)
  * `title` (String, max 100)
  * `description` (String, max 1000)
  * `dueDate` (Date YYYY-MM-DD)
  * `file` (Attachment binary file, max 5MB, optional)
* **Response Model (201 Created):**
  ```json
  {
    "homeworkId": "3b9c3210-4e10-4820-bbf5-894c244243f4"
  }
  ```
* **Validation Rules:**
  * `dueDate`: Must be equal to or later than current server date.

---

## 15. Notification APIs

### 15.1. List Notification Alerts Logs
* **Endpoint:** `GET /api/notifications/logs`
* **HTTP Method:** `GET`
* **Response Model (200 OK):**
  ```json
  {
    "items": [
      {
        "notificationId": "a1b9c321-4e10-4820-bbf5-894c244243d9",
        "recipientPhone": "9876543210",
        "messageBody": "Dear Parent, Rahul was absent today.",
        "status": "Delivered",
        "createdOn": "2026-06-18T09:45:00Z"
      }
    ]
  }
  ```
* **Authorization Rules:** `RequiresRole(SchoolAdmin)`

---

## 16. Dashboard APIs

### 16.1. Get Admin Dashboard Metrics
* **Endpoint:** `GET /api/dashboard/admin`
* **HTTP Method:** `GET`
* **Response Model (200 OK):**
  ```json
  {
    "collections": {
      "totalBilled": 1250000.00,
      "totalCollected": 950000.00,
      "outstandingDues": 300000.00
    },
    "attendance": {
      "todayAttendanceRate": 94.2,
      "absentAlertsSent": 18
    },
    "registrations": {
      "totalActiveStudents": 840,
      "pendingApplications": 12
    }
  }
  ```
* **Authorization Rules:** `RequiresRole(SchoolAdmin)`
* **Business Rules:**
  * Aggregations run dynamically using fast Dapper queries with index coverage, returning dashboard snapshots in under 100ms.

### 16.2. Get Parent Dashboard Child Summary
* **Endpoint:** `GET /api/dashboard/parent`
* **HTTP Method:** `GET`
* **Response Model (200 OK):**
  ```json
  {
    "children": [
      {
        "studentId": "e1a9c321-4e10-4820-bbf5-894c244243b7",
        "fullName": "Rahul Sharma",
        "class": "Class 3A",
        "attendancePercentage": 96.8,
        "pendingHomeworkCount": 2,
        "feeOutstanding": 20000.00,
        "nextInvoiceNo": "INV-2026-0004"
      }
    ]
  }
  ```
* **Authorization Rules:** `RequiresRole(Parent)`
* **Business Rules:**
  * The API automatically extracts the requesting parent's authenticated user ID, resolves the matching `GuardianId` from the registry, and checks linked sibling records. Cross-parent lookups are blocked.
