using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnionSurvey.Data.Models;
using UnionSurvey.Model;

namespace UnionSurvey.Data.IRepository
{
    public interface ITransactionRepo : IRepository<Transaction>
    {
        Task SaveUserSurvey(Transaction tnx);
        Task SaveStartTransaction(Transaction tnx);
        Task SaveCompletedTransaction(Transaction tnx, string requestFrom);
        Task<IList<SP_FinancialLog>> FinancialLogsAsync(string userName);

        Task<Transaction?> GetLastWithdrawAsync(string userName);
        Task SaveWithdrawTransactioinSignatre(TransactionWithdrawSignature signature);
        Task<TransactionWithdrawSignature?> GetTransactionSignatures(int tnxId);
        Task SaveTransactionAdjustment(int amount, string userName, string loggedInUser);
        Task<SP_AdminDashboardStat> SP_AdminDashboardStatAsync();
        Task<decimal> GetMaxDeposityAmountAsync(string userName);
        Task SaveManualFailedTransaction(Transaction tnx);
    }
}
