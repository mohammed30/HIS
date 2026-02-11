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
            "HIS.Inventory", "HIS.Inventory.ManageWarehouses", "HIS.Inventory.StockOperations" 
        });
        await GrantPermissionsAsync("PatientsUser", new[] { 
            "HIS.Patients", "HIS.Appointments", "HIS.Billing" 
        });

        await GrantPermissionsAsync("Pharmacist", new[] { 
            "HIS.Pharmacy", "HIS.Pharmacy.Prescriptions", "HIS.Pharmacy.Dispensing", "HIS.Pharmacy.Stock" 
        });

        await GrantPermissionsAsync("AdminStaff", new[] { 
            "HIS.Pharmacy", "HIS.Pharmacy.Prescriptions", "HIS.Pharmacy.Dispensing", "HIS.Pharmacy.Stock" 
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
