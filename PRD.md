# Product Requirements Document (PRD)
# Hospital Information System (HIS)

---

## 1. Executive Summary

This document outlines the product requirements for a Hospital Information System (HIS) built using **ABP Framework Free Edition** with **ASP.NET Core 10** for the backend and **Angular** for the frontend. The system will manage hospital operations including patient registration, appointments, pharmacy, room management, and nursing services with a comprehensive **User Activity Logging System**.

---

## 2. Technology Stack

| Layer | Technology |
|-------|-----------|
| **Backend Framework** | ABP Framework Free Edition (8.x+) |
| **Backend Runtime** | ASP.NET Core 10 |
| **Frontend Framework** | Angular 18+ |
| **Database** | SQL Server 2022 / PostgreSQL |
| **ORM** | Entity Framework Core 8 |
| **Authentication** | ABP Identity Module (OpenIddict) |
| **Authorization** | ABP Permission System |
| **Logging** | Serilog + Custom Activity Logging |
| **API Documentation** | Swagger/OpenAPI |
| **Real-time** | SignalR (for notifications) |
| **Testing** | ABP TestBase | and  Playwright for E2E testing |

---

## 3. Modules & Features

### 3.1 Reception Module (الاستقبال)

> **Status: ✅ COMPLETED (Major Features Ready)**
> **Priority: HIGH - First Delivery**

#### 3.1.1 Patient Management
- [x] Patient registration with detailed demographics
- [ ] National ID / Passport integration
- [x] Patient search (by ID, name, phone, national ID)
- [x] Patient profile view with medical history summary
- [ ] Photo capture integration
- [x] Emergency contact information

#### 3.1.2 Insurance Management (الوضع التأميني)
- [x] Insurance company master data
- [x] Insurance plan/policy configuration
- [x] Patient insurance enrollment
- [x] Coverage verification
- [ ] Insurance approval workflow
- [ ] Co-payment calculations
- [ ] Insurance claims tracking

#### 3.1.3 Payment & Billing (المدفوعات والمؤجلات)
- [x] Cash payment processing
- [x] Credit/Debit card integration
- [x] Payment receipt generation
- [x] Deferred payment tracking
- [x] Payment installment plans
- [x] Outstanding balance management
- [x] Refund processing
- [x] Payment methods master data management
- [ ] Daily cash reconciliation

#### 3.1.4 Laboratory Reception (استقبال المعمل)
- [x] Comprehensive patient registration integration
- [x] Quick patient search and selection
- [x] Lab test selection with search by code/name
- [x] Real-time price calculation based on contracts
- [x] Support for multiple payment methods (Cash, POS, Transfer, Credit)
- [x] Printing options (Work Order, Barcode, Ticket)
- [x] Integrated accounting and discount management
- [x] Automatic age calculation (Years, Months, Days)

---

### 3.2 Booking & Appointment Module (خدمة الحجز)

> **Status: 🟡 PARTIALLY COMPLETED**
> **Priority: HIGH - First Delivery**

#### 3.2.1 Clinic Appointments (حجز العيادة)
- [x] Doctor schedule management
- [x] Appointment booking with time slots
- [x] Appointment confirmation/cancellation
- [ ] Walk-in patient handling
- [ ] Appointment reminders (SMS/Email)
- [x] Queue management system (Basic Waiting List)
- [x] Doctor availability calendar

#### 3.2.2 Laboratory Appointments (حجز المعمل)
- [x] Lab service catalog
- [x] Lab appointment scheduling
- [x] Sample collection scheduling
- [x] Lab preparation instructions

#### 3.2.3 Emergency Services (الطوارئ)

> **Status: ✅ COMPLETED**
- [ ] Triage classification
- [ ] Priority-based queue
- [ ] Emergency bay assignment

---

### 3.3 Referral & Clinical Services (خدمة التوجيه)

> **Status: 🔴 PENDING (Major Work Remaining)**
> **Priority: HIGH - First Delivery**

#### 3.3.1 Doctor Referrals
- [x] Internal referral (doctor to doctor)
- [x] Lab test orders
- [x] Radiology orders
- [x] Procedure orders
- [x] Referral status tracking

#### 3.3.2 Inpatient Admission (حجز المريض)
- [x] Admission request from clinic
- [x] Bed/room selection
- [x] Admission approval workflow
- [x] Admission documentation

#### 3.3.3 Prescription Management (صرف الأدوية والمستهلكات)
- [x] Electronic prescription (e-Rx) (Basic UI)
- [x] Drug database integration
- [x] Drug-drug interaction alerts
- [x] Prescription history
- [x] Consumables ordering

---

### 3.4 Pharmacy Module (خدمة الصيدلية)

> **Status: ✅ COMPLETED (Phase 3 Ready)**
> **Priority: MEDIUM**

#### 3.4.1 Drug Inventory (تسجيل الأدوية)
- [x] Drug master data management
- [x] Drug categories and classifications
- [x] Batch/Lot tracking
- [x] Expiry date management
- [x] Barcode support

#### 3.4.2 Receiving & Stock (استلام الأدوية)
- [x] Purchase order creation
- [x] Goods receipt processing
- [x] Stock level management
- [x] Minimum stock alerts
- [x] Stock transfer between locations

#### 3.4.3 Dispensing (صرف الأدوية)
- [x] Prescription verification
- [x] Dispensing workflow
- [x] Patient counseling notes
- [x] Dispensing label printing
- [x] Controlled substance tracking

#### 3.4.4 Procurement (شراء الكميات)
- [x] Supplier management
- [x] Purchase requisitions
- [x] Price comparison
- [x] Order tracking

#### 3.4.5 Retail & POS (نقاط البيع)
- [x] Direct retail sales to non-patients/guests
- [x] Barcode scanning for sales
- [x] Receipt generation
- [x] Daily sales reporting

---

### 3.5 Room & Bed Management (خدمات الغرف)

> **Status: ✅ COMPLETED (Phase 1 & 2 & 3 Support Ready)**
> **Priority: MEDIUM**

#### 3.5.1 Room Configuration
- [x] Room type definitions (ICU, Ward, Private, etc.)
- [x] Bed configuration per room
- [x] Room amenities tracking
- [x] Room pricing (Daily Rate)

#### 3.5.2 Availability Management (إتاحة الغرف والأسرة)
- [x] Real-time bed availability dashboard
- [x] Bed status tracking (Available, Occupied, Reserved, Maintenance)
- [ ] Housekeeping integration
- [x] Room assignment to reception

#### 3.5.3 Reservation System (تنظيم الحجز)
- [x] Advance room booking (Calendar View)
- [x] Room upgrade/downgrade (Dynamic Pricing)
- [x] Patient transfer between rooms (Patient Transfer History)
- [x] Discharge planning (Medical Discharge & final billing)

---

### 3.6 Nursing Services (خدمات التمريض)

> **Status: ✅ COMPLETED (Phase 1 & 2 Ready)**
> **Priority: MEDIUM**

#### 3.6.1 Doctor Orders Execution (صرف أوامر الدكتور)
- [x] Medication administration records (MAR)
- [x] Vital signs documentation
- [x] Nursing care plans
- [ ] Order acknowledgment workflow
- [x] Nursing shift handover

#### 3.6.2 Patient Care
- [x] Patient rounds documentation
- [x] Pain assessment
- [x] Fall risk assessment
- [x] Wound care tracking
- [x] Input/Output charting

---

### 3.7 User Activity Logging System (نظام تسجيل نشاط المستخدمين)

> **Status: ✅ COMPLETED**
> **Priority: HIGH - Required in All Modules**

#### 3.7.1 Audit Trail
- [x] User login/logout tracking
- [x] Session management
- [x] Failed login attempts
- [x] IP address logging

#### 3.7.2 Data Change Logging
- [ ] Create operations logging
- [ ] Update operations logging (before/after values)
- [ ] Delete operations logging (soft delete tracking)
- [ ] Bulk operation logging

#### 3.7.3 Activity Reports
- [x] User activity timeline
- [ ] Module-wise activity reports
- [x] Data access reports
- [x] Suspicious activity alerts
- [x] Audit log search and filtering

#### 3.7.4 Compliance Features
- [ ] Log retention policies
- [ ] Log export (CSV, Excel, PDF)
- [ ] Log archival
- [ ] HIPAA/local regulation compliance

---

### 3.8 Financial & Inventory Module (النظام المحاسبي والمخزون)

> **Status: ✅ COMPLETED (Core Features Ready)**
> **Priority: HIGH (Per Client Request)**

#### 3.8.1 Inventory & Purchasing (المخزون والمشتريات)
- [x] **Supplier Integration**: Link purchases directly to suppliers (`ربط المشتريات بالموردين`).
- [x] **Warehouse Management**: Link purchases to central stores (`ربط المشتريات بالمخزن`).
- [x] **Departmental Consumption**: Track consumption by service departments (`ربط المخزن بالأقسام الخدمية`).
- [x] **Inventory Valuation**: Implement **LIFO** (Last-In, First-Out) method for pricing (`اعتماد طريقة LIFO`).
- [x] **Stock Cards**: Detailed tracking of Incoming/Outgoing (Item, Quantity, Price, Total) (`كرت الصنف`).

#### 3.8.2 Accounting Entries (القيود المحاسبية)
- [x] **Automated Receiving Entries**: 
  - Debit: Warehouse/Department Sub-Account (`حساب المخزن الفرعي`).
  - Credit: Suppliers (`حساب الموردين`).
- [x] **Automated Dispensing Entries**:
  - Debit: Department Expense (`حساب القسم مدين`).
  - Credit: Warehouse/Inventory (`حساب المخزن دائن`).
- [x] **Revenue Entries**: Track revenue per employee/doctor vs. cash/receivables.
- [x] **Voucher Management** (سندات القبض والصرف):
  - [x] **Payment Vouchers**: Manage payments to suppliers and expenses.
  - [x] **Receipt Vouchers**: Manage payments from patients/insurance.
  - [x] **Bank Transactions**: Track bank deposits and withdrawals.
  - [x] **Advance Payments (Deposits)**: Collect deposits against inpatient admissions (Implemented in Admission modal).
  - [ ] **Consolidated Final Invoice**: Automatic aggregation of room charges, medications, lab tests, and operations upon discharge.
- [x] **Operations & Doctor Fees** (أجور العمليات والأطباء):
  - [x] **Surgery Pricing**: Add cost of operation and consumables to inpatient bill.
  - [x] **Doctor Entitlements**: Calculate and allocate surgeon/anesthesiologist fees (percentage or flat rate).
- [x] **Claims Management** (المطالبات):
  - [x] **Contract Claims**: Manage insurance claims and contract billing.
- [ ] **Advanced Reporting**:
  - [x] **Daily Accounts Report**: Summary of daily financial transactions.
  - [x] **Customer Debts Report**: Detailed report of outstanding patient balances.
  - [x] **Discounts Report**: Track discounts given to patients.

#### 3.8.3 Financial Statements (القوائم المالية)
- [x] **Income Statement** (`قائمة الدخل`):
    - **Revenue**:
        - Gross Sales.
        - (-) Sales Returns, Allowances & Discounts.
        - (=) Net Sales.
    - **Cost of Goods Sold (COGS)**:
        - Beginning Inventory.
        - (+) Net Purchases.
        - (=) Goods Available for Sale.
        - (-) Ending Inventory.
        - (=) Cost of Goods Sold.
    - **Profitability**:
        - (=) Gross Profit.
        - (-) Selling Expenses.
        - (-) Administrative Expenses.
        - (=) Operating Income (Income from Operations).
    - **Other Items & Taxes**:
        - (+) Other Revenues and Gains.
        - (-) Other Expenses and Losses.
        - (=) Income Before Income Tax.
        - (-) Income Tax.
        - (=) Net Income.
    - **Comprehensive Income** (`قائمة الدخل الشامل`):
        - Foreign currency translation differences.
        - Change in fair value of hedging instruments.
        - Actuarial gains/losses (Defined benefit plans).
        - Gains/losses reclassified from hedging.
        - (=) Comprehensive Income.
        - (=) Total Comprehensive Income for the Year.

- [x] **Balance Sheet** (`الميزانية العمومية/المركز المالي`):
  - [x] Must be in vertical format (`يجب أن تكون في الوضع الرأسي`).
  - [x] Includes a column for previous budget amounts (`وجود عمود يتضمن مبالغ الميزانية السابقة`).
- [x] **Cash Flow Statement** (`التدفقات النقدية`).
- [x] **Statement of Changes in Equity** (`التغير في حقوق الملكية`).

#### 3.8.4 Administrative Functions (الوظائف الإدارية)
- [x] **Organizational Structure**: Link administrative jobs/titles to specific departments (`ربط الوظائف الإدارية بالأقسام`).

---

## 4. ABP Framework Implementation Structure

### 4.1 Solution Structure

```
HIS/
├── src/
│   ├── HIS.Domain.Shared/           # Shared constants, enums, localization
│   ├── HIS.Domain/                  # Entities, repositories, domain services
│   ├── HIS.Application.Contracts/   # DTOs, application service interfaces
│   ├── HIS.Application/             # Application services implementation
│   ├── HIS.EntityFrameworkCore/     # EF Core DbContext, migrations
│   ├── HIS.HttpApi/                 # API Controllers
│   ├── HIS.HttpApi.Host/            # API host (ASP.NET Core 10)
│   └── angular/                     # Angular frontend application
├── test/
│   ├── HIS.Domain.Tests/
│   ├── HIS.Application.Tests/
│   └── HIS.HttpApi.Tests/
└── docs/
```

### 4.2 Core Entities

```
Domain/
├── Patients/
│   ├── Patient.cs
│   ├── PatientInsurance.cs
│   └── PatientPayment.cs
├── Appointments/
│   ├── Appointment.cs
│   ├── DoctorSchedule.cs
│   └── TimeSlot.cs
├── Clinical/
│   ├── Referral.cs
│   ├── Prescription.cs
│   └── MedicalOrder.cs
├── Pharmacy/
│   ├── Drug.cs
│   ├── DrugStock.cs
│   ├── Dispensing.cs
│   └── PurchaseOrder.cs
├── Rooms/
│   ├── Room.cs
│   ├── Bed.cs
│   └── BedAssignment.cs
├── Nursing/
│   ├── NursingOrder.cs
│   ├── VitalSign.cs
│   └── MedicationAdministration.cs
└── Logging/
    ├── ActivityLog.cs
    └── AuditEntry.cs
```

---

## 5. User Activity Logging Implementation

### 5.1 ActivityLog Entity

```csharp
public class ActivityLog : FullAuditedAggregateRoot<Guid>
{
    public Guid? UserId { get; set; }
    public string UserName { get; set; }
    public string Module { get; set; }          // e.g., "Patient", "Pharmacy"
    public string Action { get; set; }          // e.g., "Create", "Update", "Delete", "View"
    public string EntityType { get; set; }      // e.g., "Patient"
    public string EntityId { get; set; }
    public string Description { get; set; }
    public string OldValues { get; set; }       // JSON
    public string NewValues { get; set; }       // JSON
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public DateTime Timestamp { get; set; }
    public ActivityLogLevel Level { get; set; } // Info, Warning, Critical
}
```

### 5.2 Automatic Logging via Interceptor

```csharp
public class ActivityLoggingInterceptor : IAbpInterceptor
{
    // Automatically logs all Create, Update, Delete operations
    // Captures before/after states for auditing
}
```

### 5.3 Logging API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/activity-logs` | Get paginated activity logs |
| GET | `/api/activity-logs/{id}` | Get specific log entry |
| GET | `/api/activity-logs/user/{userId}` | Get logs by user |
| GET | `/api/activity-logs/entity/{type}/{id}` | Get logs by entity |
| GET | `/api/activity-logs/export` | Export logs to CSV/Excel |

---

## 6. User Roles & Permissions

| Role | Description | Key Permissions |
|------|-------------|-----------------|
| **Admin** | System administrator | Full access, user management, settings |
| **Receptionist** | Front desk staff | Patient registration, appointments, payments |
| **Doctor** | Medical practitioner | Patient records, prescriptions, referrals |
| **Nurse** | Nursing staff | Order execution, vital signs, patient care |
| **Pharmacist** | Pharmacy staff | Drug dispensing, inventory management |
| **Lab Technician** | Laboratory staff | Sample processing, results entry |
| **Billing Officer** | Finance staff | Payments, invoicing, reports |
| **Auditor** | Compliance officer | Read-only access to activity logs |

---

## 7. Non-Functional Requirements

### 7.1 Performance
- Page load time < 3 seconds
- API response time < 500ms for standard operations
- Support 100+ concurrent users

### 7.2 Security
- HTTPS only
- JWT-based authentication
- Role-based access control (RBAC)
- Data encryption at rest
- Activity logging for all sensitive operations
- Session timeout after 30 minutes of inactivity

### 7.3 Availability
- 99.5% uptime target
- Automatic database backup
- Disaster recovery plan

### 7.4 Scalability
- Horizontal scaling support
- Database connection pooling
- Caching layer (Redis optional)

---

## 8. Delivery Phases

### Phase 1 (First Delivery) - Core Reception [✅ COMPLETED]
| Module | Features | Duration |
|--------|----------|----------|
| Patient Management | Full CRUD, search, demographics | 2 weeks |
| Insurance Management | Basic enrollment, verification | 1 week |
| Payment Processing | Cash, deferred payments | 1 week |
| Appointment Booking | Clinic, lab, emergency | 2 weeks |
| Referral System | Basic referrals, prescriptions | 1 week |
| **Activity Logging** | Core logging infrastructure | 1 week |

**Total Phase 1: ~8 weeks**

### Phase 2 - Pharmacy & Support Services [✅ COMPLETED]
| Module | Features | Duration |
|--------|----------|----------|
| Pharmacy Module | Full implementation | 3 weeks |
| Room Management | Full implementation | 2 weeks |

**Total Phase 2: ~5 weeks**

### Phase 3 - Clinical & Nursing [✅ COMPLETED]
| Module | Features | Duration |
|--------|----------|----------|
| Nursing Services | Full implementation | 3 weeks |
| Advanced Reporting | Dashboards, analytics | 2 weeks |

**Total Phase 3: ~5 weeks**

---

## 9. Development Setup Commands

```bash
# Install ABP CLI
dotnet tool install -g Volo.Abp.Cli

# Create new ABP solution
abp new HIS -t app --ui angular --dbms SqlServer --version 8.3.0

# Database migration
dotnet ef database update -p src/HIS.EntityFrameworkCore

# Run backend
cd src/HIS.HttpApi.Host
dotnet run

# Run frontend
cd angular
npm install
ng serve
```

---

## 10. Appendix

### 10.1 Original Requirements (Arabic)

```
التسليم الاول بيبقى واجهه الاستقبال من بيانات المريض التفصيلية خاصة الوضع التاميني و المدفوعات و المؤجلات
ثم خدمة الحجز لعياده او معمل او طوارئ
ثم خدمه التوجيه من دكتور العياده لفحوصات او حجز المريض و صرف ادويه له او مستهلكات

خدمة الصيدليه
تسجيل الادوية و استلمها و صرفها و شراء كمياتها

خدمات الغرف
اتاحه الغرف و الاسره امام الاستقبال لتنظيم الحجز

خدمات التمريض
صرف اوامر الدكتور للحالات
```

### 10.2 References
- [ABP Framework Documentation](https://docs.abp.io/)
- [ABP Free Template](https://abp.io/get-started)
- [Angular Documentation](https://angular.dev/)
- [ASP.NET Core 10 Documentation](https://learn.microsoft.com/aspnet/core/)

---

**Document Version:** 1.1  
**Last Updated:** February 26, 2026  
**Status:** In Progress - Core Modules Ready
