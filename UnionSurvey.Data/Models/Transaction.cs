using System;
using System.Collections.Generic;

namespace UnionSurvey.Data.Models;

public partial class Transaction
{
    public int TransactionId { get; set; }

    public string? Description { get; set; }

    public DateOnly TransactionDate { get; set; }

    public TimeOnly TransactionTime { get; set; }

    /// <summary>
    /// Record Status will be either IN or OUT
    /// </summary>
    public string RecordStatus { get; set; } = null!;

    public decimal Amount { get; set; }

    public string TransAccountName { get; set; } = null!;

    public int? SurveyId { get; set; }

    /// <summary>
    /// It will be either In Progress, Pending, Completed
    /// </summary>
    public string TransactionStatus { get; set; } = null!;

    public string? CryptoTransactionId { get; set; }

    public string? CryptoTransactionStatus { get; set; }

    public string? CryptoReceiverAddress { get; set; }

    public string? CryptoSenderAddress { get; set; }

    public string? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual Survey? Survey { get; set; }

    public virtual ICollection<TransactionLog> TransactionLogs { get; set; } = new List<TransactionLog>();

    public virtual ICollection<TransactionWithdrawSignature> TransactionWithdrawSignatures { get; set; } = new List<TransactionWithdrawSignature>();
}
