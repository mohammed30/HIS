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

---

## 3. Modules & Features

### 3.1 Reception Module (الاستقبال)

> **Priority: HIGH - First Delivery**

#### 3.1.1 Patient Management
- [ ] Patient registration with detailed demographics
- [ ] National ID / Passport integration
- [ ] Patient search (by ID, name, phone, national ID)
- [ ] Patient profile view with medical history summary
- [ ] Photo capture integration
- [ ] Emergency contact information

#### 3.1.2 Insurance Management (الوضع التأميني)
- [ ] Insurance company master data
- [ ] Insurance plan/policy configuration
- [ ] Patient insurance enrollment
- [ ] Coverage verification
- [ ] Insurance approval workflow
- [ ] Co-payment calculations
- [ ] Insurance claims tracking

#### 3.1.3 Payment & Billing (المدفوعات والمؤجلات)
- [ ] Cash payment processing
- [ ] Credit/Debit card integration
- [ ] Payment receipt generation
- [ ] Deferred payment tracking
- [ ] Payment installment plans
- [ ] Outstanding balance management
- [ ] Refund processing
- [ ] Daily cash reconciliation

---

### 3.2 Booking & Appointment Module (خدمة الحجز)

> **Priority: HIGH - First Delivery**

#### 3.2.1 Clinic Appointments (حجز العيادة)
- [ ] Doctor schedule management
- [ ] Appointment booking with time slots
- [ ] Appointment confirmation/cancellation
- [ ] Walk-in patient handling
- [ ] Appointment reminders (SMS/Email)
- [ ] Queue management system
- [ ] Doctor availability calendar

#### 3.2.2 Laboratory Appointments (حجز المعمل)
- [ ] Lab service catalog
- [ ] Lab appointment scheduling
- [ ] Sample collection scheduling
- [ ] Lab preparation instructions

#### 3.2.3 Emergency Services (الطوارئ)
- [ ] Emergency patient registration (fast track)
- [ ] Triage classification
- [ ] Priority-based queue
- [ ] Emergency bay assignment

---

### 3.3 Referral & Clinical Services (خدمة التوجيه)

> **Priority: HIGH - First Delivery**

#### 3.3.1 Doctor Referrals
- [ ] Internal referral (doctor to doctor)
- [ ] Lab test orders
- [ ] Radiology orders
- [ ] Procedure orders
- [ ] Referral status tracking

#### 3.3.2 Inpatient Admission (حجز المريض)
- [ ] Admission request from clinic
- [ ] Bed/room selection
- [ ] Admission approval workflow
- [ ] Admission documentation

#### 3.3.3 Prescription Management (صرف الأدوية والمستهلكات)
- [ ] Electronic prescription (e-Rx)
- [ ] Drug database integration
- [ ] Drug-drug interaction alerts
- [ ] Prescription history
- [ ] Consumables ordering

---

### 3.4 Pharmacy Module (خدمة الصيدلية)

> **Priority: MEDIUM**

#### 3.4.1 Drug Inventory (تسجيل الأدوية)
- [ ] Drug master data management
- [ ] Drug categories and classifications
- [ ] Batch/Lot tracking
- [ ] Expiry date management
- [ ] Barcode support

#### 3.4.2 Receiving & Stock (استلام الأدوية)
- [ ] Purchase order creation
- [ ] Goods receipt processing
- [ ] Stock level management
- [ ] Minimum stock alerts
- [ ] Stock transfer between locations

#### 3.4.3 Dispensing (صرف الأدوية)
- [ ] Prescription verification
- [ ] Dispensing workflow
- [ ] Patient counseling notes
- [ ] Dispensing label printing
- [ ] Controlled substance tracking

#### 3.4.4 Procurement (شراء الكميات)
- [ ] Supplier management
- [ ] Purchase requisitions
- [ ] Price comparison
- [ ] Order tracking

---

### 3.5 Room & Bed Management (خدمات الغرف)

> **Priority: MEDIUM**

#### 3.5.1 Room Configuration
- [ ] Room type definitions (ICU, Ward, Private, etc.)
- [ ] Bed configuration per room
- [ ] Room amenities tracking
- [ ] Room pricing

#### 3.5.2 Availability Management (إتاحة الغرف والأسرة)
- [ ] Real-time bed availability dashboard
- [ ] Bed status tracking (Available, Occupied, Reserved, Maintenance)
- [ ] Housekeeping integration
- [ ] Room assignment to reception

#### 3.5.3 Reservation System (تنظيم الحجز)
- [ ] Advance room booking
- [ ] Room upgrade/downgrade
- [ ] Patient transfer between rooms
- [ ] Discharge planning

---

### 3.6 Nursing Services (خدمات التمريض)

> **Priority: MEDIUM**

#### 3.6.1 Doctor Orders Execution (صرف أوامر الدكتور)
- [ ] Medication administration records (MAR)
- [ ] Vital signs documentation
- [ ] Nursing care plans
- [ ] Order acknowledgment workflow
- [ ] Nursing shift handover

#### 3.6.2 Patient Care
- [ ] Patient rounds documentation
- [ ] Pain assessment
- [ ] Fall risk assessment
- [ ] Wound care tracking
- [ ] Input/Output charting

---

### 3.7 User Activity Logging System (نظام تسجيل نشاط المستخدمين)

> **Priority: HIGH - Required in All Modules**

#### 3.7.1 Audit Trail
- [ ] User login/logout tracking
- [ ] Session management
- [ ] Failed login attempts
- [ ] IP address logging

#### 3.7.2 Data Change Logging
- [ ] Create operations logging
- [ ] Update operations logging (before/after values)
- [ ] Delete operations logging (soft delete tracking)
- [ ] Bulk operation logging

#### 3.7.3 Activity Reports
- [ ] User activity timeline
- [ ] Module-wise activity reports
- [ ] Data access reports
- [ ] Suspicious activity alerts
- [ ] Audit log search and filtering

#### 3.7.4 Compliance Features
- [ ] Log retention policies
- [ ] Log export (CSV, Excel, PDF)
- [ ] Log archival
- [ ] HIPAA/local regulation compliance

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

### Phase 1 (First Delivery) - Core Reception
| Module | Features | Duration |
|--------|----------|----------|
| Patient Management | Full CRUD, search, demographics | 2 weeks |
| Insurance Management | Basic enrollment, verification | 1 week |
| Payment Processing | Cash, deferred payments | 1 week |
| Appointment Booking | Clinic, lab, emergency | 2 weeks |
| Referral System | Basic referrals, prescriptions | 1 week |
| **Activity Logging** | Core logging infrastructure | 1 week |

**Total Phase 1: ~8 weeks**

### Phase 2 - Pharmacy & Support Services
| Module | Features | Duration |
|--------|----------|----------|
| Pharmacy Module | Full implementation | 3 weeks |
| Room Management | Full implementation | 2 weeks |

**Total Phase 2: ~5 weeks**

### Phase 3 - Clinical & Nursing
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

**Document Version:** 1.0  
**Created:** January 25, 2026  
**Status:** Draft - Pending Approval
