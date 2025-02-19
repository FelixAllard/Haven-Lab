import { test, expect } from '@playwright/test';

const waitFor = (ms) => new Promise(resolve => setTimeout(resolve, ms));

// test('navigation test', async ({ page }) => {
//   // Navigate to the homepage
//   await page.setViewportSize({ width: 1920, height: 1080 });

//   await page.goto('http://localhost:3000/admin/login');
//   await page.getByLabel('Username').click();
//   await page.getByLabel('Username').fill('Andrei');
//   await page.getByLabel('Password').click();
//   await page.getByLabel('Password').fill('Password1!');
//   await page.getByRole('button', { name: 'Login', exact: true }).click();
//   await waitFor(1000);

//   await page.goto('http://localhost:3000/'); 

//   // Toggle navigation menu by using aria-label (if button has it)
//   await page.locator('[aria-label="Toggle navigation"]').click();

//   // Click on the 'Orders' link by role and name
//   await page.locator('role=link[name="Orders"]').click();

//   // Optionally, close the navigation by clicking the close button
//   await page.locator('[aria-label="Close"]').click();

//   await page.getByRole('button', { name: 'Logout' }).click();
// });
