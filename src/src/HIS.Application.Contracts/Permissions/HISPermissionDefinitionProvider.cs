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

        // Laboratory
        var laboratory = hisGroup.AddPermission(HISPermissions.Laboratory.Default, L("Permission:Laboratory"));
        laboratory.AddChild(HISPermissions.Laboratory.CreateSample, L("Permission:CreateSample"));
        laboratory.AddChild(HISPermissions.Laboratory.UpdateResults, L("Permission:UpdateResults"));
        laboratory.AddChild(HISPermissions.Laboratory.ApproveResults, L("Permission:ApproveResults"));

        // Inventory
        var inventory = hisGroup.AddPermission(HISPermissions.Inventory.Default, L("Permission:Inventory"));
        inventory.AddChild(HISPermissions.Inventory.ManageWarehouses, L("Permission:ManageWarehouses"));
        inventory.AddChild(HISPermissions.Inventory.StockOperations, L("Permission:StockOperations"));

        // Billing
        var billing = hisGroup.AddPermission(HISPermissions.Billing.Default, L("Permission:Billing"));
        billing.AddChild(HISPermissions.Billing.ManageInvoices, L("Permission:ManageInvoices"));
        billing.AddChild(HISPermissions.Billing.ChartOfAccounts, L("Permission:ChartOfAccounts"));
        billing.AddChild(HISPermissions.Billing.JournalEntries, L("Permission:JournalEntries"));

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
