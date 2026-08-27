using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Reports
{
    public interface IInsuranceClaimReportAppService : IApplicationService
    {
        Task<PagedResultDto<InsuranceClaimReportDto>> GetListAsync(GetInsuranceClaimsInput input);
        Task<byte[]> GetPrintDocumentAsync(GetInsuranceClaimsInput input);
    }
}
