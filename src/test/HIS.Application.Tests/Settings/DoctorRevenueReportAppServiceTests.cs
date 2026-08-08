using System;
using System.Threading.Tasks;
using HIS.Settings;
using HIS.Billing;
using HIS.Patients;
using HIS.Services;
using Shouldly;
using Xunit;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using System.Collections.Generic;

namespace HIS.Settings.Tests;

public abstract class DoctorRevenueReportAppServiceTests<TStartupModule> : SettingsTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IDoctorRevenueReportAppService _doctorRevenueReportAppService;
    private readonly IRepository<Doctor, Guid> _doctorRepository;
    private readonly IRepository<Invoice, Guid> _invoiceRepository;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<ServiceItem, Guid> _serviceItemRepository;

    protected DoctorRevenueReportAppServiceTests()
    {
        _doctorRevenueReportAppService = GetRequiredService<IDoctorRevenueReportAppService>();
        _doctorRepository = GetRequiredService<IRepository<Doctor, Guid>>();
        _invoiceRepository = GetRequiredService<IRepository<Invoice, Guid>>();
        _patientRepository = GetRequiredService<IRepository<Patient, Guid>>();
        _serviceItemRepository = GetRequiredService<IRepository<ServiceItem, Guid>>();
    }

    [Fact]
    public async Task GetReportAsync_Should_Calculate_Doctor_Revenue_Correctly()
    {
        // Arrange
        Guid doctorId = Guid.NewGuid();
        Guid patientId = Guid.NewGuid();
        Guid serviceId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            var doctor = new Doctor(doctorId, null, "DOC_REV", "د. أحمد", Guid.Empty, Guid.Empty)
            {
                DoctorPercentage = 50 // 50%
            };
            await _doctorRepository.InsertAsync(doctor);

            var patient = new Patient(patientId, null, "MRN_REV", "مريض", "تجريبي", new DateTime(1990, 1, 1), Gender.Male, IdentityType.NationalId, "1231231231", "0501231231");
            await _patientRepository.InsertAsync(patient);

            var service = new ServiceItem(serviceId, "SRV_REV", "خدمة", ServiceCategory.Consultation) { Price = 200m };
            await _serviceItemRepository.InsertAsync(service);

            // Create Invoice
            var invoice = new Invoice(Guid.NewGuid(), null, patientId, "INV_REV_01")
            {
                Status = InvoiceStatus.Issued,
                TotalAmount = 200m,
                NetAmount = 200m
            };
            
            var invoiceItem = new InvoiceItem(Guid.NewGuid(), null, invoice.Id, "خدمة", 200m)
            {
                Quantity = 1
            };
            invoice.Items.Add(invoiceItem);

            await _invoiceRepository.InsertAsync(invoice);
        });

        // Act
        DoctorRevenueReportDto report = null;
        await WithUnitOfWorkAsync(async () =>
        {
            report = await _doctorRevenueReportAppService.GetReportAsync(new DoctorRevenueReportInput
            {
                FromDate = DateTime.Now.AddDays(-1),
                ToDate = DateTime.Now.AddDays(1)
            });
        });

        // Assert
        report.ShouldNotBeNull();
        report.Lines.ShouldContain(d => d.DoctorId == doctorId);
        
        var docReport = report.Lines.Find(d => d.DoctorId == doctorId);
        docReport.TotalRevenue.ShouldBe(200m); // Total service value
        docReport.DoctorAmount.ShouldBe(100m);  // 50% of 200
        docReport.HospitalAmount.ShouldBe(100m); // 50% of 200
        docReport.Details.Count.ShouldBeGreaterThanOrEqualTo(1);
    }
    
    [Fact]
    public async Task GetReportAsync_Should_Not_Crash_On_Empty_Services()
    {
        // Act
        DoctorRevenueReportDto report = null;
        await WithUnitOfWorkAsync(async () =>
        {
            report = await _doctorRevenueReportAppService.GetReportAsync(new DoctorRevenueReportInput
            {
                FromDate = DateTime.Now.AddDays(-1),
                ToDate = DateTime.Now.AddDays(1)
            });
        });

        // Assert
        report.ShouldNotBeNull();
    }
}
