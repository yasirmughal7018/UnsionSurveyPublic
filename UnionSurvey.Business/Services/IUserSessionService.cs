using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionSurvey.Business.Services
{
    public interface IUserSessionService
    {
        Task<bool> IsSessionValidAsync(string userId, string sessionId);
        Task CreateSessionAsync(string userId, string sessionId, DateTime expirationTime);
        Task InvalidateAllUserSessionsAsync(string userId);
        Task InvalidateSessionAsync(string sessionId);
    }
}
