using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Tazkarti.Models;
using System.Threading.Tasks;
using Tazkarti.Helpers;
namespace Tazkarti.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;


        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            
        }

        public IActionResult SignUp()
        {
            return View(new SignUpViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel input)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    Email = input.Email,
                    UserName = input.Email.Split('@')[0],

                    
                };

                var result = await _userManager.CreateAsync(user, input.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(input);
        }
        public IActionResult Login()
        {
            return View(new SignInViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(SignInViewModel input)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(input.Email);
                if (user is null)
                {
                    ModelState.AddModelError("", "Email does not exist");
                    return View(input);
                }

                var result = await _signInManager.PasswordSignInAsync(user, input.Password, input.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid login attempt");
                    return View(input);
                }
            }

            return View(input);
        }
        public IActionResult ForgetPassword()
        {
            return View(new ForgetPasswordViewModel());
        }


        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel input)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(input.Email);
                if (user is null)
                {
                    ModelState.AddModelError("", "Email does not exist");
                    return View(input);
                }

                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetPasswordLink = Url.Action("ResetPassword", "Account", new { Email = input.Email, Token = token }, Request.Scheme);
                    var email = new Email
                    {
                        Title = "Reset Password",
                        Body = resetPasswordLink,
                        To = input.Email
                    };
                    EmailSettings.SendEmail(email);
                    return RedirectToAction("CompleteForgetPassword");
                }
            }

            return View(input);
        }
        public IActionResult CompleteForgetPassword()
        {
            return View();
        }

        public IActionResult ResetPassword(string email, string token)
        {
            return View(new ResetPasswordViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel input)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(input.Email);

                if (user is null)
                {
                    ModelState.AddModelError("", "Email does not exist");

                }

                var result = await _userManager.ResetPasswordAsync(user, input.Token, input.Password);

                if (result.Succeeded)
                    return RedirectToAction("Login");

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    _logger.LogError(error.Description);
                }
            }

            return View(input);
        }
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
