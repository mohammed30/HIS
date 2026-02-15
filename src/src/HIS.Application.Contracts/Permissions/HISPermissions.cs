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

    public static class Reception
    {
        public const string Default = GroupName + ".Reception";
        public const string LaboratoryReception = Default + ".LaboratoryReception";
        public const string Tickets = Default + ".Tickets";
        public const string InsuranceCompanies = Default + ".InsuranceCompanies";
        public const string InsurancePlans = Default + ".InsurancePlans";
        public const string Invoices = Default + ".Invoices";
        public const string Payments = Default + ".Payments";
    }

    public static class Laboratory
    {
        public const string Default = GroupName + ".Laboratory";
        public const string CreateSample = Default + ".CreateSample";
        public const string UpdateResults = Default + ".UpdateResults";
        public const string ApproveResults = Default + ".ApproveResults";
        public const string Catalog = Default + ".Catalog";
        public const string Requests = Default + ".Requests";
        public const string Appointments = Default + ".Appointments";
    }

    public static class Emergency
    {
        public const string Default = GroupName + ".Emergency";
        public const string Dashboard = Default + ".Dashboard";
    }

    public static class Inventory
    {
        public const string Default = GroupName + ".Inventory";
        public const string ManageWarehouses = Default + ".ManageWarehouses";
        public const string StockOperations = Default + ".StockOperations";
        public const string Dashboard = Default + ".Dashboard";
        public const string Suppliers = Default + ".Suppliers";
        public const string PurchaseOrders = Default + ".PurchaseOrders";
        public const string DepartmentalConsumption = Default + ".DepartmentalConsumption";
    }

    public static class Billing
    {
        public const string Default = GroupName + ".Billing";
        public const string ManageInvoices = Default + ".ManageInvoices";
        public const string ChartOfAccounts = Default + ".ChartOfAccounts";
        public const string JournalEntries = Default + ".JournalEntries";
        public const string Payments = Default + ".Payments";
        public const string DeferredPayments = Default + ".DeferredPayments";
        public const string FinancialReports = Default + ".FinancialReports";
    }

    public static class Definitions
    {
        public const string Default = GroupName + ".Definitions";
        public const string Nationalities = Default + ".Nationalities";
        public const string Professions = Default + ".Professions";
        public const string Contracts = Default + ".Contracts";
        public const string PatientCategories = Default + ".PatientCategories";
        public const string ReferralSources = Default + ".ReferralSources";
        public const string Services = Default + ".Services";
        public const string Radiology = Default + ".Radiology";
        public const string PriceLists = Default + ".PriceLists";
        public const string PaymentMethods = Default + ".PaymentMethods";
    }

    public static class Pharmacy
    {
        public const string Default = GroupName + ".Pharmacy";
        public const string Dispensing = Default + ".Dispensing";
        public const string Prescriptions = Default + ".Prescriptions";
        public const string Stock = Default + ".Stock";
        public const string Drugs = Default + ".Drugs";
        public const string DrugsCreate = Drugs + ".Create";
        public const string DrugsEdit = Drugs + ".Edit";
        public const string DrugsDelete = Drugs + ".Delete";
        public const string POS = Default + ".POS";
    }

    public static class Nursing
    {
        public const string Default = GroupName + ".Nursing";
        public const string PatientList = Default + ".PatientList";
        public const string VitalSigns = Default + ".VitalSigns";
        public const string MedicationAdministration = Default + ".MedicationAdministration";
        public const string CarePlans = Default + ".CarePlans";
        public const string Assessments = Default + ".Assessments";
        public const string FluidBalance = Default + ".FluidBalance";
        public const string ShiftHandover = Default + ".ShiftHandover";
    }

    public static class Inpatient
    {
        public const string Default = GroupName + ".Inpatient";
        public const string Rooms = Default + ".Rooms";
        public const string Admissions = Default + ".Admissions";
        public const string Reservations = Default + ".Reservations";
        public const string Dashboard = Default + ".Dashboard";
    }

    public static class Operations
    {
        public const string Default = GroupName + ".Operations";
        public const string PrintTicket = Default + ".PrintTicket";
        public const string Manage = Default + ".Manage";
    }
}

