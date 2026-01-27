using System;
using System.Linq;
using System.Threading.Tasks;
using HIS.Appointments;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.Appointments;

public class DoctorScheduleAppService : CrudAppService<
    DoctorSchedule,
    DoctorScheduleDto,
    Guid,
    PagedAndSortedResultRequestDto,
    CreateUpdateDoctorScheduleDto>, IDoctorScheduleAppService
{
    private readonly IRepository<HIS.Settings.Doctor, Guid> _doctorRepository;

    public DoctorScheduleAppService(IRepository<DoctorSchedule, Guid> repository, IRepository<HIS.Settings.Doctor, Guid> doctorRepository) 
        : base(repository)
    {
        _doctorRepository = doctorRepository;
    }

    public override async Task<DoctorScheduleDto> CreateAsync(CreateUpdateDoctorScheduleDto input)
    {
        var existingSchedule = await Repository.FirstOrDefaultAsync(x => x.DoctorId == input.DoctorId && x.DayOfWeek == input.DayOfWeek);
        if (existingSchedule != null)
        {
            // Update existing logic
            existingSchedule.StartTime = input.StartTime;
            existingSchedule.EndTime = input.EndTime;
            existingSchedule.SlotDuration = input.SlotDuration;
            existingSchedule.IsActive = input.IsActive;

            await Repository.UpdateAsync(existingSchedule);
            return await MapToGetOutputDtoAsync(existingSchedule);
        }

        return await base.CreateAsync(input);
    }
    

}
