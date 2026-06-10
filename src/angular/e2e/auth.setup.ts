import { test as setup, expect } from '@playwright/test';
import * as path from 'path';

const authFile = path.join(__dirname, '../.auth/user.json');

setup('authenticate', async ({ page }) => {
  // Perform authentication steps. Replace these actions with your app's login.
  await page.goto('/account/login');
  
  // Fill in the login form. 
  // Standard ABP login page uses LoginInput.UserNameOrEmailAddress
  await page.locator('input[id="LoginInput_UserNameOrEmailAddress"], input[name="LoginInput.UserNameOrEmailAddress"], input[name="userNameOrEmailAddress"]').fill('admin');
  await page.locator('input[id="LoginInput_Password"], input[name="LoginInput.Password"], input[name="password"]').fill('Abc.123'); // Use the correct test password
  
  // Submit
  await page.getByRole('button', { name: /login|دخول/i }).click();

  // Wait until the page receives the cookies/tokens and redirects
  // Wait a bit to ensure tokens are fully written to storage
  await expect(page).not.toHaveURL(/.*login.*/i, { timeout: 30000 });
  await page.waitForTimeout(10000);

  // End of authentication steps.
  await page.context().storageState({ path: authFile });
});
