import type { Gender } from './gender.enum';
import type { MaritalStatus } from './marital-status.enum';
import type { IdentityType } from './identity-type.enum';
import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CreateUpdatePatientDto {
  fullNameAr: string | null;
  fullNameEn?: string | null;
  firstNameAr?: string;
  middleNameAr?: string | null;
  lastNameAr?: string;
  firstNameEn?: string | null;
  middleNameEn?: string | null;
  lastNameEn?: string | null;
  dateOfBirth?: string | null;
  gender?: Gender;
  maritalStatus?: MaritalStatus | null;
  nationalityId?: string | null;
  professionId?: string | null;
  contractId?: string | null;
  paymentMethodId: string | null;
  referralSourceId?: string | null;
  identityType?: IdentityType;
  identityNumber?: string;
  identityExpiryDate?: string | null;
  identityIssueDate?: string | null;
  identityIssuePlace?: string | null;
  passportNumber?: string | null;
  passportIssueDate?: string | null;
  passportIssuePlace?: string | null;
  passportExpiryDate?: string | null;
  visaNumber?: string | null;
  visaIssueDate?: string | null;
  visaIssuePlace?: string | null;
  visaExpiryDate?: string | null;
  mobileNumber: string;
  phoneNumber?: string | null;
  email?: string | null;
  address?: string | null;
  city?: string | null;
  sponsorName?: string | null;
  sponsorId?: string | null;
  emergencyContactName?: string | null;
  emergencyContactRelation?: string | null;
  emergencyContactPhone?: string | null;
  cardNumber?: string | null;
  taxFile?: string | null;
  bloodType?: string | null;
  allergies?: string | null;
  notes?: string | null;
  isSocialSecurity?: boolean;
  isActive?: boolean;
}

export interface GetPatientsInput extends PagedAndSortedResultRequestDto {
  searchText?: string | null;
  mrn?: string | null;
  identityNumber?: string | null;
  mobileNumber?: string | null;
  gender?: Gender | null;
  paymentMethodId?: string | null;
  isActive?: boolean | null;
}

export interface PatientDto extends FullAuditedEntityDto<string> {
  mrn?: string;
  firstNameAr?: string;
  middleNameAr?: string | null;
  lastNameAr?: string;
  firstNameEn?: string | null;
  middleNameEn?: string | null;
  lastNameEn?: string | null;
  fullNameAr?: string;
  fullNameEn?: string | null;
  dateOfBirth?: string | null;
  age?: number | null;
  gender?: Gender;
  maritalStatus?: MaritalStatus | null;
  nationalityId?: string | null;
  nationalityName?: string | null;
  professionId?: string | null;
  professionName?: string | null;
  contractId?: string | null;
  contractName?: string | null;
  paymentMethodId?: string | null;
  paymentMethodName?: string | null;
  referralSourceId?: string | null;
  referralSourceName?: string | null;
  identityType?: IdentityType;
  identityNumber?: string;
  identityExpiryDate?: string | null;
  identityIssueDate?: string | null;
  identityIssuePlace?: string | null;
  passportNumber?: string | null;
  passportIssueDate?: string | null;
  passportIssuePlace?: string | null;
  passportExpiryDate?: string | null;
  visaNumber?: string | null;
  visaIssueDate?: string | null;
  visaIssuePlace?: string | null;
  visaExpiryDate?: string | null;
  mobileNumber?: string;
  phoneNumber?: string | null;
  email?: string | null;
  address?: string | null;
  city?: string | null;
  sponsorName?: string | null;
  sponsorId?: string | null;
  emergencyContactName?: string | null;
  emergencyContactRelation?: string | null;
  emergencyContactPhone?: string | null;
  cardNumber?: string | null;
  taxFile?: string | null;
  bloodType?: string | null;
  allergies?: string | null;
  notes?: string | null;
  photoUrl?: string | null;
  isSocialSecurity?: boolean;
  isActive?: boolean;
}

export interface PatientLookupDto {
  id?: string;
  mrn?: string;
  fullNameAr?: string;
  mobileNumber?: string;
}

export interface PatientServiceItemDto {
  date?: string;
  invoiceNumber?: string;
  serviceDescription?: string;
  quantity?: number;
  unitPrice?: number;
  totalPrice?: number;
  status?: string;
  isPaid?: boolean;
}

export interface PatientServicesReportDto {
  patientId?: string;
  patientName?: string;
  mrn?: string;
  reportDate?: string;
  services?: PatientServiceItemDto[];
  totalAmountInvoiced?: number;
  totalAmountPaid?: number;
  totalAmountDue?: number;
}
