import { test, expect } from '@playwright/test';

test.describe('Reception Module UI', () => {

    test('should load insurance companies', async ({ page }) => {
        await page.goto('/reception/insurance-companies');
        await expect(page.locator('h1, h2, h3, h4, h5, .card-title').filter({ hasText: /Insurance Companies|شركات التأمين/i }).first()).toBeVisible({ timeout: 15000 });
        await expect(page.locator('table, ngx-datatable, .table').first()).toBeVisible({ timeout: 15000 });
    });

    test('should load insurance plans', async ({ page }) => {
        await page.goto('/reception/insurance-plans');
        await expect(page.locator('h1, h2, h3, h4, h5, .card-title').filter({ hasText: /Plans|الخطط|خطط التأمين/i }).first()).toBeVisible({ timeout: 15000 });
    });

    test('should load invoices', async ({ page }) => {
        await page.goto('/reception/invoices');
        await expect(page.locator('h1, h2, h3, h4, h5, .card-title').filter({ hasText: /Invoices|الفواتير/i }).first()).toBeVisible({ timeout: 15000 });
    });

    test('should load payments', async ({ page }) => {
        await page.goto('/reception/payments');
        await expect(page.locator('h1, h2, h3, h4, h5, .card-title').filter({ hasText: /Payments|المدفوعات/i }).first()).toBeVisible({ timeout: 15000 });
    });

    test('should load laboratory reception', async ({ page }) => {
        await page.goto('/reception/laboratory-reception');
        await expect(page.locator('h1, h2, h3, h4, h5, .card-title').filter({ hasText: /Laboratory|المختبر|استقبال المختبر/i }).first()).toBeVisible({ timeout: 15000 });
    });

});
