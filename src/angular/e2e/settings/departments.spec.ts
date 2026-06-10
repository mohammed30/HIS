import { test, expect } from '@playwright/test';

test.describe('Departments Settings UI', () => {
    // Uses global authentication from auth.setup.ts

    test('should load departments list', async ({ page }) => {
        // Navigate to departments settings page
        // Standard ABP route for an entity called Departments might be /settings/departments or /departments
        // Assuming route is /settings/departments based on typical ABP structure
        await page.goto('/settings/departments');
        
        // Ensure page is loaded
        await expect(page.locator('table.table')).toBeVisible({ timeout: 15000 });
        
        // Check header title
        const header = page.locator('h1, h2, h3').filter({ hasText: /Departments|الأقسام/i }).first();
        await expect(header).toBeVisible();
    });

    test('should open create department modal and cancel', async ({ page }) => {
        await page.goto('/settings/departments');
        
        // Wait for New button
        const newBtn = page.locator('button.btn-primary').filter({ hasText: /إضافة/i }).first();
        await expect(newBtn).toBeVisible();
        await newBtn.click();
        
        // Wait for modal
        const modal = page.locator('.modal-content');
        await expect(modal).toBeVisible();
        
        // Find cancel button and click
        const cancelBtn = modal.locator('button').filter({ hasText: /Cancel|إلغاء/i });
        await cancelBtn.click();
        
        // Modal should close
        await expect(modal).not.toBeVisible();
    });
});
