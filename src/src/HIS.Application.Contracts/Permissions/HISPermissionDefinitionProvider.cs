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
        hisGroup.AddPermission(HISPermissions.Settings.Default, L("Permission:Settings"));

        // Patients
        var patients = hisGroup.AddPermission(HISPermissions.Patients.Default, L("Permission:Patients"));
        patients.AddChild(HISPermissions.Patients.Create, L("Permission:Create"));
        patients.AddChild(HISPermissions.Patients.Edit, L("Permission:Edit"));
        patients.AddChild(HISPermissions.Patients.Delete, L("Permission:Delete"));

        // Appointments
        var appointments = hisGroup.AddPermission(HISPermissions.Appointments.Default, L("Permission:Appointments"));
        appointments.AddChild(HISPermissions.Appointments.Create, L("Permission:Create"));
        appointments.AddChild(HISPermissions.Appointments.Edit, L("Permission:Edit"));
        appointments.AddChild(HISPermissions.Appointments.Delete, L("Permission:Delete"));

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

        // Emergency
        var emergency = hisGroup.AddPermission(HISPermissions.Emergency.Default, L("Permission:Emergency"));
        emergency.AddChild(HISPermissions.Emergency.Dashboard, L("Permission:EmergencyDashboard"));

        // Inventory
        var inventory = hisGroup.AddPermission(HISPermissions.Inventory.Default, L("Permission:Inventory"));
        inventory.AddChild(HISPermissions.Inventory.ManageWarehouses, L("Permission:ManageWarehouses"));
        inventory.AddChild(HISPermissions.Inventory.StockOperations, L("Permission:StockOperations"));
        inventory.AddChild(HISPermissions.Inventory.Dashboard, L("Permission:Dashboard"));

        // Billing
        var billing = hisGroup.AddPermission(HISPermissions.Billing.Default, L("Permission:Billing"));
        billing.AddChild(HISPermissions.Billing.ManageInvoices, L("Permission:ManageInvoices"));
        billing.AddChild(HISPermissions.Billing.ChartOfAccounts, L("Permission:ChartOfAccounts"));
        billing.AddChild(HISPermissions.Billing.JournalEntries, L("Permission:JournalEntries"));
        billing.AddChild(HISPermissions.Billing.Payments, L("Permission:Payments"));
        billing.AddChild(HISPermissions.Billing.DeferredPayments, L("Permission:DeferredPayments"));

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
        drugs.AddChild(HISPermissions.Pharmacy.DrugsCreate, L("Permission:Create"));
        drugs.AddChild(HISPermissions.Pharmacy.DrugsEdit, L("Permission:Edit"));
        drugs.AddChild(HISPermissions.Pharmacy.DrugsDelete, L("Permission:Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<HISResource>(name);
    }
}
