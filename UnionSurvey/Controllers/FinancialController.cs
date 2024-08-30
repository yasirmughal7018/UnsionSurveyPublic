using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using UnionSurvey.Business.Services;
using UnionSurvey.Data;
using UnionSurvey.Data.Models;
using UnionSurvey.Data.Repository;
using UnionSurvey.Model;
using YM.Common;

namespace UnionSurvey.Controllers
{
    public class FinancialController(IFinancialService financialService
                                , IAppUserSerice userSerice
                                , UserManager<IdentityUser> userManager
                                , ILogger<FinancialController> logger)
                    : BaseController(logger)
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Logs()
        {
            var user = await userManager.FindByNameAsync(LoginUserName);
            if (user == null) { return RedirectToAction("Login", "Account"); }



            UserTransactionDetailModel model = new UserTransactionDetailModel();
            model.UserFinancial = await userSerice.GetUserFinancial(LoginUserName);

            var activeUsers = (await userSerice.SP_GetActiveUserHierarchyAsync(user.Id))
                        .Count(i => i.Level == 2);
            model.UserFinancial.ActiveUsers = activeUsers;

            return View(model);
        }
        public IActionResult Deposit() { TransactionModel mdoel = new TransactionModel(); return View(mdoel); }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> SaveDeposit(int amount)
        {

            return Json(new { success = true, message = "Error" });
        }

        [HttpPost]
        public async Task<IActionResult> SaveDepositWithApi(CryptoTransactionModel cryptoTrasction)
        {
            try
            {
                    return Json(new { success = true, message = "No found crypto transaction" });

            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        public async Task<IActionResult> Withdraw()
        {
            TransactionModel model =  new TransactionModel();
            return View(model);
        }
        public async Task<IActionResult> SaveWithDraw(int amount)
        {

            return Json(new { success = false, message = "Error" });
        }

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1); // Instance-level lock
        public async Task<IActionResult> ValidateSignature(int tnxId)
        {
            
                    return Json(new { success = true, message = "", Data = 0 });
        }

        [HttpPost]
        public async Task<IActionResult> SaveWithdrawWithApi(CryptoTransactionModel cryptoTrasction)
        {

                    return Json(new { success = true, message = "No found crypto transaction" });
        }
    }
}
