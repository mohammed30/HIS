import { test, expect } from '@playwright/test';

test.describe('Radiology API', () => {
    let apiContext;
    let token;

    test.beforeAll(async ({ playwright, request }) => {
        // 1. Authenticate to get the token
        // In ABP, we usually hit the /connect/token or login endpoint
        // For simplicity reusing the browser login flow or a direct API login if available
        // Here we simulate the login via API if Identity Server allows ROPC, or we just rely on the existing auth state 
        // if we were using 'storageState'. 
        // However, for pure API tests, we often request a token.

        // Setup a context if we need specific headers
        apiContext = await playwright.request.newContext({
            baseURL: 'http://localhost:44321', // Backend URL directly
            ignoreHTTPSErrors: true
        });

        // Use the browser login to get cookies/token if ROPC is disabled, 
        // OR simply assume we are running against the local dev env which might allow cookie auth
        // For this example, we'll try to use the API structure.
    });

    // Alternatively, we can just use the Page to login and get the storage state
    test.beforeEach(async ({ page }) => {
        await page.goto('http://localhost:4200/account/login');
        await page.fill('input[name="userNameOrEmailAddress"]', 'admin');
        await page.fill('input[name="password"]', '1q2w3E*');
        await page.click('button[type="submit"]');
        await page.waitForURL('/');
    });

    test('should CRUD radiology items via API', async ({ page, request, playwright }) => {
        // Get the access token from the browser local storage or cookies after login
        // ABP Angular template stores token in OAuth storage commonly
        const accessToken = await page.evaluate(() => {
            return sessionStorage.getItem('access_token') || localStorage.getItem('access_token');
            // Note: ABP might store it in a specific key depending on oidc lib
        });

        // If we can't easily grab the token (HttpOnly cookies etc), we'll rely on the session cookie shared if distinct host
        // But localhost:4200 vs localhost:44300 are cross-origin.

        // Fallback: Make requests using the PAGE's request context (which shares cookies if same domain)
        // Since we are cross-domain (4200 vs 443xx), we need the token.

        // Let's assume we can hit the API directly using the known credentials if we had the token endpoint set up.
        // For now, let's demo the structure assuming we have a way to auth or we are testing public endpoints (unlikely).

        // REAL IMPLEMENTATION:
        // 1. Hit Identity Server Token Endpoint
        const tokenResponse = await request.post('https://localhost:44321/connect/token', {
            form: {
                grant_type: 'password',
                client_id: 'HIS_Web', // Default client
                username: 'admin',
                password: '1q2w3E*',
                scope: 'HIS offline_access'
            },
            ignoreHTTPSErrors: true
        });

        // If the above fails (due to rigid IdSrv setup), we might skip auth for the demo or use the UI-driven token extraction.
        // Let's try the UI extraction as it's more robust for default templates.

        const oidcInfo = await page.evaluate(() => {
            // Try to find OIDC data in storage
            for (let i = 0; i < localStorage.length; i++) {
                const key = localStorage.key(i);
                if (key && key.startsWith('oidc.user')) {
                    return JSON.parse(localStorage.getItem(key)).access_token;
                }
            }
            for (let i = 0; i < sessionStorage.length; i++) {
                const key = sessionStorage.key(i);
                if (key && key.startsWith('oidc.user')) {
                    return JSON.parse(sessionStorage.getItem(key)).access_token;
                }
            }
            return null;
        });

        if (!oidcInfo) {
            console.log('Warn: Could not find access token in storage. API calls might fail if authorized.');
        }

        const api = await playwright.request.newContext({
            baseURL: 'https://localhost:44321', // Your backend HTTPS port
            extraHTTPHeaders: {
                'Authorization': `Bearer ${oidcInfo}`,
                '__tenant': ''
            },
            ignoreHTTPSErrors: true
        });

        // 1. CREATE
        const newCode = `API-${Math.floor(Math.random() * 10000)}`;
        const createRes = await api.post('/api/app/service-item/radiology', {
            data: {
                code: newCode,
                name: 'API Created X-Ray',
                modality: 'X-Ray',
                bodyPart: 'Arm',
                price: 50.0,
                instructions: 'Via Playwright API',
                isActive: true,
                category: 3
            }
        });
        expect(createRes.ok()).toBeTruthy();
        const createdItem = await createRes.json();
        expect(createdItem.name).toBe('API Created X-Ray');

        // 2. GET
        const getRes = await api.get(`/api/app/service-item/radiology?Filter=${newCode}`);
        expect(getRes.ok()).toBeTruthy();
        const list = await getRes.json();
        expect(list.items.length).toBeGreaterThan(0);
        const found = list.items.find(x => x.code === newCode);
        expect(found).toBeDefined();

        // 3. DELETE
        const deleteRes = await api.delete(`/api/app/service-item/${found.id}`);
        expect(deleteRes.ok()).toBeTruthy();
    });
});
