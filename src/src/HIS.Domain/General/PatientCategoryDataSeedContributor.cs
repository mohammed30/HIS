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
        // 1. Clean up unused categories
        await DeleteIfExistsAsync("Card");
        await DeleteIfExistsAsync("شبكة");
        await DeleteIfExistsAsync("Bank Transfer");
        await DeleteIfExistsAsync("تحويل بنكي");
        await DeleteIfExistsAsync("Client Balance");
        await DeleteIfExistsAsync("رصيد عميل");
        await DeleteIfExistsAsync("Cheque");
        await DeleteIfExistsAsync("شيك");
        await DeleteIfExistsAsync("Insurance"); // Will be replaced by Contract
        await DeleteIfExistsAsync("تأمين");
        
        // Remove other junk as requested
        await DeleteIfExistsAsync("One of us");
        await DeleteIfExistsAsync("Poor");
        await DeleteIfExistsAsync("VIP");
        await DeleteIfExistsAsync("Staff");
        await DeleteIfExistsAsync("Charity");

        // 2. Seed Hospital Specific Categories
        await CreateCategoryIfNotExistsAsync("نقدي", "Cash", "CASH");
        await CreateCategoryIfNotExistsAsync("تعاقد", "Contract", "CONTRACT");
        await CreateCategoryIfNotExistsAsync("ضمان اجتماعي", "Social Security", "SOCIAL_SECURITY");
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
