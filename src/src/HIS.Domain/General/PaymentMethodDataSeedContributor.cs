using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;

namespace HIS.General;

public class PaymentMethodDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<PaymentMethod, Guid> _paymentMethodRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;
    private readonly IPermissionManager _permissionManager;

    public PaymentMethodDataSeedContributor(
        IRepository<PaymentMethod, Guid> paymentMethodRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant,
        IPermissionManager permissionManager)
    {
        _paymentMethodRepository = paymentMethodRepository;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
        _permissionManager = permissionManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        // 1. Grant Permissions to Admin and AdminStaff
        const string paymentMethodsPermission = "HIS.Definitions.PaymentMethods";
        
        await _permissionManager.SetForRoleAsync("admin", paymentMethodsPermission, true);
        await _permissionManager.SetForRoleAsync("AdminStaff", paymentMethodsPermission, true);

        // 2. Seed Data
        await CreatePaymentMethodAsync("نقدي", "Cash", "CASH", true);
        await CreatePaymentMethodAsync("شبكة", "Card", "CARD", false);
        await CreatePaymentMethodAsync("تحويل بنكي", "Bank Transfer", "TRANSFER", false);
        await CreatePaymentMethodAsync("رصيد عميل", "Client Balance", "BALANCE", false);
        await CreatePaymentMethodAsync("شيك", "Cheque", "CHEQUE", false);
        await CreatePaymentMethodAsync("تأمين", "Insurance", "INSURANCE", false);
    }

    private async Task CreatePaymentMethodAsync(string nameAr, string nameEn, string code, bool isDefault)
    {
        if (await _paymentMethodRepository.AnyAsync(x => x.Code == code))
        {
            return;
        }

        var paymentMethod = new PaymentMethod(
            _guidGenerator.Create(),
            nameAr,
            nameEn,
            code,
            isDefault,
            _currentTenant.Id
        );

        await _paymentMethodRepository.InsertAsync(paymentMethod);
    }
}
