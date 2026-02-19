using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;

namespace HIS.General;

public class PatientCategoryDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<PatientCategory, Guid> _patientCategoryRepository;
    private readonly IGuidGenerator _guidGenerator;
    private readonly ICurrentTenant _currentTenant;

    public PatientCategoryDataSeedContributor(
        IRepository<PatientCategory, Guid> patientCategoryRepository,
        IGuidGenerator guidGenerator,
        ICurrentTenant currentTenant)
    {
        _patientCategoryRepository = patientCategoryRepository;
        _guidGenerator = guidGenerator;
        _currentTenant = currentTenant;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        // 1. Clean up Non-Payment Method Categories (Strict Sync)
        await DeleteIfExistsAsync("One of us");
        await DeleteIfExistsAsync("واحد منا");
        await DeleteIfExistsAsync("Poor");
        await DeleteIfExistsAsync("فقير");
        
        // Remove categories that are NOT payment methods
        await DeleteIfExistsAsync("VIP");
        await DeleteIfExistsAsync("كبار الشخصيات");
        await DeleteIfExistsAsync("Staff");
        await DeleteIfExistsAsync("موظف");
        await DeleteIfExistsAsync("Charity");
        await DeleteIfExistsAsync("جمعية خيرية");

        // 2. Seed Only Payment Method Categories
        await CreateCategoryIfNotExistsAsync("نقدي", "Cash", "CASH");
        await CreateCategoryIfNotExistsAsync("شبكة", "Card", "CARD");
        await CreateCategoryIfNotExistsAsync("تأمين", "Insurance", "INSURANCE");
        
        // Add missing payment methods
        await CreateCategoryIfNotExistsAsync("تحويل بنكي", "Bank Transfer", "TRANSFER");
        await CreateCategoryIfNotExistsAsync("رصيد عميل", "Client Balance", "BALANCE");
        await CreateCategoryIfNotExistsAsync("شيك", "Cheque", "CHEQUE");
    }

    private async Task DeleteIfExistsAsync(string name)
    {
        var category = await _patientCategoryRepository.FindAsync(x => x.NameEn == name || x.NameAr == name);
        if (category != null)
        {
            await _patientCategoryRepository.DeleteAsync(category);
        }
    }

    private async Task CreateCategoryIfNotExistsAsync(string nameAr, string nameEn, string code)
    {
        if (!await _patientCategoryRepository.AnyAsync(x => x.Code == code || x.NameEn == nameEn))
        {
            var category = new PatientCategory(
                _guidGenerator.Create(),
                nameAr,
                nameEn,
                code,
                _currentTenant.Id
            );

            await _patientCategoryRepository.InsertAsync(category);
        }
    }
}
