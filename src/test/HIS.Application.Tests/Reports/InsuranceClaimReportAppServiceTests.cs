using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HIS.Billing;
using HIS.Insurance;
using HIS.Patients;
using HIS.Reports;
using NSubstitute;
using Volo.Abp.Domain.Repositories;
using Xunit;
using Volo.Abp.Users;
using System.Runtime.Serialization;

namespace HIS.Application.Tests.Reports
{
    public class InsuranceClaimReportAppServiceTests
    {
        private readonly IRepository<Invoice, Guid> _invoiceRepository;
        private readonly IRepository<PatientInsurance, Guid> _patientInsuranceRepository;
        private readonly IRepository<InsurancePlan, Guid> _insurancePlanRepository;
        private readonly IRepository<InsuranceCompany, Guid> _insuranceCompanyRepository;
        private readonly IRepository<Patient, Guid> _patientRepository;
        private readonly IRepository<HIS.Inpatient.Admission, Guid> _admissionRepository;
        private readonly InsuranceClaimReportAppService _service;

        public InsuranceClaimReportAppServiceTests()
        {
            _invoiceRepository = Substitute.For<IRepository<Invoice, Guid>>();
            _patientInsuranceRepository = Substitute.For<IRepository<PatientInsurance, Guid>>();
            _insurancePlanRepository = Substitute.For<IRepository<InsurancePlan, Guid>>();
            _insuranceCompanyRepository = Substitute.For<IRepository<InsuranceCompany, Guid>>();
            _patientRepository = Substitute.For<IRepository<Patient, Guid>>();
            _admissionRepository = Substitute.For<IRepository<HIS.Inpatient.Admission, Guid>>();
            _admissionRepository.GetListAsync(Arg.Any<System.Linq.Expressions.Expression<Func<HIS.Inpatient.Admission, bool>>>(), Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>())
                .Returns(Task.FromResult(new List<HIS.Inpatient.Admission>()));

            _service = new InsuranceClaimReportAppService(
                _invoiceRepository,
                _patientInsuranceRepository,
                _insurancePlanRepository,
                _insuranceCompanyRepository,
                _patientRepository,
                _admissionRepository
            );
        }

        private T CreateEntity<T>(Guid id)
        {
            var entity = (T)FormatterServices.GetUninitializedObject(typeof(T));
            var idProperty = entity.GetType().GetProperty("Id");
            if (idProperty != null && idProperty.CanWrite)
            {
                idProperty.SetValue(entity, id);
            }
            return entity;
        }

        [Fact]
        public async Task GetListAsync_Should_Filter_By_Date_And_Company()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var patientId = Guid.NewGuid();
            var patientInsuranceId = Guid.NewGuid();
            var today = DateTime.UtcNow.Date;

            // Mock Data
            var company = CreateEntity<InsuranceCompany>(companyId);
            company.NameAr = "Prime Health";

            var plan = CreateEntity<InsurancePlan>(planId);
            plan.InsuranceCompanyId = companyId;
            plan.NameAr = "Gold Plan";
            plan.CoveragePercentage = 80;
            
            var patientInsurance = CreateEntity<PatientInsurance>(patientInsuranceId);
            patientInsurance.PatientId = patientId;
            patientInsurance.InsurancePlanId = planId;
            patientInsurance.PolicyNumber = "POL-123";
            
            var invoiceId1 = Guid.NewGuid();
            var invoice1 = CreateEntity<Invoice>(invoiceId1);
            invoice1.PatientId = patientId;
            invoice1.PatientInsuranceId = patientInsuranceId;
            invoice1.InvoiceDate = today;
            invoice1.TotalAmount = 1000;
            
            var patient = CreateEntity<Patient>(patientId);
            patient.FirstNameAr = "أحمد";
            patient.LastNameAr = "محمد";
            
            var invoiceItem1 = CreateEntity<InvoiceItem>(Guid.NewGuid());
            invoiceItem1.InvoiceId = invoiceId1;
            invoiceItem1.Description = "CBC";
            invoiceItem1.UnitPrice = 1000;
            invoiceItem1.Quantity = 1;
            invoiceItem1.IsCoveredByInsurance = true;
            invoiceItem1.InsurancePercentage = 80;
            invoiceItem1.ServiceCode = "85025";
            
            invoice1.Items = new List<InvoiceItem> { invoiceItem1 };

            // Set up repositories
            var invoiceList = new List<Invoice> { invoice1 }.AsQueryable();
            _invoiceRepository.WithDetailsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Invoice, object>>[]>()).Returns(Task.FromResult(invoiceList));
            _patientInsuranceRepository.FindAsync(patientInsuranceId).Returns(Task.FromResult(patientInsurance));
            _insurancePlanRepository.FindAsync(planId).Returns(Task.FromResult(plan));
            _insuranceCompanyRepository.FindAsync(companyId).Returns(Task.FromResult(company));
            _patientRepository.FindAsync(patientId).Returns(Task.FromResult(patient));

            var input = new GetInsuranceClaimsInput
            {
                InsuranceCompanyId = companyId,
                StartDate = today.AddDays(-1),
                EndDate = today.AddDays(1),
                SkipCount = 0,
                MaxResultCount = 10
            };

            // Act
            var result = await _service.GetListAsync(input);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            var claim = result.Items.First();
            Assert.Equal("أحمد محمد", claim.PatientName);
            Assert.Equal("POL-123", claim.PolicyNumber);
            Assert.Single(claim.Items);
            Assert.Equal(800, claim.Items.First().InsuranceCoverage);
        }

        [Fact]
        public async Task GetListAsync_EmptyResult_When_Company_Mismatches()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var differentCompanyId = Guid.NewGuid(); // Mismatch
            var planId = Guid.NewGuid();
            var patientId = Guid.NewGuid();
            var patientInsuranceId = Guid.NewGuid();
            var today = DateTime.UtcNow.Date;

            var company = CreateEntity<InsuranceCompany>(differentCompanyId);
            company.NameAr = "Different Health";

            var plan = CreateEntity<InsurancePlan>(planId);
            plan.InsuranceCompanyId = differentCompanyId;
            plan.NameAr = "Gold Plan";
            plan.CoveragePercentage = 80;
            
            var patientInsurance = CreateEntity<PatientInsurance>(patientInsuranceId);
            patientInsurance.PatientId = patientId;
            patientInsurance.InsurancePlanId = planId;
            patientInsurance.PolicyNumber = "POL-123";
            
            var invoiceId1 = Guid.NewGuid();
            var invoice1 = CreateEntity<Invoice>(invoiceId1);
            invoice1.PatientId = patientId;
            invoice1.PatientInsuranceId = patientInsuranceId;
            invoice1.InvoiceDate = today;
            invoice1.TotalAmount = 1000;
            invoice1.Items = new List<InvoiceItem>();
            
            var patient = CreateEntity<Patient>(patientId);
            patient.FirstNameAr = "أحمد";
            patient.LastNameAr = "محمد";
            
            var invoiceList = new List<Invoice> { invoice1 }.AsQueryable();
            _invoiceRepository.WithDetailsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Invoice, object>>[]>()).Returns(Task.FromResult(invoiceList));
            _patientInsuranceRepository.FindAsync(patientInsuranceId).Returns(Task.FromResult(patientInsurance));
            _insurancePlanRepository.FindAsync(planId).Returns(Task.FromResult(plan));
            _insuranceCompanyRepository.FindAsync(differentCompanyId).Returns(Task.FromResult(company));
            _patientRepository.FindAsync(patientId).Returns(Task.FromResult(patient));

            var input = new GetInsuranceClaimsInput
            {
                InsuranceCompanyId = companyId, // Querying for companyId, but data is differentCompanyId
                StartDate = today.AddDays(-1),
                EndDate = today.AddDays(1)
            };

            // Act
            var result = await _service.GetListAsync(input);

            // Assert
            Assert.Equal(0, result.TotalCount);
            Assert.Empty(result.Items);
        }

        [Fact]
        public async Task GetListAsync_Should_Filter_By_PatientType_Inpatient()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var today = DateTime.UtcNow.Date;
            
            var invoiceId1 = Guid.NewGuid();
            var invoice1 = CreateEntity<Invoice>(invoiceId1);
            invoice1.PatientId = patientId;
            invoice1.PatientInsuranceId = Guid.NewGuid();
            invoice1.InvoiceDate = today;
            invoice1.TotalAmount = 1000;
            
            var invoiceItem1 = CreateEntity<InvoiceItem>(Guid.NewGuid());
            invoiceItem1.InvoiceId = invoiceId1;
            invoiceItem1.UnitPrice = 1000;
            invoiceItem1.Quantity = 1;
            invoiceItem1.IsCoveredByInsurance = true;
            invoiceItem1.InsurancePercentage = 100;
            invoice1.Items = new List<InvoiceItem> { invoiceItem1 };

            var admission = CreateEntity<HIS.Inpatient.Admission>(Guid.NewGuid());
            admission.PatientId = patientId;
            admission.AdmissionDate = today.AddDays(-1);
            admission.DischargeDate = today.AddDays(1);

            var invoiceList = new List<Invoice> { invoice1 }.AsQueryable();
            _invoiceRepository.WithDetailsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Invoice, object>>[]>()).Returns(Task.FromResult(invoiceList));
            
            _patientInsuranceRepository.FindAsync(Arg.Any<Guid>()).Returns(Task.FromResult(CreateEntity<PatientInsurance>(Guid.NewGuid())));
            _insurancePlanRepository.FindAsync(Arg.Any<Guid>()).Returns(Task.FromResult(CreateEntity<InsurancePlan>(Guid.NewGuid())));
            _insuranceCompanyRepository.FindAsync(Arg.Any<Guid>()).Returns(Task.FromResult(CreateEntity<InsuranceCompany>(Guid.NewGuid())));
            _patientRepository.FindAsync(Arg.Any<Guid>()).Returns(Task.FromResult(CreateEntity<Patient>(Guid.NewGuid())));
            
            // Mock admission
            _admissionRepository.GetListAsync(Arg.Any<System.Linq.Expressions.Expression<Func<HIS.Inpatient.Admission, bool>>>(), Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>())
                .Returns(Task.FromResult(new List<HIS.Inpatient.Admission> { admission }));

            var input = new GetInsuranceClaimsInput { PatientType = 1 }; // Inpatient

            // Act
            var result = await _service.GetListAsync(input);

            // Assert
            Assert.Equal(1, result.TotalCount);
        }

        [Fact]
        public async Task GetListAsync_Should_Filter_By_PatientType_Outpatient()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var today = DateTime.UtcNow.Date;
            
            var invoiceId1 = Guid.NewGuid();
            var invoice1 = CreateEntity<Invoice>(invoiceId1);
            invoice1.PatientId = patientId;
            invoice1.PatientInsuranceId = Guid.NewGuid();
            invoice1.InvoiceDate = today;
            invoice1.TotalAmount = 1000;
            
            var invoiceItem1 = CreateEntity<InvoiceItem>(Guid.NewGuid());
            invoiceItem1.InvoiceId = invoiceId1;
            invoiceItem1.UnitPrice = 1000;
            invoiceItem1.Quantity = 1;
            invoiceItem1.IsCoveredByInsurance = true;
            invoiceItem1.InsurancePercentage = 100;
            invoice1.Items = new List<InvoiceItem> { invoiceItem1 };

            // Outpatient = no admission covering the invoice date
            var admission = CreateEntity<HIS.Inpatient.Admission>(Guid.NewGuid());
            admission.PatientId = patientId;
            admission.AdmissionDate = today.AddDays(-10);
            admission.DischargeDate = today.AddDays(-9); // Not overlapping with 'today'

            var invoiceList = new List<Invoice> { invoice1 }.AsQueryable();
            _invoiceRepository.WithDetailsAsync(Arg.Any<System.Linq.Expressions.Expression<Func<Invoice, object>>[]>()).Returns(Task.FromResult(invoiceList));
            
            _patientInsuranceRepository.FindAsync(Arg.Any<Guid>()).Returns(Task.FromResult(CreateEntity<PatientInsurance>(Guid.NewGuid())));
            _insurancePlanRepository.FindAsync(Arg.Any<Guid>()).Returns(Task.FromResult(CreateEntity<InsurancePlan>(Guid.NewGuid())));
            _insuranceCompanyRepository.FindAsync(Arg.Any<Guid>()).Returns(Task.FromResult(CreateEntity<InsuranceCompany>(Guid.NewGuid())));
            _patientRepository.FindAsync(Arg.Any<Guid>()).Returns(Task.FromResult(CreateEntity<Patient>(Guid.NewGuid())));
            
            // Mock admission
            _admissionRepository.GetListAsync(Arg.Any<System.Linq.Expressions.Expression<Func<HIS.Inpatient.Admission, bool>>>(), Arg.Any<bool>(), Arg.Any<System.Threading.CancellationToken>())
                .Returns(Task.FromResult(new List<HIS.Inpatient.Admission> { admission }));

            var input = new GetInsuranceClaimsInput { PatientType = 2 }; // Outpatient

            // Act
            var result = await _service.GetListAsync(input);

            // Assert
            Assert.Equal(1, result.TotalCount);
            
            // Test Inpatient for same data should be 0
            var inputInpatient = new GetInsuranceClaimsInput { PatientType = 1 };
            var resultInpatient = await _service.GetListAsync(inputInpatient);
            Assert.Equal(0, resultInpatient.TotalCount);
        }
    }
}
