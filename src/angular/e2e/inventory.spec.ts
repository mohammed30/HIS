import { test, expect } from '@playwright/test';

test.describe('Inventory Module', () => {

    test.beforeEach(async ({ page }) => {
        await page.goto('/inventory');
        // If redirected to login (case-insensitive check or check for login input)
        if (page.url().toLowerCase().includes('/account/login')) {
            await page.waitForSelector('input[name*="UserNameOrEmailAddress"]', { timeout: 15000 });
            await page.fill('input[name*="UserNameOrEmailAddress"]', 'admin');
            await page.fill('input[name*="Password"]', '1q2w3E*');
            await page.click('button[type="submit"]');
            await page.waitForURL('**/inventory', { timeout: 30000 });
        }
    });

    test('should navigate to inventory dashboard', async ({ page }) => {
        await page.goto('/inventory');
        // Wait for any inventory specific element
        await page.waitForLoadState('networkidle');
        await expect(page.locator('h5')).toContainText(/Inventory|المخزن/);
    });

    test('should open warehouses list', async ({ page }) => {
        await page.goto('/inventory');
        await page.click('button:has-text("Warehouse"), button:has-text("المستودعات")');
        await expect(page.locator('ngx-datatable')).toBeVisible();
    });

    test('should open stock levels for a warehouse', async ({ page }) => {
        await page.goto('/inventory');
        // Click on a warehouse in the list (assuming a grid exists)
        await page.waitForSelector('ngx-datatable-row-wrapper');
        await page.locator('ngx-datatable-row-wrapper').first().click();
        
        await expect(page.locator('h5')).toContainText(/Stock Levels|أرصدة المخزون/);
    });

    test('should open receive stock modal', async ({ page }) => {
        await page.goto('/inventory');
        await page.click('button:has-text("Receive Stock"), button:has-text("استلام")');
        // Check for modal visibility
        await expect(page.locator('.modal-title')).toContainText(/Receive|استلام/);
    });
});
