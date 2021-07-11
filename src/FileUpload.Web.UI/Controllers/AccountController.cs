using FileUpload.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Neptuo;
using Neptuo.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FileUpload.Controllers
{
    [Route("[controller]")]
    public class AccountController : Controller
    {
        private readonly AccountOptions options;

        public AccountController(IOptions<AccountOptions> options)
        {
            Ensure.NotNull(options, "options");
            this.options = options.Value;
        }

        [Route("[action]")]
        public IActionResult Login() => View();

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                string password = HashProvider.Sha256($"{model.Username}.{model.Password}");

                AccountModel account = options.Accounts.FirstOrDefault(a => a.Username == model.Username && a.Password == password);
                if (account == null)
                {
                    ModelState.AddModelError(nameof(LoginModel.Username), "No such combination of the username and the password.");
                    return View();
                }

                ClaimsPrincipal principal = CreatePrincipal(account);
                AuthenticationProperties authProperties = CreateAuthenticationProperties();
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authProperties
                ).ConfigureAwait(false);

                return RedirectTo();
            }

            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectTo();
        }

        private static AuthenticationProperties CreateAuthenticationProperties()
        {
            return new AuthenticationProperties()
            {
                AllowRefresh = true
            };
        }

        private static ClaimsPrincipal CreatePrincipal(AccountModel account)
        {
            var claims = new List<Claim>(1);

            claims.Add(new Claim(ClaimTypes.Name, account.Username));

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return new ClaimsPrincipal(claimsIdentity);
        }

        private IActionResult RedirectTo()
        {
            string returnUrl = HttpContext.Request.Form[CookieAuthenticationDefaults.ReturnUrlParameter];
            if (!String.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction(nameof(MainController.Index), "Main");
        }
    }
}
