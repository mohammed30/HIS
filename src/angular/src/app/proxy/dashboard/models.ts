
export interface DashboardSummaryDto {
  totalDoctors?: number;
  totalPatients?: number;
  totalRooms?: number;
  occupancyRate?: number;
  monthlyVisits?: MonthlyVisitDto[];
  roomStatus?: RoomStatusSummaryDto;
}

export interface MonthlyVisitDto {
  month?: string;
  count?: number;
}

export interface RoomStatusSummaryDto {
  occupied?: number;
  available?: number;
  maintenance?: number;
}
