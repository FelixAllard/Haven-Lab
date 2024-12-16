import { test, expect } from '@playwright/test';

test('AddToCartAndCheckout', async ({ page }) => {
    await page.goto('http://localhost:3000/');
    await page.getByLabel('Toggle navigation').click();
    await page.getByRole('link', { name: 'Products' }).click();
    await page.getByLabel('Close').click();
    await page.locator('.btn > a').first().click();
    await page.getByRole('button', { name: 'Add to Cart' }).click();
    await page.getByLabel('Toggle navigation').click();
    await page.getByRole('link', { name: 'Cart' }).click();
    await page.getByLabel('Close').click();
    await page.getByRole('button', { name: '+' }).click();
    await page.getByRole('button', { name: '+' }).click();
    await page.getByRole('button', { name: '-' }).click();
    await page.getByText('2', { exact: true }).click();
    await page.getByRole('button', { name: 'Create Draft Order' }).click();
    await page.getByText('Total', { exact: true }).click();
});