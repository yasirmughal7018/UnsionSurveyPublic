using Nethereum.Contracts;
using Nethereum.JsonRpc.Client;
using Microsoft.AspNetCore.SignalR;
using UnionSurvey.Data.Models;
using Microsoft.EntityFrameworkCore;
using UnionSurvey.Model;

namespace UnionSurvey.Hubs
{
    public class ChatHub : Hub
    {
        private readonly SurveyUnionContext _context;

        public ChatHub(SurveyUnionContext context)
        {
            _context = context;
        }

        public async Task SendMessageToAgent(string userId, string message)
        {
            //int parsedUserId = int.Parse(userId);

            // Check if the user has a previous chat history
            var lastChat = await _context.ChatCommunications
                                          .Where(cc => cc.UserName == userId)
                                          .OrderByDescending(cc => cc.TimeStamp)
                                          .FirstOrDefaultAsync();

            AgentModel agent = null;

            if (lastChat != null)
            {
                // Retrieve the last agent the user interacted with
                var previousAgent = await _context.AppUsers
                                                  .Where(a => a.UserName == lastChat.AgentName)
                                                  .FirstOrDefaultAsync();


                // Check if the previous agent is available
                if (previousAgent != null && IsAgentAvailable(previousAgent.UserName))
                {
                    agent = new AgentModel
                    {
                        AgentName = previousAgent.UserName,
                        AgentStatus = previousAgent.AgentStatus ?? false,
                    };
                }
            }

            // If no previous agent or the previous agent is unavailable, assign a new agent
            if (agent == null)
            {
                var agentUser = await _context.AppUsers.FirstOrDefaultAsync(a => a.AgentStatus == true);
                if (agentUser != null)
                {
                    agent = new AgentModel
                    {
                        AgentName = agentUser.UserName,
                        AgentStatus = agentUser.AgentStatus ?? false,
                    };
                }

            }

            if (agent != null)
            {
                // Set agent status to busy
                await SetAgentStatus(agent.AgentName, false);

                var chat = new ChatCommunication
                {
                    UserName = userId,
                    AgentName = agent.AgentName,
                    Message = message,
                    IsFromAgent = false,
                    TimeStamp = DateTime.Now
                };

                _context.ChatCommunications.Add(chat);
                await _context.SaveChangesAsync();


                // Notify the agent
                await Clients.All.SendAsync("ReceiveMessage", userId, message, false);
            }
        }

        private bool IsAgentAvailable(string agentId)
        {
            var agent = _context.AppUsers
                                .Where(a => a.UserName == agentId && a.AgentStatus == true)
                                .FirstOrDefault();

            return agent != null;
        }

        public async Task SetAgentStatus(string agentId, bool status)
        {
            var agent = await _context.AppUsers
                                      .Where(a => a.UserName == agentId)
                                      .FirstOrDefaultAsync();

            if (agent != null)
            {
                agent.AgentStatus = status;
                await _context.SaveChangesAsync();
            }
        }


        public async Task SendMessageToUser(string agentId, string userId, string message)
        {
            var chat = new ChatCommunication
            {
                UserName = userId,
                AgentName = agentId,
                Message = message,
                IsFromAgent = true,
                TimeStamp = DateTime.UtcNow
            };

            _context.ChatCommunications.Add(chat);
            await _context.SaveChangesAsync();

            // Notify the user
            await Clients.All.SendAsync("ReceiveMessage", agentId, message, true);

        }
        public async Task LoadChatHistory(string userId, string? agentId)
        {
            var chatHistory = await (from cc in _context.ChatCommunications
                                     join u in _context.AppUsers on cc.UserName equals u.UserName
                                     join a in _context.AppUsers on cc.AgentName equals a.UserName
                                     where cc.UserName == userId 
                                            && cc.AgentName == agentId
                                            && cc.TimeStamp.Date == DateTime.UtcNow.Date 
                                     orderby cc.TimeStamp
                                     select new
                                     {
                                         cc.Id,
                                         cc.Message,
                                         cc.IsFromAgent,
                                         UserName = u.UserName,
                                         AgentName = a.UserName
                                     })
                                     .OrderBy(i => i.Id)
                                     .ToListAsync();

            await Clients.Caller.SendAsync("ReceiveChatHistory", chatHistory);
        }

    }
}
