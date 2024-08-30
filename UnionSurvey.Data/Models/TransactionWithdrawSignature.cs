using System;
using System.Collections.Generic;

namespace UnionSurvey.Data.Models;

public partial class TransactionWithdrawSignature
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public int TransactionId { get; set; }

    public long Nonce { get; set; }

    public long ExpireTime { get; set; }

    public string Signature { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public virtual Transaction Transaction { get; set; } = null!;
}
