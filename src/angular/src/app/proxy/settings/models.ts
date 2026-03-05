import type { FullAuditedEntityDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface ClinicDto extends FullAuditedEntityDto<string> {
  code?: string;
  nameAr?: string;
  nameEn?: string;
  departmentId?: string;
  departmentName?: string;
  location?: string;
  roomNumber?: string;
  extensionNumber?: string;
  capacity?: number;
  appointmentDuration?: number;
  consultationFee?: number;
  isActive?: boolean;
  sortOrder?: number;
}

export interface CreateUpdateClinicDto {
  code?: string;
  nameAr?: string;
  nameEn?: string;
  departmentId?: string;
  location?: string;
  roomNumber?: string;
  extensionNumber?: string;
  capacity?: number;
  appointmentDuration?: number;
  consultationFee?: number;
  isActive?: boolean;
  sortOrder?: number;
}

export interface CreateUpdateDepartmentDto {
  code?: string;
  nameAr?: string;
  nameEn?: string;
  description?: string;
  location?: string;
  extensionNumber?: string;
  managerId?: string;
  isActive?: boolean;
  sortOrder?: number;
  isMedical?: boolean;
  costCenterId?: string;
}

export interface CreateUpdateDoctorDto {
  code?: string;
  nameAr?: string;
  nameEn?: string;
  specialtyId?: string;
  departmentId?: string;
  licenseNumber?: string;
  licenseExpiryDate?: string;
  mobileNumber?: string;
  email?: string;
  degree?: string;
  consultationFee?: number;
  followUpFee?: number;
  appointmentDuration?: number;
  bio?: string;
  userId?: string;
  isActive?: boolean;
  sortOrder?: number;
}

export interface CreateUpdateLaboratoryDto {
  code?: string;
  nameAr?: string;
  nameEn?: string;
  description?: string;
  location?: string;
  extensionNumber?: string;
  managerId?: string;
  workingHours?: string;
  is24Hours?: boolean;
  isActive?: boolean;
  sortOrder?: number;
}

export interface CreateUpdateSpecialtyDto {
  code?: string;
  nameAr?: string;
  nameEn?: string;
  description?: string;
  isActive?: boolean;
  sortOrder?: number;
}

export interface DepartmentDto extends FullAuditedEntityDto<string> {
  code?: string;
  nameAr?: string;
  nameEn?: string;
  description?: string;
  location?: string;
  extensionNumber?: string;
  managerId?: string;
  isActive?: boolean;
  sortOrder?: number;
  isMedical?: boolean;
  costCenterId?: string;
}

export interface DoctorDto extends FullAuditedEntityDto<string> {
  code?: string;
  nameAr?: string;
  nameEn?: string;
  specialtyId?: string;
  specialtyName?: string;
  departmentId?: string;
  departmentName?: string;
  licenseNumber?: string;
  licenseExpiryDate?: string;
  mobileNumber?: string;
  email?: string;
  degree?: string;
  consultationFee?: number;
  followUpFee?: number;
  appointmentDuration?: number;
  photoUrl?: string;
  bio?: string;
  userId?: string;
  isActive?: boolean;
  sortOrder?: number;
}

export interface GetClinicsInput extends PagedAndSortedResultRequestDto {
  searchText?: string;
  departmentId?: string;
  isActive?: boolean;
}

export interface GetDepartmentsInput extends PagedAndSortedResultRequestDto {
  searchText?: string;
  isActive?: boolean;
  isMedical?: boolean;
}

export interface GetDoctorsInput extends PagedAndSortedResultRequestDto {
  searchText?: string;
  specialtyId?: string;
  departmentId?: string;
  isActive?: boolean;
}

export interface GetLaboratoriesInput extends PagedAndSortedResultRequestDto {
  searchText?: string;
  isActive?: boolean;
}

export interface GetSpecialtiesInput extends PagedAndSortedResultRequestDto {
  searchText?: string;
  isActive?: boolean;
}

export interface HospitalSettingsDto {
  hospitalName: string;
  hospitalAddress?: string;
  hospitalPhone?: string;
  hospitalEmail?: string;
  hospitalLogo?: string;
  hospitalTaxNumber?: string;
}

export interface LaboratoryDto extends FullAuditedEntityDto<string> {
  code?: string;
  nameAr?: string;
  nameEn?: string;
  description?: string;
  location?: string;
  extensionNumber?: string;
  managerId?: string;
  workingHours?: string;
  is24Hours?: boolean;
  isActive?: boolean;
  sortOrder?: number;
}

export interface LookupDto {
  id?: string;
  name?: string;
}

export interface SpecialtyDto extends FullAuditedEntityDto<string> {
  code?: string;
  nameAr?: string;
  nameEn?: string;
  description?: string;
  isActive?: boolean;
  sortOrder?: number;
}

export interface JobTitleDto extends FullAuditedEntityDto<string> {
  nameAr?: string;
  nameEn?: string;
  description?: string;
  departmentId?: string;
  departmentName?: string;
}

export interface CreateUpdateJobTitleDto {
  nameAr?: string;
  nameEn?: string;
  description?: string;
  departmentId?: string;
}
