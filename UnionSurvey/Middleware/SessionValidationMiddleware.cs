using Azure;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using UnionSurvey.Business.Services;
using YM.Common;

namespace UnionSurvey.Middleware
{
    public class SessionValidationMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context, IUserSessionService userSessionService)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var userId = context.User.FindFirst(ClaimTypes.Name)?.Value ?? "";

                if (context.Session != null && userId.ToHasValue())
                {
                    //var sessionId = context.Session.Id;
                    var sessionId = context.Session.GetString("SessionId") ?? "";

                    bool isSessionValidate = await userSessionService.IsSessionValidAsync(userId, sessionId);

                    // Validate the session
                    if (sessionId.ToHasValue() == false || isSessionValidate == false)
                    {

                        // Clear the session
                        context.Session.Clear();

                        // Clear the cookies
                        foreach (var cookie in context.Request.Cookies.Keys)
                        {
                            context.Response.Cookies.Delete(cookie);
                        }

                        // If session is invalid, log the user out and redirect to login page
                        await context.SignOutAsync();
                        context.Response.Redirect("/Account/Login");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }

}
