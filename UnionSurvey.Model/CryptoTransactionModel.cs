using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionSurvey.Model
{
    public class CryptoTransactionModel
    {
        public int Amount { get; set; }
        public int TnxId { get; set; }
        public string CryptoTrxId { get; set; } = null!;
        public string SenderAddress { get; set; } = null!;
        public string ReceiverAddress { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
