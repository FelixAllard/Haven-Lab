import { test, expect } from '@playwright/test';

test('GetOneProduct', async ({ page }) => {
  await page.goto('http://localhost:3000/');
  await page.getByLabel('Toggle navigation').click();
  await page.getByRole('link', { name: 'Products' }).click();
  await page.getByLabel('Close').click();
  await expect(page.getByRole('heading', { name: 'Our Products' })).toBeVisible();

  await page.getByText('Example Product').click();
  await page.locator('div:nth-child(2) > .card > .card-body > .btn > a').click();
  await page.locator('body').press('ControlOrMeta+s');
  await expect(page.locator('h1')).toMatchAriaSnapshot(`- heading "Felix" [level=1]`);
  await page.getByText('Description:Deacsioja').click();
});
test('GoBackFromProduct', async ({ page }) => {
  await page.goto('http://localhost:3000/product/8069973540909');
  await expect(page.locator('h1')).toMatchAriaSnapshot(`- heading "Example Product" [level=1]`);
  await page.getByText('Description:This is the product descriptionVendor: VC ShopZWeight: 250 lbPrice').click();
  await page.locator('.col-md-4').first().click();
  await page.locator('.btn').first().click();
  await expect(page.locator('h1')).toMatchAriaSnapshot(`- heading "Our Products" [level=1]`);
  await page.getByText('Example ProductQuantity: Price: $5View Product').click();
});