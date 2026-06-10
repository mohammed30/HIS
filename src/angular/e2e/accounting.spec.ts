import { test, expect } from '@playwright/test';

test.describe('Accounting Module UI', () => {

    test('should load chart of accounts', async ({ page }) => {
        await page.goto('/accounting/chart-of-accounts');
        await expect(page.locator('h1, h2, h3, h4, h5, .card-title').filter({ hasText: /Chart of Accounts|شجرة الحسابات|الدليل المحاسبي/i }).first()).toBeVisible({ timeout: 15000 });
        await expect(page.locator('table, ngx-datatable, .tree-container, .table').first()).toBeVisible({ timeout: 15000 });
    });

    test('should load journal entries', async ({ page }) => {
        await page.goto('/accounting/journal-entries');
        await expect(page.locator('h1, h2, h3, h4, h5, .card-title').filter({ hasText: /Journal Entries|قيود اليومية/i }).first()).toBeVisible({ timeout: 15000 });
        await expect(page.locator('table, ngx-datatable, .table').first()).toBeVisible({ timeout: 15000 });
    });

    test('should load payment vouchers', async ({ page }) => {
        await page.goto('/accounting/payment-vouchers');
        await expect(page.locator('h1, h2, h3, h4, h5, .card-title').filter({ hasText: /Payment Vouchers|سندات الصرف/i }).first()).toBeVisible({ timeout: 15000 });
    });

    test('should load receipt vouchers', async ({ page }) => {
        await page.goto('/accounting/receipt-vouchers');
        await expect(page.locator('h1, h2, h3, h4, h5, .card-title').filter({ hasText: /Receipt Vouchers|سندات القبض/i }).first()).toBeVisible({ timeout: 15000 });
    });

});
