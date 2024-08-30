using System;
using System.Collections.Generic;

namespace UnionSurvey.Data.Models;

public partial class TransactionInternal
{
    public int TransactionId { get; set; }

    public string TransactionType { get; set; } = null!;

    public DateOnly TransactionDate { get; set; }

    public TimeOnly TransactionTime { get; set; }

    public string RecordStatus { get; set; } = null!;

    public decimal Amount { get; set; }

    public decimal? ComissionPercentage { get; set; }

    public decimal? FromAvailableBalance { get; set; }

    public int SurveyId { get; set; }

    public string? FromUserName { get; set; }

    public string ToUserName { get; set; } = null!;

    public bool IsCommisionTransfered { get; set; }

    public bool IsFirstSurveyCommision { get; set; }

    public virtual AppUser? FromUserNameNavigation { get; set; }

    public virtual Survey Survey { get; set; } = null!;

    public virtual AppUser ToUserNameNavigation { get; set; } = null!;
}
