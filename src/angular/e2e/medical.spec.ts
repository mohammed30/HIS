import { test, expect } from '@playwright/test';

test.describe('Medical Modules UI', () => {

    test('should load inpatient dashboard', async ({ page }) => {
        await page.goto('/inpatient/dashboard');
        await expect(page.locator('h1, h2, h3, h4, h5, .card-title').filter({ hasText: /Dashboard|لوحة القيادة|تنويم/i }).first()).toBeVisible({ timeout: 15000 });
    });

    test('should load inpatient admissions', async ({ page }) => {
        await page.goto('/inpatient/admissions');
        await expect(page.locator('h1, h2, h3, h4, h5, .card-title').filter({ hasText: /Admissions|الدخول/i }).first()).toBeVisible({ timeout: 15000 });
        await expect(page.locator('table, ngx-datatable, .table').first()).toBeVisible({ timeout: 15000 });
    });

    test('should load radiology requests', async ({ page }) => {
        await page.goto('/radiology/requests');
        await expect(page.locator('h1, h2, h3, h4, h5, .card-title').filter({ hasText: /Requests|الطلبات|طلبات الأشعة/i }).first()).toBeVisible({ timeout: 15000 });
    });

    test('should load radiology results', async ({ page }) => {
        await page.goto('/radiology/results');
        await expect(page.locator('h1, h2, h3, h4, h5, .card-title').filter({ hasText: /Results|النتائج|نتائج الأشعة/i }).first()).toBeVisible({ timeout: 15000 });
    });

    // Lab, Pharmacy, Emergency modules use loadChildren. Their inner structure might vary.
    // If they have default routes, they will load.
    // Let's add basic tests to just visit the route and ensure no generic error.
    
    test('should navigate to laboratory', async ({ page }) => {
        const response = await page.goto('/laboratory');
        expect(response?.status()).not.toBe(404);
        expect(response?.status()).not.toBe(500);
    });

    test('should navigate to emergency', async ({ page }) => {
        const response = await page.goto('/emergency');
        expect(response?.status()).not.toBe(404);
        expect(response?.status()).not.toBe(500);
    });

    test('should navigate to pharmacy', async ({ page }) => {
        const response = await page.goto('/pharmacy');
        expect(response?.status()).not.toBe(404);
        expect(response?.status()).not.toBe(500);
    });

});
