import { RestService } from '@abp/ng.core';
import { PagedResultDto, PagedAndSortedResultRequestDto } from '@abp/ng.core';
import { Injectable } from '@angular/core';
import { JobTitleDto, CreateUpdateJobTitleDto } from './dtos/job-title-dto';

@Injectable({
    providedIn: 'root',
})
export class JobTitleService {
    apiName = 'Default';

    create = (input: CreateUpdateJobTitleDto) =>
        this.restService.request<any, JobTitleDto>({
            method: 'POST',
            url: '/api/app/job-title',
            body: input,
        },
            { apiName: this.apiName });

    delete = (id: string) =>
        this.restService.request<any, void>({
            method: 'DELETE',
            url: `/api/app/job-title/${id}`,
        },
            { apiName: this.apiName });

    get = (id: string) =>
        this.restService.request<any, JobTitleDto>({
            method: 'GET',
            url: `/api/app/job-title/${id}`,
        },
            { apiName: this.apiName });

    getList = (input: PagedAndSortedResultRequestDto) =>
        this.restService.request<any, PagedResultDto<JobTitleDto>>({
            method: 'GET',
            url: '/api/app/job-title',
            params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
        },
            { apiName: this.apiName });

    update = (id: string, input: CreateUpdateJobTitleDto) =>
        this.restService.request<any, JobTitleDto>({
            method: 'PUT',
            url: `/api/app/job-title/${id}`,
            body: input,
        },
            { apiName: this.apiName });

    constructor(private restService: RestService) { }
}
