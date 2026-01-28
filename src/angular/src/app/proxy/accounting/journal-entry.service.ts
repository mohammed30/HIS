import { Injectable } from '@angular/core';
import { RestService, PagedResultDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import { JournalEntryDto } from './models';

@Injectable({
    providedIn: 'root',
})
export class JournalEntryService {
    apiName = 'Default';

    constructor(private restService: RestService) { }

    getList(input: PagedAndSortedResultRequestDto) {
        return this.restService.request<any, PagedResultDto<JournalEntryDto>>({
            method: 'GET',
            url: '/api/app/journal-entry',
            params: input,
        },
            { apiName: this.apiName });
    }
}
