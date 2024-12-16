import { test, expect } from '@playwright/test';

test('Login-Access-And-Visible-Test', async ({ page }) => {
  await page.goto('http://localhost:3000/');
  await page.getByLabel('Toggle navigation').click();
  await page.goto('http://localhost:3000/orders');
  await expect(page.getByRole('heading', { name: 'Owner Login' })).toBeVisible();
  await page.goto('http://localhost:3000/orders/1');
  await expect(page.getByRole('heading', { name: 'Owner Login' })).toBeVisible();
  await page.goto('http://localhost:3000/admin/product/update/1');
  await expect(page.getByRole('heading', { name: 'Owner Login' })).toBeVisible();
  await page.goto('http://localhost:3000/admin/product/create');
  await expect(page.getByRole('heading', { name: 'Owner Login' })).toBeVisible();
  await page.goto('http://localhost:3000/admin/order/update/1');
  await expect(page.getByRole('heading', { name: 'Owner Login' })).toBeVisible();
  await page.goto('http://localhost:3000/');
  await page.getByLabel('Toggle navigation').click();
  await expect(page.getByRole('button', { name: 'Orders' })).toBeHidden();
  await page.getByRole('link', { name: 'Products' }).click();
  await page.getByLabel('Close').click();
  await page.locator('div:nth-child(2) > .card > .card-body > .btn > a').click();
  await expect(page.getByRole('button', { name: 'Delete Product' })).toBeHidden();
  await expect(page.getByRole('button', { name: 'Update Product' })).toBeHidden();

  await page.goto('http://localhost:3000/admin/login');
  await page.getByLabel('Username').click();
  await page.getByLabel('Username').fill('Andrei');
  await page.getByLabel('Password').click();
  await page.getByLabel('Password').fill('Password1!');
  await page.getByRole('button', { name: 'Login', exact: true }).click();
  await page.getByLabel('Toggle navigation').click();
  await expect(page.getByRole('link', { name: 'Orders' })).toBeVisible();
  await page.getByRole('link', { name: 'Orders' }).click();
  await expect(page.url()).toBe('http://localhost:3000/orders');
  await page.getByRole('link', { name: 'Products' }).click();
  await page.getByLabel('Close').click();
  await page.locator('div:nth-child(2) > .card > .card-body > .btn > a').click();
  await expect(page.getByRole('button', { name: 'Delete Product' })).toBeVisible();
  await expect(page.getByRole('link', { name: 'Update Product' })).toBeVisible();
  await page.getByRole('button', { name: 'Logout' }).click();
});