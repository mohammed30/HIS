using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Account.Web.Pages.Account;

namespace HIS.HttpApi.Host.Pages.Account
{
    public class LoginModel : Volo.Abp.Account.Web.Pages.Account.LoginModel
    {
        public LoginModel(
            Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider schemeProvider,
            Microsoft.Extensions.Options.IOptions<Volo.Abp.Account.Web.AbpAccountOptions> accountOptions,
            Microsoft.Extensions.Options.IOptions<Microsoft.AspNetCore.Identity.IdentityOptions> identityOptions,
            Volo.Abp.Identity.IdentityDynamicClaimsPrincipalContributorCache identityDynamicClaimsPrincipalContributorCache,
            Microsoft.AspNetCore.Hosting.IWebHostEnvironment webHostEnvironment)
            : base(schemeProvider, accountOptions, identityOptions, identityDynamicClaimsPrincipalContributorCache, webHostEnvironment)
        {
        }
    }
}
