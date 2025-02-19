import { test, expect } from '@playwright/test';

const waitFor = (ms) => new Promise(resolve => setTimeout(resolve, ms));

// test('test', async ({ page }) => {

//   await page.goto('http://localhost:3000/admin/login');
//   await page.getByLabel('Username').click();
//   await page.getByLabel('Username').fill('Andrei');
//   await page.getByLabel('Password').click();
//   await page.getByLabel('Password').fill('Password1!');
//   await page.getByRole('button', { name: 'Login', exact: true }).click();
//   await waitFor(1000);

//   await page.goto('http://localhost:3000/');
//   await page.getByLabel('Toggle navigation').click();
//   await page.getByRole('link', { name: 'Orders' }).click();
//   await page.getByLabel('Close').click();
//   await expect(page.getByRole('button', { name: 'View Details' }).first()).toBeVisible();
//   await page.getByRole('button', { name: 'View Details' }).first().click();
//   await expect(page.getByText('Order Number:')).toBeVisible();

//   await page.getByRole('button', { name: 'Logout' }).click();
// });

