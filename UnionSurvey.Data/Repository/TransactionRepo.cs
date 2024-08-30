using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnionSurvey.Data.IRepository;
using UnionSurvey.Data.Models;
using UnionSurvey.Model;
using YM.Common;
using static UnionSurvey.Common.Constant;

namespace UnionSurvey.Data.Repository
{
    public class TransactionRepo(SurveyUnionContext context
                                , IAppUserRepo userRepo)
                    : Repository<Transaction>(context), ITransactionRepo
    {
        public async Task SaveUserSurvey(Transaction tnx)
        {
            try
            {
                var financial = await _context.UserFinancials
                                        .FirstOrDefaultAsync(i => i.UserName == tnx.TransAccountName)
                                 ?? throw new Exception("There is Financial found");

                SurveyUserMapping entity = new()
                {
                    SurveyId = tnx.SurveyId ?? 0,
                    UserName = tnx.TransAccountName,
                    IsSurveyCompleted = true,
                    CompletedDate = new DateTime(tnx.TransactionDate, tnx.TransactionTime)
                };

                bool isFirstSurvey = financial.LastSurveyDate == null;
                financial.UserBalance += tnx.Amount;
                financial.TodayIncome = tnx.Amount;
                financial.LastSurveyDate = entity.CompletedDate;

                IList<TransactionInternal> transactionInternals = [];

                //Commision Caluclate, if he completed his last survey.
                var parentUsers = (await userRepo.SP_GetUserHierarchyChildToParentAsync(tnx.TransAccountName))
                                    .Where(i => i.LastSurveyDate.HasValue == true)
                                    .ToList();

                foreach (var team in parentUsers)
                {
                    decimal percentage = team.Level switch
                    {
                        2 => 0.05m,   // 1% Commission
                        3 => 0.008m,  // 0.8% Commission
                        4 => 0.004m,  // 0.4% Commission
                        _ => 0
                    };

                    decimal amount = financial.TodayIncome * percentage;

                    TransactionInternal tnxinternal = new()
                    {
                        TransactionType = "COMMISSION",
                        TransactionDate = tnx.TransactionDate,
                        TransactionTime = tnx.TransactionTime,
                        RecordStatus = tnx.RecordStatus,
                        Amount = amount,
                        ComissionPercentage = percentage,
                        FromAvailableBalance = financial.UserBalance,
                        SurveyId = tnx.SurveyId!.Value,
                        FromUserName = tnx.TransAccountName,
                        ToUserName = team.UserName,
                        IsCommisionTransfered = false,
                        IsFirstSurveyCommision = isFirstSurvey
                    };
                    transactionInternals.Add(tnxinternal);
                }

                await _context.TransactionInternals.AddRangeAsync(transactionInternals);

                _context.UserFinancials.Update(financial);
                _context.SurveyUserMappings.Add(entity);
                await base.AddAsync(tnx);
            }
            catch (Exception ex) { throw ex; }
        }
        public async Task SaveStartTransaction(Transaction tnx)
        {
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
            TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.UtcNow);

            tnx.TransactionDate = currentDate;
            tnx.TransactionTime = currentTime;
            tnx.ModifiedDate = DateTime.UtcNow;
            tnx.ModifiedBy = tnx.TransAccountName;
            tnx.SurveyId = null;
            tnx.TransactionStatus = TransactionStatus.START.ToString();

            TransactionLog log = new()
            {
                LogDate = currentDate,
                LogTime = currentTime,
                TransactionStatus = TransactionStatus.START.ToString(),
                LogBy = tnx.ModifiedBy
            };

            tnx.TransactionLogs.Add(log);

            await base.AddAsync(tnx);
        }
        public async Task SaveCompletedTransaction(Transaction tnx, string requestFrom)
        {
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
            TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.UtcNow);

            tnx.ModifiedBy = tnx.TransAccountName;
            tnx.ModifiedDate = DateTime.UtcNow;

            TransactionLog log = new()
            {
                TransactionId = tnx.TransactionId,
                LogDate = currentDate,
                LogTime = currentTime,
                TransactionStatus = tnx.TransactionStatus,
                LogBy = tnx.ModifiedBy
                
            };

            if (tnx.TransactionStatus == TransactionStatus.COMPLETED.ToString())
            {
                UserFinancial? userFinancial = await _context.UserFinancials.FirstOrDefaultAsync(i => i.UserName == tnx.TransAccountName);

                if (userFinancial != null)
                {
                    if (requestFrom == "deposit")
                    {
                        userFinancial.UserBalance += tnx.Amount;
                        userFinancial.LastDepositDate = new DateTime(log.LogDate, log.LogTime);
                    }
                    else
                    {
                        userFinancial.UserBalance -= tnx.Amount;
                        userFinancial.LastDepositDate = new DateTime(log.LogDate, log.LogTime);
                    }

                    _context.UserFinancials.Update(userFinancial);
                }
            }

            tnx.TransactionLogs.Add(log);
            await base.UpdateAsync(tnx);
        }
        public async Task SaveTransactionAdjustment(int amount, string userName, string loggedInUser)
        {
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
            TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.UtcNow);

            Transaction tnx = new()
            {
                Description = "Adjustment",
                RecordStatus = "IN",
                Amount = amount,
                TransAccountName = userName,
                TransactionDate = currentDate,
                TransactionTime = currentTime,
                SurveyId = null,
                TransactionStatus = TransactionStatus.COMPLETED.ToString(),
                ModifiedBy = loggedInUser,
                ModifiedDate = DateTime.UtcNow,
            };

            TransactionLog log = new()
            {
                LogDate = currentDate,
                LogTime = currentTime,
                TransactionStatus = TransactionStatus.COMPLETED.ToString(),
                LogBy = tnx.ModifiedBy,
            };

            if (tnx.TransactionStatus == TransactionStatus.COMPLETED.ToString())
            {
                UserFinancial? userFinancial = await _context.UserFinancials.FirstOrDefaultAsync(i => i.UserName == tnx.TransAccountName);

                if (userFinancial != null)
                {
                    userFinancial.UserBalance += tnx.Amount;

                    _context.UserFinancials.Update(userFinancial);
                }
            }

            tnx.TransactionLogs.Add(log);

            await base.AddAsync(tnx);
        }
        public async Task<IList<SP_FinancialLog>> FinancialLogsAsync(string userName)
        {
            IList<SP_FinancialLog> financialLogs = [];

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SP_FinancialLogs";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter p_UserName = new SqlParameter("@UserName", userName.ToSafeDbValue());

                command.Parameters.Add(p_UserName);
                try
                {
                    _context.Database.OpenConnection();
                    using (var result = await command.ExecuteReaderAsync())
                    {
                        while (await result.ReadAsync())
                        {
                            var log = new SP_FinancialLog()
                            {
                                TransactionId = result.GetFieldValue<int>(result.GetOrdinal("TransactionId")),
                                Description = result.GetFieldValue<string>(result.GetOrdinal("Description")),
                                LogDate = result.GetFieldValue<DateOnly>(result.GetOrdinal("TransactionDate")),
                                LogTime = result.GetFieldValue<TimeOnly>(result.GetOrdinal("TransactionTime")),
                                TransactionStatus = result.GetFieldValue<string>(result.GetOrdinal("TransactionStatus")),
                                Amount = result.GetFieldValue<decimal>(result.GetOrdinal("Amount")),
                                TransAccountName = result.GetFieldValue<string>(result.GetOrdinal("TransAccountName")),
                                CryptoTransactionId = result.GetFieldValue<string>(result.GetOrdinal("CryptoTransactionId")),
                                CryptoTransactionStatus = result.GetFieldValue<string>(result.GetOrdinal("CryptoTransactionStatus")),
                                CryptoReceiverAddress = result.GetFieldValue<string>(result.GetOrdinal("CryptoReceiverAddress")),
                                CryptoSenderAddress = result.GetFieldValue<string>(result.GetOrdinal("CryptoSenderAddress")),
                                RecordStatus = result.GetFieldValue<string>(result.GetOrdinal("RecordStatus")),
                            };

                            financialLogs.Add(log);
                        }
                    }
                }
                catch (Exception ex) { throw ex; }
                finally { await _context.Database.CloseConnectionAsync(); }
            }

            return financialLogs;

        }

        public async Task<Transaction?> GetLastWithdrawAsync(string userName)
                                        => await base.GetAllAsync().Where(i => i.TransAccountName == userName
                                                                        && i.Description == "Withdraw")
                                                              .OrderByDescending(i => i.TransactionDate)
                                                              .ThenByDescending(i => i.TransactionTime)
                                                              .FirstOrDefaultAsync();
        public async Task SaveWithdrawTransactioinSignatre(TransactionWithdrawSignature signature)
        {
            signature.CreatedDate = DateTime.UtcNow;

            DateOnly currentDate = DateOnly.FromDateTime(signature.CreatedDate);
            TimeOnly currentTime = TimeOnly.FromDateTime(signature.CreatedDate);

            Transaction tnx = base.GetById(signature.TransactionId);
            tnx.TransactionStatus = TransactionStatus.INPROGRESS.ToString();
            tnx.TransactionDate = currentDate;
            tnx.TransactionTime = currentTime;

            TransactionLog log = new()
            {
                TransactionId = tnx.TransactionId,
                LogDate = currentDate,
                LogTime = currentTime,
                TransactionStatus = tnx.TransactionStatus
            };

            tnx.TransactionLogs.Add(log);

            _context.TransactionWithdrawSignatures.Add(signature);
            await base.UpdateAsync(tnx);
        }

        public async Task<TransactionWithdrawSignature?> GetTransactionSignatures(int tnxId)
                => await _context.TransactionWithdrawSignatures
                                .FirstOrDefaultAsync(i => i.TransactionId == tnxId);

        public async Task<SP_AdminDashboardStat> SP_AdminDashboardStatAsync()
        {
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow);

            SqlParameter p_CurrentDate = new("@CurrentDate", currentDate);
            var stats = await _context.SP_AdminDashboardStats
                        .FromSqlRaw("EXEC SP_AdminDashboardStat @CurrentDate", p_CurrentDate)
                        .ToListAsync();


            return stats.FirstOrDefault() ?? new SP_AdminDashboardStat();
        }

        public async Task<decimal> GetMaxDeposityAmountAsync(string userName)
        {
            var tnxs = await base.GetAllAsync()
                            .Where(i => i.TransAccountName == userName
                                        && i.Description == "Deposit"
                                        && i.TransactionStatus == "COMPLETED")
                            .ToListAsync();

            if (tnxs.Count != 0)
                return tnxs.Max(i => i.Amount);
            else
                return 0m;

        }

        public async Task SaveManualFailedTransaction(Transaction tnx)
        {
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
            TimeOnly currentTime = TimeOnly.FromDateTime(DateTime.UtcNow);

            TransactionLog log = new()
            {
                TransactionId = tnx.TransactionId,
                LogDate = currentDate,
                LogTime = currentTime,
                TransactionStatus = tnx.TransactionStatus,
                LogBy = tnx.ModifiedBy,
            };

            tnx.TransactionLogs.Add(log);
            await base.UpdateAsync(tnx);
        }
    }
}
