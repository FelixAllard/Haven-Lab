import { test, expect } from '@playwright/test';

test('test', async ({ page }) => {
  await page.goto('http://localhost:3000/');
  await page.getByLabel('Toggle navigation').click();
  await page.getByRole('link', { name: 'Orders' }).click();
  await page.getByLabel('Close').click();
  await expect(page.locator('.btn').first()).toBeVisible();
  await page.locator('.btn').first().click();
  await expect(page.getByRole('link', { name: 'Edit' })).toBeVisible();
  await page.getByRole('link', { name: 'Edit' }).click();
  await page.getByLabel('Note').click();
  await page.getByLabel('Note').fill('aaaaaaaaaa');
  await page.getByLabel('Yes').check();
  await page.getByLabel('Tags').click();
  await page.getByLabel('Tags').fill('ccccccc');
  await page.getByLabel('Name').click();
  await page.getByLabel('Name').fill('Regine Wang');
  await page.getByLabel('Address 1').click();
  await page.getByLabel('Name').fill('2095 Place Nobel');
  await page.getByLabel('Address 2').click();
  await page.getByLabel('Address 2').fill('2111 Place Nobel');
  await page.getByLabel('City').click();
  await page.getByLabel('City').fill('Brossard');
  await page.getByLabel('Zip').click();
  await page.getByLabel('Zip').fill('H2G 2L3');
  await page.getByLabel('Province').click();
  await page.getByLabel('Province').fill('Ontario');
  await page.getByLabel('Country').click();
  await page.getByLabel('Country').fill('United-States');
  await page.getByLabel('Shipping Phone').click();
  await page.getByLabel('Shipping Phone').fill('123456789');
  await page.getByLabel('Company').click();
  await page.getByLabel('Company').fill('company1');
  await expect(page.getByRole('button', { name: 'Update Order' })).toBeVisible();
  await page.getByRole('button', { name: 'Update Order' }).click();
});