import { test, expect } from '@playwright/test';

// test('SendEmailWithTemplate', async ({ page }) => {
//   await page.goto('http://localhost:3000/');
//   await page.setViewportSize({ width: 1280, height: 720 });
//   await page.getByRole('button', { name: 'Owner Login' }).click();
//   await page.getByLabel('Username').click();
//   await page.getByLabel('Username').fill('Andrei');
//   await page.getByLabel('Password').click();
//   await page.getByLabel('Password').fill('Password1!');
//   await page.getByRole('button', { name: 'Login', exact: true }).click();
//   await page.getByLabel('Toggle navigation').click();
//   await page.getByRole('link', { name: 'Emails' }).click();
//   await page.getByLabel('Close').click();
//   await page.locator('div:nth-child(6)').first().click();
//   await page.getByLabel('Email To').click();
//   await page.getByLabel('Email To').fill('xilef992@gmail.com');
//   await page.getByLabel('Email Title').click();
//   await page.getByLabel('Email Title').fill('A Title');
//   await page.getByLabel('Template Name').click();
//   await page.getByLabel('Template Name').fill('Default');
//   await page.getByLabel('Header').click();
//   await page.getByLabel('Header').fill('Header');
//   await page.getByLabel('Body').click();
//   await page.getByLabel('Body').fill('Body');
//   await page.getByLabel('Footer').click();
//   await page.getByLabel('Footer').fill('Footer');
//   await page.getByLabel('Correspondant Name').click();
//   await page.getByLabel('Correspondant Name').fill('Felix');
//   await page.getByLabel('Sender Name').click();
//   await page.getByLabel('Sender Name').fill('Felix');
//   await page.getByRole('button', { name: 'Send Email' }).click();
//   ///await expect(page.getByText('Email sent successfully!')).toBeVisible();
//   await expect(page.getByText('Owner', { exact: true })).toBeVisible();
//   await expect(page.getByRole('button', { name: 'Logout' })).toBeVisible();
//   await page.getByRole('button', { name: 'Logout' }).click();
//   await page.goto('http://localhost:3000/');

  
// });
test('RESTTemplates', async ({ page }) => {
  await page.goto('http://localhost:3000/');
  await page.getByLabel('Toggle navigation').click();
  await page.getByLabel('Close').click();
  await page.getByRole('button', { name: 'Owner Login' }).click();
  await page.getByLabel('Username').click();
  await page.getByLabel('Username').fill('Andrei');
  await page.getByLabel('Password').click();
  await page.getByLabel('Password').fill('Password1!');
  await page.getByRole('button', { name: 'Login', exact: true }).click();
  await page.getByLabel('Toggle navigation').click();
  await page.getByRole('link', { name: 'Emails' }).click();

  await page.getByRole('link', { name: 'Emails' }).click();
  await page.getByLabel('Close').click();
  await page.getByRole('link', { name: 'Add Template' }).click();
  await page.getByLabel('Template Name:').click();
  await page.getByLabel('Template Name:').fill('playfun');

  await page.getByLabel('HTML Format:').click();
  await page.getByLabel('HTML Format:').fill('s');
  await page.getByRole('button', { name: 'Send Template' }).click();
  await page.getByRole('link', { name: 'Go back' }).click();
  await expect(page.getByText('Owner', { exact: true })).toBeVisible();
  await expect(page.getByRole('button', { name: 'Logout' })).toBeVisible();
  await page.getByRole('button', { name: 'Logout' }).click();

});