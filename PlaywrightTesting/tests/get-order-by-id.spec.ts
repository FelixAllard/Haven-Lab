import { test, expect } from '@playwright/test';

test('test', async ({ page }) => {
  await page.goto('http://localhost:3000/');
  await page.getByLabel('Toggle navigation').click();
  await page.getByRole('link', { name: 'Orders' }).click();
  await page.getByLabel('Close').click();
  await expect(page.getByRole('button', { name: 'View Details' }).first()).toBeVisible();
  await page.getByRole('button', { name: 'View Details' }).first().click();
  await expect(page.getByText('Order Number:')).toBeVisible();
});