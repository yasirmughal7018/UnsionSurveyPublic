using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnionSurvey.Data.Models;
using UnionSurvey.Model;

namespace UnionSurvey.Data.IRepository
{
    public interface IAppUserRepo : IRepository<AppUser>
    {
        Task<IList<SP_UserFinancialStatus>> GetAllFromProc();
        Task<UserFinancial> GetUserFinancial(string userName);

        Task<IList<SP_UserHierarchy>> SP_GetActiveUserHierarchyAsync(string userId);
        Task<IList<SP_UserHierarchyWithStats>> SP_GetUserHierarchyAsync(string userId, DateTime? fromDate, DateTime? toDate);
        Task<IList<SP_UserHierarchy>> SP_GetUserHierarchyChildToParentAsync(string userName);

        Task<bool> IsSessionValidAsync(string userName, string sessionId);
        Task CreateSessionAsync(UserSession userSession);
        Task InvalidateAllUserSessionsAsync(string userName);
        Task InvalidateSessionAsync(string sessionId);
    }

}
