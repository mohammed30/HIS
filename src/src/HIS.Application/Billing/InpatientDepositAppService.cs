using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Application.Dtos;
using Volo.Abp;
using Microsoft.AspNetCore.Authorization;
using HIS.Permissions;
using HIS.Accounting;
using HIS.Patients;
using HIS.Inpatient;

namespace HIS.Billing;

[Authorize(HISPermissions.Billing.Default)]
public class InpatientDepositAppService : CrudAppService<
    InpatientDeposit,
    InpatientDepositDto,
    Guid,
    GetInpatientDepositsInput,
    CreateInpatientDepositDto>, IInpatientDepositAppService
{
    private readonly IRepository<JournalEntry, Guid> _journalEntryRepository;
    private readonly IRepository<Account, Guid> _accountRepository;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<Admission, Guid> _admissionRepository;

    public InpatientDepositAppService(
        IRepository<InpatientDeposit, Guid> repository,
        IRepository<JournalEntry, Guid> journalEntryRepository,
        IRepository<Account, Guid> accountRepository,
        IRepository<Patient, Guid> patientRepository,
        IRepository<Admission, Guid> admissionRepository) : base(repository)
    {
        _journalEntryRepository = journalEntryRepository;
        _accountRepository = accountRepository;
        _patientRepository = patientRepository;
        _admissionRepository = admissionRepository;
    }

    public override async Task<InpatientDepositDto> CreateAsync(CreateInpatientDepositDto input)
    {
        var admission = await _admissionRepository.GetAsync(input.AdmissionId);
        if (admission.Status == AdmissionStatus.Discharged)
        {
            throw new UserFriendlyException("Cannot add deposit for a discharged patient.");
        }

        var deposit = new InpatientDeposit(
            GuidGenerator.Create(),
            CurrentTenant.Id,
            input.PatientId,
            input.AdmissionId,
            $"DEP-{DateTime.Now:yyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}",
            input.Amount
        )
        {
            PaymentMethod = input.PaymentMethod,
            ReferenceNumber = input.ReferenceNumber,
            Notes = input.Notes,
            ReceivedBy = CurrentUser?.UserName
        };

        // Create Journal Entry
        var cashAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "1110"); // Cash
        var unearnedRevenueAccount = await _accountRepository.FirstOrDefaultAsync(x => x.Code == "2100"); // Unearned Revenue / Deposits

        if (cashAccount != null && unearnedRevenueAccount != null)
        {
            var patient = await _patientRepository.GetAsync(input.PatientId);
            var patientName = !string.IsNullOrWhiteSpace(patient.FullNameAr) ? patient.FullNameAr : patient.MRN;

            var je = new JournalEntry(
                GuidGenerator.Create(),
                DateTime.Now,
                $"JE-{deposit.ReceiptNumber}",
                $"دفعة مقدمة تنويم - المريض: {patientName}"
            );

            // Debit Cash, Credit Unearned Revenue
            je.AddLine(GuidGenerator, cashAccount.Id, input.Amount, 0);
            je.AddLine(GuidGenerator, unearnedRevenueAccount.Id, 0, input.Amount);

            var createdJe = await _journalEntryRepository.InsertAsync(je, autoSave: true);
            deposit.JournalEntryId = createdJe.Id;
        }

        await Repository.InsertAsync(deposit);

        // Update Admission PaidAmount
        admission.PaidAmount += input.Amount;
        await _admissionRepository.UpdateAsync(admission);

        var dto = ObjectMapper.Map<InpatientDeposit, InpatientDepositDto>(deposit);
        await EnrichDtoAsync(dto);
        return dto;
    }

    protected override async Task<IQueryable<InpatientDeposit>> CreateFilteredQueryAsync(GetInpatientDepositsInput input)
    {
        var queryable = await base.CreateFilteredQueryAsync(input);

        return queryable
            .WhereIf(input.PatientId.HasValue, x => x.PatientId == input.PatientId!.Value)
            .WhereIf(input.AdmissionId.HasValue, x => x.AdmissionId == input.AdmissionId!.Value)
            .WhereIf(input.Status.HasValue, x => x.Status == input.Status!.Value);
    }

    protected override IQueryable<InpatientDeposit> ApplyDefaultSorting(IQueryable<InpatientDeposit> query)
    {
        return query.OrderByDescending(x => x.DepositDate);
    }

    public override async Task<InpatientDepositDto> GetAsync(Guid id)
    {
        var dto = await base.GetAsync(id);
        await EnrichDtoAsync(dto);
        return dto;
    }

    public override async Task<PagedResultDto<InpatientDepositDto>> GetListAsync(GetInpatientDepositsInput input)
    {
        var result = await base.GetListAsync(input);
        foreach (var dto in result.Items)
        {
            await EnrichDtoAsync(dto);
        }
        return result;
    }

    private async Task EnrichDtoAsync(InpatientDepositDto dto)
    {
        var patient = await _patientRepository.FindAsync(dto.PatientId);
        if (patient != null)
        {
            dto.PatientName = patient.FullNameAr ?? patient.FullNameEn;
        }
    }
}
