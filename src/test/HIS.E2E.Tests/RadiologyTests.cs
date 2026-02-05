using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.Xunit; // Optional helper, but we can standard usage
using Xunit;

namespace HIS.E2E.Tests;

public class RadiologyTests : IAsyncLifetime
{
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IPage _page;

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true }); // Use Headless = false to see it
        _page = await _browser.NewPageAsync(new BrowserNewPageOptions { BaseURL = "http://localhost:4200" });
    }

    public async Task DisposeAsync()
    {
        await _browser.DisposeAsync();
        _playwright.Dispose();
    }

    [Fact]
    public async Task Should_Navigate_To_Radiology()
    {
        // 1. Login
        await _page.GotoAsync("/account/login");
        
        // Wait for login or redirect
        if (await _page.UrlAsync().ContinueWith(t => t.Result.EndsWith("/account/login")))
        {
            await _page.FillAsync("input[name=\"userNameOrEmailAddress\"]", "admin");
            await _page.FillAsync("input[name=\"password\"]", "1q2w3E*");
            await _page.ClickAsync("button[type=\"submit\"]");
            await _page.WaitForURLAsync("/");
        }

        // 2. Navigate to Radiology
        await _page.GotoAsync("/services/radiology");
        
        // 3. Verify
        await _page.WaitForSelectorAsync("h5.card-title");
        var title = await _page.TextContentAsync("h5.card-title");
        
        Assert.Matches("Radiology|الأشعة", title);
    }
}
