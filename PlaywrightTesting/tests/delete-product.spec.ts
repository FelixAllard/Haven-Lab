import { test, expect } from '@playwright/test';

const waitFor = (ms) => new Promise(resolve => setTimeout(resolve, ms));

test('DeleteProductTest', async ({ page }) => {
    await page.setViewportSize({ width: 1920, height: 1080 });
    await page.goto('http://localhost:3000/admin/login');
    await page.getByLabel('Username').click();
    await page.getByLabel('Username').fill('Andrei');
    await page.getByLabel('Password').click();
    await page.getByLabel('Password').fill('Password1!');
    await page.getByRole('button', { name: 'Login', exact: true }).click();
    await waitFor(1000);

    await page.goto('http://localhost:3000/');
    await page.getByLabel('Toggle navigation').click();
    await page.getByRole('link', { name: 'Products' }).click();
    await page.getByLabel('Close').click();
    await page.locator('div:nth-child(5) > .card > .card-body > .btn').click();
    await page.getByRole('button', { name: 'Delete Product' }).click();

    await page.getByRole('button', { name: 'Logout' }).click();

});