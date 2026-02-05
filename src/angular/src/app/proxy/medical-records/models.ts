import type { EntityDto } from '@abp/ng.core';
import type { AllergenType } from './allergen-type.enum';
import type { AllergySeverity } from './allergy-severity.enum';
import type { AllergyStatus } from './allergy-status.enum';
import type { DiagnosisType } from './diagnosis-type.enum';
import type { DiagnosisStatus } from './diagnosis-status.enum';
import type { NoteType } from './note-type.enum';

export interface AllergyDto extends EntityDto<string> {
  patientId?: string;
  allergenType?: AllergenType;
  allergenNameAr?: string;
  allergenNameEn?: string;
  reaction?: string;
  severity?: AllergySeverity;
  onsetDate?: string;
  status?: AllergyStatus;
  notes?: string;
}

export interface CreateUpdateAllergyDto {
  patientId?: string;
  allergenType?: AllergenType;
  allergenNameAr?: string;
  allergenNameEn?: string;
  reaction?: string;
  severity?: AllergySeverity;
  onsetDate?: string;
  status?: AllergyStatus;
  notes?: string;
}

export interface CreateUpdateDiagnosisDto {
  patientId?: string;
  visitId?: string;
  icD10Code?: string;
  diagnosisNameAr?: string;
  diagnosisNameEn?: string;
  diagnosisDate?: string;
  type?: DiagnosisType;
  status?: DiagnosisStatus;
  notes?: string;
}

export interface CreateUpdateMedicalHistoryDto {
  patientId?: string;
  conditionAr?: string;
  conditionEn?: string;
  icD10Code?: string;
  diagnosedDate?: string;
  resolvedDate?: string;
  isChronic?: boolean;
  notes?: string;
}

export interface CreateUpdatePatientNoteDto {
  patientId?: string;
  visitId?: string;
  noteType?: NoteType;
  title?: string;
  content?: string;
  isPrivate?: boolean;
}

export interface CreateUpdateVitalSignDto {
  patientId?: string;
  visitId?: string;
  recordedAt?: string;
  temperature?: number;
  bloodPressureSystolic?: number;
  bloodPressureDiastolic?: number;
  heartRate?: number;
  respiratoryRate?: number;
  oxygenSaturation?: number;
  weight?: number;
  height?: number;
  notes?: string;
}

export interface DiagnosisDto extends EntityDto<string> {
  patientId?: string;
  visitId?: string;
  icD10Code?: string;
  diagnosisNameAr?: string;
  diagnosisNameEn?: string;
  diagnosisDate?: string;
  type?: DiagnosisType;
  status?: DiagnosisStatus;
  diagnosedById?: string;
  diagnosedByName?: string;
  notes?: string;
}

export interface MedicalHistoryDto extends EntityDto<string> {
  patientId?: string;
  conditionAr?: string;
  conditionEn?: string;
  icD10Code?: string;
  diagnosedDate?: string;
  resolvedDate?: string;
  isChronic?: boolean;
  notes?: string;
}

export interface PatientMedicalSummaryDto {
  patientId?: string;
  patientName?: string;
  bloodType?: string;
  age?: number;
  activeAllergiesCount?: number;
  chronicConditionsCount?: number;
  activeDiagnosesCount?: number;
  latestVitals?: VitalSignDto;
  activeAllergies?: AllergyDto[];
  chronicConditions?: MedicalHistoryDto[];
}

export interface PatientNoteDto extends EntityDto<string> {
  patientId?: string;
  visitId?: string;
  noteType?: NoteType;
  title?: string;
  content?: string;
  createdByName?: string;
  isPrivate?: boolean;
  creationTime?: string;
}

export interface VitalSignDto extends EntityDto<string> {
  patientId?: string;
  visitId?: string;
  recordedAt?: string;
  temperature?: number;
  bloodPressureSystolic?: number;
  bloodPressureDiastolic?: number;
  heartRate?: number;
  respiratoryRate?: number;
  oxygenSaturation?: number;
  weight?: number;
  height?: number;
  bmi?: number;
  recordedByName?: string;
  notes?: string;
}
