using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HIS.Laboratory.Dtos;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Laboratory;

public interface ILabAppService : IApplicationService
{
    // Lab Test Categories
    Task<List<LabTestCategoryDto>> GetCategoriesWithTestsAsync();
    Task<List<LabTestCategoryDto>> GetCategoriesAsync();

    // Lab Tests (Catalog)
    Task<PagedResultDto<LabTestDto>> GetTestsAsync(PagedAndSortedResultRequestDto input);
    Task<LabTestDto> CreateTestAsync(CreateUpdateLabTestDto input);
    Task<LabTestDto> UpdateTestAsync(Guid id, CreateUpdateLabTestDto input);
    Task DeleteTestAsync(Guid id);

    // Lab Requests
    Task<PagedResultDto<LabRequestDto>> GetRequestsAsync(GetLabRequestsInput input);
    Task<LabRequestDto> CreateRequestAsync(CreateLabRequestDto input);
    Task<LabRequestDto> CollectSampleAsync(Guid id);
    Task<LabRequestDto> CompleteRequestAsync(Guid id, UpdateLabResultDto input);
    Task<Volo.Abp.Content.IRemoteStreamContent> GetResultPdfAsync(Guid id);
    Task<Volo.Abp.Content.IRemoteStreamContent> GetSampleBarcodePdfAsync(Guid id);
    Task<Volo.Abp.Content.IRemoteStreamContent> GetRequestOrderPdfAsync(Guid id);

    // Lab Appointments (حجوزات المعمل)
    Task<PagedResultDto<LabAppointmentDto>> GetAppointmentsAsync(PagedAndSortedResultRequestDto input);
    Task<LabAppointmentDto> GetAppointmentAsync(Guid id);
    Task<LabAppointmentDto> CreateAppointmentAsync(CreateLabAppointmentDto input);
    Task<LabAppointmentDto> UpdateAppointmentAsync(Guid id, UpdateLabAppointmentDto input);
    Task CancelAppointmentAsync(Guid id);
    Task<LabAppointmentDto> ConfirmAppointmentAsync(Guid id);
    Task<LabAppointmentDto> CheckInAppointmentAsync(Guid id);
    Task<LabAppointmentDto> CompleteAppointmentAsync(Guid id);
}
