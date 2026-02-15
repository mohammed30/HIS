import { mapEnumToOptions } from '@abp/ng.core';

export enum BankTransactionType {
    Deposit = 0,
    Withdrawal = 1,
    Transfer = 2,
    BankFee = 3,
    Interest = 4,
}

export const bankTransactionTypeOptions = mapEnumToOptions(BankTransactionType);
