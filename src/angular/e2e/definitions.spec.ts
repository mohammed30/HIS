import { test, expect } from '@playwright/test';

test.describe('Definitions Module E2E Tests', () => {

    test.beforeEach(async ({ page }) => {
        // Robust Login logic
        await page.goto('/', { timeout: 60000 });
        if (page.url().toLowerCase().includes('/account/login')) {
            await page.waitForSelector('input[name*="UserNameOrEmailAddress"]', { timeout: 30000 });
            await page.fill('input[name*="UserNameOrEmailAddress"]', 'admin');
            await page.fill('input[name*="Password"]', '1q2w3E*');
            await page.click('button[type="submit"]');
            // Wait for sidebar or some global element to ensure dashboard loaded
            await page.waitForURL('**/', { timeout: 60000 });
            await page.waitForSelector('.fas.fa-home', { timeout: 30000 });
        }
    });

    test('Nationalities: should create and delete a nationality', async ({ page }) => {
        await page.goto('/definitions/nationalities', { timeout: 60000 });
        await page.waitForSelector('.card-header', { timeout: 30000 });

        const uniqueName = `TestNat-${Math.floor(Math.random() * 1000)}`;

        // Add
        await page.click('button:has-text("Add"), button:has-text("إضافة")');
        await page.waitForSelector('.modal.show');
        await page.fill('.modal input[type="text"]', uniqueName);
        await page.click('.modal button:has-text("Save"), .modal button:has-text("حفظ")');

        // Search and Verify
        await page.fill('input[placeholder*="Search"], input[placeholder*="بحث"]', uniqueName);
        await page.waitForTimeout(1000); // Wait for debounce/search
        const row = page.locator('table tbody tr').first();
        await expect(row).toContainText(uniqueName);

        // Delete
        await row.locator('button.btn-outline-danger').click();
        // ABP Confirmation
        await page.click('.modal-footer button:has-text("Yes"), .modal-footer button:has-text("نعم")');
        
        // Final verify
        await expect(page.locator('table tbody')).not.toContainText(uniqueName);
    });

    test('Professions: should create and delete a profession', async ({ page }) => {
        await page.goto('/definitions/professions', { timeout: 60000 });
        await page.waitForSelector('.card-header', { timeout: 30000 });

        const uniqueName = `TestProf-${Math.floor(Math.random() * 1000)}`;

        // Add
        await page.click('button:has-text("Add"), button:has-text("إضافة")');
        await page.waitForSelector('.modal.show');
        await page.locator('.modal input[type="text"]').first().fill(uniqueName);
        await page.click('.modal button:has-text("Save"), .modal button:has-text("حفظ")');

        // Search and Verify
        await page.fill('input[placeholder*="Search"], input[placeholder*="بحث"]', uniqueName);
        await page.waitForTimeout(1000);
        const row = page.locator('table tbody tr').first();
        await expect(row).toContainText(uniqueName);

        // Delete
        await row.locator('button.btn-outline-danger').click();
        await page.click('.modal-footer button:has-text("Yes"), .modal-footer button:has-text("نعم")');
        await expect(page.locator('table tbody')).not.toContainText(uniqueName);
    });

    test('Departments: should create and delete a department', async ({ page }) => {
        await page.goto('/settings/departments', { timeout: 60000 });
        await page.waitForSelector('.card-header', { timeout: 30000 });

        const uniqueName = `TestDept-${Math.floor(Math.random() * 1000)}`;

        // Add
        await page.click('button:has-text("Add"), button:has-text("إضافة")');
        await page.waitForSelector('.modal.show');
        
        // Department fields are nameAr, nameEn, description, location
        await page.locator('.modal input[type="text"]').nth(0).fill(uniqueName); // Name (Ar)
        await page.locator('.modal input[type="text"]').nth(1).fill(`${uniqueName}-En`); // Name (En)
        
        await page.click('.modal button:has-text("Save"), .modal button:has-text("حفظ")');

        // Search and Verify
        await page.fill('input[placeholder*="Search"], input[placeholder*="بحث"]', uniqueName);
        await page.waitForTimeout(1000);
        const row = page.locator('table tbody tr').first();
        await expect(row).toContainText(uniqueName);

        // Delete
        await row.locator('button.btn-outline-danger').click();
        await page.click('.modal-footer button:has-text("Yes"), .modal-footer button:has-text("نعم")');
        await expect(page.locator('table tbody')).not.toContainText(uniqueName);
    });

});
