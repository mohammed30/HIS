namespace HIS.Permissions;

public static class HISPermissions
{
    public const string GroupName = "HIS";

    public static class Settings
    {
        public const string Default = GroupName + ".Settings";
    }

    public static class Patients
    {
        public const string Default = GroupName + ".Patients";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Appointments
    {
        public const string Default = GroupName + ".Appointments";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }

    public static class Laboratory
    {
        public const string Default = GroupName + ".Laboratory";
        public const string CreateSample = Default + ".CreateSample";
        public const string UpdateResults = Default + ".UpdateResults";
        public const string ApproveResults = Default + ".ApproveResults"; // For Manager
    }

    public static class Inventory
    {
        public const string Default = GroupName + ".Inventory";
        public const string ManageWarehouses = Default + ".ManageWarehouses";
        public const string StockOperations = Default + ".StockOperations";
    }

    public static class Billing
    {
        public const string Default = GroupName + ".Billing";
        public const string ManageInvoices = Default + ".ManageInvoices";
        public const string ChartOfAccounts = Default + ".ChartOfAccounts";
        public const string JournalEntries = Default + ".JournalEntries";
    }
}

