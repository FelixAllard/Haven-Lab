import { test, expect } from '@playwright/test';

test('test', async ({ page }) => {
  await page.goto('http://localhost:3000/');
  
  // Accessing elements using screen.getByRole instead of destructuring
  const toggleNavigationButton = await page.locator('button[aria-label="Toggle navigation"]');
  await toggleNavigationButton.click();
  
  // Click on the 'Orders' link by role and name
  await page.locator('role=link[name="Orders"]').click();
  
  // Optionally, close the navigation by clicking the close button
  await page.locator('[aria-label="Close"]').click();
  
  const firstViewButton = page.locator('button:has-text("View")').first();
  await firstViewButton.click();
});
