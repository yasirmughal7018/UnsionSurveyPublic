using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using UnionSurvey.Common;
using UnionSurvey.Data.IRepository;
using UnionSurvey.Data.Models;
using UnionSurvey.Model;
using YM.Common;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace UnionSurvey.Data.Repository
{
    public class AppUserRepo(SurveyUnionContext context)
                    : Repository<AppUser>(context), IAppUserRepo
    {

        public async new Task AddAsync(AppUser user)
        {
            user.IsActive = false;
            user.IsDelete = false;
            user.UserCode = user.UserName.ToSafeUniqueCode();
            user.CreatedBy = user.UserName;
            user.CreatedDate = DateTime.UtcNow;
            user.ReferalCode = (user.ReferalCode ?? "").ToHasValue()
                                                ? user.ReferalCode : Constant.US_ADMIN_REFERAL_CODE;

            user.UserNameNavigation = new UserFinancial { UserName = user.UserName };

            await base.AddAsync(user);

        }

        public async Task<IList<SP_UserFinancialStatus>> GetAllFromProc()
        {
            IList<SP_UserFinancialStatus> users = [];

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SP_UserFinancialStatus";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                try
                {
                    _context.Database.OpenConnection();
                    using (var result = await command.ExecuteReaderAsync())
                    {
                        while (await result.ReadAsync())
                        {
                            var item = new SP_UserFinancialStatus()
                            {
                                Id = result.GetFieldValue<string>(result.GetOrdinal("Id")),
                                UserName = result.GetFieldValue<string>(result.GetOrdinal("UserName")),
                                FirstName = result.GetFieldValue<string>(result.GetOrdinal("FirstName")),
                                LastName = result.GetFieldValue<string>(result.GetOrdinal("LastName")),
                                UserCode = result.GetFieldValue<string>(result.GetOrdinal("UserCode")),
                                ReferalCode = result.GetFieldValue<string>(result.GetOrdinal("ReferalCode")),
                                Email = result.GetFieldValue<string>(result.GetOrdinal("Email")),
                                PhoneNumber = result.GetFieldValue<string>(result.GetOrdinal("PhoneNumber")),
                                UserBalance = result.GetFieldValue<decimal>(result.GetOrdinal("UserBalance")),
                                TeamComission = result.GetFieldValue<decimal>(result.GetOrdinal("TeamComission")),
                                TodayIncome = result.GetFieldValue<decimal>(result.GetOrdinal("TodayIncome")),
                                TransactionDate = result.IsDBNull(result.GetOrdinal("TransactionDate"))
                                                    ? (DateOnly?)null
                                                    : result.GetFieldValue<DateOnly>(result.GetOrdinal("TransactionDate")),
                                TransactionStatus = result.IsDBNull(result.GetOrdinal("TransactionStatus"))
                                                    ? (string?)null
                                                    : result.GetFieldValue<string>(result.GetOrdinal("TransactionStatus")),
                                TransactionId = result.IsDBNull(result.GetOrdinal("TransactionId"))
                                                    ? (int?)null
                                                    : result.GetFieldValue<int>(result.GetOrdinal("TransactionId")),
                                CreatedDate = result.GetFieldValue<DateTime>(result.GetOrdinal("CreatedDate")),
                                ModifiedDate = result.IsDBNull(result.GetOrdinal("ModifiedDate"))
                                                    ? (DateTime?)null
                                                    : result.GetFieldValue<DateTime>(result.GetOrdinal("ModifiedDate"))
                            };

                            users.Add(item);
                        }
                    }
                }
                catch (Exception ex) { throw ex; }
                finally { await _context.Database.CloseConnectionAsync(); }
            }

            return users;
        }

        public async Task<UserFinancial> GetUserFinancial(string userName)
                    => await _context.UserFinancials.FindAsync(userName) ?? new UserFinancial();

        public async Task<IList<SP_UserHierarchy>> SP_GetActiveUserHierarchyAsync(string userId)
        {
            IList<SP_UserHierarchy> users = [];

            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {
                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SP_ActiveUserHierarchy";
                        command.CommandType = CommandType.StoredProcedure;

                        SqlParameter p_UserId = new("@UserId", userId);

                        command.Parameters.Add(p_UserId);

                        using (var result = await command.ExecuteReaderAsync())
                        {
                            while (await result.ReadAsync())
                            {
                                var item = new SP_UserHierarchy()
                                {
                                    Id = result.GetFieldValue<string>(result.GetOrdinal("Id")),
                                    UserName = result.GetFieldValue<string>(result.GetOrdinal("UserName")),
                                    FirstName = result.GetFieldValue<string>(result.GetOrdinal("FirstName")),
                                    LastName = result.GetFieldValue<string>(result.GetOrdinal("LastName")),
                                    UserCode = result.GetFieldValue<string>(result.GetOrdinal("UserCode")),
                                    ReferalCode = result.GetFieldValue<string>(result.GetOrdinal("ReferalCode")),
                                    Level = result.GetFieldValue<int>(result.GetOrdinal("Level")),
                                    UserBalance = result.GetFieldValue<decimal>(result.GetOrdinal("UserBalance")),
                                    TodayIncome = result.GetFieldValue<decimal>(result.GetOrdinal("TodayIncome")),
                                    TeamComission = result.GetFieldValue<decimal>(result.GetOrdinal("TeamComission")),
                                    LastSurveyDate = result.IsDBNull(result.GetOrdinal("LastSurveyDate"))
                                                    ? null
                                                    : result.GetFieldValue<DateTime>(result.GetOrdinal("LastSurveyDate"))
                                };

                                users.Add(item);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new InvalidOperationException("An error occurred while accessing the database.", ex);
            }

            return users;
        }

        public async Task<IList<SP_UserHierarchyWithStats>> SP_GetUserHierarchyAsync(string userId, DateTime? fromDate, DateTime? toDate)
        {
            IList<SP_UserHierarchyWithStats> users = [];

            try
            {
                using (var connection = _context.Database.GetDbConnection())
                {

                    await connection.OpenAsync();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "SP_UserHierarchyWithStats";
                        command.CommandType = CommandType.StoredProcedure;

                        SqlParameter p_UserId = new("@UserId", userId);
                        SqlParameter p_FromDate = new("@FromDate", fromDate.ToSafeDbValue());
                        SqlParameter p_ToDate = new("@ToDate", toDate.ToSafeDbValue());

                        command.Parameters.AddRange(new[] { p_UserId, p_FromDate, p_ToDate });

                        using (var result = await command.ExecuteReaderAsync())
                        {
                            while (await result.ReadAsync())
                            {
                                var item = new SP_UserHierarchyWithStats()
                                {
                                    Id = result.GetFieldValue<string>(result.GetOrdinal("Id")),
                                    UserName = result.GetFieldValue<string>(result.GetOrdinal("UserName")),
                                    FirstName = result.GetFieldValue<string>(result.GetOrdinal("FirstName")),
                                    LastName = result.GetFieldValue<string>(result.GetOrdinal("LastName")),
                                    UserCode = result.GetFieldValue<string>(result.GetOrdinal("UserCode")),
                                    ReferalCode = result.GetFieldValue<string>(result.GetOrdinal("ReferalCode")),
                                    Level = result.GetFieldValue<int>(result.GetOrdinal("Level")),
                                    UserBalance = result.GetFieldValue<decimal>(result.GetOrdinal("UserBalance")),
                                    TodayIncome = result.GetFieldValue<decimal>(result.GetOrdinal("TodayIncome")),
                                    TeamComission = result.GetFieldValue<decimal>(result.GetOrdinal("TeamComission")),
                                    LastSurveyDate = result.IsDBNull(result.GetOrdinal("LastSurveyDate"))
                                                    ? null
                                                    : result.GetFieldValue<DateTime>(result.GetOrdinal("LastSurveyDate")),
                                    TeamBalance = result.GetFieldValue<decimal>(result.GetOrdinal("TeamBalance")),
                                    DirectPromoters = result.GetFieldValue<int>(result.GetOrdinal("DirectPromoters")),
                                    TotalSize = result.GetFieldValue<int>(result.GetOrdinal("TotalSize")),
                                    DepositBalance = result.GetFieldValue<decimal>(result.GetOrdinal("DepositBalance")),
                                    NewlyIncreased = result.GetFieldValue<int>(result.GetOrdinal("NewlyIncreased")),
                                    TodaySurveyCompletedUsers = result.GetFieldValue<int>(result.GetOrdinal("TodaySurveyCompletedUsers")),

                                };

                                users.Add(item);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new InvalidOperationException("An error occurred while accessing the database.", ex);
            }

            return users;
        }

        public async Task<IList<SP_UserHierarchy>> SP_GetUserHierarchyChildToParentAsync(string userName)
        {

            SqlParameter p_UserName = new("@UserName", userName);
            var users = await _context.SP_UserHierarchyWithStats
                        .FromSqlRaw("EXEC SP_UserHierarchyChildToParent @UserName", p_UserName)
                        .ToListAsync();


            return users;

            //IList<SP_UserHierarchyWithStats> users = [];

            //try
            //{
            //    using (var connection = _context.Database.GetDbConnection())
            //    {

            //        await connection.OpenAsync();

            //        using (var command = connection.CreateCommand())
            //        {
            //            command.CommandText = "SP_UserHierarchyChildToParent";
            //            command.CommandType = CommandType.StoredProcedure;

            //            SqlParameter p_UserName = new("@UserName", userName);
            //            command.Parameters.Add(p_UserName);

            //            using (var result = await command.ExecuteReaderAsync())
            //            {
            //                while (await result.ReadAsync())
            //                {
            //                    var item = new SP_UserHierarchyWithStats()
            //                    {
            //                        Id = result.GetFieldValue<string>(result.GetOrdinal("Id")),
            //                        UserName = result.GetFieldValue<string>(result.GetOrdinal("UserName")),
            //                        FirstName = result.GetFieldValue<string>(result.GetOrdinal("FirstName")),
            //                        LastName = result.GetFieldValue<string>(result.GetOrdinal("LastName")),
            //                        UserCode = result.GetFieldValue<string>(result.GetOrdinal("UserCode")),
            //                        ReferalCode = result.GetFieldValue<string>(result.GetOrdinal("ReferalCode")),
            //                        Level = result.GetFieldValue<int>(result.GetOrdinal("Level")),
            //                        UserBalance = result.GetFieldValue<decimal>(result.GetOrdinal("UserBalance")),
            //                        TodayIncome = result.GetFieldValue<decimal>(result.GetOrdinal("TodayIncome")),
            //                        TeamComission = result.GetFieldValue<decimal>(result.GetOrdinal("TeamComission")),
            //                        LastSurvey = result.IsDBNull(result.GetOrdinal("LastSurveyDate"))
            //                                        ? (DateOnly?)null
            //                                        : DateOnly.FromDateTime(result.GetFieldValue<DateTime>(result.GetOrdinal("LastSurveyDate")))

            //                    };

            //                    users.Add(item);
            //                }
            //            }
            //        }
            //    }

            //}
            //catch (Exception ex)
            //{
            //    // Log or handle the exception
            //    throw new InvalidOperationException("An error occurred while accessing the database.", ex);
            //}

            //return users;
        }


        public async Task<bool> IsSessionValidAsync(string userName, string sessionId)
        {
            return await _context.UserSessions
                .AnyAsync(s => s.UserName == userName
                        && s.SessionId == sessionId
                        && s.IsActive
                        && s.ExpirationTime > DateTime.UtcNow);
        }

        public async Task CreateSessionAsync(UserSession userSession)
        {
            _context.UserSessions.Add(userSession);
            await _context.SaveChangesAsync();
        }

        public async Task InvalidateAllUserSessionsAsync(string userName)
        {
            try
            {
                var sessions = await _context.UserSessions
                                            .Where(s => s.UserName == userName && s.IsActive)
                                            .ToListAsync();

                foreach (var session in sessions)
                {
                    session.IsActive = false;
                }

                await _context.SaveChangesAsync();
            }
            catch (Exception ex) { throw ex; }
        }

        public async Task InvalidateSessionAsync(string sessionId)
        {
            var session = await _context.UserSessions
                                        .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session != null)
            {
                session.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

    }
}
