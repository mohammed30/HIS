namespace HIS.Permissions;

public static class HISPermissions
{
    public const string GroupName = "HIS";

    public static class Inventory
    {
        public const string Default = GroupName + ".Inventory";
        public const string ManageWarehouses = Default + ".ManageWarehouses";
        public const string StockOperations = Default + ".StockOperations";
    }
}

