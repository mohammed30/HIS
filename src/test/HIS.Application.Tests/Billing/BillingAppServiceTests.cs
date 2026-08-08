using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Accounting;
using HIS.Billing;
using HIS.Patients;
using HIS.Services;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace HIS.Billing.Tests;

public abstract class BillingAppServiceTests<TStartupModule> : BillingTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IInvoiceAppService _invoiceAppService;
    private readonly IPaymentAppService _paymentAppService;
    private readonly IRepository<Invoice, Guid> _invoiceRepository;
    private readonly IRepository<Patient, Guid> _patientRepository;
    private readonly IRepository<ServiceItem, Guid> _serviceItemRepository;
    private readonly IRepository<Account, Guid> _accountRepository;
    private readonly IRepository<JournalEntry, Guid> _journalEntryRepository;

    protected BillingAppServiceTests()
    {
        _invoiceAppService = GetRequiredService<IInvoiceAppService>();
        _paymentAppService = GetRequiredService<IPaymentAppService>();
        _invoiceRepository = GetRequiredService<IRepository<Invoice, Guid>>();
        _patientRepository = GetRequiredService<IRepository<Patient, Guid>>();
        _serviceItemRepository = GetRequiredService<IRepository<ServiceItem, Guid>>();
        _accountRepository = GetRequiredService<IRepository<Account, Guid>>();
        _journalEntryRepository = GetRequiredService<IRepository<JournalEntry, Guid>>();
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Invoice_And_Calculate_Totals()
    {
        // 1. Arrange
        Guid patientId = Guid.NewGuid();
        Guid serviceItemId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await EnsureAccountMappingsAreFilledAsync();

            var patient = new Patient(patientId, null, "MRN001", "جون", "دو", new DateTime(1990, 1, 1), Gender.Male, IdentityType.NationalId, "1234567890", "0500000000");
            await _patientRepository.InsertAsync(patient);

            var serviceItem = new ServiceItem(serviceItemId, "S001", "Consultation", ServiceCategory.Consultation) { Price = 100m };
            await _serviceItemRepository.InsertAsync(serviceItem);
        });

        // 2. Act
        InvoiceDto result = null;
        await WithUnitOfWorkAsync(async () =>
        {
            var createDto = new CreateUpdateInvoiceDto
            {
                PatientId = patientId,
                DueDate = DateTime.Now.AddDays(7),
                DiscountAmount = 10m,
                TaxPercentage = 15m,
                Items = new List<CreateUpdateInvoiceItemDto>
                {
                    new CreateUpdateInvoiceItemDto
                    {
                        ServiceCode = "S001",
                        Description = "Consultation",
                        Quantity = 2,
                        UnitPrice = 100m,
                        DiscountPercentage = 0
                    }
                }
            };

            result = await _invoiceAppService.CreateAsync(createDto);
        });

        // 3. Assert
        await WithUnitOfWorkAsync(async () =>
        {
            result.ShouldNotBeNull();
            var invoice = await _invoiceRepository.GetAsync(result.Id);
            invoice.ShouldNotBeNull();
            
            // Total amount: 2 * 100 = 200
            // SubTotal: 200 - 10 (DiscountAmount) = 190
            // Tax: 190 * 0.15 = 28.5
            // Net Amount: 190 + 28.5 = 218.5
            invoice.TotalAmount.ShouldBe(200m);
            invoice.DiscountAmount.ShouldBe(10m);
            invoice.TaxAmount.ShouldBe(28.5m);
            invoice.NetAmount.ShouldBe(218.5m);
            invoice.Status.ShouldBe(InvoiceStatus.Issued);
        });
    }

    [Fact]
    public async Task PayInvoice_Should_Update_Status_And_Create_JournalEntry()
    {
        // 1. Arrange
        Guid invoiceId = Guid.Empty;
        Guid patientId = Guid.NewGuid();
        await WithUnitOfWorkAsync(async () =>
        {
            await EnsureAccountMappingsAreFilledAsync();

            var patient = new Patient(patientId, null, "MRN002", "جين", "دو", new DateTime(1995, 1, 1), Gender.Female, IdentityType.NationalId, "0987654321", "0501111111");
            await _patientRepository.InsertAsync(patient);

            var createDto = new CreateUpdateInvoiceDto
            {
                PatientId = patientId,
                DueDate = DateTime.Now,
                Items = new List<CreateUpdateInvoiceItemDto>
                {
                    new CreateUpdateInvoiceItemDto
                    {
                        Description = "X-Ray",
                        Quantity = 1,
                        UnitPrice = 500m
                    }
                }
            };

            var createdInvoice = await _invoiceAppService.CreateAsync(createDto);
            invoiceId = createdInvoice.Id;
        });

        // 2. Act
        await WithUnitOfWorkAsync(async () =>
        {
            var paymentDto = new CreatePaymentDto
            {
                InvoiceId = invoiceId,
                PatientId = patientId,
                Amount = 500m,
                PaymentMethod = HIS.Billing.PaymentMethod.Cash,
                ReferenceNumber = "REC-001"
            };
            await _paymentAppService.CreateAsync(paymentDto);
        });

        // 3. Assert
        await WithUnitOfWorkAsync(async () =>
        {
            var invoice = await _invoiceRepository.GetAsync(invoiceId);
            invoice.Status.ShouldBe(InvoiceStatus.Paid);
            invoice.PaidAmount.ShouldBe(500m);

            var journalEntries = await _journalEntryRepository.GetListAsync();
            var paymentJv = journalEntries.FirstOrDefault(je => je.ReferenceNumber.StartsWith("PAY") && je.Description.Contains("سند قبض"));
            paymentJv.ShouldNotBeNull();
            paymentJv.IsPosted.ShouldBeTrue();
        });
    }
    [Fact]
    public async Task CreateAsync_Should_Create_Posted_JournalEntry()
    {
        // 1. Arrange
        Guid patientId = Guid.NewGuid();
        Guid serviceItemId = Guid.NewGuid();

        await WithUnitOfWorkAsync(async () =>
        {
            await EnsureAccountMappingsAreFilledAsync();

            var patient = new Patient(patientId, null, "MRN003", "علي", "أحمد", new DateTime(1990, 1, 1), Gender.Male, IdentityType.NationalId, "1234567891", "0500000001");
            await _patientRepository.InsertAsync(patient);

            var serviceItem = new ServiceItem(serviceItemId, "S002", "Checkup", ServiceCategory.Consultation) { Price = 100m };
            await _serviceItemRepository.InsertAsync(serviceItem);
        });

        // 2. Act
        InvoiceDto result = null;
        await WithUnitOfWorkAsync(async () =>
        {
            var createDto = new CreateUpdateInvoiceDto
            {
                PatientId = patientId,
                DueDate = DateTime.Now,
                Items = new List<CreateUpdateInvoiceItemDto>
                {
                    new CreateUpdateInvoiceItemDto
                    {
                        ServiceCode = "S002",
                        Description = "Checkup",
                        Quantity = 1,
                        UnitPrice = 100m
                    }
                }
            };
            result = await _invoiceAppService.CreateAsync(createDto);
        });

        // 3. Assert
        await WithUnitOfWorkAsync(async () =>
        {
            var journalEntries = await _journalEntryRepository.GetListAsync();
            var invoiceJv = journalEntries.FirstOrDefault(je => je.ReferenceNumber == result.InvoiceNumber);
            
            invoiceJv.ShouldNotBeNull();
            invoiceJv.IsPosted.ShouldBeTrue(); // Must be posted
        });
    }

    [Fact]
    public async Task GetPendingApprovalsAsync_Should_Return_Pending_Invoices()
    {
        // 1. Arrange
        Guid patientId = Guid.NewGuid();
        await WithUnitOfWorkAsync(async () =>
        {
            var patient = new Patient(patientId, null, "MRN_PEND", "مريض", "معلق", new DateTime(1990, 1, 1), Gender.Male, IdentityType.NationalId, "2234567891", "0500000002");
            await _patientRepository.InsertAsync(patient);

            var invoice = new Invoice(Guid.NewGuid(), null, patientId, "INV-PEND-01")
            {
                Status = InvoiceStatus.PendingApproval,
                TotalAmount = 500m,
                NetAmount = 500m
            };
            await _invoiceRepository.InsertAsync(invoice);
        });

        // 2. Act
        List<InvoiceDto> pendingInvoices = null;
        await WithUnitOfWorkAsync(async () =>
        {
            pendingInvoices = await _invoiceAppService.GetPendingApprovalsAsync();
        });

        // 3. Assert
        pendingInvoices.ShouldNotBeNull();
        pendingInvoices.ShouldContain(i => i.InvoiceNumber == "INV-PEND-01");
        pendingInvoices.All(i => i.Status == InvoiceStatus.PendingApproval).ShouldBeTrue();
    }

    [Fact]
    public async Task ApproveInvoiceAsync_Should_Change_Status_To_Issued()
    {
        // 1. Arrange
        Guid invoiceId = Guid.NewGuid();
        await WithUnitOfWorkAsync(async () =>
        {
            await EnsureAccountMappingsAreFilledAsync();
            var patient = new Patient(Guid.NewGuid(), null, "MRN_APP", "مريض", "معتمد", new DateTime(1990, 1, 1), Gender.Male, IdentityType.NationalId, "3234567891", "0500000003");
            await _patientRepository.InsertAsync(patient);

            var invoice = new Invoice(invoiceId, null, patient.Id, "INV-APP-01")
            {
                Status = InvoiceStatus.PendingApproval,
                TotalAmount = 500m,
                NetAmount = 500m
            };
            await _invoiceRepository.InsertAsync(invoice);
        });

        // 2. Act
        await WithUnitOfWorkAsync(async () =>
        {
            await _invoiceAppService.ApproveInvoiceAsync(invoiceId);
        });

        // 3. Assert
        await WithUnitOfWorkAsync(async () =>
        {
            var invoice = await _invoiceRepository.GetAsync(invoiceId);
            invoice.Status.ShouldBe(InvoiceStatus.Issued);
        });
    }

    [Fact]
    public async Task RejectInvoiceAsync_Should_Change_Status_To_Rejected()
    {
        // 1. Arrange
        Guid invoiceId = Guid.NewGuid();
        await WithUnitOfWorkAsync(async () =>
        {
            var patient = new Patient(Guid.NewGuid(), null, "MRN_REJ", "مريض", "مرفوض", new DateTime(1990, 1, 1), Gender.Male, IdentityType.NationalId, "4234567891", "0500000004");
            await _patientRepository.InsertAsync(patient);

            var invoice = new Invoice(invoiceId, null, patient.Id, "INV-REJ-01")
            {
                Status = InvoiceStatus.PendingApproval,
                TotalAmount = 500m,
                NetAmount = 500m
            };
            await _invoiceRepository.InsertAsync(invoice);
        });

        // 2. Act
        await WithUnitOfWorkAsync(async () =>
        {
            await _invoiceAppService.RejectInvoiceAsync(invoiceId);
        });

        // 3. Assert
        await WithUnitOfWorkAsync(async () =>
        {
            var invoice = await _invoiceRepository.GetAsync(invoiceId);
            invoice.Status.ShouldBe(InvoiceStatus.Rejected);
        });
    }
}
