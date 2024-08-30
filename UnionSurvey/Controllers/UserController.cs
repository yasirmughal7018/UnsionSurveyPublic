using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using UnionSurvey.Business.Services;
using UnionSurvey.Data;
using UnionSurvey.Model;
using YM.Common;

namespace UnionSurvey.Controllers
{
    public class UserController(IAppUserSerice userSerice
                                , IFinancialService financialService
                                , UserManager<IdentityUser> userManager
                                , ILogger<AccountController> logger)
                    : AdminController(logger)
    {
        public async Task<IActionResult> Index()
        {
            var users = await userSerice.GetAllFromProc();
            return View(users);
        }

        public async Task<IActionResult> Profile()
        {

            var users = await userSerice.GetByIdAsync(User);
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserProfileModel model)
        {
            if (ModelState.IsValid)
            {
                await userSerice.SaveAsync(model, LoginUserName);
            }

            return View(model);
        }

        public async Task<IActionResult> Details(string userName)
        {
            var user = await userManager.FindByNameAsync(userName);

            UserTransactionDetailModel model = new UserTransactionDetailModel();

            model.UserFinancial = await userSerice.GetUserFinancial(userName);

            var activeUsers = (await userSerice.SP_GetActiveUserHierarchyAsync(user!.Id))
                        .Count(i => i.Level == 2);
            model.UserFinancial.ActiveUsers = activeUsers;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdjustmentAmount(string username, int amount)
        {
            if (username.ToHasValue() && amount > 0)
            {
                return Json(new { success = true, message = "Amount is adjusted.", Data = 0 });
            }

            return Json(new { success = true, message = "Error" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateUser(string username, int tid)
        {
            if (username.ToHasValue() && tid > 0)
            {
                return Json(new { success = true, message = "MANUAL FAILED", Data = 0 });
            }

            return Json(new { success = true, message = "Error" });
        }
    }
}
