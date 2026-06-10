import { test, expect } from '@playwright/test';

test.describe('Inventory Module UI', () => {

    test('should load inventory dashboard', async ({ page }) => {
        await page.goto('/inventory/dashboard');
        await expect(page.locator('h1, h2, h3, h4, h5, .card-title').filter({ hasText: /Dashboard|لوحة القيادة|لوحة المؤشرات/i }).first()).toBeVisible({ timeout: 15000 });
    });

    test('should load warehouse management', async ({ page }) => {
        await page.goto('/inventory/warehouse-management');
        await expect(page.locator('h1, h2, h3, h4, h5, .card-title').filter({ hasText: /Warehouse|المستودعات/i }).first()).toBeVisible({ timeout: 15000 });
        await expect(page.locator('table, ngx-datatable, .table').first()).toBeVisible({ timeout: 15000 });
    });

    test('should load receive stock', async ({ page }) => {
        await page.goto('/inventory/receive-stock');
        await expect(page.locator('h1, h2, h3, h4, h5, .card-title').filter({ hasText: /Receive Stock|استلام|توريد/i }).first()).toBeVisible({ timeout: 15000 });
    });

    test('should load purchase invoices', async ({ page }) => {
        await page.goto('/inventory/purchase-invoices');
        await expect(page.locator('h1, h2, h3, h4, h5, .card-title').filter({ hasText: /Purchase Invoices|فواتير المشتريات/i }).first()).toBeVisible({ timeout: 15000 });
    });

});
