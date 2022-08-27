using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BugTrackerPetProj.Interfaces;
using BugTrackerPetProj.Models;
using BugTrackerPetProj.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BugTrackerPetProj.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;

        private readonly SignInManager<User> _signInManager;

        private readonly IRepository _repository;

        private readonly IEmailService _emailService;

        private readonly IEncryptionService _encryptionService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IRepository repository, IEmailService emailService,
                                 IEncryptionService encryptionService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _repository = repository;
            _emailService = emailService;
            _encryptionService = encryptionService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string? returnUrl)
        {
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                model.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid Email. Please check if you use correct email adress and try again.");
                    return View(model);
                }

                var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, model.Password);

                if (isPasswordCorrect)
                {
                    //var result = await _signInManager.SignInAsync(user, isPersistent:model.RememberMe);
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
                        else return RedirectToAction("Index", "Home", new { id = user.Id });
                    }

                    ViewBag.Title = "Invalid sign in attempt";
                    ViewBag.ErrorMessage = "There were some problems when trying to login, please try again. " +
                        "If the problem persists, please contact us at this email address voitenkodevpost@gmail.com";
                    return View("Error");
                    
                }

                ModelState.AddModelError(string.Empty, "Wrong Password");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt");
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registration(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    UserName = $"{model.FirstName}" + $"{model.LastName}",
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    //var confirmationLink = Url.Action("ConfirmEmail", "Account",
                    //    new { userId = user.Id, token = token }, Request.Scheme);


                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home", new { id = user.Id });

                    //ViewBag.ErrorTitle = "Registration successful";
                    //ViewBag.ErrorMessage = "Before you can Login, please confirm your " +
                    //    "email by clicking on the confirmation link we have emailed you";
                    //return View("Error");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null && token == null) return RedirectToAction("index", "home");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"The user ID {userId} is invalid";
                return View("NotFound");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded) return View();

            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null) return Json(true);
            else return Json($"Email {email} is already in use");
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                              new { ReturnUrl = returnUrl }, Request.Scheme);
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login", model);
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Error loading external login information");
                return View("Login", model);
            }
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            User user = null;

            if (email != null) user = await _userManager.FindByEmailAsync(email);

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                                info.ProviderKey, isPersistent: false, bypassTwoFactor: false);

            if (signInResult.Succeeded) return LocalRedirect(returnUrl);

            return View("Login", model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = _repository.GetUserByEmail(model.Email);

            if(user != null)
            {
                var userId = _encryptionService.Encrypt(user.Id);
                var message = _emailService.GeneratePasswordResetMail(user.Email, userId, user.UserName);
                _emailService.Send(message);
                return View("Success");
            }

            ViewBag.Title = "User not found";
            return View("Error");
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult ChangePassword(string encryptedUserId)
        {
            var userId = _encryptionService.Decrypt(encryptedUserId);
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            model.Id = userId;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                var user = _repository.GetUserById(model.Id);

                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                //var test = await _userManager.SetTwoFactorEnabledAsync(user, true);
                //var flag = await _userManager.GetTwoFactorEnabledAsync(user);
                //var provider = await _userManager.GetValidTwoFactorProvidersAsync(user);
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("Error", model);
                }

                return RedirectToAction("Login", "Account");
            }
            return View(model);
        }

    }
}

