using Microsoft.AspNetCore.Mvc;
using UnionSurvey.Business.Services;
using UnionSurvey.Model;

namespace UnionSurvey.Controllers
{
    public class MemberController(IAppUserSerice userSerice
                                , ILogger<MemberController> logger)
                    : BaseController(logger)
    {
        public async Task<IActionResult> Index()
        {
          var model = await  userSerice.GetUserHierarchyAsync(User, null, null);
            return View(model);
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
    }
}
