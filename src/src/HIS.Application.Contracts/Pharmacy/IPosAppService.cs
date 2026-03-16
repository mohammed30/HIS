using System;
using System.Threading.Tasks;
using HIS.Pharmacy.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace HIS.Pharmacy;

public interface IPosAppService : IApplicationService
{
    Task<PosProductDto> GetProductByBarcodeAsync(string barcode);
    Task<PosProductDto> GetProductByIdAsync(Guid id);
    Task<System.Collections.Generic.List<PosProductDto>> SearchProductsAsync(string query);
    Task<Guid> ProcessSaleAsync(PosSaleDto input);
    Task RefundSaleAsync(string invoiceNumber);
    Task<IRemoteStreamContent> GetInvoicePdfAsync(string idOrNumber);
}
