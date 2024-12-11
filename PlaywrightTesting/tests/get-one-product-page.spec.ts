import { test, expect } from '@playwright/test';

test('GetOneProduct', async ({ page }) => {
  await page.goto('http://localhost:3000/');
  await page.getByLabel('Toggle navigation').click();
  await page.getByRole('link', { name: 'Products' }).click();
  await page.getByLabel('Close').click();
  await page.getByRole('heading', { name: 'Our Products' }).click();
  await page.getByRole('heading', { name: 'Felix' }).click();
  await page.locator('div:nth-child(2) > .card > .card-body > .btn > a').click();
  await page.getByRole('heading', { name: 'Felix' }).click();
  await page.getByText('Description:').click();
});

test('GoBackFromProduct', async ({ page }) => {
  await page.goto('http://localhost:3000/product/8073775972397');
  await page.getByRole('heading', { name: 'Felix' }).click();
  await page.getByText('Deacsioja').click();
  await page.locator('.btn').first().click();
  await page.getByRole('heading', { name: 'Our Products' }).click();
});