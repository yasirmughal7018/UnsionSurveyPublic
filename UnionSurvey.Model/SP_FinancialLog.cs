using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionSurvey.Model
{
    public class SP_FinancialLog
    {
        public int TransactionId { get; set; }
        public string Description { get; set; } = null!;
        public DateOnly LogDate { get; set; }
        public TimeOnly LogTime { get; set; }
        public string TransactionStatus { get; set; } = null!;
        public decimal Amount { get; set; }
        public string TransAccountName { get; set; } = null!;
        public string? CryptoTransactionId { get; set; }
        public string? CryptoTransactionStatus { get; set; }
        public string? CryptoReceiverAddress { get; set; }
        public string? CryptoSenderAddress { get; set; }
        public string RecordStatus { get; set; } = null!;

    }
}
