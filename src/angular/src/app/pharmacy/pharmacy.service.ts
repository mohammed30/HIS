import { Injectable } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class PharmacyService {
    apiName = 'Default';

    constructor(private restService: RestService) { }

    getPendingPrescriptions(): Observable<any[]> {
        return this.restService.request({
            method: 'GET',
            url: '/api/app/pharmacy/pending-prescriptions'
        });
    }

    getPrescription(id: string): Observable<any> {
        return this.restService.request({
            method: 'GET',
            url: `/api/app/pharmacy/prescription/${id}`
        });
    }

    getPharmacyStock(): Observable<any[]> {
        return this.restService.request({
            method: 'GET',
            url: '/api/app/pharmacy/pharmacy-stock'
        });
    }

    dispenseMedication(input: any): Observable<void> {
        return this.restService.request({
            method: 'POST',
            url: `/api/app/pharmacy/dispense-medication`,
            body: input
        });
    }
}
