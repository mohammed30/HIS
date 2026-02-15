using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Inpatient;

public class ReservationDto : FullAuditedEntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public string? PatientName { get; set; }
    public Guid RoomId { get; set; }
    public string? RoomNumber { get; set; }
    public Guid? BedId { get; set; }
    public string? BedNumber { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ReservationStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class CreateUpdateReservationDto
{
    public Guid PatientId { get; set; }
    public Guid RoomId { get; set; }
    public Guid? BedId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public string? Notes { get; set; }
}

public class GetReservationsInput : PagedAndSortedResultRequestDto
{
    public Guid? PatientId { get; set; }
    public Guid? RoomId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public ReservationStatus? Status { get; set; }
}

public interface IReservationAppService : Volo.Abp.Application.Services.ICrudAppService<
    ReservationDto, 
    Guid, 
    GetReservationsInput, 
    CreateUpdateReservationDto>
{

}
