import { test, expect } from '@playwright/test';

test('CheckIfElementsAccessible', async ({ page }) => {
  await page.goto('http://localhost:3000/admin/product/create');
  await expect(page.locator('h2')).toContainText('Create a New Product');
  await expect(page.getByRole('heading', { name: 'Create a New Product' })).toBeVisible();
  await page.locator('div').filter({ hasText: /^Title$/ }).click();
  await page.getByText('Description').click();
  await page.locator('textarea[name="body_html"]').click();
  await page.getByText('Vendor').click();
  await page.locator('input[name="vendor"]').click();
  await page.getByText('Published Scope').click();
  await page.getByText('Status').click();
  await page.getByRole('heading', { name: 'Variants' }).click();
  await page.getByText('Variant Title').click();
  await page.getByText('Price').click();
  await page.getByText('Inventory Quantity').click();
  await page.getByText('SKU').click();
  await expect(page.locator('input[name="variants\\.0\\.title"]')).toHaveValue('Default Title');
  await page.locator('input[name="variants\\.0\\.price"]').click();
  await page.locator('input[name="variants\\.0\\.inventory_quantity"]').click();
  await page.getByRole('button', { name: 'Submit' }).click();
  await expect(page.getByText('Title', { exact: true })).toBeVisible();
  await page.getByText('Description').click();
  await page.getByText('Vendor').click();
  await page.getByText('Published Scope').click();
  await page.getByText('Status').click();
  await page.getByRole('heading', { name: 'Variants' }).click();
  await page.getByText('Variant Title').click();
  await page.getByText('Price').click();
  await page.getByText('Price').click();
  await page.getByText('Inventory Quantity').click();
  await page.getByText('SKU').click();
  
});

test('CheckIfCanAddProduct', async ({ page }) => {
  await page.goto('http://localhost:3000/admin/product/create');
  await page.locator('input[name="title"]').click();
  await page.locator('input[name="title"]').fill('P');
  await page.locator('textarea[name="body_html"]').click();
  await page.locator('textarea[name="body_html"]').fill('T');
  await page.locator('input[name="vendor"]').click();
  await page.locator('input[name="vendor"]').fill('P');
  await page.locator('input[name="variants\\.0\\.title"]').click();
  await page.locator('input[name="variants\\.0\\.title"]').fill('Default Titl');
  await page.locator('input[name="variants\\.0\\.price"]').click();
  await page.getByRole('button', { name: 'Submit' }).click();

  await expect(page.getByRole('heading', { name: 'Create a New Product' })).toBeHidden();

  
});