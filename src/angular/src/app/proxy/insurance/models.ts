import type { InsurancePlanType } from './insurance-plan-type.enum';
import type { PatientInsuranceStatus } from './patient-insurance-status.enum';
import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CreateUpdateInsuranceCompanyDto {
  code?: string;
  nameAr?: string;
  nameEn?: string;
  phone?: string;
  email?: string;
  address?: string;
  contactPerson?: string;
  contactPhone?: string;
  website?: string;
  notes?: string;
  isActive?: boolean;
  sortOrder?: number;
}

export interface CreateUpdateInsurancePlanDto {
  insuranceCompanyId?: string;
  code?: string;
  nameAr?: string;
  nameEn?: string;
  planType?: InsurancePlanType;
  coveragePercentage?: number;
  maxCoverageAmount?: number;
  coPaymentPercentage?: number;
  deductibleAmount?: number;
  includesMedications?: boolean;
  includesLab?: boolean;
  includesRadiology?: boolean;
  includesInpatient?: boolean;
  notes?: string;
  isActive?: boolean;
  sortOrder?: number;
}

export interface CreateUpdatePatientInsuranceDto {
  patientId?: string;
  insurancePlanId?: string;
  policyNumber?: string;
  cardNumber?: string;
  startDate?: string;
  endDate?: string;
  isPrimary?: boolean;
  status?: PatientInsuranceStatus;
  subscriberName?: string;
  relationToSubscriber?: string;
  employerName?: string;
  notes?: string;
}

export interface GetInsuranceCompaniesInput extends PagedAndSortedResultRequestDto {
  searchText?: string;
  isActive?: boolean;
}

export interface GetInsurancePlansInput extends PagedAndSortedResultRequestDto {
  searchText?: string;
  insuranceCompanyId?: string;
  isActive?: boolean;
}

export interface GetPatientInsurancesInput extends PagedAndSortedResultRequestDto {
  patientId?: string;
  insurancePlanId?: string;
  status?: PatientInsuranceStatus;
}

export interface InsuranceCompanyDto extends FullAuditedEntityDto<string> {
  code?: string;
  nameAr?: string;
  nameEn?: string;
  phone?: string;
  email?: string;
  address?: string;
  contactPerson?: string;
  contactPhone?: string;
  website?: string;
  notes?: string;
  isActive?: boolean;
  sortOrder?: number;
}

export interface InsurancePlanDto extends FullAuditedEntityDto<string> {
  insuranceCompanyId?: string;
  insuranceCompanyName?: string;
  code?: string;
  nameAr?: string;
  nameEn?: string;
  planType?: InsurancePlanType;
  coveragePercentage?: number;
  maxCoverageAmount?: number;
  coPaymentPercentage?: number;
  deductibleAmount?: number;
  includesMedications?: boolean;
  includesLab?: boolean;
  includesRadiology?: boolean;
  includesInpatient?: boolean;
  notes?: string;
  isActive?: boolean;
  sortOrder?: number;
}

export interface LookupDto {
  id?: string;
  name?: string;
}

export interface PatientInsuranceDto extends FullAuditedEntityDto<string> {
  patientId?: string;
  patientName?: string;
  insurancePlanId?: string;
  insurancePlanName?: string;
  insuranceCompanyName?: string;
  policyNumber?: string;
  cardNumber?: string;
  startDate?: string;
  endDate?: string;
  isPrimary?: boolean;
  status?: PatientInsuranceStatus;
  subscriberName?: string;
  relationToSubscriber?: string;
  employerName?: string;
  notes?: string;
}
