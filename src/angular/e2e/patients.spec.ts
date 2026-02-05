import { test, expect } from '@playwright/test';

test.describe('Patients Management UI', () => {

    test.beforeEach(async ({ page }) => {
        await page.goto('/account/login');
        if (page.url().includes('/account/login')) {
            await page.fill('input[name="userNameOrEmailAddress"]', 'admin');
            await page.fill('input[name="password"]', '1q2w3E*');
            await page.click('button[type="submit"]');
            await page.waitForURL('/', { timeout: 15000 });
        }
    });

    test('should create and search for a new patient', async ({ page }) => {
        await page.goto('/patients');

        // 1. Open Create Modal
        await page.click('button.btn-primary:has-text("New"), button.btn-primary:has-text("جديد")');
        await expect(page.locator('abp-modal')).toBeVisible();

        // 2. Fill Form
        const uniqueId = Math.floor(Math.random() * 10000);
        const firstName = `TestPatientAr${uniqueId}`;
        const lastName = 'FamilyName';

        await page.fill('input[name="firstNameAr"]', firstName);
        await page.fill('input[name="lastNameAr"]', lastName);
        await page.fill('input[name="dateOfBirth"]', '1990-01-01');
        await page.selectOption('select[name="gender"]', { value: '1' }); // Male
        await page.fill('input[name="mobileNumber"]', `050000${uniqueId}`);

        // 3. Save
        await page.click('abp-modal button.btn-primary:has-text("Save"), abp-modal button.btn-primary:has-text("حفظ")');
        await expect(page.locator('abp-modal')).not.toBeVisible();

        // 4. Search
        await page.fill('input[placeholder="Search..."], input[placeholder="بحث..."]', firstName);
        // Wait for debounce or grid update
        await page.waitForTimeout(1000);

        // 5. Verify
        await expect(page.locator('ngx-datatable-body')).toContainText(firstName);
    });

    test('should edit an existing patient', async ({ page }) => {
        // Create a patient first to ensure we have one to edit (or rely on seed/previous test if sequential)
        // For robustness, let's just pick the first one from the list
        await page.goto('/patients');
        await page.waitForSelector('ngx-datatable-body-row');

        const firstRow = page.locator('ngx-datatable-body-row').first();
        const originalName = await firstRow.locator('.fw-bold').innerText(); // Assuming MRN or Name is bold

        // Click Edit (blue pencil button)
        await firstRow.locator('.btn-outline-primary').click();
        await expect(page.locator('abp-modal')).toBeVisible();

        // Change Middle Name (to avoid breaking constraints on primary names)
        const newMiddleName = `Updated${Math.floor(Math.random() * 100)}`;
        await page.fill('input[name="middleNameAr"]', newMiddleName);

        // Save
        await page.click('abp-modal button.btn-primary');
        await expect(page.locator('abp-modal')).not.toBeVisible();

        // Use backend refresh or UI wait
        await page.waitForTimeout(1000);

        // Verify (Need to check if middle name is shown in table? Template says fullNameAr shows just name)
        // If the table shows full name, it might update.
        // Or we can open edit again to check.
        await firstRow.locator('.btn-outline-primary').click();
        await expect(page.locator('input[name="middleNameAr"]')).toHaveValue(newMiddleName);
    });
});
