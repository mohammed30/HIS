using System;
using Volo.Abp.Application.Dtos;

namespace HIS.Insurance;

#region InsuranceCompany DTOs
public class InsuranceCompanyDto : FullAuditedEntityDto<Guid>
{
    public string Code { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactPhone { get; set; }
    public string? Website { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class CreateUpdateInsuranceCompanyDto
{
    public string? Code { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactPhone { get; set; }
    public string? Website { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}

public class GetInsuranceCompaniesInput : PagedAndSortedResultRequestDto
{
    public string? SearchText { get; set; }
    public bool? IsActive { get; set; }
}
#endregion

#region InsurancePlan DTOs
public class InsurancePlanDto : FullAuditedEntityDto<Guid>
{
    public Guid InsuranceCompanyId { get; set; }
    public string? InsuranceCompanyName { get; set; }
    public string Code { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public InsurancePlanType PlanType { get; set; }
    public InsurancePlanClass PlanClass { get; set; }
    public decimal CoveragePercentage { get; set; }
    public decimal? MaxCoverageAmount { get; set; }
    public decimal CoPaymentPercentage { get; set; }
    public decimal DeductibleAmount { get; set; }
    public bool IncludesMedications { get; set; }
    public bool IncludesLab { get; set; }
    public bool IncludesRadiology { get; set; }
    public bool IncludesInpatient { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    public int SortOrder { get; set; }
}

public class CreateUpdateInsurancePlanDto
{
    public Guid InsuranceCompanyId { get; set; }
    public string? Code { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public InsurancePlanType PlanType { get; set; } = InsurancePlanType.Individual;
    public InsurancePlanClass PlanClass { get; set; } = InsurancePlanClass.ClassB;
    public decimal CoveragePercentage { get; set; } = 80;
    public decimal? MaxCoverageAmount { get; set; }
    public decimal CoPaymentPercentage { get; set; } = 20;
    public decimal DeductibleAmount { get; set; } = 0;
    public bool IncludesMedications { get; set; } = true;
    public bool IncludesLab { get; set; } = true;
    public bool IncludesRadiology { get; set; } = true;
    public bool IncludesInpatient { get; set; } = false;
    public string? Notes { get; set; }
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}

public class GetInsurancePlansInput : PagedAndSortedResultRequestDto
{
    public string? SearchText { get; set; }
    public Guid? InsuranceCompanyId { get; set; }
    public bool? IsActive { get; set; }
}
#endregion

#region PatientInsurance DTOs
public class PatientInsuranceDto : FullAuditedEntityDto<Guid>
{
    public Guid PatientId { get; set; }
    public string? PatientName { get; set; }
    public Guid InsurancePlanId { get; set; }
    public string? InsurancePlanName { get; set; }
    public string? InsuranceCompanyName { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public string? CardNumber { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsPrimary { get; set; }
    public PatientInsuranceStatus Status { get; set; }
    public string? SubscriberName { get; set; }
    public string? RelationToSubscriber { get; set; }
    public string? EmployerName { get; set; }
    public string? Notes { get; set; }
}

public class CreateUpdatePatientInsuranceDto
{
    public Guid PatientId { get; set; }
    public Guid InsurancePlanId { get; set; }
    public string PolicyNumber { get; set; } = string.Empty;
    public string? CardNumber { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsPrimary { get; set; } = true;
    public PatientInsuranceStatus Status { get; set; } = PatientInsuranceStatus.Active;
    public string? SubscriberName { get; set; }
    public string? RelationToSubscriber { get; set; }
    public string? EmployerName { get; set; }
    public string? Notes { get; set; }
}

public class GetPatientInsurancesInput : PagedAndSortedResultRequestDto
{
    public Guid? PatientId { get; set; }
    public Guid? InsurancePlanId { get; set; }
    public PatientInsuranceStatus? Status { get; set; }
}
#endregion

#region InsuranceServicePrice DTOs
public class InsuranceServicePriceDto : FullAuditedEntityDto<Guid>
{
    public Guid InsurancePlanId { get; set; }
    public string? InsurancePlanName { get; set; }
    public Guid ServiceItemId { get; set; }
    public string? ServiceItemName { get; set; }
    public string? ServiceItemCode { get; set; }
    public decimal CustomPrice { get; set; }
    public string? Notes { get; set; }
}

public class CreateUpdateInsuranceServicePriceDto
{
    public Guid InsurancePlanId { get; set; }
    public Guid ServiceItemId { get; set; }
    public decimal CustomPrice { get; set; }
    public string? Notes { get; set; }
}

public class GetInsuranceServicePricesInput : PagedAndSortedResultRequestDto
{
    public Guid? InsurancePlanId { get; set; }
    public Guid? ServiceItemId { get; set; }
}
#endregion
