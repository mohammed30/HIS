import type { InsurancePlanType } from './insurance-plan-type.enum';
import type { InsurancePlanClass } from './insurance-plan-class.enum';
import type { PatientInsuranceStatus } from './patient-insurance-status.enum';
import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CreateUpdateInsuranceCompanyDto {
  code?: string | null;
  nameAr?: string;
  nameEn?: string | null;
  phone?: string | null;
  email?: string | null;
  address?: string | null;
  contactPerson?: string | null;
  contactPhone?: string | null;
  website?: string | null;
  notes?: string | null;
  isActive?: boolean;
  sortOrder?: number;
}

export interface CreateUpdateInsurancePlanDto {
  insuranceCompanyId?: string;
  code?: string | null;
  nameAr?: string;
  nameEn?: string | null;
  planType?: InsurancePlanType;
  planClass?: InsurancePlanClass;
  coveragePercentage?: number;
  maxCoverageAmount?: number | null;
  coPaymentPercentage?: number;
  deductibleAmount?: number;
  includesMedications?: boolean;
  includesLab?: boolean;
  includesRadiology?: boolean;
  includesInpatient?: boolean;
  notes?: string | null;
  isActive?: boolean;
  sortOrder?: number;
}

export interface CreateUpdateInsuranceServicePriceDto {
  insurancePlanId?: string;
  serviceItemId?: string;
  customPrice?: number;
  notes?: string | null;
}

export interface CreateUpdatePatientInsuranceDto {
  patientId?: string;
  insurancePlanId?: string;
  policyNumber?: string;
  cardNumber?: string | null;
  startDate?: string;
  endDate?: string;
  isPrimary?: boolean;
  status?: PatientInsuranceStatus;
  subscriberName?: string | null;
  relationToSubscriber?: string | null;
  employerName?: string | null;
  notes?: string | null;
}

export interface GetInsuranceCompaniesInput extends PagedAndSortedResultRequestDto {
  searchText?: string | null;
  isActive?: boolean | null;
}

export interface GetInsurancePlansInput extends PagedAndSortedResultRequestDto {
  searchText?: string | null;
  insuranceCompanyId?: string | null;
  isActive?: boolean | null;
}

export interface GetInsuranceReportInput extends PagedAndSortedResultRequestDto {
  fromDate?: string | null;
  toDate?: string | null;
  insuranceCompanyId?: string | null;
  insurancePlanId?: string | null;
}

export interface GetInsuranceServicePricesInput extends PagedAndSortedResultRequestDto {
  insurancePlanId?: string | null;
  serviceItemId?: string | null;
}

export interface GetPatientInsurancesInput extends PagedAndSortedResultRequestDto {
  patientId?: string | null;
  insurancePlanId?: string | null;
  status?: PatientInsuranceStatus | null;
}

export interface InsuranceCompanyDto extends FullAuditedEntityDto<string> {
  code?: string;
  nameAr?: string;
  nameEn?: string | null;
  phone?: string | null;
  email?: string | null;
  address?: string | null;
  contactPerson?: string | null;
  contactPhone?: string | null;
  website?: string | null;
  notes?: string | null;
  isActive?: boolean;
  sortOrder?: number;
}

export interface InsuranceDetailedClaimDto {
  invoiceId?: string;
  invoiceNumber?: string;
  invoiceDate?: string;
  patientId?: string;
  patientName?: string;
  insurancePlanName?: string;
  totalAmount?: number;
  insuranceShare?: number;
  patientShare?: number;
  status?: string;
}

export interface InsurancePlanDto extends FullAuditedEntityDto<string> {
  insuranceCompanyId?: string;
  insuranceCompanyName?: string | null;
  code?: string;
  nameAr?: string;
  nameEn?: string | null;
  planType?: InsurancePlanType;
  planClass?: InsurancePlanClass;
  coveragePercentage?: number;
  maxCoverageAmount?: number | null;
  coPaymentPercentage?: number;
  deductibleAmount?: number;
  includesMedications?: boolean;
  includesLab?: boolean;
  includesRadiology?: boolean;
  includesInpatient?: boolean;
  notes?: string | null;
  isActive?: boolean;
  sortOrder?: number;
}

export interface InsuranceServicePriceDto extends FullAuditedEntityDto<string> {
  insurancePlanId?: string;
  insurancePlanName?: string | null;
  serviceItemId?: string;
  serviceItemName?: string | null;
  serviceItemCode?: string | null;
  customPrice?: number;
  notes?: string | null;
}

export interface InsuranceSummaryDto {
  insuranceCompanyId?: string;
  insuranceCompanyName?: string;
  invoiceCount?: number;
  totalBilled?: number;
  totalInsuranceShare?: number;
  totalPatientShare?: number;
}

export interface LookupDto {
  id?: string;
  name?: string;
}

export interface PatientInsuranceDto extends FullAuditedEntityDto<string> {
  patientId?: string;
  patientName?: string | null;
  insurancePlanId?: string;
  insurancePlanName?: string | null;
  insuranceCompanyName?: string | null;
  policyNumber?: string;
  cardNumber?: string | null;
  startDate?: string;
  endDate?: string;
  isPrimary?: boolean;
  status?: PatientInsuranceStatus;
  subscriberName?: string | null;
  relationToSubscriber?: string | null;
  employerName?: string | null;
  notes?: string | null;
}
