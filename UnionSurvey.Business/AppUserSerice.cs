using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UnionSurvey.Business.Common;
using UnionSurvey.Business.Services;
using UnionSurvey.Data.IRepository;
using UnionSurvey.Data.Models;
using UnionSurvey.Model;
using YM.Common;

namespace UnionSurvey.Business
{
    public class AppUserSerice(IAppUserRepo userRepo
                            , UserManager<IdentityUser> userManager)
                                : IAppUserSerice
    {

        public async Task<UserProfileModel> GetByIdAsync(ClaimsPrincipal uPrinciple)
        {
            var dbUser = await userManager.GetUserAsync(uPrinciple);
            if (dbUser == null)
                return null!;

            UserProfileModel user = new()
            {
                Id = dbUser.Id,
                UserName = dbUser.UserName!,
                Email = dbUser.Email,
                PhoneNumber = dbUser.PhoneNumber,
            };

            var appUser = await userRepo.GetByIdAsync(dbUser.UserName!);
            if (appUser != null)
            {
                user.FirstName = appUser.FirstName;
                user.LastName = appUser.LastName;
                user.Gender = appUser.Gender;
                user.BrithDate = appUser.BrithDate;
                user.PostalCode = appUser.PostalCode;
                user.Country = appUser.Country;
                user.Address = appUser.Address;
                user.UserCode = appUser.UserCode;
                user.ReferalCode = appUser.ReferalCode;
                user.IsAgent = appUser.IsAgent ?? false;
            }

            return user;

        }
        public async Task SaveAsync(UserProfileModel user, string userName)
        {
            bool doAdd = false;
            AppUser dbUser = await userRepo.GetByIdAsync(userName);

            IdentityUser? identityUser = await userManager.FindByNameAsync(userName);

            identityUser!.Email = user.Email;
            identityUser!.PhoneNumber = user.PhoneNumber;

            if (dbUser == null)
            {
                dbUser = new AppUser
                {
                    Id = identityUser.Id!,
                    UserName = identityUser.UserName!,
                };
                doAdd = true;
            }
            dbUser.FirstName = user.FirstName;
            dbUser.LastName = user.LastName;
            dbUser.Gender = user.Gender;
            dbUser.IsActive = true;
            dbUser.IsDelete = false;
            dbUser.PostalCode = user.PostalCode;
            dbUser.Country = user.Country;
            dbUser.Address = user.Address;
            dbUser.BrithDate = user.BrithDate;
            dbUser.ModifiedBy = userName;
            dbUser.ModifiedDate = DateTime.UtcNow;

            await userManager.UpdateAsync(identityUser);

            await (doAdd
                     ? userRepo.AddAsync(dbUser)
                     : userRepo.UpdateAsync(dbUser));
        }
        public async Task SaveAsync(AppUser user, string userName)
        {
            await (userName.ToHasValue()
                     ? userRepo.UpdateAsync(user)
                     : userRepo.AddAsync(user));
        }
        public async Task<IList<SP_UserFinancialStatus>> GetAllFromProc()
                                              => await userRepo.GetAllFromProc();

        public async Task<UserFinancial> GetUserFinancial(string userName)
                                        => await userRepo.GetUserFinancial(userName);

        public async Task<IList<SP_UserHierarchy>> SP_GetActiveUserHierarchyAsync(string userId)
                                        => await userRepo.SP_GetActiveUserHierarchyAsync(userId);
        public async Task<UserHierarchyModel> GetUserHierarchyAsync(ClaimsPrincipal uPrinciple, DateTime? fromDate, DateTime? toDate)
        {
            var dbUser = await userManager.GetUserAsync(uPrinciple);
            if (dbUser == null)
                return default!;

            var users = await userRepo.SP_GetUserHierarchyAsync(dbUser.Id, fromDate, toDate);
            if(users == null) return default!;

            IList<UserHierarchyModel> usersHierarchies = users
                                                            .Select(i => i.ToUserHierarchyModel())
                                                            .ToList();

            UserHierarchyModel userHierarchy = new()
            {
                TeamBalance = users.Sum(i => i.TeamBalance),
                DirectPromoters = users.Sum(i => i.DirectPromoters),
                TotalSize = users.Sum(i => i.TotalSize),
                DepositBalance = users.Sum(i => i.DepositBalance),
                NewlyIncreased = users.Sum(i => i.NewlyIncreased),
                TodaySurveyCompletedUsers = users.Sum(i => i.TodaySurveyCompletedUsers),

                Users = usersHierarchies,
            };

            return userHierarchy;
        }
    
    }
}
