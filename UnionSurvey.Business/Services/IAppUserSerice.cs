using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UnionSurvey.Data.Models;
using UnionSurvey.Model;

namespace UnionSurvey.Business.Services
{
    public interface IAppUserSerice
    {
        Task<UserProfileModel> GetByIdAsync(ClaimsPrincipal uPrinciple);
        Task SaveAsync(AppUser user, string userName);
        Task SaveAsync(UserProfileModel user, string userName);
        Task<IList<SP_UserFinancialStatus>> GetAllFromProc();
        Task<UserFinancial> GetUserFinancial(string userName);
        Task<UserHierarchyModel> GetUserHierarchyAsync(ClaimsPrincipal uPrinciple, DateTime? fromDate, DateTime? toDate);
        Task<IList<SP_UserHierarchy>> SP_GetActiveUserHierarchyAsync(string userId);
    }
}
