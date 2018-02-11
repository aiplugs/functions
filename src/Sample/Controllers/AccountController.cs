using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sample.Models;

namespace Sample.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        const string SAMPLE_USER = "sample@local";
        const string SAMPLE_PASS = "P@ssw0rd";
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var signin = await _signInManager.PasswordSignInAsync(SAMPLE_USER, SAMPLE_PASS, false, lockoutOnFailure: false);
            if (signin.Succeeded == false)
            {
                var user = new ApplicationUser { UserName = SAMPLE_USER, Email = SAMPLE_USER };
                var signup = await _userManager.CreateAsync(user, SAMPLE_PASS);

                if (signup.Succeeded == false)
                    throw new InvalidOperationException("Cannot register sample user.");

                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            return RedirectToLocal(returnUrl);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}
