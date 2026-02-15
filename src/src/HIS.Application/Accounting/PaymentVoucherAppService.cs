using System;
using System.Linq;
using System.Threading.Tasks;
using HIS.Accounting.Dtos;
using HIS.Inventory;
using HIS.Permissions;
using HIS.General;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace HIS.Accounting
{
    [Authorize(HISPermissions.Billing.Payments)]
    public class PaymentVoucherAppService : 
        CrudAppService<
            PaymentVoucher, 
            PaymentVoucherDto, 
            Guid, 
            PagedAndSortedResultRequestDto, 
            CreateUpdatePaymentVoucherDto>, 
        IPaymentVoucherAppService
    {
        private readonly IRepository<Supplier, Guid> _supplierRepository;
        private readonly IRepository<PaymentMethod, Guid> _paymentMethodRepository;
        private readonly IRepository<Account, Guid> _accountRepository;

        public PaymentVoucherAppService(
            IRepository<PaymentVoucher, Guid> repository,
            IRepository<Supplier, Guid> supplierRepository,
            IRepository<PaymentMethod, Guid> paymentMethodRepository,
            IRepository<Account, Guid> accountRepository) 
            : base(repository)
        {
            _supplierRepository = supplierRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _accountRepository = accountRepository;
        }

        protected override async Task<IQueryable<PaymentVoucher>> CreateFilteredQueryAsync(PagedAndSortedResultRequestDto input)
        {
            return await Repository.WithDetailsAsync(x => x.Lines);
        }

        public override async Task<PaymentVoucherDto> GetAsync(Guid id)
        {
            var query = await Repository.WithDetailsAsync(x => x.Lines);
            var entity = await AsyncExecuter.FirstOrDefaultAsync(query, x => x.Id == id);
            
            var dto = MapToGetOutputDto(entity);
            
            // Populate lookup names manually or via extra queries if needed
            if (entity.SupplierId.HasValue)
            {
                var supplier = await _supplierRepository.FindAsync(entity.SupplierId.Value);
                dto.SupplierName = supplier?.Name;
            }

            if (entity.PaymentMethodId.HasValue)
            {
                var pm = await _paymentMethodRepository.FindAsync(entity.PaymentMethodId.Value);
                dto.PaymentMethodName = pm?.NameEn ?? pm?.NameAr;
            }

            // Populate account names for lines
            foreach (var lineDto in dto.Lines)
            {
                var account = await _accountRepository.FindAsync(lineDto.AccountId);
                lineDto.AccountName = account?.Name;
            }

            return dto;
        }

        public override async Task<PaymentVoucherDto> CreateAsync(CreateUpdatePaymentVoucherDto input)
        {
            // Custom logic: Generate Voucher Number
            // In a real app, use a Domain Service for number generation.
            string voucherNumber = "PV-" + DateTime.Now.Ticks.ToString().Substring(10); // Simple temporary generation

            var entity = MapToEntity(input);
            entity.VoucherNumber = voucherNumber;

            // Ensure lines are correctly associated (EF Core usually handles this via navigation property)
            
            await Repository.InsertAsync(entity, autoSave: true);

            return await GetAsync(entity.Id);
        }
    }
}
