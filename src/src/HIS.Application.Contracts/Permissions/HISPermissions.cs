namespace HIS.Permissions;

public static class HISPermissions
{
    public const string GroupName = "HIS";

    public static class Settings
    {
        public const string Default = GroupName + ".Settings";
        public const string Pharmacy = Default + ".Pharmacy";
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
        public const string PurchaseRequisitions = Default + ".PurchaseRequisitions";
        public const string PurchaseOrders = Default + ".PurchaseOrders";
        public const string DepartmentalConsumption = Default + ".DepartmentalConsumption";
    }

    public static class Billing
    {
        public const string Default = GroupName + ".Billing";
        public const string ManageInvoices = Default + ".ManageInvoices";
        public const string ChartOfAccounts = Default + ".ChartOfAccounts";
        public const string JournalEntries = Default + ".JournalEntries";
        public const string JournalEntriesPost = JournalEntries + ".Post";
        public const string Payments = Default + ".Payments";
        public const string DeferredPayments = Default + ".DeferredPayments";
        public const string FinancialReports = Default + ".FinancialReports";
        public const string DailyReport = FinancialReports + ".DailyReport";
        public const string CustomerDebtsReport = FinancialReports + ".CustomerDebtsReport";
        public const string DiscountsReport = FinancialReports + ".DiscountsReport";
        public const string IncomeStatement = FinancialReports + ".IncomeStatement";
        public const string BalanceSheet = FinancialReports + ".BalanceSheet";
        public const string AccountStatement = FinancialReports + ".AccountStatement";
        public const string ReceiptVouchers = Default + ".ReceiptVouchers";
        public const string PaymentVouchers = Default + ".PaymentVouchers";
        public const string BankTransactions = Default + ".BankTransactions";
        public const string ContractClaims = Default + ".ContractClaims";
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
        public const string POS_Approval = POS + ".Approval";
        public const string POS_ToDispense = POS + ".ToDispense";
        public const string POS_Returns = POS + ".Returns";
        public const string POS_ReturnedInvoices = POS + ".ReturnedInvoices";
        public const string POS_Dispensed = POS + ".Dispensed";
        public const string POS_ReturnedRequests = POS + ".ReturnedRequests";
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
        public const string InternalRequestReturn = Default + ".InternalRequestReturn";
    }

    public static class Inpatient
    {
        public const string Default = GroupName + ".Inpatient";
        public const string Rooms = Default + ".Rooms";
        public const string RoomsCreate = Rooms + ".Create";
        public const string RoomsEdit = Rooms + ".Edit";
        public const string RoomsDelete = Rooms + ".Delete";
        public const string Admissions = Default + ".Admissions";
        public const string AdmissionsCreate = Admissions + ".Create";
        public const string AdmissionsEdit = Admissions + ".Edit";
        public const string AdmissionsDelete = Admissions + ".Delete";
        public const string Reservations = Default + ".Reservations";
        public const string ReservationsCreate = Reservations + ".Create";
        public const string ReservationsEdit = Reservations + ".Edit";
        public const string ReservationsDelete = Reservations + ".Delete";
        public const string Dashboard = Default + ".Dashboard";
    }

    public static class Operations
    {
        public const string Default = GroupName + ".Operations";
        public const string PrintTicket = Default + ".PrintTicket";
        public const string Manage = Default + ".Manage";
        public const string Report = Default + ".Report";
    }

    public static class HR
    {
        public const string Default = GroupName + ".HR";
        public const string Employees = Default + ".Employees";
        public const string EmployeesCreate = Employees + ".Create";
        public const string EmployeesEdit = Employees + ".Edit";
        public const string EmployeesDelete = Employees + ".Delete";
        public const string CompensationItems = Default + ".CompensationItems";
        public const string LeaveTypes = Default + ".LeaveTypes";
        public const string EmployeeLeaves = Default + ".EmployeeLeaves";
        public const string Loans = Default + ".Loans";
        public const string Payroll = Default + ".Payroll";
        public const string PayrollProcess = Payroll + ".Process";
        public const string Penalties = Default + ".Penalties";
        public const string Attendance = Default + ".Attendance";
        public const string Reports = Default + ".Reports";
        public const string PaySlip = Default + ".PaySlip";
    }

    public static class ActivityLogs
    {
        public const string Default = GroupName + ".ActivityLogs";
    }

    public static class Radiology
    {
        public const string Default = GroupName + ".Radiology";
        public const string Requests = Default + ".Requests";
    }

    public static class Notifications
    {
        public const string Default = GroupName + ".Notifications";
        public const string Manage  = Default + ".Manage"; // Admin only
    }
}

