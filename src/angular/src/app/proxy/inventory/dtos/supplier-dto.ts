import { AuditedEntityDto } from '@abp/ng.core';

export interface SupplierDto extends AuditedEntityDto<string> {
    name: string;
    contactPerson: string;
    phone: string;
    email: string;
    address: string;
    taxId: string;
}

export interface CreateUpdateSupplierDto {
    name: string;
    contactPerson?: string;
    phone?: string;
    email?: string;
    address?: string;
    taxId?: string;
}
