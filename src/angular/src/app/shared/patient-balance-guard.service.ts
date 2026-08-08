import { Injectable, inject } from '@angular/core';
import { Subject } from 'rxjs';
import { AdmissionService } from '../proxy/inpatient/admission.service';
import { PatientAdmissionStatusDto } from '../proxy/inpatient/models';
import { ToasterService } from '@abp/ng.theme.shared';

@Injectable({
  providedIn: 'root'
})
export class PatientBalanceGuardService {
  private admissionService = inject(AdmissionService);
  private toaster = inject(ToasterService);

  public admissionStatus$ = new Subject<PatientAdmissionStatusDto>();
  private currentStatus: PatientAdmissionStatusDto | null = null;

  checkPatient(patientId: string) {
    if (!patientId) {
      this.currentStatus = null;
      this.admissionStatus$.next(null);
      return;
    }

    this.admissionService.getPatientAdmissionStatus(patientId).subscribe(status => {
      this.currentStatus = status;
      this.admissionStatus$.next(status);

      if (status.isAdmitted) {
        if (status.isServicesStopped) {
          this.toaster.error('تم إيقاف الخدمات لهذا المريض المنوم. يرجى مراجعة الحسابات.', 'إيقاف الخدمات');
        } else if (status.availableBalance !== undefined && status.availableBalance < 500) {
           this.toaster.warn(`المبلغ المدفوع المتبقي للمريض قارب على الانتهاء. المتبقي: ${status.availableBalance} ريال.`, 'تنبيه الرصيد');
        }
      }
    });
  }

  getCurrentStatus(): PatientAdmissionStatusDto | null {
    return this.currentStatus;
  }

  canProceedWithService(cost: number = 0): boolean {
    if (!this.currentStatus || !this.currentStatus.isAdmitted) {
      return true; // Not an inpatient, no restrictions from this guard
    }

    if (this.currentStatus.isServicesStopped) {
      this.toaster.error('لا يمكن تقديم خدمة لهذا المريض. تم إيقاف الخدمات.', 'مرفوض');
      return false;
    }

    if (this.currentStatus.availableBalance !== undefined && cost > this.currentStatus.availableBalance) {
      this.toaster.error('عذراً، الرصيد المتبقي للمريض لا يغطي تكلفة هذه الخدمة. يرجى مراجعة قسم الحسابات.', 'رصيد غير كافٍ');
      return false;
    }

    return true;
  }
}
