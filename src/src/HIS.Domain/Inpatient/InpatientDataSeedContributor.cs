using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;


namespace HIS.Inpatient
{
    public class InpatientDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IPermissionManager _permissionManager;
        private readonly ICurrentTenant _currentTenant;

        public InpatientDataSeedContributor(
            IPermissionManager permissionManager,
            ICurrentTenant currentTenant)
        {
            _permissionManager = permissionManager;
            _currentTenant = currentTenant;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            // Grant permissions to admin role or user
            // Assuming 'admin' is the provider key for the admin role
            
            await _permissionManager.SetForRoleAsync("admin", "HIS.Inpatient.Rooms", true);
            await _permissionManager.SetForRoleAsync("admin", "HIS.Inpatient.Rooms.Create", true);
            await _permissionManager.SetForRoleAsync("admin", "HIS.Inpatient.Rooms.Edit", true);
            await _permissionManager.SetForRoleAsync("admin", "HIS.Inpatient.Rooms.Delete", true);

            await _permissionManager.SetForRoleAsync("admin", "HIS.Inpatient.Reservations", true);
            await _permissionManager.SetForRoleAsync("admin", "HIS.Inpatient.Reservations.Create", true);
            await _permissionManager.SetForRoleAsync("admin", "HIS.Inpatient.Reservations.Edit", true);
            await _permissionManager.SetForRoleAsync("admin", "HIS.Inpatient.Reservations.Delete", true);
        }
    }
}
