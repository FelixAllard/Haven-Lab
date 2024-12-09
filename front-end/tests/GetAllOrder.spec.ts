import { test, expect } from '@playwright/test';

test('navigation test', async ({ page }) => {
  // Navigate to the homepage
  await page.goto('http://localhost:3000/'); 

  // Toggle navigation menu by using aria-label (if button has it)
  await page.locator('[aria-label="Toggle navigation"]').click();

  // Click on the 'Orders' link by role and name
  await page.locator('role=link[name="Orders"]').click();

  // Optionally, close the navigation by clicking the close button
  await page.locator('[aria-label="Close"]').click();
});
