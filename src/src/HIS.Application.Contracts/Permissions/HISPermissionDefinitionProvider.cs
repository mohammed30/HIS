using HIS.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace HIS.Permissions;

public class HISPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var hisGroup = context.AddGroup(HISPermissions.GroupName, L("Permission:HIS"));

        // Settings
        var settings = hisGroup.AddPermission(HISPermissions.Settings.Default, L("Permission:Settings"));
        settings.AddChild(HISPermissions.Settings.Pharmacy, L("Permission:PharmacySettings"));

        // Patients
        var patients = hisGroup.AddPermission(HISPermissions.Patients.Default, L("Permission:Patients"));
        patients.AddChild(HISPermissions.Patients.Create, L("Permission:PatientsCreate"));
        patients.AddChild(HISPermissions.Patients.Edit, L("Permission:PatientsEdit"));
        patients.AddChild(HISPermissions.Patients.Delete, L("Permission:PatientsDelete"));

        // Appointments
        var appointments = hisGroup.AddPermission(HISPermissions.Appointments.Default, L("Permission:Appointments"));
        appointments.AddChild(HISPermissions.Appointments.Create, L("Permission:AppointmentsCreate"));
        appointments.AddChild(HISPermissions.Appointments.Edit, L("Permission:AppointmentsEdit"));
        appointments.AddChild(HISPermissions.Appointments.Delete, L("Permission:AppointmentsDelete"));

        // Reception
        var reception = hisGroup.AddPermission(HISPermissions.Reception.Default, L("Permission:Reception"));
        reception.AddChild(HISPermissions.Reception.LaboratoryReception, L("Permission:LaboratoryReception"));
        reception.AddChild(HISPermissions.Reception.Tickets, L("Permission:Tickets"));
        reception.AddChild(HISPermissions.Reception.InsuranceCompanies, L("Permission:InsuranceCompanies"));
        reception.AddChild(HISPermissions.Reception.InsurancePlans, L("Permission:InsurancePlans"));
        reception.AddChild(HISPermissions.Reception.Invoices, L("Permission:Invoices"));
        reception.AddChild(HISPermissions.Reception.Payments, L("Permission:Payments"));

        // Laboratory
        var laboratory = hisGroup.AddPermission(HISPermissions.Laboratory.Default, L("Permission:Laboratory"));
        laboratory.AddChild(HISPermissions.Laboratory.CreateSample, L("Permission:CreateSample"));
        laboratory.AddChild(HISPermissions.Laboratory.UpdateResults, L("Permission:UpdateResults"));
        laboratory.AddChild(HISPermissions.Laboratory.ApproveResults, L("Permission:ApproveResults"));
        laboratory.AddChild(HISPermissions.Laboratory.Catalog, L("Permission:Catalog"));
        laboratory.AddChild(HISPermissions.Laboratory.Requests, L("Permission:Requests"));
        laboratory.AddChild(HISPermissions.Laboratory.Appointments, L("Permission:LabAppointments"));

        // Radiology (FIXED: was incorrectly nested under Laboratory)
        var radiology = hisGroup.AddPermission(HISPermissions.Radiology.Default, L("Permission:RadiologyDept"));
        radiology.AddChild(HISPermissions.Radiology.Requests, L("Permission:RadiologyRequests"));

        // Emergency
        var emergency = hisGroup.AddPermission(HISPermissions.Emergency.Default, L("Permission:Emergency"));
        emergency.AddChild(HISPermissions.Emergency.Dashboard, L("Permission:EmergencyDashboard"));

        // Inventory
        var inventory = hisGroup.AddPermission(HISPermissions.Inventory.Default, L("Permission:Inventory"));
        inventory.AddChild(HISPermissions.Inventory.ManageWarehouses, L("Permission:ManageWarehouses"));
        inventory.AddChild(HISPermissions.Inventory.StockOperations, L("Permission:StockOperations"));
        inventory.AddChild(HISPermissions.Inventory.Dashboard, L("Permission:Dashboard"));
        inventory.AddChild(HISPermissions.Inventory.Suppliers, L("Permission:Suppliers"));
        inventory.AddChild(HISPermissions.Inventory.PurchaseRequisitions, L("Permission:PurchaseRequisitions"));
        inventory.AddChild(HISPermissions.Inventory.PurchaseOrders, L("Permission:PurchaseOrders"));
        inventory.AddChild(HISPermissions.Inventory.DepartmentalConsumption, L("Permission:DepartmentalConsumption"));

        // Billing
        var billing = hisGroup.AddPermission(HISPermissions.Billing.Default, L("Permission:Billing"));
        billing.AddChild(HISPermissions.Billing.ManageInvoices, L("Permission:ManageInvoices"));
        billing.AddChild(HISPermissions.Billing.ChartOfAccounts, L("Permission:ChartOfAccounts"));
        var journalEntries = billing.AddChild(HISPermissions.Billing.JournalEntries, L("Permission:JournalEntries"));
        journalEntries.AddChild(HISPermissions.Billing.JournalEntriesPost, L("Permission:JournalEntriesPost"));
        billing.AddChild(HISPermissions.Billing.Payments, L("Permission:Payments"));
        billing.AddChild(HISPermissions.Billing.DeferredPayments, L("Permission:DeferredPayments"));
        var financialReports = billing.AddChild(HISPermissions.Billing.FinancialReports, L("Permission:FinancialReports"));
        financialReports.AddChild(HISPermissions.Billing.DailyReport, L("Permission:DailyReport"));
        financialReports.AddChild(HISPermissions.Billing.CustomerDebtsReport, L("Permission:CustomerDebtsReport"));
        financialReports.AddChild(HISPermissions.Billing.DiscountsReport, L("Permission:DiscountsReport"));
        financialReports.AddChild(HISPermissions.Billing.IncomeStatement, L("Permission:IncomeStatement"));
        financialReports.AddChild(HISPermissions.Billing.BalanceSheet, L("Permission:BalanceSheet"));
        financialReports.AddChild(HISPermissions.Billing.AccountStatement, L("Permission:AccountStatement"));
        billing.AddChild(HISPermissions.Billing.ReceiptVouchers, L("Permission:ReceiptVouchers"));
        billing.AddChild(HISPermissions.Billing.PaymentVouchers, L("Permission:PaymentVouchers"));
        billing.AddChild(HISPermissions.Billing.BankTransactions, L("Permission:BankTransactions"));
        billing.AddChild(HISPermissions.Billing.ContractClaims, L("Permission:ContractClaims"));

        // Definitions
        var definitions = hisGroup.AddPermission(HISPermissions.Definitions.Default, L("Permission:Definitions"));
        definitions.AddChild(HISPermissions.Definitions.Nationalities, L("Permission:Nationalities"));
        definitions.AddChild(HISPermissions.Definitions.Professions, L("Permission:Professions"));
        definitions.AddChild(HISPermissions.Definitions.Contracts, L("Permission:Contracts"));
        definitions.AddChild(HISPermissions.Definitions.PatientCategories, L("Permission:PatientCategories"));
        definitions.AddChild(HISPermissions.Definitions.ReferralSources, L("Permission:ReferralSources"));
        definitions.AddChild(HISPermissions.Definitions.Services, L("Permission:Services"));
        definitions.AddChild(HISPermissions.Definitions.Radiology, L("Permission:Radiology"));
        definitions.AddChild(HISPermissions.Definitions.PriceLists, L("Permission:PriceLists"));
        definitions.AddChild(HISPermissions.Definitions.PaymentMethods, L("Permission:PaymentMethods"));

        // Pharmacy
        var pharmacy = hisGroup.AddPermission(HISPermissions.Pharmacy.Default, L("Permission:Pharmacy"));
        pharmacy.AddChild(HISPermissions.Pharmacy.Prescriptions, L("Permission:Prescriptions"));
        pharmacy.AddChild(HISPermissions.Pharmacy.Dispensing, L("Permission:Dispensing"));
        pharmacy.AddChild(HISPermissions.Pharmacy.Stock, L("Permission:Stock"));

        var drugs = pharmacy.AddChild(HISPermissions.Pharmacy.Drugs, L("Permission:Drugs"));
        drugs.AddChild(HISPermissions.Pharmacy.DrugsCreate, L("Permission:DrugsCreate"));
        drugs.AddChild(HISPermissions.Pharmacy.DrugsEdit, L("Permission:DrugsEdit"));
        drugs.AddChild(HISPermissions.Pharmacy.DrugsDelete, L("Permission:DrugsDelete"));
        pharmacy.AddChild(HISPermissions.Pharmacy.POS, L("Permission:PharmacyPOS"));

        // Nursing
        var nursing = hisGroup.AddPermission(HISPermissions.Nursing.Default, L("Permission:Nursing"));
        nursing.AddChild(HISPermissions.Nursing.PatientList, L("Permission:PatientList"));
        nursing.AddChild(HISPermissions.Nursing.VitalSigns, L("Permission:VitalSigns"));
        nursing.AddChild(HISPermissions.Nursing.MedicationAdministration, L("Permission:MedicationAdministration"));
        nursing.AddChild(HISPermissions.Nursing.CarePlans, L("Permission:CarePlans"));
        nursing.AddChild(HISPermissions.Nursing.Assessments, L("Permission:Assessments"));
        nursing.AddChild(HISPermissions.Nursing.FluidBalance, L("Permission:FluidBalance"));
        nursing.AddChild(HISPermissions.Nursing.ShiftHandover, L("Permission:ShiftHandover"));
        nursing.AddChild(HISPermissions.Nursing.InternalRequestReturn, L("Permission:InternalRequestReturn"));

        // Inpatient
        var inpatient = hisGroup.AddPermission(HISPermissions.Inpatient.Default, L("Permission:Inpatient"));
        var rooms = inpatient.AddChild(HISPermissions.Inpatient.Rooms, L("Permission:RoomManagement"));
        rooms.AddChild(HISPermissions.Inpatient.RoomsCreate, L("Permission:RoomsCreate"));
        rooms.AddChild(HISPermissions.Inpatient.RoomsEdit, L("Permission:RoomsEdit"));
        rooms.AddChild(HISPermissions.Inpatient.RoomsDelete, L("Permission:RoomsDelete"));
        
        var admissions = inpatient.AddChild(HISPermissions.Inpatient.Admissions, L("Permission:Admissions"));
        admissions.AddChild(HISPermissions.Inpatient.AdmissionsCreate, L("Permission:AdmissionsCreate"));
        admissions.AddChild(HISPermissions.Inpatient.AdmissionsEdit, L("Permission:AdmissionsEdit"));
        admissions.AddChild(HISPermissions.Inpatient.AdmissionsDelete, L("Permission:AdmissionsDelete"));

        var reservations = inpatient.AddChild(HISPermissions.Inpatient.Reservations, L("Permission:Reservations"));
        reservations.AddChild(HISPermissions.Inpatient.ReservationsCreate, L("Permission:ReservationsCreate"));
        reservations.AddChild(HISPermissions.Inpatient.ReservationsEdit, L("Permission:ReservationsEdit"));
        reservations.AddChild(HISPermissions.Inpatient.ReservationsDelete, L("Permission:ReservationsDelete"));
        
        inpatient.AddChild(HISPermissions.Inpatient.Dashboard, L("Permission:InpatientDashboard"));

        // Operations
        var operations = hisGroup.AddPermission(HISPermissions.Operations.Default, L("Permission:Operations"));
        operations.AddChild(HISPermissions.Operations.PrintTicket, L("Permission:PrintTicket"));
        operations.AddChild(HISPermissions.Operations.Manage, L("Permission:Manage"));
        operations.AddChild(HISPermissions.Operations.Report, L("Permission:OperationsReport"));

        // HR (شؤون العاملين)
        var hr = hisGroup.AddPermission(HISPermissions.HR.Default, L("Permission:HR"));
        var hrEmployees = hr.AddChild(HISPermissions.HR.Employees, L("Permission:HREmployees"));
        hrEmployees.AddChild(HISPermissions.HR.EmployeesCreate, L("Permission:EmployeesCreate"));
        hrEmployees.AddChild(HISPermissions.HR.EmployeesEdit, L("Permission:EmployeesEdit"));
        hrEmployees.AddChild(HISPermissions.HR.EmployeesDelete, L("Permission:EmployeesDelete"));
        hr.AddChild(HISPermissions.HR.CompensationItems, L("Permission:HRCompensationItems"));
        hr.AddChild(HISPermissions.HR.LeaveTypes, L("Permission:HRLeaveTypes"));
        hr.AddChild(HISPermissions.HR.EmployeeLeaves, L("Permission:HREmployeeLeaves"));
        hr.AddChild(HISPermissions.HR.Loans, L("Permission:HRLoans"));
        var hrPayroll = hr.AddChild(HISPermissions.HR.Payroll, L("Permission:HRPayroll"));
        hrPayroll.AddChild(HISPermissions.HR.PayrollProcess, L("Permission:HRPayrollProcess"));
        hr.AddChild(HISPermissions.HR.Penalties, L("Permission:HRPenalties"));
        hr.AddChild(HISPermissions.HR.Attendance, L("Permission:HRAttendance"));
        hr.AddChild(HISPermissions.HR.Reports, L("Permission:HRReports"));
        hr.AddChild(HISPermissions.HR.PaySlip, L("Permission:HRPaySlip"));

        // Activity Logs
        hisGroup.AddPermission(HISPermissions.ActivityLogs.Default, L("Permission:ActivityLogs"));

        // Notifications
        var notifications = hisGroup.AddPermission(HISPermissions.Notifications.Default, L("Permission:Notifications"));
        notifications.AddChild(HISPermissions.Notifications.Manage, L("Permission:NotificationsManage"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<HISResource>(name);
    }
}
