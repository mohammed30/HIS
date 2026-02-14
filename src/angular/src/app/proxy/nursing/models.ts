import { AuditedEntityDto, EntityDto, FullAuditedEntityDto } from '@abp/ng.core';

export interface MedicationAdministrationDto extends AuditedEntityDto<string> {
    patientId: string;
    patientName?: string;
    medicalOrderId: string;
    drugName?: string;
    administrationTime: string;
    status: AdministrationStatus;
    dosage?: string;
    notes?: string;
}

export interface CreateMedicationAdministrationDto {
    patientId: string;
    medicalOrderId: string;
    administrationTime: string;
    status: AdministrationStatus;
    dosage?: string;
    notes?: string;
}

export interface CarePlanDto extends AuditedEntityDto<string> {
    patientId: string;
    patientName?: string;
    diagnosis: string;
    goal: string;
    interventions?: string;
    evaluation?: string;
    status: CarePlanStatus;
    dateCreate: string;
}

export interface CreateCarePlanDto {
    patientId: string;
    diagnosis: string;
    goal: string;
    interventions?: string;
    status: CarePlanStatus;
}

export interface DueMedicationDto {
    medicationOrderId: string;
    patientId: string;
    patientName?: string;
    drugName: string;
    dose: string;
    route: string;
    frequency: string;
    scheduledTime: string;
    status: string; // Pending, Given, Missed
}

// --- Phase 2 Enums ---

export enum PainLocation {
    Head = 0,
    Neck = 1,
    Chest = 2,
    Abdomen = 3,
    Back = 4,
    Arms = 5,
    Legs = 6,
    General = 7
}

export enum RiskLevel {
    Low = 0,
    Medium = 1,
    High = 2
}

export enum WoundStage {
    Stage1 = 1,
    Stage2 = 2,
    Stage3 = 3,
    Stage4 = 4,
    Unstageable = 5
}

export enum FluidType {
    Input = 0,
    Output = 1
}

export enum FluidMetric {
    // Input
    Oral = 0,
    IV = 1,
    TubeFeeding = 2,

    // Output
    Urine = 10,
    Stool = 11,
    Vomit = 12,
    Drain = 13,
    Sweat = 14
}

export enum ShiftType {
    Morning = 0,
    Evening = 1,
    Night = 2
}

// --- Phase 2 DTOs ---

export interface PatientRoundDto extends FullAuditedEntityDto<string> {
    patientId: string;
    note: string;
    nurseId?: string;
}

export interface CreatePatientRoundDto {
    patientId: string;
    note: string;
}

export interface PainAssessmentDto extends FullAuditedEntityDto<string> {
    patientId: string;
    painScore: number;
    location: PainLocation;
    characteristics?: string;
    intervention?: string;
    assessmentTime: string;
}

export interface CreatePainAssessmentDto {
    patientId: string;
    painScore: number;
    location: PainLocation;
    characteristics?: string;
    intervention?: string;
    assessmentTime: string;
}

export interface FallRiskAssessmentDto extends FullAuditedEntityDto<string> {
    patientId: string;
    totalScore: number;
    riskLevel: RiskLevel;
    historyOfFalls: boolean;
    secondaryDiagnosis: boolean;
    ambulatoryAid: boolean;
    iVTherapy: boolean;
    gaitProblem: boolean;
    mentalStatusIssue: boolean;
    assessmentTime: string;
}

export interface CreateFallRiskAssessmentDto {
    patientId: string;
    historyOfFalls: boolean;
    secondaryDiagnosis: boolean;
    ambulatoryAid: boolean;
    iVTherapy: boolean;
    gaitProblem: boolean;
    mentalStatusIssue: boolean;
    assessmentTime: string;
}

export interface WoundCareDto extends FullAuditedEntityDto<string> {
    patientId: string;
    location: string;
    stage: WoundStage;
    length: number;
    width: number;
    depth: number;
    exudate?: string;
    treatment?: string;
    notes?: string;
    assessmentTime: string;
}

export interface CreateWoundCareDto {
    patientId: string;
    location: string;
    stage: WoundStage;
    length: number;
    width: number;
    depth: number;
    exudate?: string;
    treatment?: string;
    notes?: string;
    assessmentTime: string;
}

export interface FluidBalanceDto extends FullAuditedEntityDto<string> {
    patientId: string;
    type: FluidType;
    metric: FluidMetric;
    amount: number;
    entryTime: string;
    notes?: string;
}

export interface CreateFluidBalanceDto {
    patientId: string;
    type: FluidType;
    metric: FluidMetric;
    amount: number;
    entryTime: string;
    notes?: string;
}

export interface FluidBalanceSummaryDto {
    totalInput: number;
    totalOutput: number;
    balance: number;
}

export interface ShiftHandoverDto extends FullAuditedEntityDto<string> {
    shift: ShiftType;
    handoverTime: string;
    notes: string;
    outgoingNurseId: string;
    incomingNurseId: string;
}

export interface CreateShiftHandoverDto {
    shift: ShiftType;
    notes: string;
    incomingNurseId: string;
}

export enum AdministrationStatus {

    Given = 0,
    Refused = 1,
    Skipped = 2,
    Late = 3
}

export enum CarePlanStatus {
    Active = 0,
    Resolved = 1,
    Discontinued = 2
}
