import { test, expect } from '@playwright/test';

test.describe('Radiology Module', () => {

    // Helper for login - in a real app, strict auth state storage is better
    test.beforeEach(async ({ page }) => {
        await page.goto('/services/radiology');

        if (page.url().toLowerCase().includes('/account/login')) {
            await page.waitForSelector('input[name*="UserNameOrEmailAddress"]', { timeout: 15000 });
            await page.fill('input[name*="UserNameOrEmailAddress"]', 'admin');
            await page.fill('input[name*="Password"]', '1q2w3E*');
            await page.click('button[type="submit"]');
            await page.waitForURL('**/services/radiology', { timeout: 30000 });
        }
    });

    test('should navigate to radiology page', async ({ page }) => {
        await page.goto('/services/radiology');
        await expect(page).toHaveURL('/services/radiology');
        // Looking for the localized header we added
        // Assuming English for now, or the key if not translated in build
        await expect(page.locator('h5.card-title')).toContainText(/Radiology|الأشعة/);
    });

    test('should disable create button if form is invalid', async ({ page }) => {
        await page.goto('/services/radiology');

        await page.click('button:has-text("New Exam"), button:has-text("فحص جديد")');

        const saveBtn = page.locator('div.modal-footer button.btn-primary');
        // Form is empty, invalid
        await expect(saveBtn).toBeDisabled();
    });

    // We can add a create test, but it changes data. 
    // Ideally we delete it after, or run against a test DB.
    test('should create a new radiology exam', async ({ page }) => {
        await page.goto('/services/radiology');
        await page.click('button:has-text("New Exam"), button:has-text("فحص جديد")');

        // Fill Form
        // Note: Selectors might need adjustment based on generated IDs or structure
        // We used formControlName in the template
        const code = `TEST-${Math.floor(Math.random() * 1000)}`;
        await page.fill('input[formControlName="code"]', code);
        await page.locator('select[formControlName="modality"]').selectOption('X-Ray');
        await page.fill('input[formControlName="name"]', 'Playwright Test X-Ray');
        await page.fill('input[formControlName="price"]', '199');
        await page.fill('input[formControlName="bodyPart"]', 'Chest');

        await page.click('div.modal-footer button.btn-primary');

        // Verify appearance in list
        await expect(page.locator('ngx-datatable')).toContainText('Playwright Test X-Ray');
    });
    test('should allow ordering an X-Ray for a patient', async ({ page }) => {
        // 1. Navigate to Patients list
        await page.click('text=Patients'); // Assuming menu item exists
        // If menu is closed/collapsed this might fail, fallback to direct URL
        await page.goto('/patients');
        await page.waitForURL('**/patients');

        // 2. Select first patient
        // Wait for table to load
        await page.waitForSelector('ngx-datatable-row-wrapper');
        const firstRow = page.locator('ngx-datatable-row-wrapper').first();

        // Click actions button (assuming standard ABP or custom dropdown)
        // If the implementation uses a grid with actions column:
        await firstRow.locator('.dropdown-toggle').click();
        await page.click('text=Medical Record'); // Localized: 'السجل الطبي' or similar

        // 3. Click "Orders" tab
        // Use loose matching for Arabic/English
        await page.click('button:has-text("Orders"), button:has-text("الطلبات")');

        // 4. Click "New Order"
        await page.click('text=طلب جديد');

        // 5. Select Radiology and Item
        // Wait for modal
        await expect(page.locator('#orderModal')).toBeVisible();

        // Select Item (index 1 because 0 is placeholder)
        // Note: This requires items to be seeded/loaded
        await page.locator('select').nth(1).selectOption({ index: 1 });

        await page.fill('textarea', 'Playwright Order Test');

        // 6. Save
        await page.click('button:has-text("حفظ الطلب")');

        // 7. Verify order appears in list
        await expect(page.locator('table tbody')).toContainText('Radiology');
        await expect(page.locator('table tbody')).toContainText('Playwright Order Test'); // Use a detail/note check if column exists, or just verify row count increased
    });
});
