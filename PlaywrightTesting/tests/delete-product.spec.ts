import { test, expect } from '@playwright/test';

test('DeleteProductTest', async ({ page }) => {
    
    await page.goto('http://localhost:3000/');
    await page.getByLabel('Toggle navigation').click();
    await page.getByRole('link', { name: 'Products' }).click();
    await page.getByLabel('Close').click();
    await page.locator('div:nth-child(14) > .card > .card-body > .btn > a').click();
    await page.getByRole('button', { name: 'Delete Product' }).click();

});