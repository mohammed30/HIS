import { RestService } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PosProductDto, PosSaleDto } from './models';

@Injectable({
    providedIn: 'root',
})
export class PosService {
    apiName = 'Default';

    constructor(private restService: RestService) { }

    getProductByBarcode(barcode: string): Observable<PosProductDto> {
        return this.restService.request({
            url: `/api/app/pos/product-by-barcode`,
            method: 'GET',
            params: { barcode },
        }, { apiName: this.apiName });
    }

    getProductById(id: string): Observable<PosProductDto> {
        return this.restService.request({
            url: `/api/app/pos/product/${id}`,
            method: 'GET',
        }, { apiName: this.apiName });
    }

    processSale(input: PosSaleDto): Observable<void> {
        return this.restService.request({
            url: `/api/app/pos/process-sale`,
            method: 'POST',
            body: input,
        }, { apiName: this.apiName });
    }
}
