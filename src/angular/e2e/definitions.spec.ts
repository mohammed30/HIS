import { test, expect } from '@playwright/test';

test.describe('Definitions Module UI', () => {

    test('should load nationalities', async ({ page }) => {
        await page.goto('/definitions/nationalities');
        await expect(page.locator('h1, h2, h3, h4, h5, .card-title').filter({ hasText: /Nationalities|الجنسيات/i }).first()).toBeVisible({ timeout: 15000 });
        await expect(page.locator('table, ngx-datatable, .table').first()).toBeVisible({ timeout: 15000 });
    });

    test('should load payment methods', async ({ page }) => {
        await page.goto('/definitions/payment-methods');
        await expect(page.locator('h1, h2, h3, h4, h5, .card-title').filter({ hasText: /Payment Methods|طرق الدفع/i }).first()).toBeVisible({ timeout: 15000 });
    });

    test('should load professions', async ({ page }) => {
        await page.goto('/definitions/professions');
        await expect(page.locator('h1, h2, h3, h4, h5, .card-title').filter({ hasText: /Professions|المهن/i }).first()).toBeVisible({ timeout: 15000 });
    });

    test('should load contracts', async ({ page }) => {
        await page.goto('/definitions/contracts');
        await expect(page.locator('h1, h2, h3, h4, h5, .card-title').filter({ hasText: /Contracts|العقود/i }).first()).toBeVisible({ timeout: 15000 });
    });

    test('should load patient categories', async ({ page }) => {
        await page.goto('/definitions/patient-categories');
        await expect(page.locator('h1, h2, h3, h4, h5, .card-title').filter({ hasText: /Categories|الفئات|فئات المرضى/i }).first()).toBeVisible({ timeout: 15000 });
    });

    test('should load referral sources', async ({ page }) => {
        await page.goto('/definitions/referral-sources');
        await expect(page.locator('h1, h2, h3, h4, h5, .card-title').filter({ hasText: /Referral|التحويل|مصادر التحويل/i }).first()).toBeVisible({ timeout: 15000 });
    });

});
