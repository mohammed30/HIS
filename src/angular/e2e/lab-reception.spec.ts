import { test, expect } from '@playwright/test';

test.describe('Laboratory Reception E2E Tests', () => {

    test.beforeEach(async ({ page }) => {
        // Login
        await page.goto('/account/login');
        if (page.url().includes('/account/login')) {
            await page.fill('input[name="userNameOrEmailAddress"]', 'admin');
            await page.fill('input[name="password"]', '1q2w3E*');
            await page.click('button[type="submit"]');
            await page.waitForURL('/', { timeout: 15000 });
        }

        // Navigate to Lab Reception (assuming it's a route)
        // Based on previous research, it might be /reception/lab-reception
        // Let's check the route provider if possible, but usually it's under receptionist menu.
        await page.goto('/reception/lab-reception');
        await page.waitForLoadState('networkidle');
    });

    test('should prevent saving when mandatory fields are missing', async ({ page }) => {
        // Clear everything if needed or just start fresh
        await page.click('button:has-text("مريض جديد")');

        // Click Save without filling anything
        await page.click('button:has-text("حفظ")');

        // Check for warning toaster
        const warningToaster = page.locator('.toast-warning');
        await expect(warningToaster).toBeVisible();
        await expect(warningToaster).toContainText('يرجى إكمال البيانات المطلوبة');
    });

    test('should save a new patient successfully when all mandatory fields are filled', async ({ page }) => {
        const uniqueId = Math.floor(Math.random() * 10000);
        const fullNameAr = `مريض تجريبي ${uniqueId}`;
        const mobile = `05555${uniqueId}`;

        // 1. Fill Name
        await page.locator('label:has-text("الاسم (عربي)") + div input').fill(fullNameAr);

        // 2. Fill Mobile
        await page.locator('label:has-text("الموبايل") + input').fill(mobile);

        // 3. Fill DOB
        await page.locator('label:has-text("تاريخ الميلاد") + input').fill('1990-05-15');

        // 4. Select Category (نقدا)
        // We might need to wait for lookups to load
        const categorySelect = page.locator('label:has-text("الفئة (طريقة الدفع)") + select');
        await categorySelect.selectOption({ index: 1 }); // Usually first option after "-- اختر --"

        // 5. Save
        await page.click('button:has-text("حفظ")');

        // 6. Verify success
        const successToaster = page.locator('.toast-success');
        await expect(successToaster).toBeVisible();
        await expect(successToaster).toContainText('بنجاح');

        // 7. Verify MRN is generated (not empty)
        const mrnInput = page.locator('label:has-text("رقم الملف") + input');
        await expect(mrnInput).not.toHaveValue('NEW');
        await expect(mrnInput).not.toHaveValue('');
    });

    test('should search and edit an existing patient', async ({ page }) => {
        const uniqueId = Math.floor(Math.random() * 10000);
        const testName = `SearchTest ${uniqueId}`;

        // First create a patient to search for
        await page.locator('label:has-text("الاسم (عربي)") + div input').fill(testName);
        await page.locator('label:has-text("الموبايل") + input').fill('0500000000');
        await page.locator('label:has-text("تاريخ الميلاد") + input').fill('1985-10-10');
        await page.locator('label:has-text("فئة المريض") + select').selectOption({ index: 1 });
        await page.click('button:has-text("حفظ")');
        await page.locator('.toast-success').waitFor({ state: 'visible' });

        // Clear view
        await page.click('button:has-text("مريض جديد")');

        // Search for the patient
        const searchInput = page.locator('label:has-text("الاسم (عربي)") + div input');
        await searchInput.fill(testName);
        await page.click('button:has-text("الاسم (عربي)") + div button'); // Search icon button

        // Wait for results
        const firstResult = page.locator('.list-group-item:has-text("' + testName + '")');
        await expect(firstResult).toBeVisible();
        await firstResult.click();

        // Update a field (e.g. Identity Number)
        const identityInput = page.locator('label:has-text("رقم الهوية") + input');
        const newId = `ID${uniqueId}`;
        await identityInput.fill(newId);

        // Update category and check payment method link (Phase 2 requirement)
        const categorySelect = page.locator('label:has-text("الفئة (طريقة الدفع)") + select');
        await categorySelect.selectOption({ label: 'نقدا' }); // Assuming 'نقدا' is an option

        // Wait for potential debounce or async logic if any
        // Verify payment method in booking tab (this depends on how booking tab is structured)
        // For now, let's just ensure we can select it.

        // Save
        await page.click('button:has-text("حفظ")');
        await expect(page.locator('.toast-success')).toBeVisible();

        // Verify update persisted
        await page.click('button:has-text("مريض جديد")');
        await searchInput.fill(testName);
        await page.click('button:has-text("الاسم (عربي)") + div button');
        await firstResult.click();
        await expect(identityInput).toHaveValue(newId);
    });

    test('should calculate age correctly in years, months, and days', async ({ page }) => {
        const dobInput = page.locator('label:has-text("تاريخ الميلاد") + input');

        // Set date to exactly 2 years, 3 months, and 5 days ago
        const today = new Date();
        const testDate = new Date(today.getFullYear() - 2, today.getMonth() - 3, today.getDate() - 5);
        const dobString = testDate.toISOString().split('T')[0];

        await dobInput.fill(dobString);
        await dobInput.dispatchEvent('change'); // Trigger calculation if not automatic on fill

        // Verify age fields
        await expect(page.locator('[(ngModel)]="ageYears"')).toHaveValue('2');
        await expect(page.locator('[(ngModel)]="ageMonths"')).toHaveValue('3');
        await expect(page.locator('[(ngModel)]="ageDays"')).toHaveValue('5');
    });
});
