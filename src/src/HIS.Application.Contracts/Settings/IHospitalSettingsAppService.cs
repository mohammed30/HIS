using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace HIS.Settings;

public interface IHospitalSettingsAppService : IApplicationService
{
    Task<HospitalSettingsDto> GetAsync();

    Task UpdateAsync(HospitalSettingsDto input);
}
