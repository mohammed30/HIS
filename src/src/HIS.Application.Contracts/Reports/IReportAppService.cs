using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace HIS.Reports;

public interface IReportAppService : IApplicationService
{
    Task<PagedResultDto<PaidTicketDto>> GetPaidTicketsAsync(GetPaidTicketsInput input);
    Task RefundTicketAsync(Guid appointmentId);
    Task<IRemoteStreamContent> GetPaidTicketsPdfAsync(GetPaidTicketsInput input);

    Task<PagedResultDto<PharmacySalesDto>> GetPharmacySalesAsync(GetPharmacySalesInput input);
    Task<IRemoteStreamContent> GetPharmacySalesPdfAsync(GetPharmacySalesInput input);
}
