using Microsoft.AspNetCore.Mvc;
using UnionSurvey.Business.Services;
using UnionSurvey.Data;

namespace UnionSurvey.Controllers
{
    public class DashboardController(IFinancialService financialService
                                    , ILogger<DashboardController> logger)
                    : AdminController(logger)
    {
        public async Task<IActionResult> Index()
        {
           SP_AdminDashboardStat model = new SP_AdminDashboardStat();

            return View(model);
        }
    }
}
