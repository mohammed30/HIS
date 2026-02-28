import type { AuditedEntityDto, EntityDto, FullAuditedEntityDto } from '@abp/ng.core';
import type { CarePlanStatus } from './care-plan-status.enum';
import type { FluidType } from './fluid-type.enum';
import type { FluidMetric } from './fluid-metric.enum';
import type { AdministrationStatus } from './administration-status.enum';
import type { PainLocation } from './pain-location.enum';
import type { ShiftType } from './shift-type.enum';
import type { WoundStage } from './wound-stage.enum';
import type { RiskLevel } from './risk-level.enum';

export interface CarePlanDto extends AuditedEntityDto<string> {
  patientId?: string;
  patientName?: string;
  diagnosis?: string;
  goal?: string;
  interventions?: string;
  evaluation?: string;
  status?: CarePlanStatus;
  dateCreate?: string;
}

export interface CreateCarePlanDto {
  patientId?: string;
  diagnosis?: string;
  goal?: string;
  interventions?: string;
  status?: CarePlanStatus;
}

export interface CreateFallRiskAssessmentDto {
  patientId?: string;
  historyOfFalls?: boolean;
  secondaryDiagnosis?: boolean;
  ambulatoryAid?: boolean;
  ivTherapy?: boolean;
  gaitProblem?: boolean;
  mentalStatusIssue?: boolean;
  assessmentTime?: string;
}

export interface CreateFluidBalanceDto {
  patientId?: string;
  type?: FluidType;
  metric?: FluidMetric;
  amount?: number;
  entryTime?: string;
  notes?: string;
}

export interface CreateMedicationAdministrationDto {
  patientId?: string;
  medicalOrderId?: string;
  administrationTime?: string;
  status?: AdministrationStatus;
  dosage?: string;
  notes?: string;
}

export interface CreatePainAssessmentDto {
  patientId?: string;
  painScore?: number;
  location?: PainLocation;
  characteristics?: string;
  intervention?: string;
  assessmentTime?: string;
}

export interface CreatePatientRoundDto {
  patientId?: string;
  note?: string;
}

export interface CreateShiftHandoverDto {
  shift?: ShiftType;
  notes?: string;
  incomingNurseId?: string;
}

export interface CreateWoundCareDto {
  patientId?: string;
  location?: string;
  stage?: WoundStage;
  length?: number;
  width?: number;
  depth?: number;
  exudate?: string;
  treatment?: string;
  notes?: string;
  assessmentTime?: string;
}

export interface DueMedicationDto extends EntityDto<string> {
  drugName?: string;
  dosage?: string;
  route?: string;
  frequency?: string;
  instructions?: string;
  orderDate?: string;
}

export interface FallRiskAssessmentDto extends FullAuditedEntityDto<string> {
  patientId?: string;
  totalScore?: number;
  riskLevel?: RiskLevel;
  historyOfFalls?: boolean;
  secondaryDiagnosis?: boolean;
  ambulatoryAid?: boolean;
  ivTherapy?: boolean;
  gaitProblem?: boolean;
  mentalStatusIssue?: boolean;
  assessmentTime?: string;
}

export interface FluidBalanceDto extends FullAuditedEntityDto<string> {
  patientId?: string;
  type?: FluidType;
  metric?: FluidMetric;
  amount?: number;
  entryTime?: string;
  notes?: string;
}

export interface FluidBalanceSummaryDto {
  totalInput?: number;
  totalOutput?: number;
  balance?: number;
}

export interface MedicationAdministrationDto extends AuditedEntityDto<string> {
  patientId?: string;
  patientName?: string;
  medicalOrderId?: string;
  drugName?: string;
  administrationTime?: string;
  status?: AdministrationStatus;
  dosage?: string;
  notes?: string;
}

export interface PainAssessmentDto extends FullAuditedEntityDto<string> {
  patientId?: string;
  painScore?: number;
  location?: PainLocation;
  characteristics?: string;
  intervention?: string;
  assessmentTime?: string;
}

export interface PatientRoundDto extends FullAuditedEntityDto<string> {
  patientId?: string;
  note?: string;
  nurseId?: string;
}

export interface ShiftHandoverDto extends FullAuditedEntityDto<string> {
  shift?: ShiftType;
  handoverTime?: string;
  notes?: string;
  outgoingNurseId?: string;
  incomingNurseId?: string;
}

export interface WoundCareDto extends FullAuditedEntityDto<string> {
  patientId?: string;
  location?: string;
  stage?: WoundStage;
  length?: number;
  width?: number;
  depth?: number;
  exudate?: string;
  treatment?: string;
  notes?: string;
  assessmentTime?: string;
}
