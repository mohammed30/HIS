import type { Gender } from './gender.enum';
import type { MaritalStatus } from './marital-status.enum';
import type { IdentityType } from './identity-type.enum';
import type { PatientCategory } from './patient-category.enum';
import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CreateUpdatePatientDto {
  firstNameAr?: string;
  middleNameAr?: string;
  lastNameAr?: string;
  firstNameEn?: string;
  middleNameEn?: string;
  lastNameEn?: string;
  dateOfBirth?: string;
  gender?: Gender;
  maritalStatus?: MaritalStatus;
  nationality?: string;
  identityType?: IdentityType;
  identityNumber?: string;
  identityExpiryDate?: string;
  mobileNumber?: string;
  phoneNumber?: string;
  email?: string;
  address?: string;
  city?: string;
  emergencyContactName?: string;
  emergencyContactRelation?: string;
  emergencyContactPhone?: string;
  category?: PatientCategory;
  bloodType?: string;
  allergies?: string;
  notes?: string;
}

export interface GetPatientsInput extends PagedAndSortedResultRequestDto {
  searchText?: string;
  mrn?: string;
  identityNumber?: string;
  mobileNumber?: string;
  gender?: Gender;
  category?: PatientCategory;
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
  nationality?: string;
  identityType?: IdentityType;
  identityNumber?: string;
  identityExpiryDate?: string;
  mobileNumber?: string;
  phoneNumber?: string;
  email?: string;
  address?: string;
  city?: string;
  emergencyContactName?: string;
  emergencyContactRelation?: string;
  emergencyContactPhone?: string;
  category?: PatientCategory;
  bloodType?: string;
  allergies?: string;
  notes?: string;
  photoUrl?: string;
  isActive?: boolean;
}

export interface PatientLookupDto {
  id?: string;
  mrn?: string;
  fullNameAr?: string;
  mobileNumber?: string;
}
