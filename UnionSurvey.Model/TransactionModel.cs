using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace UnionSurvey.Model
{
    public class TransactionModel
    {
        public int TransactionId { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal Amount { get; set; }
        public bool IsDeposit { get; set; }
        public string? SuveyMessage { get; set; }
        public string WithdrawStatus { get; set; } = null!;//NewWithdraw, ReadyForRequest.
    }
}
