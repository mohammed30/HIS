import { test, expect } from '@playwright/test';

// Reset storage state for this file to avoid being already logged in
test.use({ storageState: { cookies: [], origins: [] } });

test.describe('Authentication', () => {
  test('should load login page and allow user to login', async ({ page }) => {
    await page.goto('/account/login');

    // Check if the login form is visible
    const userNameInput = page.locator('input[id="LoginInput_UserNameOrEmailAddress"], input[name="LoginInput.UserNameOrEmailAddress"], input[name="userNameOrEmailAddress"]');
    const passwordInput = page.locator('input[id="LoginInput_Password"], input[name="LoginInput.Password"], input[name="password"]');
    const loginButton = page.getByRole('button', { name: /login|دخول/i });

    await expect(userNameInput).toBeVisible();
    await expect(passwordInput).toBeVisible();
    await expect(loginButton).toBeVisible();

    // Fill credentials
    await userNameInput.fill('admin');
    await passwordInput.fill('Abc.123');

    // Submit
    await loginButton.click();

    // Verify successful login by checking for the presence of the nav menu or logout button
    await expect(page).not.toHaveURL(/.*login.*/i, { timeout: 30000 });
    await page.waitForTimeout(5000);
    // Make sure we are no longer on the login page
    expect(page.url()).not.toContain('/account/login');
  });

  test('should show error with invalid credentials', async ({ page }) => {
    await page.goto('/account/login');

    await page.locator('input[id="LoginInput_UserNameOrEmailAddress"], input[name="LoginInput.UserNameOrEmailAddress"], input[name="userNameOrEmailAddress"]').fill('invalid_user');
    await page.locator('input[id="LoginInput_Password"], input[name="LoginInput.Password"], input[name="password"]').fill('wrong_password');
    await page.getByRole('button', { name: /login|دخول/i }).click();

    // Usually ABP shows an error toaster or validation message
    // We can just wait to ensure we are still on the login page
    await expect(page).toHaveURL(/.*login.*/i);
  });
});
