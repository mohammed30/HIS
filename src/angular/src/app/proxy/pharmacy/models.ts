import type { MedicalOrderDto } from '../clinical/models';

export interface DispenseDto {
  medicalOrderId: string;
  counselingNotes?: string | null;
}

export interface PendingPrescriptionDto extends MedicalOrderDto {
  patientName?: string;
  patientMRN?: string;
  dosage?: string;
  frequency?: string;
  route?: string;
  duration?: string;
  instructions?: string;
}
