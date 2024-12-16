import { test, expect } from '@playwright/test';

test('testIfCanAccessPaymentPage', async ({ page }) => {
  await page.goto('http://localhost:3000/');
  await page.getByLabel('Toggle navigation').click();
  await page.getByRole('link', { name: 'Cart' }).click();
  await page.getByLabel('Close').click();
  await page.getByRole('button', { name: 'Create Draft Order' }).click();
  //await expect(page.locator('span').filter({ hasText: 'Haven Lab' }).first()).toBeVisible();
  await page.getByRole('button', { name: 'Create Draft Order' }).click();
  await page.getByRole('button', { name: 'Create Draft Order' }).click();
  await page.getByRole('button', { name: 'Create Draft Order' }).click();
  await page.getByText('Haven LabWelcome to Haven LabHomeProductsCartOrdersAbout UsYour CartRecommended').click({
    button: 'right'
  });
  await page.getByText('Haven LabWelcome to Haven LabHomeProductsCartOrdersAbout UsYour CartRecommended').click();
  await page.getByRole('button', { name: 'Create Draft Order' }).click();
  await page.getByPlaceholder('Email or mobile phone number').click();
  await page.getByPlaceholder('Email or mobile phone number').fill('xilef992@gmail.com');
  await page.getByPlaceholder('First name').click();
  await page.getByPlaceholder('First name').fill('Felix');
  await page.getByPlaceholder('Last name').click();
  await page.getByPlaceholder('Last name').fill('Allard');
  await page.getByPlaceholder('Address').click();
  await page.getByPlaceholder('Address').fill('6601 Louis-Hemon');
  await page.getByPlaceholder('City').click();
  await page.getByPlaceholder('City').fill('Montreal');
  await page.getByPlaceholder('Postal code').click();
  await page.getByPlaceholder('Postal code').fill('h2g 2l1');
  await page.locator('div').filter({ hasText: /^Credit card$/ }).first().click();
  await page.locator('body').press('Tab');
});