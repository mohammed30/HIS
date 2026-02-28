import type { Gender } from './gender.enum';
import type { MaritalStatus } from './marital-status.enum';
import type { IdentityType } from './identity-type.enum';
import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CreateUpdatePatientDto {
  fullNameAr: string;
  fullNameEn?: string;
  firstNameAr?: string;
  middleNameAr?: string;
  lastNameAr?: string;
  firstNameEn?: string;
  middleNameEn?: string;
  lastNameEn?: string;
  dateOfBirth?: string;
  gender?: Gender;
  maritalStatus?: MaritalStatus;
  nationalityId?: string;
  professionId?: string;
  contractId?: string;
  paymentMethodId: string;
  referralSourceId?: string;
  identityType?: IdentityType;
  identityNumber?: string;
  identityExpiryDate?: string;
  identityIssueDate?: string;
  identityIssuePlace?: string;
  passportNumber?: string;
  passportIssueDate?: string;
  passportIssuePlace?: string;
  passportExpiryDate?: string;
  visaNumber?: string;
  visaIssueDate?: string;
  visaIssuePlace?: string;
  visaExpiryDate?: string;
  mobileNumber: string;
  phoneNumber?: string;
  email?: string;
  address?: string;
  city?: string;
  sponsorName?: string;
  sponsorId?: string;
  emergencyContactName?: string;
  emergencyContactRelation?: string;
  emergencyContactPhone?: string;
  cardNumber?: string;
  taxFile?: string;
  bloodType?: string;
  allergies?: string;
  notes?: string;
  isSocialSecurity?: boolean;
  isActive?: boolean;
}

export interface GetPatientsInput extends PagedAndSortedResultRequestDto {
  searchText?: string;
  mrn?: string;
  identityNumber?: string;
  mobileNumber?: string;
  gender?: Gender;
  paymentMethodId?: string;
  isActive?: boolean;
}

export interface PatientDto extends FullAuditedEntityDto<string> {
  mrn?: string;
  firstNameAr?: string;
  middleNameAr?: string;
  lastNameAr?: string;
  firstNameEn?: string;
  middleNameEn?: string;
  lastNameEn?: string;
  fullNameAr?: string;
  fullNameEn?: string;
  dateOfBirth?: string;
  age?: number;
  gender?: Gender;
  maritalStatus?: MaritalStatus;
  nationalityId?: string;
  nationalityName?: string;
  professionId?: string;
  professionName?: string;
  contractId?: string;
  contractName?: string;
  paymentMethodId?: string;
  paymentMethodName?: string;
  referralSourceId?: string;
  referralSourceName?: string;
  identityType?: IdentityType;
  identityNumber?: string;
  identityExpiryDate?: string;
  identityIssueDate?: string;
  identityIssuePlace?: string;
  passportNumber?: string;
  passportIssueDate?: string;
  passportIssuePlace?: string;
  passportExpiryDate?: string;
  visaNumber?: string;
  visaIssueDate?: string;
  visaIssuePlace?: string;
  visaExpiryDate?: string;
  mobileNumber?: string;
  phoneNumber?: string;
  email?: string;
  address?: string;
  city?: string;
  sponsorName?: string;
  sponsorId?: string;
  emergencyContactName?: string;
  emergencyContactRelation?: string;
  emergencyContactPhone?: string;
  cardNumber?: string;
  taxFile?: string;
  bloodType?: string;
  allergies?: string;
  notes?: string;
  photoUrl?: string;
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
