import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface ClinicDto extends FullAuditedEntityDto<string> {
  code?: string;
  nameAr?: string;
  nameEn?: string | null;
  departmentId?: string;
  departmentName?: string | null;
  location?: string | null;
  roomNumber?: string | null;
  extensionNumber?: string | null;
  capacity?: number;
  appointmentDuration?: number;
  consultationFee?: number;
  isActive?: boolean;
  sortOrder?: number;
}

export interface CreateUpdateClinicDto {
  code?: string | null;
  nameAr?: string;
  nameEn?: string | null;
  departmentId?: string;
  location?: string | null;
  roomNumber?: string | null;
  extensionNumber?: string | null;
  capacity?: number;
  appointmentDuration?: number;
  consultationFee?: number;
  isActive?: boolean;
  sortOrder?: number;
}

export interface CreateUpdateDepartmentDto {
  code?: string | null;
  nameAr?: string;
  nameEn?: string | null;
  description?: string | null;
  location?: string | null;
  extensionNumber?: string | null;
  managerId?: string | null;
  isActive?: boolean;
  sortOrder?: number;
  isMedical?: boolean;
  costCenterId?: string | null;
  createCostCenterAccount?: boolean;
  parentAccountId?: string | null;
}

export interface CreateUpdateDoctorDto {
  code?: string | null;
  nameAr?: string;
  nameEn?: string | null;
  specialtyId?: string;
  departmentId?: string;
  clinicId?: string | null;
  licenseNumber?: string | null;
  licenseExpiryDate?: string | null;
  mobileNumber?: string | null;
  email?: string | null;
  degree?: string | null;
  consultationFee?: number;
  morningConsultationFee?: number;
  eveningConsultationFee?: number;
  followUpFee?: number;
  appointmentDuration?: number;
  bio?: string | null;
  userId?: string | null;
  isActive?: boolean;
  sortOrder?: number;
}

export interface CreateUpdateLaboratoryDto {
  code?: string | null;
  nameAr?: string;
  nameEn?: string | null;
  description?: string | null;
  location?: string | null;
  extensionNumber?: string | null;
  managerId?: string | null;
  workingHours?: string | null;
  is24Hours?: boolean;
  isActive?: boolean;
  sortOrder?: number;
}

export interface CreateUpdateSpecialtyDto {
  code?: string | null;
  nameAr?: string;
  nameEn?: string | null;
  description?: string | null;
  isActive?: boolean;
  sortOrder?: number;
}

export interface DepartmentDto extends FullAuditedEntityDto<string> {
  code?: string;
  nameAr?: string;
  nameEn?: string | null;
  description?: string | null;
  location?: string | null;
  extensionNumber?: string | null;
  managerId?: string | null;
  isActive?: boolean;
  sortOrder?: number;
  isMedical?: boolean;
  costCenterId?: string | null;
  createCostCenterAccount?: boolean;
  parentAccountId?: string | null;
}

export interface DoctorDto extends FullAuditedEntityDto<string> {
  code?: string;
  nameAr?: string;
  nameEn?: string | null;
  specialtyId?: string;
  specialtyName?: string | null;
  departmentId?: string;
  departmentName?: string | null;
  clinicId?: string | null;
  clinicName?: string | null;
  licenseNumber?: string | null;
  licenseExpiryDate?: string | null;
  mobileNumber?: string | null;
  email?: string | null;
  degree?: string | null;
  consultationFee?: number;
  morningConsultationFee?: number;
  eveningConsultationFee?: number;
  followUpFee?: number;
  appointmentDuration?: number;
  photoUrl?: string | null;
  bio?: string | null;
  userId?: string | null;
  isActive?: boolean;
  sortOrder?: number;
}

export interface GetClinicsInput extends PagedAndSortedResultRequestDto {
  searchText?: string | null;
  departmentId?: string | null;
  isActive?: boolean | null;
}

export interface GetDepartmentsInput extends PagedAndSortedResultRequestDto {
  searchText?: string | null;
  isActive?: boolean | null;
  isMedical?: boolean | null;
}

export interface GetDoctorsInput extends PagedAndSortedResultRequestDto {
  searchText?: string | null;
  specialtyId?: string | null;
  departmentId?: string | null;
  isActive?: boolean | null;
}

export interface GetLaboratoriesInput extends PagedAndSortedResultRequestDto {
  searchText?: string | null;
  isActive?: boolean | null;
}

export interface GetSpecialtiesInput extends PagedAndSortedResultRequestDto {
  searchText?: string | null;
  isActive?: boolean | null;
}

export interface HospitalSettingsDto {
  hospitalName: string;
  hospitalAddress?: string | null;
  hospitalPhone?: string | null;
  hospitalEmail?: string | null;
  hospitalLogo?: string | null;
  hospitalTaxNumber?: string | null;
}

export interface InpatientSettingsDto {
  admissionDepositAmount?: number;
  requireAdvancePayment?: boolean;
}

export interface LaboratoryDto extends FullAuditedEntityDto<string> {
  code?: string;
  nameAr?: string;
  nameEn?: string | null;
  description?: string | null;
  location?: string | null;
  extensionNumber?: string | null;
  managerId?: string | null;
  workingHours?: string | null;
  is24Hours?: boolean;
  isActive?: boolean;
  sortOrder?: number;
}

export interface LookupDto {
  id?: string;
  name?: string;
}

export interface PharmacySettingsDto {
  allowNegativeStock?: boolean;
}

export interface SpecialtyDto extends FullAuditedEntityDto<string> {
  code?: string;
  nameAr?: string;
  nameEn?: string | null;
  description?: string | null;
  isActive?: boolean;
  sortOrder?: number;
}
