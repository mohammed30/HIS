using HIS.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace HIS.Permissions;

public class HISPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var inventoryGroup = context.AddGroup(HISPermissions.GroupName, L("Permission:Inventory"));
        
        var inventorySection = inventoryGroup.AddPermission(HISPermissions.Inventory.Default, L("Permission:Inventory"));
        inventorySection.AddChild(HISPermissions.Inventory.ManageWarehouses, L("Permission:ManageWarehouses"));
        inventorySection.AddChild(HISPermissions.Inventory.StockOperations, L("Permission:StockOperations"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<HISResource>(name);
    }
}
