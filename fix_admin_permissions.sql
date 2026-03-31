SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET NOCOUNT ON;

-- List of roles to grant permissions to
DECLARE @Roles TABLE (Name NVARCHAR(128));
INSERT INTO @Roles (Name) VALUES ('admin'), ('AdminStaff');

DECLARE @ProviderName NVARCHAR(1) = 'R';

-- List of permissions to grant
DECLARE @Permissions TABLE (Name NVARCHAR(128));

-- HIS Permissions
INSERT INTO @Permissions (Name) VALUES 
('HIS.Settings'),
('HIS.Patients'), ('HIS.Patients.Create'), ('HIS.Patients.Edit'), ('HIS.Patients.Delete'),
('HIS.Appointments'), ('HIS.Appointments.Create'), ('HIS.Appointments.Edit'), ('HIS.Appointments.Delete'),
('HIS.Reception'), ('HIS.Reception.LaboratoryReception'), ('HIS.Reception.Tickets'), ('HIS.Reception.InsuranceCompanies'), ('HIS.Reception.InsurancePlans'), ('HIS.Reception.Invoices'), ('HIS.Reception.Payments'),
('HIS.Laboratory'), ('HIS.Laboratory.CreateSample'), ('HIS.Laboratory.UpdateResults'), ('HIS.Laboratory.ApproveResults'), ('HIS.Laboratory.Catalog'), ('HIS.Laboratory.Requests'), ('HIS.Laboratory.Appointments'),
('HIS.Emergency'), ('HIS.Emergency.Dashboard'),
('HIS.Inventory'), ('HIS.Inventory.ManageWarehouses'), ('HIS.Inventory.StockOperations'), ('HIS.Inventory.Dashboard'), ('HIS.Inventory.Suppliers'), ('HIS.Inventory.PurchaseRequisitions'), ('HIS.Inventory.PurchaseOrders'), ('HIS.Inventory.DepartmentalConsumption'),
('HIS.Billing'), ('HIS.Billing.ManageInvoices'), ('HIS.Billing.ChartOfAccounts'), ('HIS.Billing.JournalEntries'), ('HIS.Billing.Payments'), ('HIS.Billing.DeferredPayments'), ('HIS.Billing.FinancialReports'), ('HIS.Billing.FinancialReports.DailyReport'), ('HIS.Billing.FinancialReports.CustomerDebtsReport'), ('HIS.Billing.FinancialReports.DiscountsReport'), ('HIS.Billing.FinancialReports.IncomeStatement'), ('HIS.Billing.BalanceSheet'), ('HIS.Billing.AccountStatement'), ('HIS.Billing.ReceiptVouchers'), ('HIS.Billing.PaymentVouchers'), ('HIS.Billing.BankTransactions'), ('HIS.Billing.ContractClaims'),
('HIS.Definitions'), ('HIS.Definitions.Nationalities'), ('HIS.Definitions.Professions'), ('HIS.Definitions.Contracts'), ('HIS.Definitions.PatientCategories'), ('HIS.Definitions.ReferralSources'), ('HIS.Definitions.Services'), ('HIS.Definitions.Radiology'), ('HIS.Definitions.PriceLists'), ('HIS.Definitions.PaymentMethods'),
('HIS.Pharmacy'), ('HIS.Pharmacy.Dispensing'), ('HIS.Pharmacy.Prescriptions'), ('HIS.Pharmacy.Stock'), ('HIS.Pharmacy.Drugs'), ('HIS.Pharmacy.Drugs.Create'), ('HIS.Pharmacy.Drugs.Edit'), ('HIS.Pharmacy.Drugs.Delete'), ('HIS.Pharmacy.POS'),
('HIS.Nursing'), ('HIS.Nursing.PatientList'), ('HIS.Nursing.VitalSigns'), ('HIS.Nursing.MedicationAdministration'), ('HIS.Nursing.CarePlans'), ('HIS.Nursing.Assessments'), ('HIS.Nursing.FluidBalance'), ('HIS.Nursing.ShiftHandover'),
('HIS.Inpatient'), ('HIS.Inpatient.Rooms'), ('HIS.Inpatient.Rooms.Create'), ('HIS.Inpatient.Rooms.Edit'), ('HIS.Inpatient.Rooms.Delete'), ('HIS.Inpatient.Admissions'), ('HIS.Inpatient.Admissions.Create'), ('HIS.Inpatient.Admissions.Edit'), ('HIS.Inpatient.Admissions.Delete'), ('HIS.Inpatient.Reservations'), ('HIS.Inpatient.Reservations.Create'), ('HIS.Inpatient.Reservations.Edit'), ('HIS.Inpatient.Reservations.Delete'), ('HIS.Inpatient.Dashboard'),
('HIS.Operations'), ('HIS.Operations.PrintTicket'), ('HIS.Operations.Manage'), ('HIS.Operations.Report'),
('HIS.HR'), ('HIS.HR.Employees'), ('HIS.HR.Employees.Create'), ('HIS.HR.Employees.Edit'), ('HIS.HR.Employees.Delete'), ('HIS.HR.CompensationItems'), ('HIS.HR.LeaveTypes'), ('HIS.HR.EmployeeLeaves'), ('HIS.HR.Loans'), ('HIS.HR.Payroll'), ('HIS.HR.Payroll.Process'), ('HIS.HR.Penalties'), ('HIS.HR.Attendance'), ('HIS.HR.Reports'), ('HIS.HR.PaySlip');

-- ABP Identity Permissions
INSERT INTO @Permissions (Name) VALUES 
('AbpIdentity.Roles'), ('AbpIdentity.Roles.Create'), ('AbpIdentity.Roles.Update'), ('AbpIdentity.Roles.Delete'), ('AbpIdentity.Roles.ManagePermissions'),
('AbpIdentity.Users'), ('AbpIdentity.Users.Create'), ('AbpIdentity.Users.Update'), ('AbpIdentity.Users.Delete'), ('AbpIdentity.Users.ManagePermissions'), ('AbpIdentity.Users.Impersonation'),
('SettingManagement.Emailing'),
('AuditLogging.View'),
('FeatureManagement.ManageHostFeatures');

-- Process Permissions for each role
DECLARE @CurrentRoleName NVARCHAR(128);
DECLARE @AddedCount INT = 0;
DECLARE @PermissionName NVARCHAR(128);

DECLARE role_cursor CURSOR FOR SELECT Name FROM @Roles;
OPEN role_cursor;
FETCH NEXT FROM role_cursor INTO @CurrentRoleName;

WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT 'Processing permissions for role: ' + @CurrentRoleName;
    
    DECLARE perm_cursor CURSOR FOR SELECT Name FROM @Permissions;
    OPEN perm_cursor;
    FETCH NEXT FROM perm_cursor INTO @PermissionName;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM AbpPermissionGrants WHERE [Name] = @PermissionName AND [ProviderKey] = @CurrentRoleName AND [ProviderName] = @ProviderName)
        BEGIN
            INSERT INTO AbpPermissionGrants (Id, [Name], ProviderName, ProviderKey)
            VALUES (NEWID(), @PermissionName, @ProviderName, @CurrentRoleName);
            SET @AddedCount = @AddedCount + 1;
        END
        FETCH NEXT FROM perm_cursor INTO @PermissionName;
    END

    CLOSE perm_cursor;
    DEALLOCATE perm_cursor;
    
    FETCH NEXT FROM role_cursor INTO @CurrentRoleName;
END

CLOSE role_cursor;
DEALLOCATE role_cursor;

DECLARE @TotalPerms INT = (SELECT COUNT(*) FROM @Permissions);
PRINT 'Permissions processed.';
PRINT 'Total Permissions defined per role: ' + CAST(@TotalPerms AS NVARCHAR(10));
PRINT 'Total New Permissions Granted across all roles: ' + CAST(@AddedCount AS NVARCHAR(10));
GO

-- Grant Activity Logs permissions to admin and AdminStaff roles
DECLARE @PermissionsTable TABLE (Name NVARCHAR(128));
INSERT INTO @PermissionsTable (Name) VALUES ('HIS.ActivityLogs'), ('AuditLogging.View');

INSERT INTO AbpPermissionGrants (Id, Name, ProviderName, ProviderKey)
SELECT NEWID(), p.Name, 'R', 'admin'
FROM @PermissionsTable p
WHERE NOT EXISTS (SELECT 1 FROM AbpPermissionGrants WHERE Name = p.Name AND ProviderName = 'R' AND ProviderKey = 'admin');

INSERT INTO AbpPermissionGrants (Id, Name, ProviderName, ProviderKey)
SELECT NEWID(), p.Name, 'R', 'AdminStaff'
FROM @PermissionsTable p
WHERE NOT EXISTS (SELECT 1 FROM AbpPermissionGrants WHERE Name = p.Name AND ProviderName = 'R' AND ProviderKey = 'AdminStaff');
