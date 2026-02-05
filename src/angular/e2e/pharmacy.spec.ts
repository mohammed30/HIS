import { test, expect } from '@playwright/test';

test.describe('Pharmacy Module Workflow', () => {

    test.beforeEach(async ({ page }) => {
        await page.goto('/account/login');
        if (page.url().includes('/account/login')) {
            await page.fill('input[name="userNameOrEmailAddress"]', 'admin');
            await page.fill('input[name="password"]', '1q2w3E*');
            await page.click('button[type="submit"]');
            await page.waitForURL('/', { timeout: 15000 });
        }
    });

    test('should create medication order and dispense it', async ({ page }) => {
        // 1. Create Medication Order
        // Navigate to Patients -> First Patient -> Medical Record -> Orders
        await page.goto('/patients');
        await page.waitForSelector('ngx-datatable-row-wrapper', { state: 'visible' });

        // Open actions for first patient
        const firstRow = page.locator('ngx-datatable-row-wrapper').first();
        await firstRow.locator('.dropdown-toggle').click();
        await page.click('text=Medical Record');

        // Wait for Medical Record page then go to Orders tab
        await page.waitForURL(/\/medical-records\/patient\/.*/);
        await page.click('button:has-text("Orders"), button:has-text("الطلبات")');

        // Open New Order Modal
        await page.click('button:has-text("طلب جديد"), button:has-text("New Order")');

        // Select "Medication" (Type 2)
        // Adjust selector if needed. Assuming <select> with options.
        // We look for label "Medication" or "Pharmacy" depending on localization
        const typeSelect = page.locator('select').first();
        // Try selecting by value '2' (Medication enum value) if possible, or by label
        // Based on OrderType enum: Lab=0, Radiology=1, Medication=2
        await typeSelect.selectOption({ text: 'Medication' }).catch(() => typeSelect.selectOption({ index: 2 }));

        await page.waitForTimeout(500);

        // Select Medication Item
        // Assuming second select is the item list
        await page.locator('select').nth(1).selectOption({ index: 1 }); // Select first available drug

        // Fill details
        await page.fill('textarea', 'E2E Pharmacy Test');

        // Save
        await page.click('button:has-text("حفظ الطلب")');

        // Verify it appears in order list
        await expect(page.locator('table tbody')).toContainText('E2E Pharmacy Test');


        // 2. Dispense in Pharmacy
        await page.goto('/pharmacy');

        // Wait for list to load
        await page.waitForSelector('app-prescriptions-list table tbody tr');

        // Check if our order is there
        const row = page.locator('tr:has-text("E2E Pharmacy Test")');
        await expect(row).toBeVisible();

        // Click Dispense
        await row.locator('button:has-text("Dispense")').click();

        // 3. Confirm Dispensing
        await page.waitForURL(/\/pharmacy\/dispense\/.*/);
        await expect(page.locator('h5')).toContainText('Dispense Medication');

        // Click Confirm
        await page.click('button:has-text("Confirm Dispense")');

        // 4. Verify Success and Redirect
        await page.waitForURL('/pharmacy');

        // Verify order is gone from list (or marked done if we showed history, but list shows pending)
        await expect(page.locator('tr:has-text("E2E Pharmacy Test")')).not.toBeVisible();
    });

});
