using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Appointments;

public interface IDoctorScheduleAppService : ICrudAppService<
    DoctorScheduleDto,
    Guid,
    PagedAndSortedResultRequestDto,
    CreateUpdateDoctorScheduleDto>
{
}
