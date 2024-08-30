using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace UnionSurvey.Controllers
{
    [Authorize(Roles = "User")]
    public class BaseController(ILogger logger)
                     : Controller
    {
        internal readonly ILogger _logger = logger;
        internal string LoginUserName => User?.Identity?.Name!;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Now the User property is available, and you can set ViewData here
            ViewData["USNAME"] = LoginUserName;
            ViewData["USUSERNAME"] = LoginUserName;


            ViewData["UserId"] = LoginUserName;
            ViewData["UserName"] = LoginUserName;
        }
    }
}
