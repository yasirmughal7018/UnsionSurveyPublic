using System;
using System.Collections.Generic;

namespace UnionSurvey.Data.Models;

public partial class TransactionLog
{
    public int TransactionLogId { get; set; }

    public int TransactionId { get; set; }

    public DateOnly LogDate { get; set; }

    public TimeOnly LogTime { get; set; }

    public string TransactionStatus { get; set; } = null!;

    public string? LogBy { get; set; }

    public virtual Transaction Transaction { get; set; } = null!;
}
