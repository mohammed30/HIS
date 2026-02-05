import { test, expect } from '@playwright/test';

test.describe('Medical Orders API', () => {

    test.beforeEach(async ({ page }) => {
        // Ensure authentication (cookie or local storage)
        await page.goto('/account/login');
        if (page.url().includes('/account/login')) {
            await page.fill('input[name="userNameOrEmailAddress"]', 'admin');
            await page.fill('input[name="password"]', '1q2w3E*');
            await page.click('button[type="submit"]');
            await page.waitForURL('/', { timeout: 15000 });
        }
    });

    test('should create a new Medical Order via API', async ({ page, request, playwright }) => {
        // 1. Get Token using UI method (robust for template apps)
        const oidcInfo = await page.evaluate(() => {
            for (let i = 0; i < sessionStorage.length; i++) {
                const key = sessionStorage.key(i);
                if (key && key.startsWith('oidc.user')) {
                    return JSON.parse(sessionStorage.getItem(key)).access_token;
                }
            }
            return null;
        });

        const api = await playwright.request.newContext({
            baseURL: 'https://localhost:44321', // Backend Port
            extraHTTPHeaders: {
                'Authorization': `Bearer ${oidcInfo}`,
                '__tenant': ''
            },
            ignoreHTTPSErrors: true
        });

        // 2. Fetch a valid patient and service item to use
        const patientsRes = await api.get('/api/app/patient?MaxResultCount=1');
        const patients = await patientsRes.json();
        const patientId = patients.items[0].id;

        const servicesRes = await api.get('/api/app/service-item?MaxResultCount=1');
        const services = await servicesRes.json();
        const serviceItemId = services.items[0].id; // Any service item

        // 3. Create Order
        const payload = {
            patientId: patientId,
            type: 0, // Lab
            serviceItemId: serviceItemId,
            clinicalNotes: 'API Test Note',
            details: 'API Details'
        };

        const createRes = await api.post('/api/app/medical-order', { data: payload });
        expect(createRes.ok()).toBeTruthy();
        const createdOrder = await createRes.json();
        expect(createdOrder.clinicalNotes).toBe('API Test Note');

        // 4. Verify Retrieval
        const listRes = await api.get(`/api/app/medical-order?PatientId=${patientId}`);
        expect(listRes.ok()).toBeTruthy();
        const list = await listRes.json();
        const found = list.items.find((x: any) => x.id === createdOrder.id);
        expect(found).toBeDefined();
    });

});
