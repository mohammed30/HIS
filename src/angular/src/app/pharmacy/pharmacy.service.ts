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

    checkInteractions(patientId: string, newDrugName: string): Observable<string[]> {
        return this.restService.request({
            method: 'POST',
            url: `/api/app/pharmacy/check-interactions?patientId=${patientId}&newDrugName=${newDrugName}`
        });
    }

    // Drug Master Data
    getDrugs(params: any): Observable<any> {
        return this.restService.request({
            method: 'GET',
            url: '/api/app/drug',
            params: params
        });
    }

    getDrug(id: string): Observable<any> {
        return this.restService.request({
            method: 'GET',
            url: `/api/app/drug/${id}`
        });
    }

    createDrug(input: any): Observable<any> {
        return this.restService.request({
            method: 'POST',
            url: '/api/app/drug',
            body: input
        });
    }

    updateDrug(id: string, input: any): Observable<any> {
        return this.restService.request({
            method: 'PUT',
            url: `/api/app/drug/${id}`,
            body: input
        });
    }

    deleteDrug(id: string): Observable<void> {
        return this.restService.request({
            method: 'DELETE',
            url: `/api/app/drug/${id}`
        });
    }
}
