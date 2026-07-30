
export interface CreateLabReceptionOrderDto {
  patientId?: string;
  doctorId?: string | null;
  totalAmount?: number;
  paidAmount?: number;
  testIds?: string[];
}
