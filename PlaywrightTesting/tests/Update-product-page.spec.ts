import { test, expect } from '@playwright/test';

test('UpdateOneProduct', async ({ page }) => {
    await page.goto('http://localhost:3000/');
    await page.getByLabel('Toggle navigation').click();
    await page.getByRole('link', { name: 'Products' }).click();
    await page.getByLabel('Close').click();
    await page.locator('div:nth-child(3) > .card > .card-body > .btn > a').click();
    await page.getByRole('button', { name: 'Update Product' }).click();
    await page.locator('input[name="title"]').click();
    await page.locator('input[name="title"]').fill('New Product!');
    await page.getByRole('button', { name: 'Save Changes' }).click();
    await expect(page.getByText('Product saved successfully!')).toBeVisible();
});