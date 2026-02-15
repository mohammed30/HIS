using System;
using System.Linq;
using System.Threading.Tasks;
using HIS.Accounting.Dtos;
using HIS.Patients;
using HIS.Permissions;
using HIS.General;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.Accounting
{
    [Authorize(HISPermissions.Billing.Payments)]
    public class ReceiptVoucherAppService : 
        CrudAppService<
            ReceiptVoucher, 
            ReceiptVoucherDto, 
            Guid, 
            PagedAndSortedResultRequestDto, 
            CreateUpdateReceiptVoucherDto>, 
        IReceiptVoucherAppService
    {
        private readonly IRepository<Patient, Guid> _patientRepository;
        private readonly IRepository<PaymentMethod, Guid> _paymentMethodRepository;
        private readonly IRepository<Account, Guid> _accountRepository;

        public ReceiptVoucherAppService(
            IRepository<ReceiptVoucher, Guid> repository,
            IRepository<Patient, Guid> patientRepository,
            IRepository<PaymentMethod, Guid> paymentMethodRepository,
            IRepository<Account, Guid> accountRepository) 
            : base(repository)
        {
            _patientRepository = patientRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _accountRepository = accountRepository;
        }

        protected override async Task<IQueryable<ReceiptVoucher>> CreateFilteredQueryAsync(PagedAndSortedResultRequestDto input)
        {
            return await Repository.WithDetailsAsync(x => x.Lines);
        }

        public override async Task<ReceiptVoucherDto> GetAsync(Guid id)
        {
            var query = await Repository.WithDetailsAsync(x => x.Lines);
            var entity = await AsyncExecuter.FirstOrDefaultAsync(query, x => x.Id == id);
            
            var dto = MapToGetOutputDto(entity);
            
            if (entity.PatientId.HasValue)
            {
                var patient = await _patientRepository.FindAsync(entity.PatientId.Value);
                // Assuming Patient has a Name or First/Last name. Patient Class has Names.
                dto.PatientName = patient != null ? $"{patient.FirstNameAr} {patient.LastNameAr}" : "";
            }

            if (entity.PaymentMethodId.HasValue)
            {
                var pm = await _paymentMethodRepository.FindAsync(entity.PaymentMethodId.Value);
                dto.PaymentMethodName = pm?.NameEn ?? pm?.NameAr;
            }

            foreach (var lineDto in dto.Lines)
            {
                var account = await _accountRepository.FindAsync(lineDto.AccountId);
                lineDto.AccountName = account?.Name;
            }

            return dto;
        }

        public override async Task<ReceiptVoucherDto> CreateAsync(CreateUpdateReceiptVoucherDto input)
        {
            string voucherNumber = "RV-" + DateTime.Now.Ticks.ToString().Substring(10); 

            var entity = MapToEntity(input);
            entity.VoucherNumber = voucherNumber;
            
            await Repository.InsertAsync(entity, autoSave: true);

            return await GetAsync(entity.Id);
        }
    }
}
