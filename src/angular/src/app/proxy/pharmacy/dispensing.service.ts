import { RestService } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { VerifyPrescriptionDto, DispensingVerificationDto } from './models';

@Injectable({
    providedIn: 'root',
})
export class DispensingService {
    apiName = 'Default';

    constructor(private restService: RestService) { }

    verifyPrescription(input: VerifyPrescriptionDto): Observable<void> {
        return this.restService.request({
            url: `/api/app/dispensing/verify-prescription`,
            method: 'POST',
            body: input,
        }, { apiName: this.apiName });
    }

    getVerification(medicalOrderId: string): Observable<DispensingVerificationDto> {
        return this.restService.request({
            url: `/api/app/dispensing/verification/${medicalOrderId}`,
            method: 'GET',
        }, { apiName: this.apiName });
    }
}
