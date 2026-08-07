using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;

using System.Linq;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Authorization.Permissions;

namespace HIS.Identity;

public class IdentityDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IdentityUserManager _userManager;
    private readonly IdentityRoleManager _roleManager;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;
    private readonly IPermissionManager _permissionManager;
    private readonly IPermissionDefinitionManager _permissionDefinitionManager;

    public IdentityDataSeedContributor(
        IdentityUserManager userManager,
        IdentityRoleManager roleManager,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant,
        IPermissionManager permissionManager,
        IPermissionDefinitionManager permissionDefinitionManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
        _permissionManager = permissionManager;
        _permissionDefinitionManager = permissionDefinitionManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        await CreateRoleAndUserAsync("Doctor", "doctor", "doctor@his.com", "Doctor", "User", "doctor");
        await CreateRoleAndUserAsync("Nurse", "nurse", "nurse@his.com", "Nurse", "User", "nurse");
        await CreateRoleAndUserAsync("AdminStaff", "adminstaff", "adminstaff@his.com", "Admin", "Staff", "adminstaff");
        await CreateRoleAndUserAsync("LabTechnician", "labtech", "labtech@his.com", "Lab", "Technician", "labtech");
        await CreateRoleAndUserAsync("LabManager", "labmanager", "labmanager@his.com", "Lab", "Manager", "labmanager");
        await CreateRoleAndUserAsync("RadiologyTechnician", "radtech", "radtech@his.com", "Radiology", "Technician", "radtech");
        await CreateRoleAndUserAsync("RadiologyManager", "radmanager", "radmanager@his.com", "Radiology", "Manager", "radmanager");
        await CreateRoleAndUserAsync("Pharmacist", "pharmacist", "pharmacist@his.com", "Pharmacist", "User", "pharmacist");
        await CreateRoleAndUserAsync("Receptionist", "receptionist", "receptionist@his.com", "Receptionist", "User", "receptionist");
        
        await CreateRoleAndUserAsync("Security", "security", "security@his.com", "Security", "Officer", "security");
        await CreateRoleAndUserAsync("StoreKeeper", "storekeeper", "storekeeper@his.com", "Store", "Keeper", "storekeeper");
        await CreateRoleAndUserAsync("PatientsUser", "patient", "patient@his.com", "Patient", "User", "patient");

        // Grant Permissions
        await GrantPermissionsAsync("Security", new[] { "HIS.Settings" });
        await GrantPermissionsAsync("Receptionist", new[] { 
            "HIS.Reception",
            "HIS.Patients", "HIS.Patients.Create", "HIS.Patients.Edit", 
            "HIS.Appointments", "HIS.Appointments.Create", "HIS.Appointments.Edit",
            "HIS.Reception.Tickets"
        });
        await GrantPermissionsAsync("LabManager", new[] { 
            "HIS.Laboratory", "HIS.Laboratory.CreateSample", "HIS.Laboratory.UpdateResults", 
            "HIS.Laboratory.ApproveResults", "HIS.Laboratory.Catalog", "HIS.Laboratory.Requests", "HIS.Laboratory.Appointments"
        });
        await GrantPermissionsAsync("LabTechnician", new[] { 
            "HIS.Laboratory", "HIS.Laboratory.CreateSample", "HIS.Laboratory.UpdateResults", "HIS.Laboratory.Catalog", "HIS.Laboratory.Requests"
        });
        await GrantPermissionsAsync("StoreKeeper", new[] { 
            "HIS.Inventory", "HIS.Inventory.ManageWarehouses", "HIS.Inventory.StockOperations",
            "HIS.Inventory.Suppliers", "HIS.Inventory.PurchaseOrders", "HIS.Inventory.DepartmentalConsumption"
        });
        await GrantPermissionsAsync("PatientsUser", new[] { 
            "HIS.Patients", "HIS.Appointments", "HIS.Billing" 
        });

        await GrantPermissionsAsync("Pharmacist", new[] { 
            "HIS.Pharmacy", "HIS.Pharmacy.Prescriptions", "HIS.Pharmacy.Dispensing", "HIS.Pharmacy.Stock", "HIS.Pharmacy.POS"
        });

        var allPermissions = await _permissionDefinitionManager.GetPermissionsAsync();
        var allPermissionNames = allPermissions.Select(p => p.Name).ToArray();

        // Grant ALL permissions to AdminStaff
        await GrantPermissionsAsync("AdminStaff", allPermissionNames);

        // Grant ALL permissions to admin
        await GrantPermissionsAsync("admin", allPermissionNames);
        
        // Ensure Admin has EVERYTHING (Root + All Children)
        await GrantPermissionsAsync("Admin", allPermissionNames);
        
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
                try
                {
                    await _permissionManager.SetForRoleAsync(roleName, permission, true);
                }
                catch (System.ApplicationException)
                {
                    // Some built-in ABP permissions (e.g. AbpIdentity.UserLookup) are
                    // not compatible with the Role provider and must be skipped.
                }
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
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
        {
            user = new IdentityUser(
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
        else
        {
            // Reset password to match the requested password
            if(await _userManager.HasPasswordAsync(user))
            {
                await _userManager.RemovePasswordAsync(user);
            }
            await _userManager.AddPasswordAsync(user, password);
            
            // Ensure they are in the role
            if (!await _userManager.IsInRoleAsync(user, roleName))
            {
                await _userManager.AddToRoleAsync(user, roleName);
            }
        }
    }
}
