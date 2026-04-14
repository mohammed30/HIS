import type { EntityDto } from '@abp/ng.core';
import type { AllergenType } from './allergen-type.enum';
import type { AllergySeverity } from './allergy-severity.enum';
import type { AllergyStatus } from './allergy-status.enum';
import type { DiagnosisType } from './diagnosis-type.enum';
import type { DiagnosisStatus } from './diagnosis-status.enum';
import type { NoteType } from './note-type.enum';
import type { Gender } from '../patients/gender.enum';

export interface AllergyDto extends EntityDto<string> {
  patientId?: string;
  allergenType?: AllergenType;
  allergenNameAr?: string;
  allergenNameEn?: string | null;
  reaction?: string | null;
  severity?: AllergySeverity;
  onsetDate?: string | null;
  status?: AllergyStatus;
  notes?: string | null;
}

export interface CreateUpdateAllergyDto {
  patientId?: string;
  allergenType?: AllergenType;
  allergenNameAr?: string;
  allergenNameEn?: string | null;
  reaction?: string | null;
  severity?: AllergySeverity;
  onsetDate?: string | null;
  status?: AllergyStatus;
  notes?: string | null;
}

export interface CreateUpdateDiagnosisDto {
  patientId?: string;
  visitId?: string | null;
  icD10Code?: string | null;
  diagnosisNameAr?: string;
  diagnosisNameEn?: string | null;
  diagnosisDate?: string;
  type?: DiagnosisType;
  status?: DiagnosisStatus;
  notes?: string | null;
}

export interface CreateUpdateMedicalHistoryDto {
  patientId?: string;
  conditionAr?: string;
  conditionEn?: string | null;
  icD10Code?: string | null;
  diagnosedDate?: string | null;
  resolvedDate?: string | null;
  isChronic?: boolean;
  notes?: string | null;
}

export interface CreateUpdatePatientNoteDto {
  patientId?: string;
  visitId?: string | null;
  noteType?: NoteType;
  title?: string;
  content?: string;
  isPrivate?: boolean;
}

export interface CreateUpdateVitalSignDto {
  patientId?: string;
  visitId?: string | null;
  recordedAt?: string;
  temperature?: number | null;
  bloodPressureSystolic?: number | null;
  bloodPressureDiastolic?: number | null;
  heartRate?: number | null;
  respiratoryRate?: number | null;
  oxygenSaturation?: number | null;
  weight?: number | null;
  height?: number | null;
  notes?: string | null;
}

export interface DiagnosisDto extends EntityDto<string> {
  patientId?: string;
  visitId?: string | null;
  icD10Code?: string | null;
  diagnosisNameAr?: string;
  diagnosisNameEn?: string | null;
  diagnosisDate?: string;
  type?: DiagnosisType;
  status?: DiagnosisStatus;
  diagnosedById?: string | null;
  diagnosedByName?: string | null;
  notes?: string | null;
}

export interface MedicalHistoryDto extends EntityDto<string> {
  patientId?: string;
  conditionAr?: string;
  conditionEn?: string | null;
  icD10Code?: string | null;
  diagnosedDate?: string | null;
  resolvedDate?: string | null;
  isChronic?: boolean;
  notes?: string | null;
}

export interface PatientMedicalSummaryDto {
  patientId?: string;
  patientName?: string;
  bloodType?: string | null;
  age?: number | null;
  gender?: Gender;
  activeAllergiesCount?: number;
  chronicConditionsCount?: number;
  activeDiagnosesCount?: number;
  latestVitals?: VitalSignDto | null;
  activeAllergies?: AllergyDto[];
  chronicConditions?: MedicalHistoryDto[];
}

export interface PatientNoteDto extends EntityDto<string> {
  patientId?: string;
  visitId?: string | null;
  noteType?: NoteType;
  title?: string;
  content?: string;
  createdByName?: string | null;
  isPrivate?: boolean;
  creationTime?: string;
}

export interface VitalSignDto extends EntityDto<string> {
  patientId?: string;
  visitId?: string | null;
  recordedAt?: string;
  temperature?: number | null;
  bloodPressureSystolic?: number | null;
  bloodPressureDiastolic?: number | null;
  heartRate?: number | null;
  respiratoryRate?: number | null;
  oxygenSaturation?: number | null;
  weight?: number | null;
  height?: number | null;
  bmi?: number | null;
  recordedByName?: string | null;
  notes?: string | null;
}
