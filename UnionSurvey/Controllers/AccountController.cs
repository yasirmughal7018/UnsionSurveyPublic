using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UnionSurvey.Models;
using YM.Common;
using Microsoft.VisualBasic;
using UnionSurvey.Data.Models;
using UnionSurvey.Common;
using UnionSurvey.Business.Services;
using Nethereum.Contracts.QueryHandlers.MultiCall;
using Microsoft.AspNetCore.Authorization;
using UnionSurvey.Business;
using NBitcoin.Secp256k1;
using UnionSurvey.Services;

namespace UnionSurvey.Controllers
{
    [Authorize]
    public class AccountController(UserManager<IdentityUser> userManager
                                    , SignInManager<IdentityUser> signInManager
                                    , RoleManager<IdentityRole> roleManager
                                    , IUserSessionService userSessionService
                                    , SensitiveDataService sensitiveDataService
                                    , IAppUserSerice userService)
                    : Controller
    {
        [AllowAnonymous]
        public IActionResult Login()
        {

            ViewData["UserId"] = "walking_user";

            //var sensitiveData = "This is my secret data";
            //var encryptedData = sensitiveDataService.Protect(sensitiveData);


            //var uncrypted = sensitiveDataService.Unprotect(encryptedData);

            return View(new LoginViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                string error = "Invalid login attempt.";
                if (ModelState.IsValid)
                {
                    error = "User doesn't exist";
                    var user = await userManager.FindByNameAsync(model.UserName);

                    if (user != null)
                    {
                        error = "Password is incorrect.";
                        var checkPassword = await userManager.CheckPasswordAsync(user, model.Password);

                        if (checkPassword)
                        {
                            // Sign out any existing sessions before creating a new one
                            await signInManager.SignOutAsync();

                            error = "username or password is not correct.";
                            var result = await signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                            if (result.Succeeded)
                            {

                                // Invalidate any existing sessions for the user
                                await userSessionService.InvalidateAllUserSessionsAsync(user.UserName!);

                                // Create a new session
                                //var sessionId = HttpContext.Session.Id;
                                var sessionId = Guid.NewGuid().ToString();
                                HttpContext.Session.SetString("SessionId", sessionId);

                                await userSessionService.CreateSessionAsync(user.UserName!, sessionId, DateTime.UtcNow.AddMinutes(30)); // Session expiration set to 30 minutes


                                var claims = new List<Claim>
                                                        {
                                                            new(ClaimTypes.NameIdentifier, user.Id),
                                                            new(ClaimTypes.Name, user.UserName!),
                                                            new("SessionId", sessionId)
                                                        };


                                //var getClaims = await userManager.GetClaimsAsync(user);

                                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                                                         new ClaimsPrincipal(claimsIdentity));

                                var appUser = await userService.GetByIdAsync(User);




                                if (User.IsInRole("Admin"))
                                    // Redirect to a protected page after successful login
                                    return RedirectToAction("Index", "Dashboard");
                                else if (appUser.IsAgent)
                                {
                                    ViewData["AgentId"] = appUser.UserName;
                                    ViewData["AgentName"] = appUser.UserName;

                                    return RedirectToAction("AgentChatBoard", "ChatBoard");
                                }
                                else
                                {
                                    //var userFinancial = await userServices.GetUserFinancial(User.Identity!.Name!);

                                    //if (userFinancial == null || userFinancial.AvailableBalance == 0)
                                    //    return RedirectToAction("Index", "Finance");
                                    //else
                                    return RedirectToAction("Index", "Home");
                                }
                            }
                        }
                    }

                }

                ModelState.AddModelError(string.Empty, error);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, ex.Message);
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var sessionId = HttpContext.Session.Id;
            await userSessionService.InvalidateSessionAsync(sessionId);

            // Clear the session
            HttpContext.Session.Clear();

            // Clear the cookies
            foreach (var cookie in HttpContext.Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }

        [AllowAnonymous]
        public IActionResult Register(string? id)
        {
            UserViewModel model = new();
            //if (id.ToHasValue())
            model.ReferalCode = id ?? Constant.US_ADMIN_REFERAL_CODE;

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = new IdentityUser { UserName = model.UserName };
                    var result = await userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        AppUser appUser = new()
                        {
                            Id = user.Id,
                            UserName = model.UserName,
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            ReferalCode = model.ReferalCode
                        };

                        await userService.SaveAsync(appUser, "");

                        // Ensure the "User" role exists
                        if (!await roleManager.RoleExistsAsync("User"))
                            await roleManager.CreateAsync(new IdentityRole("User"));

                        // Assign the "User" role to the user
                        await userManager.AddToRoleAsync(user, "User");

                        //await userServices.SaveUserFinancial(user.UserName);
                        //await signInManager.SignInAsync(user, isPersistent: false);

                        return RedirectToAction(nameof(Login));
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            return View(model);
        }
    }
}
