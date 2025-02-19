import { test, expect } from '@playwright/test';

test('OwnerLoginAndLogout_Successful', async ({ page }) => {
  await page.setViewportSize({ width: 1920, height: 1080 });
  await page.goto('http://localhost:3000/');
  await page.mouse.wheel(0, 1000);
  await page.getByRole('button', { name: 'Owner Login' }).click();
  await page.getByLabel('Username').click();
  await page.getByLabel('Username').fill('Andrei');
  await page.getByLabel('Password').click();
  await page.getByLabel('Password').fill('Password1!');
  await page.getByRole('button', { name: 'Login', exact: true }).click();
  await expect(page.getByText('Owner', { exact: true })).toBeVisible();
  await expect(page.getByRole('button', { name: 'Logout' })).toBeVisible();
  await page.getByRole('button', { name: 'Logout' }).click();
  await page.goto('http://localhost:3000/');
});

test('OwnerLogin_Unsuccessful', async ({ page }) => {
  await page.setViewportSize({ width: 1920, height: 1080 });
  await page.goto('http://localhost:3000/');
  await page.mouse.wheel(0, 1000);
  await page.getByRole('button', { name: 'Owner Login' }).click();
  await page.getByLabel('Username').click();
  await page.getByLabel('Username').fill('WrongUsername');
  await page.getByLabel('Password').click();
  await page.getByLabel('Password').fill('WrongPassword');
  await page.getByRole('button', { name: 'Login', exact: true }).click();
  await expect(page.getByText('Unauthorized: Invalid credentials')).toBeVisible();
});