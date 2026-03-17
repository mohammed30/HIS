using System;
using System.Threading.Tasks;
using HIS.Pharmacy.Dtos;
using HIS.Services;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using MiniExcelLibs;
using Volo.Abp.Content;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Volo.Abp;

using HIS.Permissions;

namespace HIS.Pharmacy;

public class DrugAppService : CrudAppService<Drug, DrugDto, Guid, PagedAndSortedResultRequestDto, CreateUpdateDrugDto>, IDrugAppService
{
    protected override string GetPolicyName { get; set; } = HISPermissions.Pharmacy.Drugs;
    protected override string GetListPolicyName { get; set; } = HISPermissions.Pharmacy.Drugs;
    protected override string CreatePolicyName { get; set; } = HISPermissions.Pharmacy.DrugsCreate;
    protected override string UpdatePolicyName { get; set; } = HISPermissions.Pharmacy.DrugsEdit;
    protected override string DeletePolicyName { get; set; } = HISPermissions.Pharmacy.DrugsDelete;

    private readonly IRepository<ServiceItem, Guid> _serviceItemRepository;

    public DrugAppService(
        IRepository<Drug, Guid> repository,
        IRepository<ServiceItem, Guid> serviceItemRepository) 
        : base(repository)
    {
        _serviceItemRepository = serviceItemRepository;
    }

    public override async Task<DrugDto> CreateAsync(CreateUpdateDrugDto input)
    {
        // 1. Create corresponding Service Item so it can be ordered
        var serviceItem = new ServiceItem(
            GuidGenerator.Create(),
            input.Barcode, // Use barcode as code
            $"{input.BrandName} {input.Strength} - {input.Form}", // Descriptive Name
            ServiceCategory.Pharmacy
        );
        serviceItem.Price = input.Price;
        
        await _serviceItemRepository.InsertAsync(serviceItem);

        // 2. Create Drug
        var drug = ObjectMapper.Map<CreateUpdateDrugDto, Drug>(input);
        drug.ServiceItemId = serviceItem.Id;

        await Repository.InsertAsync(drug);

        return ObjectMapper.Map<Drug, DrugDto>(drug);
    }

    public async Task<IRemoteStreamContent> GetImportTemplateAsync()
    {
        var templateData = new List<DrugImportDto>
        {
            new DrugImportDto
            {
                Barcode = "123456789",
                BrandName = "Example Drug",
                ScientificName = "Example Scientific",
                Strength = "500mg",
                Form = "Tablet",
                Manufacturer = "Company Name",
                BatchNumberPrefix = "EXP",
                MinimumStockLevel = 10,
                ReorderLevel = 20,
                BinLocation = "A-01",
                IsControlled = "No",
                LegalCategory = "GSL",
                Price = 10.50m
            }
        };

        var memoryStream = new MemoryStream();
        memoryStream.SaveAs(templateData);
        memoryStream.Seek(0, SeekOrigin.Begin);

        return new RemoteStreamContent(memoryStream, "DrugImportTemplate.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    public async Task ImportExcelAsync(IRemoteStreamContent input)
    {
        using (var stream = input.GetStream())
        {
            var rows = stream.Query<DrugImportDto>().ToList();

            foreach (var row in rows)
            {
                if (string.IsNullOrWhiteSpace(row.Barcode) || string.IsNullOrWhiteSpace(row.BrandName))
                {
                    continue;
                }

                // Follow logic from CreateAsync
                var serviceItem = new ServiceItem(
                    GuidGenerator.Create(),
                    row.Barcode,
                    $"{row.BrandName} {row.Strength} - {row.Form}",
                    ServiceCategory.Pharmacy
                );
                serviceItem.Price = row.Price;

                await _serviceItemRepository.InsertAsync(serviceItem);

                var drug = new Drug(
                    GuidGenerator.Create(),
                    row.Barcode,
                    row.BrandName,
                    row.ScientificName,
                    row.Strength,
                    row.Form,
                    row.Manufacturer
                );
                drug.BatchNumberPrefix = row.BatchNumberPrefix;
                drug.MinimumStockLevel = row.MinimumStockLevel;
                drug.ReorderLevel = row.ReorderLevel;
                drug.BinLocation = row.BinLocation;
                drug.IsControlled = row.IsControlled?.Equals("Yes", StringComparison.OrdinalIgnoreCase) ?? false;
                drug.LegalCategory = row.LegalCategory;
                drug.ServiceItemId = serviceItem.Id;

                await Repository.InsertAsync(drug);
            }
        }
    }
}
