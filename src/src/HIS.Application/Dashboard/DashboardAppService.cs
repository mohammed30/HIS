using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using HIS.Patients;
using HIS.Appointments;
using HIS.Rooms;
using HIS.Settings; // For Doctor
using System.Collections.Generic;

namespace HIS.Dashboard
{
    public class DashboardAppService : ApplicationService, IDashboardAppService
    {
        private readonly IRepository<Doctor, Guid> _doctorRepository;
        private readonly IRepository<Patient, Guid> _patientRepository;
        private readonly IRepository<Room, Guid> _roomRepository;
        private readonly IRepository<Bed, Guid> _bedRepository;
        private readonly IRepository<Appointment, Guid> _appointmentRepository;

        public DashboardAppService(
            IRepository<Doctor, Guid> doctorRepository,
            IRepository<Patient, Guid> patientRepository,
            IRepository<Room, Guid> roomRepository,
            IRepository<Bed, Guid> bedRepository,
            IRepository<Appointment, Guid> appointmentRepository)
        {
            _doctorRepository = doctorRepository;
            _patientRepository = patientRepository;
            _roomRepository = roomRepository;
            _bedRepository = bedRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            var summary = new DashboardSummaryDto();

            // Total Doctors
            summary.TotalDoctors = await _doctorRepository.CountAsync();

            // Total Patients
            summary.TotalPatients = await _patientRepository.CountAsync();

            // Total Rooms (You can count beds instead if more relevant)
            summary.TotalRooms = await _roomRepository.CountAsync();

            // Room Occupancy
            var allBeds = await _bedRepository.GetListAsync();
            int totalBeds = allBeds.Count;
            if (totalBeds > 0)
            {
                int occupiedBeds = allBeds.Count(x => x.Status == BedStatus.Occupied);
                summary.RoomStatus.Occupied = occupiedBeds;
                summary.RoomStatus.Available = allBeds.Count(x => x.Status == BedStatus.Available);
                summary.RoomStatus.Maintenance = allBeds.Count(x => x.Status == BedStatus.Maintenance);

                summary.OccupancyRate = (int)Math.Round((double)occupiedBeds / totalBeds * 100);
            }

            // Monthly Visits (Appointments created per month in the current year)
            int currentYear = DateTime.Now.Year;
            var appointments = await _appointmentRepository.GetListAsync(x => x.AppointmentDate.Year == currentYear);
            
            for (int i = 1; i <= 12; i++)
            {
                summary.MonthlyVisits.Add(new MonthlyVisitDto
                {
                    Month = i.ToString("D2"),
                    Count = appointments.Count(x => x.AppointmentDate.Month == i)
                });
            }

            return summary;
        }
    }
}
