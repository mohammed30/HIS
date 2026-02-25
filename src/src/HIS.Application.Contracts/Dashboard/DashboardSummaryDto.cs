using System.Collections.Generic;

namespace HIS.Dashboard
{
    public class DashboardSummaryDto
    {
        public int TotalDoctors { get; set; }
        public int TotalPatients { get; set; }
        public int TotalRooms { get; set; }
        public int OccupancyRate { get; set; } // Percentage

        public List<MonthlyVisitDto> MonthlyVisits { get; set; } = new();
        public RoomStatusSummaryDto RoomStatus { get; set; } = new();
    }

    public class MonthlyVisitDto
    {
        public string Month { get; set; } // e.g., "01", "02", or "Jan", "Feb"
        public int Count { get; set; }
    }

    public class RoomStatusSummaryDto
    {
        public int Occupied { get; set; }
        public int Available { get; set; }
        public int Maintenance { get; set; }
    }
}
