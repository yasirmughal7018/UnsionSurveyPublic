using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnionSurvey.Model
{
    public class TransactionWithdrawSignatureModel
    {
        public int Id { get; set; }

        public string UserName { get; set; } = null!;

        public int TransactionId { get; set; }

        public long Nonce { get; set; }

        public long ExpireTime { get; set; }

        public string Signature { get; set; } = null!;

        public long WithdrawAmount { get; set; }

        public string? Message { get; set; }
    }
}
