using System;
using System.Threading.Tasks;
using HIS.Inventory;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Microsoft.Extensions.Configuration;

namespace HIS.Data
{
    public class SupplierDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<Supplier, Guid> _supplierRepository;
        private readonly IGuidGenerator _guidGenerator;

        public SupplierDataSeedContributor(
            IRepository<Supplier, Guid> supplierRepository,
            IGuidGenerator guidGenerator)
        {
            _supplierRepository = supplierRepository;
            _guidGenerator = guidGenerator;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (await _supplierRepository.GetCountAsync() > 0)
            {
                return;
            }

            await _supplierRepository.InsertAsync(
                new Supplier(
                    _guidGenerator.Create(),
                    "شركة التوريدات الطبية الحديثة",
                    "أحمد محمد",
                    "0100200300",
                    "info@medical-supplies.com",
                    "القاهرة، مصر",
                    "TAX-123456"
                )
            );

            await _supplierRepository.InsertAsync(
                new Supplier(
                    _guidGenerator.Create(),
                    "موزع الأدوية العالمي",
                    "سارة علي",
                    "0111222333",
                    "sales@pharma-distributor.com",
                    "الرياض، السعودية",
                    "TAX-789012"
                )
            );

            await _supplierRepository.InsertAsync(
                new Supplier(
                    _guidGenerator.Create(),
                    "تجهيزات المستشفيات المتخصصة",
                    "خالد محمود",
                    "0122333444",
                    "support@hospital-equip.com",
                    "دبي، الإمارات",
                    "TAX-345678"
                )
            );
            
            await _supplierRepository.InsertAsync(
                new Supplier(
                    _guidGenerator.Create(),
                    "Medical Supplies Co.",
                    "John Doe",
                    "123-456-789",
                    "contact@medsupply.com",
                    "New York, USA",
                    "TAX-US-999"
                )
            );
        }
    }
}
