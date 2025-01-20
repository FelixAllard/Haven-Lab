import { test, expect } from '@playwright/test';

test('GetOneProduct', async ({ page }) => {
  await page.setViewportSize({ width: 1920, height: 1080 });
  await page.goto('http://localhost:3000/');
  await page.getByLabel('Toggle navigation').click();
  await page.getByRole('link', { name: 'Products' }).click();
  await page.getByLabel('Close').click();
  await page.locator('div:nth-child(2) > .card > .card-body > .btn').click();
  await page.getByText('Description:').click();
});