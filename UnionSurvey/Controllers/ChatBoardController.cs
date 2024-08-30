using Microsoft.AspNetCore.Mvc;
using UnionSurvey.Data.Models;
using UnionSurvey.Model;
using YM.Common;

namespace UnionSurvey.Controllers
{
    public class ChatBoardController : Controller
    {
        private readonly SurveyUnionContext _context;

        public ChatBoardController(SurveyUnionContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var isAgent = HttpContext.Session.GetString("IsAgent")?.ToSafeBool() ?? false;

            if (isAgent)
            {
                var agentId = User.Identity.Name; // HttpContext.Session.GetString("AgentId") ?? "";

                // Retrieve the related userId and username
                var userInfo = _context.ChatCommunications
                                       .Where(cc => cc.AgentName == agentId)
                                       .Select(cc => new { cc.UserName }) // Assuming User entity has a UserName property
                                       .FirstOrDefault();

                if (userInfo != null)
                {
                    var userId = userInfo?.UserName ?? "";
                    var userName = _context.Users.Where(u => u.UserName == userInfo.UserName)
                                                .Select(u => u.UserName).FirstOrDefault();

                    var agentName = _context.AppUsers
                                            .Where(a => a.UserName == agentId)
                                            .Select(a => a.UserName) // Assuming Agent entity has an AgentName property
                                            .FirstOrDefault();

                    // Pass the agentId, userId, userName, and agentName to the view
                    ViewData["AgentId"] = agentId;
                    ViewData["UserId"] = userId;
                    ViewData["UserName"] = userName;
                    ViewData["AgentName"] = agentName;
                }

                return View("AgentChatBoard"); // Render agent chat board
            }
            else
            {
                var userIds = User.Identity.Name; //HttpContext.Session.GetString("UserId") ?? "";

                var userName = _context.AppUsers
                                       .Where(u => u.UserName == userIds)
                                       .Select(u => u.UserName) // Assuming User entity has a UserName property
                                       .FirstOrDefault();

                // Pass the userId and userName to the view
                ViewData["UserId"] = userName;
                ViewData["UserName"] = userName;
                return View("UserChatBoard"); // Render user chat board
            }
        }

        public IActionResult GetUserListForAgent(string agentId)
        {
            var users = _context.ChatCommunications
                                .Where(cc => cc.AgentName == User.Identity.Name)
                                .Select(cc => new ChatUserModel
                                {
                                   UserId = cc.UserName,
                                   Username = cc.UserName
                                })
                                .Distinct() // Ensure the list is unique
                                .ToArray();

            return Json(users);
        }
        public IActionResult AgentChatBoard()
        {
            var agentId = User.Identity?.Name;//HttpContext.Session.GetString("AgentId") ?? "";

            if (agentId == "")
            {
                return RedirectToAction("Login", "Account");
            }

            // Get the list of users the agent has communicated with
            var users = _context.ChatCommunications
                                .Where(cc => cc.AgentName == agentId)
                                .Select(cc => new
                                {
                                    //cc.UserId,
                                    UserName = _context.AppUsers
                                                       .Where(u => u.UserName == cc.UserName)
                                                       .Select(u => u.UserName)
                                                       .FirstOrDefault()
                                })
                                .Distinct() // Ensure no duplicate users
                                .ToList();

            if (users.Count == 0)
            {
                // If no users are found, log or debug
                Console.WriteLine("No users found for agentId: " + agentId);
            }

            // Retrieve the agent's name
            var agentName = _context.AppUsers
                                    .Where(a => a.UserName == agentId)
                                    .Select(a => a.UserName)
                                    .FirstOrDefault();

            // Pass the agentId, agentName, and user list to the view
            ViewData["AgentId"] = agentId;
            ViewData["AgentName"] = agentName;
            ViewData["Users"] = users;

            return View(); // Render the agent chat board view
        }



        public IActionResult UserChatBoard()
        {
            // Retrieve UserId from session

            return View(); // Render agent chat board

        }
    }
}
