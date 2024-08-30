using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnionSurvey.Business.Services;
using UnionSurvey.Data.IRepository;
using UnionSurvey.Data.Models;

namespace UnionSurvey.Business
{
    public class UserSessionService(IAppUserRepo appUserRepo)
                              : IUserSessionService
    {
        public async Task<bool> IsSessionValidAsync(string userName, string sessionId)
                         => await appUserRepo.IsSessionValidAsync(userName, sessionId);

        public async Task CreateSessionAsync(string userName, string sessionId, DateTime expirationTime)
        {
            var session = new UserSession
            {
                UserName = userName,
                SessionId = sessionId,
                IsActive = true,
                LoginTime = DateTime.UtcNow,
                LastActivityTime = DateTime.UtcNow,
                ExpirationTime = expirationTime
            };

            await appUserRepo.CreateSessionAsync(session);
        }

        public async Task InvalidateAllUserSessionsAsync(string userName)
        {
            await appUserRepo.InvalidateAllUserSessionsAsync(userName);
        }

        public async Task InvalidateSessionAsync(string sessionId)
        {
            await appUserRepo.InvalidateSessionAsync(sessionId);
        }
    }

}
