using System;
using System.Threading.Tasks;
using HIS.Pharmacy.Dtos;
using Volo.Abp.Application.Services;

namespace HIS.Pharmacy;

public interface IPosAppService : IApplicationService
{
    Task<PosProductDto> GetProductByBarcodeAsync(string barcode);
    Task<PosProductDto> GetProductByIdAsync(Guid id);
    Task ProcessSaleAsync(PosSaleDto input);
}
