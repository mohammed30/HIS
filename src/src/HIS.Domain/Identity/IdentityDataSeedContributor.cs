using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

using Volo.Abp.PermissionManagement;

namespace HIS.Identity;

public class IdentityDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IdentityUserManager _userManager;
    private readonly IdentityRoleManager _roleManager;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;
    private readonly IPermissionManager _permissionManager;

    public IdentityDataSeedContributor(
        IdentityUserManager userManager,
        IdentityRoleManager roleManager,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant,
        IPermissionManager permissionManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
        _permissionManager = permissionManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        await CreateRoleAndUserAsync("Doctor", "doctor", "doctor@his.com", "Doctor", "User", "Abc.123");
        await CreateRoleAndUserAsync("Nurse", "nurse", "nurse@his.com", "Nurse", "User", "Abc.123");
        await CreateRoleAndUserAsync("AdminStaff", "adminstaff", "adminstaff@his.com", "Admin", "Staff", "Abc.123");
        await CreateRoleAndUserAsync("LabTechnician", "labtech", "labtech@his.com", "Lab", "Technician", "Abc.123");
        await CreateRoleAndUserAsync("LabManager", "labmanager", "labmanager@his.com", "Lab", "Manager", "Abc.123");
        await CreateRoleAndUserAsync("RadiologyTechnician", "radtech", "radtech@his.com", "Radiology", "Technician", "Abc.123");
        await CreateRoleAndUserAsync("RadiologyManager", "radmanager", "radmanager@his.com", "Radiology", "Manager", "Abc.123");
        await CreateRoleAndUserAsync("Pharmacist", "pharmacist", "pharmacist@his.com", "Pharmacist", "User", "Abc.123");
        await CreateRoleAndUserAsync("Receptionist", "receptionist", "receptionist@his.com", "Receptionist", "User", "Abc.123");
        
        await CreateRoleAndUserAsync("Security", "security", "security@his.com", "Security", "Officer", "Abc.123");
        await CreateRoleAndUserAsync("StoreKeeper", "storekeeper", "storekeeper@his.com", "Store", "Keeper", "Abc.123");
        await CreateRoleAndUserAsync("PatientsUser", "patient", "patient@his.com", "Patient", "User", "Abc.123");

        // Grant Permissions
        await GrantPermissionsAsync("Security", new[] { "HIS.Settings" });
        await GrantPermissionsAsync("Receptionist", new[] { 
            "HIS.Reception",
            "HIS.Patients", "HIS.Patients.Create", "HIS.Patients.Edit", 
            "HIS.Appointments", "HIS.Appointments.Create", "HIS.Appointments.Edit",
            "HIS.Reception.Tickets"
        });
        await GrantPermissionsAsync("LabManager", new[] { 
            "HIS.Laboratory", "HIS.Laboratory.CreateSample", "HIS.Laboratory.UpdateResults", "HIS.Laboratory.ApproveResults" 
        });
        await GrantPermissionsAsync("LabTechnician", new[] { 
            "HIS.Laboratory", "HIS.Laboratory.CreateSample", "HIS.Laboratory.UpdateResults" 
        });
        await GrantPermissionsAsync("StoreKeeper", new[] { 
            "HIS.Inventory", "HIS.Inventory.ManageWarehouses", "HIS.Inventory.StockOperations",
            "HIS.Inventory.Suppliers", "HIS.Inventory.PurchaseOrders", "HIS.Inventory.DepartmentalConsumption"
        });
        await GrantPermissionsAsync("PatientsUser", new[] { 
            "HIS.Patients", "HIS.Appointments", "HIS.Billing" 
        });

        await GrantPermissionsAsync("Pharmacist", new[] { 
            "HIS.Pharmacy", "HIS.Pharmacy.Prescriptions", "HIS.Pharmacy.Dispensing", "HIS.Pharmacy.Stock" 
        });

        await GrantPermissionsAsync("AdminStaff", new[] { 
            // Settings
            "HIS.Settings",
            // Patients
            "HIS.Patients", "HIS.Patients.Create", "HIS.Patients.Edit", "HIS.Patients.Delete",
            // Appointments
            "HIS.Appointments", "HIS.Appointments.Create", "HIS.Appointments.Edit", "HIS.Appointments.Delete",
            // Reception
            "HIS.Reception", "HIS.Reception.LaboratoryReception", "HIS.Reception.Tickets",
            "HIS.Reception.InsuranceCompanies", "HIS.Reception.InsurancePlans", "HIS.Reception.Invoices", "HIS.Reception.Payments",
            // Laboratory
            "HIS.Laboratory", "HIS.Laboratory.CreateSample", "HIS.Laboratory.UpdateResults", "HIS.Laboratory.ApproveResults",
            "HIS.Laboratory.Catalog", "HIS.Laboratory.Requests", "HIS.Laboratory.Appointments",
            // Emergency
            "HIS.Emergency", "HIS.Emergency.Dashboard",
            // Pharmacy
            "HIS.Pharmacy", "HIS.Pharmacy.Prescriptions", "HIS.Pharmacy.Dispensing", "HIS.Pharmacy.Stock",
            "HIS.Pharmacy.Drugs", "HIS.Pharmacy.Drugs.Create", "HIS.Pharmacy.Drugs.Edit", "HIS.Pharmacy.Drugs.Delete", "HIS.Pharmacy.POS",
            // Billing & Accounting
            "HIS.Billing", "HIS.Billing.ManageInvoices", "HIS.Billing.ChartOfAccounts", "HIS.Billing.JournalEntries",
            "HIS.Billing.Payments", "HIS.Billing.DeferredPayments",
            "HIS.Billing.FinancialReports", "HIS.Billing.FinancialReports.DailyReport", "HIS.Billing.FinancialReports.CustomerDebtsReport",
            "HIS.Billing.FinancialReports.DiscountsReport", "HIS.Billing.FinancialReports.IncomeStatement",
            "HIS.Billing.FinancialReports.BalanceSheet", "HIS.Billing.FinancialReports.AccountStatement",
            "HIS.Billing.ReceiptVouchers", "HIS.Billing.PaymentVouchers", "HIS.Billing.BankTransactions", "HIS.Billing.ContractClaims",
            // Definitions
            "HIS.Definitions", "HIS.Definitions.Nationalities", "HIS.Definitions.Professions", "HIS.Definitions.Contracts", 
            "HIS.Definitions.PatientCategories", "HIS.Definitions.ReferralSources", "HIS.Definitions.Services", 
            "HIS.Definitions.Radiology", "HIS.Definitions.PriceLists", "HIS.Definitions.PaymentMethods",
            // Inventory
            "HIS.Inventory", "HIS.Inventory.Dashboard", "HIS.Inventory.ManageWarehouses", "HIS.Inventory.StockOperations",
            "HIS.Inventory.Suppliers", "HIS.Inventory.PurchaseRequisitions", "HIS.Inventory.PurchaseOrders", "HIS.Inventory.DepartmentalConsumption",
            // Nursing
            "HIS.Nursing", "HIS.Nursing.PatientList", "HIS.Nursing.VitalSigns", "HIS.Nursing.MedicationAdministration",
            "HIS.Nursing.CarePlans", "HIS.Nursing.Assessments", "HIS.Nursing.FluidBalance", "HIS.Nursing.ShiftHandover",
            // Inpatient
            "HIS.Inpatient",
            "HIS.Inpatient.Rooms", "HIS.Inpatient.Rooms.Create", "HIS.Inpatient.Rooms.Edit", "HIS.Inpatient.Rooms.Delete",
            "HIS.Inpatient.Admissions", "HIS.Inpatient.Admissions.Create", "HIS.Inpatient.Admissions.Edit", "HIS.Inpatient.Admissions.Delete",
            "HIS.Inpatient.Reservations", "HIS.Inpatient.Reservations.Create", "HIS.Inpatient.Reservations.Edit", "HIS.Inpatient.Reservations.Delete",
            "HIS.Inpatient.Dashboard",
            // Operations
            "HIS.Operations", "HIS.Operations.Manage", "HIS.Operations.PrintTicket", "HIS.Operations.Report",
            // HR (Personnel Affairs / شئون العاملين)
            "HIS.HR", "HIS.HR.Employees", "HIS.HR.Employees.Create", "HIS.HR.Employees.Edit", "HIS.HR.Employees.Delete",
            "HIS.HR.CompensationItems", "HIS.HR.LeaveTypes", "HIS.HR.EmployeeLeaves", "HIS.HR.Loans",
            "HIS.HR.Payroll", "HIS.HR.Payroll.Process", "HIS.HR.Penalties", "HIS.HR.Attendance",
            "HIS.HR.Reports", "HIS.HR.PaySlip"
        });
        
        // Grant HR permissions to Admin too
        await GrantPermissionsAsync("Admin", new[] {
            "HIS.HR", "HIS.HR.Employees", "HIS.HR.Employees.Create", "HIS.HR.Employees.Edit", "HIS.HR.Employees.Delete",
            "HIS.HR.CompensationItems", "HIS.HR.LeaveTypes", "HIS.HR.EmployeeLeaves", "HIS.HR.Loans",
            "HIS.HR.Payroll", "HIS.HR.Payroll.Process", "HIS.HR.Penalties", "HIS.HR.Attendance",
            "HIS.HR.Reports", "HIS.HR.PaySlip"
        });
        
        // Ensure Admin has EVERYTHING (Root + All Children)
        await GrantPermissionsAsync("admin", new[] { 
            "HIS.Settings",
            "HIS.Patients", "HIS.Patients.Create", "HIS.Patients.Edit", "HIS.Patients.Delete",
            "HIS.Appointments", "HIS.Appointments.Create", "HIS.Appointments.Edit", "HIS.Appointments.Delete",
            "HIS.Reception", "HIS.Reception.LaboratoryReception", "HIS.Reception.Tickets", 
            "HIS.Reception.InsuranceCompanies", "HIS.Reception.InsurancePlans", "HIS.Reception.Invoices", "HIS.Reception.Payments",
            "HIS.Laboratory", "HIS.Laboratory.CreateSample", "HIS.Laboratory.UpdateResults", "HIS.Laboratory.ApproveResults", 
            "HIS.Laboratory.Catalog", "HIS.Laboratory.Requests", "HIS.Laboratory.Appointments",
            "HIS.Emergency", "HIS.Emergency.Dashboard",
            "HIS.Pharmacy", "HIS.Pharmacy.Prescriptions", "HIS.Pharmacy.Dispensing", "HIS.Pharmacy.Stock", "HIS.Pharmacy.Drugs",
            "HIS.Pharmacy.Drugs.Create", "HIS.Pharmacy.Drugs.Edit", "HIS.Pharmacy.Drugs.Delete", "HIS.Pharmacy.POS",
            "HIS.Inventory", "HIS.Inventory.Dashboard", "HIS.Inventory.ManageWarehouses", "HIS.Inventory.StockOperations", 
            "HIS.Inventory.Suppliers", "HIS.Inventory.PurchaseRequisitions", "HIS.Inventory.PurchaseOrders", "HIS.Inventory.DepartmentalConsumption",
            "HIS.Billing", "HIS.Billing.ManageInvoices", "HIS.Billing.ChartOfAccounts", "HIS.Billing.JournalEntries", 
            "HIS.Billing.Payments", "HIS.Billing.DeferredPayments",
            "HIS.Billing.FinancialReports", "HIS.Billing.FinancialReports.DailyReport", "HIS.Billing.FinancialReports.CustomerDebtsReport",
            "HIS.Billing.FinancialReports.DiscountsReport", "HIS.Billing.FinancialReports.IncomeStatement",
            "HIS.Billing.FinancialReports.BalanceSheet", "HIS.Billing.FinancialReports.AccountStatement",
            "HIS.Billing.ReceiptVouchers", "HIS.Billing.PaymentVouchers", "HIS.Billing.BankTransactions", "HIS.Billing.ContractClaims",
            "HIS.Definitions", "HIS.Definitions.Nationalities", "HIS.Definitions.Professions", "HIS.Definitions.Contracts", 
            "HIS.Definitions.PatientCategories", "HIS.Definitions.ReferralSources", "HIS.Definitions.Services", 
            "HIS.Definitions.Radiology", "HIS.Definitions.PriceLists", "HIS.Definitions.PaymentMethods",
            "HIS.Nursing", "HIS.Nursing.PatientList", "HIS.Nursing.VitalSigns", "HIS.Nursing.MedicationAdministration", 
            "HIS.Nursing.CarePlans", "HIS.Nursing.Assessments", "HIS.Nursing.FluidBalance", "HIS.Nursing.ShiftHandover",
            "HIS.Inpatient", 
            "HIS.Inpatient.Rooms", "HIS.Inpatient.Rooms.Create", "HIS.Inpatient.Rooms.Edit", "HIS.Inpatient.Rooms.Delete",
            "HIS.Inpatient.Admissions", "HIS.Inpatient.Admissions.Create", "HIS.Inpatient.Admissions.Edit", "HIS.Inpatient.Admissions.Delete",
            "HIS.Inpatient.Reservations", "HIS.Inpatient.Reservations.Create", "HIS.Inpatient.Reservations.Edit", "HIS.Inpatient.Reservations.Delete",
            "HIS.Inpatient.Dashboard",
            "HIS.Operations", "HIS.Operations.Manage", "HIS.Operations.PrintTicket", "HIS.Operations.Report",
            // HR
            "HIS.HR", "HIS.HR.Employees", "HIS.HR.Employees.Create", "HIS.HR.Employees.Edit", "HIS.HR.Employees.Delete",
            "HIS.HR.CompensationItems", "HIS.HR.LeaveTypes", "HIS.HR.EmployeeLeaves", "HIS.HR.Loans",
            "HIS.HR.Payroll", "HIS.HR.Payroll.Process", "HIS.HR.Penalties", "HIS.HR.Attendance",
            "HIS.HR.Reports", "HIS.HR.PaySlip"
        });
        
        await SetAdminPasswordAsync();
    }

    private async Task SetAdminPasswordAsync()
    {
        var adminUser = await _userManager.FindByNameAsync("admin");
        if (adminUser != null)
        {
            // Simple way to reset: Remove then Add. 
            // Note: In production, do not hardcode passwords or force reset indiscriminately.
            if(await _userManager.HasPasswordAsync(adminUser))
            {
                await _userManager.RemovePasswordAsync(adminUser);
            }
            await _userManager.AddPasswordAsync(adminUser, "Abc.123");
        }
    }

    private async Task GrantPermissionsAsync(string roleName, string[] permissions)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role != null)
        {
            foreach (var permission in permissions)
            {
                await _permissionManager.SetForRoleAsync(roleName, permission, true);
            }
        }
    }

    private async Task CreateRoleAndUserAsync(
        string roleName,
        string username,
        string email,
        string name,
        string surname,
        string password)
    {
        // 1. Create Role
        if (await _roleManager.FindByNameAsync(roleName) == null)
        {
            await _roleManager.CreateAsync(new IdentityRole(_guidGenerator.Create(), roleName, _currentTenant.Id));
        }

        // 2. Create User
        if (await _userManager.FindByNameAsync(username) == null)
        {
            var user = new IdentityUser(
                _guidGenerator.Create(),
                username,
                email,
                _currentTenant.Id
            )
            {
                Name = name,
                Surname = surname
            };

            var result = await _userManager.CreateAsync(user, password);
            
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }
        }
    }
}
